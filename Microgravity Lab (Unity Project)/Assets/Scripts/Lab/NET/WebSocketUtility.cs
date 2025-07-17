using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WebSocketUtility
{

    public event Action OnConnect;
    public event Action OnDisconnect;
    public event Action OnReconnect;
    public event Action<string> OnData;

    public bool IsConnected => _isConnected;
    public string ServerUrl { get; set; } = "ws://localhost:8080";
    public bool AutoReconnect { get; set; } = true;
    public float ReconnectDelay { get; set; } = 5f;
    public int MaxReconnectAttempts { get; set; } = 5;

    private ClientWebSocket _webSocket;
    private CancellationTokenSource _cancellationSource;
    private bool _isConnected = false;
    private bool _isReconnecting = false;
    private Queue<string> _messageQueue = new Queue<string>();
    private int _reconnectAttempts = 0;
    private bool _isProcessingMessages = false;

    public WebSocketUtility()
    {
        StartMessageProcessor();
    }

    public void Connect()
    {
        _ = ConnectAsync();
    }

    public void Disconnect()
    {
        _ = DisconnectAsync();
    }

    public void SendData(string data)
    {
        if (!_isConnected)
        {
            Debug.LogWarning("[WebSocketUtility] Cannot send data: Not connected to server");
            return;
        }

        lock (_messageQueue)
        {
            _messageQueue.Enqueue(data);
        }
    }

    public void SendJson(object obj)
    {
        try
        {
            string jsonData = JsonUtility.ToJson(obj);
            SendData(jsonData);
        }
        catch (Exception e)
        {
            Debug.LogError($"[WebSocketUtility] Error serializing object to JSON: {e.Message}");
        }
    }

    private async Task ConnectAsync()
    {
        if (_isConnected)
        {
            Debug.Log("[WebSocketUtility] Already connected to the server");
            return;
        }

        try
        {
            if (_webSocket != null)
            {
                _webSocket.Dispose();
            }

            _cancellationSource = new CancellationTokenSource();
            _webSocket = new ClientWebSocket();

            Debug.Log($"[WebSocketUtility] Connecting to WebSocket server at {ServerUrl}...");
            await _webSocket.ConnectAsync(new Uri(ServerUrl), _cancellationSource.Token);

            _isConnected = true;
            _reconnectAttempts = 0;

            Debug.Log("[WebSocketUtility] Connected to the server successfully");

            if (_isReconnecting)
            {
                _isReconnecting = false;
                InvokeOnMainThread(() => OnReconnect?.Invoke());
            }
            else
            {
                InvokeOnMainThread(() => OnConnect?.Invoke());
            }

            _ = ReceiveMessagesAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"[WebSocketUtility] Error connecting to the server: {e.Message}");

            if (AutoReconnect && _reconnectAttempts < MaxReconnectAttempts)
            {
                _isReconnecting = true;
                _reconnectAttempts++;
                Debug.Log($"[WebSocketUtility] Attempting to reconnect ({_reconnectAttempts}/{MaxReconnectAttempts}) in {ReconnectDelay} seconds...");
                await Task.Delay(TimeSpan.FromSeconds(ReconnectDelay));
                _ = ConnectAsync();
            }
        }
    }

    private async Task DisconnectAsync()
    {
        if (!_isConnected || _webSocket == null)
            return;

        try
        {
            _cancellationSource?.Cancel();

            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Client disconnecting",
                    CancellationToken.None);
            }

            _isConnected = false;
            _webSocket.Dispose();
            _webSocket = null;

            Debug.Log("[WebSocketUtility] Disconnected from the server");

            InvokeOnMainThread(() => OnDisconnect?.Invoke());
        }
        catch (Exception e)
        {
            Debug.LogError($"[WebSocketUtility] Error disconnecting from the server: {e.Message}");
        }
    }

    private void StartMessageProcessor()
    {
        if (!_isProcessingMessages)
        {
            _isProcessingMessages = true;
            _ = ProcessOutgoingMessagesAsync();
        }
    }

    private async Task ProcessOutgoingMessagesAsync()
    {
        while (true)
        {
            if (_isConnected)
            {
                string message = null;

                lock (_messageQueue)
                {
                    if (_messageQueue.Count > 0)
                    {
                        message = _messageQueue.Dequeue();
                    }
                }

                if (message != null)
                {
                    await SendMessageAsync(message);
                }
            }

            await Task.Delay(8);
        }
    }

    private async Task SendMessageAsync(string message)
    {
        try
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
            {
                Debug.LogWarning("[WebSocketUtility] Cannot send message: WebSocket not in open state");
                return;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await _webSocket.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                _cancellationSource.Token);

            Debug.Log($"[WebSocketUtility] Message sent: {message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[WebSocketUtility] Error sending message: {e.Message}");

            if (_webSocket?.State != WebSocketState.Open && AutoReconnect)
            {
                await HandleConnectionFailure();
            }
        }
    }

    private async Task ReceiveMessagesAsync()
    {
        byte[] buffer = new byte[16384];//new byte[1024*1024*10];

        try
        {
            while (_isConnected && !_cancellationSource.Token.IsCancellationRequested)
            {
                WebSocketReceiveResult result = await _webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    _cancellationSource.Token);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Debug.Log("[WebSocketUtility] Connection closed by the server");
                    await HandleConnectionFailure();
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                ProcessReceivedMessage(message);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("[WebSocketUtility] WebSocket receiving operation was canceled");
        }
        catch (Exception e)
        {
            Debug.LogError($"[WebSocketUtility] Error receiving messages: {e.Message}");
            await HandleConnectionFailure();
        }
    }

    private async Task HandleConnectionFailure()
    {
        if (_isConnected)
        {
            _isConnected = false;
            InvokeOnMainThread(() => OnDisconnect?.Invoke());

            if (AutoReconnect && _reconnectAttempts < MaxReconnectAttempts)
            {
                _isReconnecting = true;
                _reconnectAttempts++;
                Debug.Log($"[WebSocketUtility] Connection lost. Attempting to reconnect ({_reconnectAttempts}/{MaxReconnectAttempts}) in {ReconnectDelay} seconds...");
                await Task.Delay(TimeSpan.FromSeconds(ReconnectDelay));
                _ = ConnectAsync();
            }
        }
    }

    private void ProcessReceivedMessage(string message)
    {
        //Debug.Log("message :: " + message);
        InvokeOnMainThread(() => {
            try
            {
                OnData?.Invoke(message);
                Debug.Log($"[WebSocketUtility] Message received: {message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[WebSocketUtility] Error processing received message: {e.StackTrace}");
            }
        });
    }

    private void InvokeOnMainThread(Action action)
    {
        UnityMainThreadDispatcher.Enqueue(action);
    }
}