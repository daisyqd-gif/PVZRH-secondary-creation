using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using BepInEx.Logging;

namespace CustomPlantClass.Networking
{
    /// <summary>
    /// Sends length-prefixed UTF-8 messages over a local named pipe.
    /// </summary>
    public static class IPCManager
    {
        /// <summary>
        /// The default local named-pipe name used by this manager.
        /// </summary>
        public const string DefaultPipeName = "CustomPlantClass networking service";

        private const int MaxMessageLength = 8 * 1024 * 1024;
        private const int ConnectTimeoutMilliseconds = 3000;

        private static readonly object ClientLock = new();
        private static readonly object ServerLock = new();
        private static readonly object StateLock = new();
        private static readonly Channel<string> OutboundMessages =
            Channel.CreateUnbounded<string>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
        private static readonly ConcurrentQueue<(string message, string data)> CommandQueue = new();
        private static readonly object ListenerLock = new();
        private static readonly object CallbackLock = new();
        private static readonly List<ICommandListener> CommandListeners = new();
        private static readonly List<(string message, Action<string> callback)> Callbacks = new();

        [ThreadStatic]
        private static bool _dispatchingIpcCommand;

        private static NamedPipeClientStream _client;
        private static NamedPipeServerStream _server;
        private static CancellationTokenSource _runCancellation;
        private static CancellationTokenSource _connectionCancellation;
        private static ManualLogSource _logger;
        private static int _runId;
        private static bool _running;

        /// <summary>
        /// Gets whether the active named-pipe connection is connected.
        /// </summary>
        public static bool IsConnected
        {
            get
            {
                lock (StateLock)
                {
                    if (!_running)
                    {
                        return false;
                    }
                }

                lock (ClientLock)
                {
                    if (_client != null && _client.IsConnected)
                    {
                        return true;
                    }
                }

                lock (ServerLock)
                {
                    return _server != null && _server.IsConnected;
                }
            }
        }

        /// <summary>
        /// Sets the logger used for IPC diagnostics.
        /// </summary>
        public static void Initialize(ManualLogSource logSource)
        {
            _logger = logSource;
        }

        /// <summary>
        /// Starts this process as the named-pipe server.
        /// </summary>
        public static void StartServer(string pipeName = null)
        {
            Start("CustomPlantClassPipeServer", token => RunServerAsync(NormalizePipeName(pipeName), token));
        }

        /// <summary>
        /// Starts this process as a client of the named-pipe server.
        /// </summary>
        public static void StartClient(string pipeName = null)
        {
            Start("CustomPlantClassPipeClient", token => RunClientAsync(NormalizePipeName(pipeName), token));
        }

        /// <summary>
        /// Starts as a server, falling back to a client if server startup fails.
        /// </summary>
        public static void StartAuto(string pipeName = null)
        {
            Start("CustomPlantClassPipeAuto", token => RunAutoAsync(NormalizePipeName(pipeName), token));
        }

        /// <summary>
        /// Stops communication and closes the active pipe streams.
        /// </summary>
        public static void StopCommunication()
        {
            CancellationTokenSource runCancellation;
            CancellationTokenSource connectionCancellation;
            lock (StateLock)
            {
                _running = false;
                Interlocked.Increment(ref _runId);
                runCancellation = _runCancellation;
                connectionCancellation = _connectionCancellation;
                _runCancellation = null;
                _connectionCancellation = null;
            }

            CancelAndDispose(runCancellation);
            CancelAndDispose(connectionCancellation);
            ClosePipe(ClientLock, ref _client);
            ClosePipe(ServerLock, ref _server);
            _logger?.LogInfo("Connection closed");
        }

        /// <summary>
        /// Queues a message for delivery to the connected pipe endpoint.
        /// </summary>
        public static void SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (!IsConnected || !OutboundMessages.Writer.TryWrite(message))
            {
                _logger?.LogInfo("Error Send: " + message);
            }
        }

        /// <summary>
        /// Queues a command message for delivery to the connected pipe endpoint.
        /// </summary>
        public static void SendMessage(string message, string data)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            SendMessage($"Message:{message} Data:{data ?? string.Empty}");
        }

        /// <summary>
        /// Queues a command for local processing without sending it over the pipe.
        /// </summary>
        public static void SendMessageLocal(string message, string data)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            CommandQueue.Enqueue((message, data ?? string.Empty));
        }

        /// <summary>
        /// Registers a listener for matching incoming command messages.
        /// </summary>
        public static void RegisterCommandListener(ICommandListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            lock (ListenerLock)
            {
                if (!CommandListeners.Contains(listener))
                {
                    CommandListeners.Add(listener);
                }
            }
        }

        /// <summary>
        /// Removes a previously registered command listener.
        /// </summary>
        /// <returns><see langword="true"/> if the listener was registered and removed.</returns>
        public static bool UnregisterCommandListener(ICommandListener listener)
        {
            lock (ListenerLock)
            {
                return CommandListeners.Remove(listener);
            }
        }

        /// <summary>
        /// Sends a ping command and registers a callback for its matching pong.
        /// </summary>
        public static void PingMod(string modName, Action<string> callback = null, string data = "")
        {
            if (string.IsNullOrEmpty(modName))
            {
                throw new ArgumentException("A mod name is required.", nameof(modName));
            }

            lock (CallbackLock)
            {
                Callbacks.Add((modName + "Pong", callback ?? (_ => { })));
            }

            SendMessage(modName + "Ping", data);
        }

        /// <summary>
        /// Dispatches queued commands to matching ping callbacks and registered listeners.
        /// Call this from the mod's main-thread update loop.
        /// </summary>
        public static void ProcessCommands()
        {
            while (CommandQueue.TryDequeue(out var command))
            {
                (string message, Action<string> callback)[] callbacks;
                ICommandListener[] listeners;

                lock (CallbackLock)
                {
                    callbacks = Callbacks.ToArray();
                }

                lock (ListenerLock)
                {
                    listeners = CommandListeners.ToArray();
                }

                foreach (var registeredCallback in callbacks)
                {
                    if (registeredCallback.message == command.message)
                    {
                        registeredCallback.callback(command.data);
                    }
                }

                foreach (ICommandListener listener in listeners)
                {
                    if (command.message != listener.CommandName)
                    {
                        continue;
                    }

                    bool wasDispatchingIpcCommand = _dispatchingIpcCommand;
                    _dispatchingIpcCommand = true;
                    try
                    {
                        listener.OnCommandReceived(command.data);
                    }
                    finally
                    {
                        _dispatchingIpcCommand = wasDispatchingIpcCommand;
                    }
                }
            }
        }

        internal static void SendPingResponse(string message, string data)
        {
            if (_dispatchingIpcCommand)
            {
                SendMessageLocal(message, data);
            }
            else
            {
                TCPManager.SendMessageLocal(message, data);
            }
        }

        private static void Start(string threadName, Func<CancellationToken, Task> run)
        {
            CancellationTokenSource previous;
            CancellationTokenSource current = new();
            CancellationToken cancellationToken = current.Token;
            int runId;

            lock (StateLock)
            {
                runId = Interlocked.Increment(ref _runId);
                _running = true;
                previous = _runCancellation;
                _runCancellation = current;
            }

            CancelAndDispose(previous);
            ClosePipe(ClientLock, ref _client);
            ClosePipe(ServerLock, ref _server);
            DrainOutboundMessages();

            var thread = new Thread(() =>
            {
                try
                {
                    run(cancellationToken).GetAwaiter().GetResult();
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                }
                catch (Exception exception)
                {
                    _logger?.LogError($"{threadName} error: {exception}");
                }
                finally
                {
                    lock (StateLock)
                    {
                        if (runId == _runId)
                        {
                            _running = false;
                        }
                    }

                    current.Dispose();
                }
            })
            {
                IsBackground = true,
                Name = threadName
            };

            thread.Start();
        }

        private static async Task RunAutoAsync(string pipeName, CancellationToken cancellationToken)
        {
            try
            {
                await RunServerAsync(pipeName, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger?.LogInfo($"Unable to start pipe server; connecting as a client instead: {exception.Message}");
                await RunClientAsync(pipeName, cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task RunServerAsync(string pipeName, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var server = new NamedPipeServerStream(
                    pipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous,
                    MaxMessageLength,
                    MaxMessageLength);

                lock (ServerLock)
                {
                    _server = server;
                }

                try
                {
                    await server.WaitForConnectionAsync(cancellationToken).ConfigureAwait(false);
                    await RunConnectionAsync(server, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    lock (ServerLock)
                    {
                        if (ReferenceEquals(_server, server))
                        {
                            _server = null;
                        }
                    }

                    server.Dispose();
                }
            }
        }

        private static async Task RunClientAsync(string pipeName, CancellationToken cancellationToken)
        {
            var client = new NamedPipeClientStream(
                ".",
                pipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous);

            lock (ClientLock)
            {
                _client = client;
            }

            try
            {
                await client.ConnectAsync(ConnectTimeoutMilliseconds, cancellationToken).ConfigureAwait(false);
                await RunConnectionAsync(client, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                lock (ClientLock)
                {
                    if (ReferenceEquals(_client, client))
                    {
                        _client = null;
                    }
                }

                client.Dispose();
            }
        }

        private static async Task RunConnectionAsync(PipeStream pipe, CancellationToken runToken)
        {
            CancellationTokenSource connectionCancellation =
                CancellationTokenSource.CreateLinkedTokenSource(runToken);
            CancellationTokenSource previous;

            lock (StateLock)
            {
                previous = _connectionCancellation;
                _connectionCancellation = connectionCancellation;
            }

            CancelAndDispose(previous);

            try
            {
                Task receiveTask = ReceiveMessagesAsync(pipe, connectionCancellation.Token);
                Task sendTask = SendMessagesAsync(pipe, connectionCancellation.Token);
                await Task.WhenAny(receiveTask, sendTask).ConfigureAwait(false);
                connectionCancellation.Cancel();
                await ObserveConnectionTaskAsync(receiveTask).ConfigureAwait(false);
                await ObserveConnectionTaskAsync(sendTask).ConfigureAwait(false);
            }
            finally
            {
                lock (StateLock)
                {
                    if (ReferenceEquals(_connectionCancellation, connectionCancellation))
                    {
                        _connectionCancellation = null;
                    }
                }

                CancelAndDispose(connectionCancellation);
            }
        }

        private static async Task ReceiveMessagesAsync(PipeStream pipe, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string message = await ReadMessageAsync(pipe, cancellationToken).ConfigureAwait(false);
                if (message == null)
                {
                    return;
                }

                if (TryParseCommand(message, out string command, out string data))
                {
                    CommandQueue.Enqueue((command, data));
                }
                else if (!message.StartsWith("Message:", StringComparison.Ordinal))
                {
                    CommandQueue.Enqueue((message, string.Empty));
                }
                else
                {
                    _logger?.LogError("Malformed IPC command: " + message);
                }
            }
        }

        private static async Task SendMessagesAsync(PipeStream pipe, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string message = await OutboundMessages.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
                await WriteMessageAsync(pipe, message, cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task<string> ReadMessageAsync(Stream stream, CancellationToken cancellationToken)
        {
            byte[] lengthBytes = new byte[sizeof(int)];
            if (!await ReadExactlyAsync(stream, lengthBytes, cancellationToken).ConfigureAwait(false))
            {
                return null;
            }

            int messageLength = BitConverter.ToInt32(lengthBytes, 0);
            if (messageLength < 0 || messageLength > MaxMessageLength)
            {
                throw new InvalidDataException($"Invalid IPC message length: {messageLength}.");
            }

            byte[] messageBytes = new byte[messageLength];
            if (!await ReadExactlyAsync(stream, messageBytes, cancellationToken).ConfigureAwait(false))
            {
                throw new EndOfStreamException("The IPC connection closed before the complete message was received.");
            }

            return Encoding.UTF8.GetString(messageBytes);
        }

        private static async Task<bool> ReadExactlyAsync(
            Stream stream,
            byte[] buffer,
            CancellationToken cancellationToken)
        {
            int offset = 0;
            while (offset < buffer.Length)
            {
                int bytesRead = await stream.ReadAsync(
                    buffer,
                    offset,
                    buffer.Length - offset,
                    cancellationToken).ConfigureAwait(false);

                if (bytesRead == 0)
                {
                    if (offset != 0)
                    {
                        throw new EndOfStreamException("The IPC connection closed in the middle of a message.");
                    }

                    return false;
                }

                offset += bytesRead;
            }

            return true;
        }

        private static async Task WriteMessageAsync(
            Stream stream,
            string message,
            CancellationToken cancellationToken)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            if (messageBytes.Length > MaxMessageLength)
            {
                throw new InvalidDataException(
                    $"IPC message exceeds the maximum size of {MaxMessageLength} bytes.");
            }

            byte[] lengthBytes = BitConverter.GetBytes(messageBytes.Length);
            await stream.WriteAsync(lengthBytes, 0, lengthBytes.Length, cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken).ConfigureAwait(false);
            await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        private static async Task ObserveConnectionTaskAsync(Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static void DrainOutboundMessages()
        {
            while (OutboundMessages.Reader.TryRead(out _))
            {
            }
        }

        private static string NormalizePipeName(string pipeName)
        {
            if (string.IsNullOrWhiteSpace(pipeName))
            {
                return DefaultPipeName;
            }

            const string pipePrefix = @"\\.\pipe\";
            return pipeName.StartsWith(pipePrefix, StringComparison.OrdinalIgnoreCase)
                ? pipeName.Substring(pipePrefix.Length)
                : pipeName;
        }

        private static bool TryParseCommand(string payload, out string message, out string data)
        {
            const string messagePrefix = "Message:";
            const string dataSeparator = " Data:";

            message = null;
            data = null;
            if (!payload.StartsWith(messagePrefix, StringComparison.Ordinal))
            {
                return false;
            }

            int separatorIndex = payload.IndexOf(dataSeparator, messagePrefix.Length, StringComparison.Ordinal);
            if (separatorIndex < 0)
            {
                return false;
            }

            message = payload.Substring(messagePrefix.Length, separatorIndex - messagePrefix.Length);
            if (message.Length == 0)
            {
                return false;
            }

            data = payload.Substring(separatorIndex + dataSeparator.Length);
            return true;
        }

        private static void ClosePipe<TPipe>(object pipeLock, ref TPipe pipe)
            where TPipe : class, IDisposable
        {
            TPipe pipeToClose;
            lock (pipeLock)
            {
                pipeToClose = pipe;
                pipe = null;
            }

            pipeToClose?.Dispose();
        }

        private static void CancelAndDispose(CancellationTokenSource cancellation)
        {
            if (cancellation == null)
            {
                return;
            }

            try
            {
                cancellation.Cancel();
            }
            catch (ObjectDisposedException)
            {
            }

            cancellation.Dispose();
        }

    }
}
