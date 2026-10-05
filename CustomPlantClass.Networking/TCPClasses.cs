using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace CustomPlantClass.Networking
{
    /// <summary>
    /// Abstract base class for defining 
    /// </summary>
    public abstract class TCPBase
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected CancellationTokenSource _cts;
        protected ClientWebSocket _clientSocket;
        protected TcpListener _serverListener;
        protected bool _running;
        protected TcpClient _serverClient;
        protected WebSocket _serverSocket;
        protected WebSocket ActiveSocket => _clientSocket ?? _serverSocket;
        public List<(string,Action<string>)> callBacks = new();
        public List<ICommandListener> commandListeners = new();
        public ConcurrentQueue<(string message, string data)> commandQueue = new();
        
        public bool IsConnected
        {
            get
            {
                WebSocket activeSocket = ActiveSocket;
                return activeSocket != null && activeSocket.State == WebSocketState.Open;
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        /// <summary>
        /// Runs when the command is recieved
        /// </summary>
        protected abstract void OnCommandReceived(string message, string data);
        /// <summary>
        /// The port that the server is running on
        /// </summary>
        public abstract int port { get; }
        /// <summary>
        /// Sends a message on the localhost server
        /// </summary>
        /// <param name="message">The name of the message</param>
        /// <param name="data">The data the message holds</param>
        public void SendMessage(string message, string data)
        {
            WebSocket activeSocket = ActiveSocket;
            if (activeSocket != null && activeSocket.State == WebSocketState.Open)
            {
                try
                {
                    string payload = $"Message:{message} Data:{data}";
                    byte[] bytes = Encoding.UTF8.GetBytes(payload);

                    activeSocket.SendAsync(
                        new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    ).GetAwaiter().GetResult();
                }
                catch (Exception arg)
                {
                    Plugin.Logger.LogError($"SendMessage error: {arg}");
                }
                return;
            }

            Plugin.Logger.LogInfo("Error Send: " + message);
        }
        /// <summary>
        /// Processes commands from the queue
        /// </summary>
        public void ProcessCommands()
        {
            while (commandQueue.TryDequeue(out var command))
            {
                OnCommandReceived(command.message,command.data);
            }
        }
        /// <summary>
        /// Starts the TCP server as a client
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws if TCP is already running</exception>
        public void StartClient()
        {
            if (_running)
            {
                throw new InvalidOperationException("Can't run a TCP server twice!");
            }
            _running = true;
            Task.Run(() => RunClientAsync(port));
        }
        /// <summary>
        /// Starts the TCP server as a server
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws if TCP is already running</exception>
        public void StartServer()
        {
            if (_running)
            {
                throw new InvalidOperationException("Can't run a TCP server twice!");
            }
            _running = true;
            Task.Run(() => RunServerAsync(port));
        }
        /// <summary>
        /// Starts the TCP server as a server. If the server is already running, switches to a client
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws if TCP is already running</exception>
        public void StartAuto()
        {
            if (_running)
            {
                throw new InvalidOperationException("Can't run a TCP server twice!");
            }
            _running = true;

            Task.Run(async () =>
            {
                try
                {
                    await RunServerAsync(port).ConfigureAwait(false);
                }
                catch
                {
                    await RunClientAsync(port).ConfigureAwait(false);
                }
            });
        }
        private async Task RunClientAsync(int port)
        {
            try
            {
                _cts = new CancellationTokenSource();
                _clientSocket = new ClientWebSocket();
                Uri uri = new Uri($"ws://127.0.0.1:{port}/");
                await _clientSocket.ConnectAsync(uri, CancellationToken.None).ConfigureAwait(false);
                Plugin.Logger.LogInfo($"TCPManager connected to {uri}");
                await ReceiveLoopClientAsync().ConfigureAwait(false);
                uri = null;
                uri = null;
            }
            catch (Exception arg)
            {
                Plugin.Logger.LogError($"TCPManager client error: {arg}");
            }
        }
        private async Task RunServerAsync(int port)
        {
            try
            {
                _serverListener = new TcpListener(IPAddress.Any, port);
                _serverListener.Start();
                Plugin.Logger.LogInfo($"TcpManager WebSocket server listening on port {port}");
                while (_running)
                {
                    try
                    {
                        _serverClient = await _serverListener.AcceptTcpClientAsync().ConfigureAwait(false);
                    }
                    catch
                    {
                        break;
                    }
                    Plugin.Logger.LogInfo("TcpManager server accepted TCP client");
                    NetworkStream stream = _serverClient.GetStream();
                    try
                    {
                        _serverSocket = WebSocket.CreateFromStream(stream, true, null, TimeSpan.FromMinutes(2.0));
                        Plugin.Logger.LogInfo("TcpManager server upgraded connection to WebSocket");
                    }
                    catch (Exception arg)
                    {
                        Plugin.Logger.LogError($"TcpManager WebSocket upgrade error: {arg}");
                        try
                        {
                            TcpClient serverClient = _serverClient;
                            if (serverClient != null)
                            {
                                serverClient.Close();
                            }
                        }
                        catch
                        {
                        }
                        _serverClient = null;
                        continue;
                    }
                    await ReceiveLoopServerAsync().ConfigureAwait(false);
                    try
                    {
                        TcpClient serverClient2 = _serverClient;
                        if (serverClient2 != null)
                        {
                            serverClient2.Close();
                        }
                    }
                    catch
                    {
                    }
                    _serverClient = null;
                    _serverSocket = null;
                }
            }
            catch (Exception arg2)
            {
                Plugin.Logger.LogError($"TcpManager server error: {arg2}");
            }
            finally
            {
                try
                {
                    TcpListener serverListener = _serverListener;
                    if (serverListener != null)
                    {
                        serverListener.Stop();
                    }
                }
                catch
                {
                }
                _serverListener = null;
                Plugin.Logger.LogInfo("TcpManager server stopped");
            }
        }
        private async Task ReceiveLoopServerAsync()
        {
            byte[] buffer = new byte[4096];
            while (_running && _serverSocket != null && _serverSocket.State == WebSocketState.Open)
            {
                try
                {
                    WebSocketReceiveResult webSocketReceiveResult = await _serverSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);
                    if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                    string @string = Encoding.UTF8.GetString(buffer, 0, webSocketReceiveResult.Count);
                    Plugin.Logger.LogInfo("TcpManager server received: " + @string);
                    var match = Regex.Match(@string, @"^Message:(?<msg>.+?)\s+Data:(?<data>.+)$");
                    string message = match.Groups["msg"].Value;
                    string data = match.Groups["data"].Value;
                    commandQueue.Enqueue((message: message, data: data));
                }
                catch (Exception arg)
                {
                    Plugin.Logger.LogError($"TcpManager server receive error: {arg}");
                    break;
                }
            }
        }
        private async Task ReceiveLoopClientAsync()
        {
            byte[] buffer = new byte[4096];
            while (_running && _clientSocket != null && _clientSocket.State == WebSocketState.Open)
            {
                try
                {
                    WebSocketReceiveResult webSocketReceiveResult = await _clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);
                    if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                    string @string = Encoding.UTF8.GetString(buffer, 0, webSocketReceiveResult.Count);
                    Plugin.Logger.LogInfo("TcpManager received: " + @string);
                    var match = Regex.Match(@string, @"^Message:(?<msg>.+?)\s+Data:(?<data>.+)$");
                    if (!match.Success)
                    {
                        Plugin.Logger.LogError("Malformed command: " + @string);
                        continue;
                    }
                    string message = match.Groups["msg"].Value;
                    string data = match.Groups["data"].Value;
                    commandQueue.Enqueue((message: message, data: data));
                }
                catch (Exception arg)
                {
                    Plugin.Logger.LogError($"TcpManager receive error: {arg}");
                    break;
                }
            }
        }
    }
}