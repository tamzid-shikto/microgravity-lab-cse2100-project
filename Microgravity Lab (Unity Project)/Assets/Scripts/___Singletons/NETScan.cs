using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;

public class NETScan : MonoBehaviour
{
    public static NETScan instance;
    static List<string> responsiveIps;

    private void Awake()
    {
        instance = this;
    }

    public static async void StartScan(Action<string> singleCB, Action<List<string>> callback)
    {
        // Start the async scan task
        await ScanSubnetAsync(singleCB , () =>
        {
            callback?.Invoke(responsiveIps);
        });
    }

    static string GetLocalIPAddress()
    {
        string localIP = "";
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }

    // Now this is an async method that does not use Unity coroutines.
    static async Task ScanSubnetAsync(Action<string> singleCB, Action callback)
    {
        responsiveIps = new List<string>();
        int loadCount = 0;
        string localIP = GetLocalIPAddress();
        if (string.IsNullOrEmpty(localIP))
        {
            Debug.LogError("Local IP not found.");
            return;
        }

        string subnet = localIP.Substring(0, localIP.LastIndexOf('.') + 1);
        Debug.Log("Scanning subnet: " + subnet + "0/24");

        List<Task> tasks = new List<Task>();

        // Create tasks for checking each IP asynchronously
        for (int i = 1; i < 255; i++)
        {
            string ip = subnet + i;
            loadCount++;

            // Add the async CheckIp task
            tasks.Add(CheckIpAsync(ip, singleCB , () =>
            {
                loadCount--;
            }));

            // Introducing a small async delay between requests
            await Task.Delay(1);
        }

        // Wait for all the tasks to complete
        await Task.WhenAll(tasks);

        // Invoke the callback once all tasks are done
        callback?.Invoke();
    }

    // This is the async method that checks a single IP address
    static async Task CheckIpAsync_(string ip, Action callback)
    {
        string url = $"http://{ip}:55050/gs";

        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(1);

            try
            {
                // Asynchronously await the GET request
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    responsiveIps.Add(ip);
                    Debug.Log("200 OK from: " + ip);
                }
            }
            catch (Exception e)
            {
                // Handle exceptions such as timeout or no response
                //Debug.LogError($"Error from {ip}: {e.Message}");
            }
        }

        // Call the callback after the IP check is completed
        callback?.Invoke();
    }
    static async Task CheckIpAsync(string ip, Action<string> singleCB, Action callback)
    {
        string url = $"http://{ip}:55050/gs";

        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(0.25); // Set the request timeout

            try
            {
                // Explicitly set a short timeout for the connection
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    responsiveIps.Add($"{ip}:55050");
                    singleCB?.Invoke($"{ip}:55050"); 
                    Debug.Log("200 OK from: " + ip);
                }
            }
            catch (TaskCanceledException ex)
            {
                // This is thrown when the request times out
                if (ex.CancellationToken == CancellationToken.None)
                {
                    //Debug.LogError($"Request timed out for {ip}");
                }
                else
                {
                    //Debug.LogError($"Request was cancelled for {ip}: {ex.Message}");
                }
            }
            catch (Exception e)
            {
                // Handle other exceptions such as network issues
                //Debug.LogError($"Error from {ip}: {e.Message}");
            }
        }

        // Call the callback after the IP check is completed
        callback?.Invoke();
    }

}
