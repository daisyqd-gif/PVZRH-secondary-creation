using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;

namespace CustomPlantClass.Networking
{
    public static class TCPManager
    {
        private static CancellationTokenSource _cts;
        private static ClientWebSocket _clientSocket;
        private static TcpListener _serverListener;
        private static bool _running;
        private static TcpClient _serverClient;
        private static WebSocket _serverSocket;
        public static ConcurrentQueue<(string message, string data)> commandQueue = new();
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
                    ModLogger.LogError($"SendMessage error: {arg}");
                }
                return;
            }

            ModLogger.LogInfo("Error Send: " + message);
        }
        public static void SendMessageLocal(string message, string data)
        {
            commandQueue.Enqueue((message, data));
        }
        [OnUnload]
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
            ModLogger.LogInfo("Connection closed");
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
                ModLogger.LogInfo($"TCPManager connected to {uri}");
                await ReceiveLoopClientAsync().ConfigureAwait(false);
                uri = null;
                uri = null;
            }
            catch (Exception arg)
            {
                ModLogger.LogError($"TCPManager client error: {arg}");
            }
        }
        private static async Task RunServerAsync(int port)
        {
            try
            {
                _serverListener = new TcpListener(IPAddress.Any, port);
                _serverListener.Start();
                ModLogger.LogInfo($"TcpManager WebSocket server listening on port {port}");
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
                    ModLogger.LogInfo("TcpManager server accepted TCP client");
                    NetworkStream stream = _serverClient.GetStream();
                    try
                    {
                        _serverSocket = WebSocket.CreateFromStream(stream, true, null, TimeSpan.FromMinutes(2.0));
                        ModLogger.LogInfo("TcpManager server upgraded connection to WebSocket");
                    }
                    catch (Exception arg)
                    {
                        ModLogger.LogError($"TcpManager WebSocket upgrade error: {arg}");
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
                ModLogger.LogError($"TcpManager server error: {arg2}");
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
                ModLogger.LogInfo("TcpManager server stopped");
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
                    ModLogger.LogInfo("TcpManager server received: " + @string);
                    var match = Regex.Match(@string, @"^Message:(?<msg>.+?)\s+Data:(?<data>.+)$");
                    string message = match.Groups["msg"].Value;
                    string data = match.Groups["data"].Value;
                    commandQueue.Enqueue((message: message, data: data));
                }
                catch (Exception arg)
                {
                    ModLogger.LogError($"TcpManager server receive error: {arg}");
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
                    ModLogger.LogInfo("TcpManager received: " + @string);
                    var match = Regex.Match(@string, @"^Message:(?<msg>.+?)\s+Data:(?<data>.+)$");
                    if (!match.Success)
                    {
                        ModLogger.LogError("Malformed command: " + @string);
                        continue;
                    }
                    string message = match.Groups["msg"].Value;
                    string data = match.Groups["data"].Value;
                    commandQueue.Enqueue((message: message, data: data));
                }
                catch (Exception arg)
                {
                    ModLogger.LogError(string.Format("TcpManager receive error: {0}", arg));
                    break;
                }
            }
        }
        public static void StartClient(string ip, int port)
        {
            _running = true;
            Task.Run(() => RunClientAsync(ip, port));
        }
        public static void StartServer(int port)
        {
            _running = true;
            Task.Run(() => RunServerAsync(port));
        }
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
        public static bool IsConnected
        {
            get
            {
                WebSocket activeSocket = ActiveSocket;
                return activeSocket != null && activeSocket.State == WebSocketState.Open;
            }
        }
        private static WebSocket ActiveSocket => _clientSocket ?? _serverSocket;
        public static void RegisterCommandListener(ICommandListener listener)
        {
            commandListeners.Add(listener);
        }
        public static void ProcessCommands()
        {
            while (commandQueue.TryDequeue(out var command))
            {
                foreach (var listener in commandListeners)
                {
                    if (command.message == listener.CommandName)
                    {
                        listener.OnCommandReceived(command.data);
                    }

                }
            }
        }
        public static List<ICommandListener> commandListeners = new();
        private class TCPBehaviour : MonoBehaviour
        {
            [OnLoad]
            public static void OnLoad()
            {
                PluginBehaviour.AddComponentToPlugin<TCPBehaviour>();
            }
            public void Awake()
            {
                if (_running) return;
                StartServer(54220);
            }
            public void Update()
            {
                ProcessCommands();
            }
        }
    }
    public interface ICommandListener
    {
        public string CommandName { get; }
        public void OnCommandReceived(string data);
    }
}