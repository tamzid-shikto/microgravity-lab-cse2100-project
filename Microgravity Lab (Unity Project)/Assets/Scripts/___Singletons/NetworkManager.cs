using System.Text;
using UnityEngine;
/*using NativeWebSocket;*/

public class NetworkManager : MonoBehaviour
{
    /*WebSocket websocket;

    async void Start()
    {
        websocket = new WebSocket("wss://echo.websocket.org");

        websocket.OnOpen += () =>
        {
            Debug.Log("WebSocket connected!");
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = Encoding.UTF8.GetString(bytes);
            Debug.Log("Received message: " + message);
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError("WebSocket Error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket closed!");
        };

        // Connect to the server
        await websocket.Connect();

        // Send a message
        string message = "Hello from Unity!";
        await websocket.SendText(message);
    }

    private async void Update()
    {
        if (websocket != null)
        {
            await websocket.DispatchMessageQueue();
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }*/
}
