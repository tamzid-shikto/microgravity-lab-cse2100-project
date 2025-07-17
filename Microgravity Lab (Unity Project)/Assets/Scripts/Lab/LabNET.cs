using NETData;
using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class LabNET : MonoBehaviour
{
    WebSocketUtility wsu = new WebSocketUtility();
    LabOnMessageNET labOnMessageNET = new LabOnMessageNET();

    public IChangableData<int> roomJoinState = new IChangableData<int>(0);
    public int roomJoinMode = -1;

    public static string activeExperimenterUID = "";
    public static string hostUID = "";

    public static bool IsSelfExpt()
    {
        return !AppHost.isVirtualMode || activeExperimenterUID == GameDB.selfUID;
    }
    public static bool IsSelfHost()
    {
        return !AppHost.isVirtualMode || hostUID == GameDB.selfUID;
    }

    private void Awake()
    {
        LabHost.labNET = this;
        labOnMessageNET.Init(GameDB.selfUID);

        if (AppHost.isVirtualMode)
        {
            ConnectToSever();
        }
    }

    private void Start()
    {
        InitDevTool();
    }

    public void Disconnect()
    {
        if(wsu.IsConnected) wsu.Disconnect();
    }

    void InitDevTool()
    {
        roomJoinState.OnChange += (int data) =>
        {
            LabHost.labDataManager.OnBasicNotify?.Invoke("Room join state: " + data);
        };

        LabHost.labDataManager.DEVTOOLS_KEYDOWN.OnChange += (KeyCode key) =>
        {
            Debug.Log("DEVTOOL INPUT : " + key.ToString());
            if (key == KeyCode.Z)
            {
                Debug.Log("$$$");
                Debug.Log(LabHost.labEnvironmentManager.GetSnapshot());
            }
        };
    }

    void ConnectToSever()
    {
        wsu.ServerUrl = AppHost.serverWS();
        wsu.OnConnect += () =>
        {
            Debug.Log("connected");
            roomJoinState.SetData(1);
            OnConnect();
        };
        wsu.OnData += (string data) =>
        {
            var tmp = JsonUtility.FromJson<NETRoomMessage>(data);
            labOnMessageNET.OnMessage(tmp);
        };
        wsu.Connect();
    }

    public void SendToServer(string action, string data)
    {
        SendToServer(action, data, "");
    }

    public void SendToServer(string action, string data, string extra)
    {
        var msg = new NETData.NETRoomMessage();
        msg.action = action;
        msg.data = data;
        msg.extra = extra;
        wsu.SendJson(msg);
    }

    void OnConnect()
    {
        if(AppHost.roomID == "")
        {
            roomJoinMode = 0;
            SendToServer("create", JsonUtility.ToJson(BuildJoinData()));
        }
        else
        {
            roomJoinMode = 1;
            SendToServer("join", JsonUtility.ToJson(BuildJoinData()));
        }
    }

    NETData.RoomJoinData BuildJoinData()
    {
        return new NETData.RoomJoinData
        {
            uid = GameDB.selfUID,
            name = GameDB.selfName,
            roomID = AppHost.roomID,
            dp = "a"
        };
    }

    private void OnDestroy()
    {
        wsu.Disconnect();
    }

    private void OnApplicationQuit()
    {
        wsu.Disconnect();
    }

    public void SendChatMessage(string message)
    {
        SendToServer("CS_chat_message", JsonUtility.ToJson(new ChatMessage
        {
            message = message,
            sender = GameDB.selfName,
        }));
    }
    public void SendObjectSpawnOrder(ObjectSpawnOrder spawnOrder)
    {
        SendToServer("CS_spawn_object", JsonUtility.ToJson(spawnOrder));
    }
    public void SendSpawnGhost(string nid)
    {
        SendToServer("CS_spawn_ghost", JsonUtility.ToJson(new NET_string
        {
            data = nid,
        }));
    }
    int COUNT = 0;
    public void SendMoveGhost(Vector3 pos)
    {
        SendToServer("CS_move_ghost", JsonUtility.ToJson(pos));
    }
    
    public void SendGravity(float g)
    {
        SendToServer("CS_gravity", JsonUtility.ToJson(new Vector2(0, g)));
    }

    public void SendPlayPause(bool targetState)
    {
        SendToServer("CS_play_pause", JsonUtility.ToJson(new LabPlayPausedata
        {
            state = targetState.ToString(),
            tick = LabHost.labEnvironmentManager.TICK_COUNT
        }));
    }
    public void SendModifyObject(LabObjectData labObjectData)
    {
        if (!AppHost.isVirtualMode) return;
        SendToServer("CS_modify_object", JsonUtility.ToJson(labObjectData));
    }
}

namespace NETData
{
    [Serializable]
    public class ChatMessage
    {
        public string sender;
        public string message;
    }
    [Serializable]
    public class ChatEvent
    {
        public string eventName;
        public string data;
    }
    [Serializable]
    public class NETRoomMessage
    {
        public string action = "";
        public string data = "";
        public string extra = "";
    }
    [Serializable]
    class RoomJoinData
    {
        public string dp = "";
        public string name = "";
        public string uid = "";
        public string roomID = "";
    }
    [Serializable]
    public class RoomDataExtended
    {
        public string hostUID = "";
        public float gravity = 9.8f;
        public int maxMember;
        public MemberData[] clients;
    }

    [Serializable]
    public class MemberData
    {
        public string dp;
        public string name;
        public string uid;
        public int role;
    }

    [Serializable]
    public class SimulationStateData
    {
        public string activeExperimenterUID = "";
        public string simulationPlaying = "True";
        public int tick;
    }
    [Serializable]
    class LabPlayPausedata
    {
        public int tick;
        public string state;
    }
    [Serializable]
    class LabEnvironmentSnapshot
    {
        public int tick;
        public LabObjectData[] LabObjectDatas;
    }
    [Serializable]
    class NET_string
    {
        public string data;
    }
}