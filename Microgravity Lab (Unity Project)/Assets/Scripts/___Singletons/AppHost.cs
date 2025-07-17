using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;

public class AppHost : MonoBehaviour
{

    public static Action<int> OnStartOfflineCall;
    public static Action<string> OnStartVirtualCall;

    public static bool isVirtualMode;
    public static string roomID;
    public static SavedLabData savedLabData;

    public List<string> serverAddresses = new List<string>();

    static List<string> keepAliveScenes = new List<string> { "HostScene" };

    static string serverHostName = "localhost:55050";
    public static string serverName = "localhost";
    public static string serverWS() => "ws://" + serverHostName;
    public static string serverHTTP() => "http://" + serverHostName;

    public static AppHost instance;

    static bool hasLoaded = false;
    static int fps;

    private void Awake()
    {
        fps = (int)UnityEngine.Random.Range(15, 60);
        instance = this;
        Set60Framerate();
    }

    public void Set60Framerate()
    {
        Application.targetFrameRate = 60;
        //Application.targetFrameRate = fps;
        //Debug.Log("Setting FPS: " + fps);
    }
    public void SetUnlimitedFramerate()
    {
        Application.targetFrameRate = 0;
    }

    void Start()
    {
        OnStartOfflineCall += (int mode) =>
        {
            isVirtualMode = false;
            LoadLabScene();
        };
        OnStartVirtualCall += (string _roomID) =>
        {
            isVirtualMode = true;
            roomID = _roomID;
            LoadLabScene();
        };

        /*NETScan.StartScan((string ip)=> {
            serverHostName = ip;
            Debug.Log(ip);
            if (hasLoaded) return;
            hasLoaded = true;
            LoadMainMenuScene();
        },(List<string> ips) =>
        {
            Debug.Log("DONE");
            if (hasLoaded) return;
            hasLoaded = true;
            LoadMainMenuScene();
        });*/

        FindServer(0, ()=>{
            LoadMainMenuScene();
        });
    }

    void FindServer(int index, Action callback)
    {
        if(index >= serverAddresses.Count)
        {
            Debug.Log(":: NO ADDR");
            serverName = "";
            callback?.Invoke();
            return;
        }
        string thisAddress = serverAddresses[index];

        GameUtility.HTTP_GET(thisAddress+"/name", (data) =>
        {
            Debug.Log("SUCCESS ADDR :: " + thisAddress);
            serverHostName = thisAddress;
            serverName = data;
            callback?.Invoke();
            return;
        }, () =>
        {
            Debug.Log("FAIL ADDR :: " + thisAddress);
            FindServer(index+1, callback);
        });
    }

    public void LoadMainMenuScene()
    {
        UnloadOtherScenes();
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Additive);
    }
    public void LoadLabScene()
    {
        UnloadOtherScenes();
        SceneManager.LoadScene("LabScene", LoadSceneMode.Additive);
    }

    public void UnloadOtherScenes()
    {
        for(int i = 0; i<SceneManager.sceneCount; i++)
        {
            string thisSceneName = SceneManager.GetSceneAt(i).name;
            if (keepAliveScenes.Contains(thisSceneName)) continue;

            if (SceneManager.GetSceneAt(i).isLoaded)
            {
                SceneManager.UnloadScene(thisSceneName);
            }
        }
    }
}
