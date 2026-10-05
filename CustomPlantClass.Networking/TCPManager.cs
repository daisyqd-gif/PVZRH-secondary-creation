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
    /// Central tool for using localhost servers and command recievers
    /// </summary>
    public static class TCPManager
    {
        private static CancellationTokenSource _cts;
        private static ClientWebSocket _clientSocket;
        private static TcpListener _serverListener;
        private static bool _running;
        private static TcpClient _serverClient;
        private static WebSocket _serverSocket;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        /// <summary>
        /// Gets the list of registered ping callbacks keyed by response message.
        /// </summary>
        public static List<(string,Action<string>)> callBacks = new();
        /// <summary>
        /// Gets the collection of command listeners registered to handle commands.
        /// </summary>
        public static List<ICommandListener> commandListeners = new();
        /// <summary>
        /// Gets the queue of incoming command messages awaiting processing.
        /// </summary>
        public static ConcurrentQueue<(string message, string data)> commandQueue = new();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        /// <summary>
        /// Sends a message on the localhost server.
        /// </summary>
        /// <param name="message">The name of the message.</param>
        /// <param name="data">The data payload carried by the message.</param>
        public static void SendMessage(string message, string data)
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
        /// Queues a message for local processing without sending it over the localhost server.
        /// </summary>
        /// <param name="message">The name of the message.</param>
        /// <param name="data">The data associated with the message.</param>
        public static void SendMessageLocal(string message, string data)
        {
            commandQueue.Enqueue((message, data));
        }
        /// <summary>
        /// Sends a ping to a mod and registers the callback that should run when the matching pong is received.
        /// </summary>
        /// <param name="modName">The name of the target mod receiver.</param>
        /// <param name="callBack">The callback to invoke when the ping succeeds.</param>
        /// <param name="data">Optional data payload sent with the ping.</param>
        public static void PingMod(string modName, Action<string> callBack = null, string data = "")
        {
            callBack = callBack ?? ((s) => { });
            commandQueue.Enqueue((modName+"Ping", data));
            callBacks.Add((modName+"Pong",callBack));
        }
        /// <summary>
        /// Stops communication and closes the active socket and listener resources.
        /// </summary>
        public static void StopCommunication()
        {
            _running = false;
            try
            {
                CancellationTokenSource cts = _cts;
                if (cts != null)
                {
                    cts.Cancel();
                }
            }
            catch
            {
            }
            try
            {
                WebSocket activeSocket = ActiveSocket;
                if (activeSocket != null && activeSocket.State == WebSocketState.Open)
                {
                    activeSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown", CancellationToken.None).GetAwaiter().GetResult();
                }
            }
            catch
            {
            }
            try
            {
                ClientWebSocket clientSocket = _clientSocket;
                if (clientSocket != null)
                {
                    clientSocket.Dispose();
                }
            }
            catch
            {
            }
            try
            {
                WebSocket serverSocket = _serverSocket;
                if (serverSocket != null)
                {
                    serverSocket.Dispose();
                }
            }
            catch
            {
            }
            _clientSocket = null;
            _serverSocket = null;
            Plugin.Logger.LogInfo("Connection closed");
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
            _serverClient = null;
            _serverListener = null;
        }
        private static async Task RunClientAsync(string ip, int port)
        {
            try
            {
                _cts = new CancellationTokenSource();
                _clientSocket = new ClientWebSocket();
                Uri uri = new Uri(string.Format("ws://{0}:{1}/", ip, port));
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
        private static async Task RunServerAsync(int port)
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
        private static async Task ReceiveLoopServerAsync()
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
        private static async Task ReceiveLoopClientAsync()
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
        /// <summary>
        /// Starts the automatic server/client networking loop for the specified port.
        /// </summary>
        /// <param name="port">The local port to bind to or connect to.</param>
        public static void StartAuto(int port)
        {
            _running = true;

            Task.Run(async () =>
            {
                try
                {
                    await RunServerAsync(port).ConfigureAwait(false);
                }
                catch
                {
                    await RunClientAsync("127.0.0.1", port).ConfigureAwait(false);
                }
            });
        }
        /// <summary>
        /// Gets a value indicating whether the active socket is currently open and connected.
        /// </summary>
        public static bool IsConnected
        {
            get
            {
                WebSocket activeSocket = ActiveSocket;
                return activeSocket != null && activeSocket.State == WebSocketState.Open;
            }
        }
        private static WebSocket ActiveSocket => _clientSocket ?? _serverSocket;
        /// <summary>
        /// Registers a command listener for incoming commands that match the listener's command name.
        /// </summary>
        /// <param name="listener">The command listener to register.</param>
        public static void RegisterCommandListener(ICommandListener listener)
        {
            commandListeners.Add(listener);
        }
        /// <summary>
        /// Processes all queued inbound commands and dispatches them to any matching callbacks or listeners.
        /// </summary>
        public static void ProcessCommands()
        {
            while (commandQueue.TryDequeue(out var command))
            {
                foreach (var i in callBacks)
                {
                    if( i .Item1 == command.message)
                    {
                        i.Item2(command.data);
                    }
                }
                foreach (var listener in commandListeners)
                {
                    if (command.message == listener.CommandName)
                    {
                        listener.OnCommandReceived(command.data);
                    }
                }
            }
        }
        internal class TCPBehaviour : MonoBehaviour
        {
            public static void OnLoad()
            {
            }
            public void Awake()
            {
                if (_running) return;
                StartAuto(54220);
            }
            public void Update()
            {
                ProcessCommands();
            }
        }
    }
    /// <summary>
    /// Defines the contract for objects that respond to incoming command messages.
    /// </summary>
    public interface ICommandListener
    {
        /// <summary>
        /// Gets the command name that the listener handles.
        /// </summary>
        public string CommandName { get; }
        /// <summary>
        /// Called when a matching command message is received.
        /// </summary>
        /// <param name="data">The command payload.</param>
        public void OnCommandReceived(string data);
    }
    /// <summary>
    /// Base implementation for listeners that respond to a ping command with a pong response.
    /// </summary>
    public abstract class PingListener : ICommandListener
    {
        /// <summary>
        /// Gets the name used to identify the listener and its ping/pong messages.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Gets the command name for the listener's ping message.
        /// </summary>
        public string CommandName => Name + "Ping";
        /// <summary>
        /// Gets the data payload sent with the outgoing pong response.
        /// </summary>
        public virtual string Data => "";

        /// <summary>
        /// Handles an incoming ping command and replies with a pong message.
        /// </summary>
        /// <param name="data">The data payload received with the ping command.</param>
        public void OnCommandReceived(string data)
        {
            OnRecieved(data);
            TCPManager.SendMessageLocal(Name+"Pong",Data);
        }
        /// <summary>
        /// Invoked when a matching ping command is received before the pong is sent.
        /// </summary>
        /// <param name="data">The incoming command payload.</param>
        public virtual void OnRecieved(string data) { }
    }
}