using NETData;
using System.Collections;
using UnityEngine;

public class LabOnMessageNET
{
    string selfUID;

    public void Init(string _selfUID)
    {
        selfUID = _selfUID;
    }
    public void OnMessage(NETData.NETRoomMessage message)
    {
        string action = message.action;
        string data = message.data;
        string extra = message.extra;

        if (action == "SC_room_data") SC_room_data(JsonUtility.FromJson<NETData.RoomDataExtended>(data));
        else if (action == "SC_sim_state_data") SC_sim_state_data(JsonUtility.FromJson<NETData.SimulationStateData>(data));
        else if (action == "SC_spawn_object") SC_spawn_object(JsonUtility.FromJson<ObjectSpawnOrder>(data));
        else if (action == "SC_request_env_cache") SC_request_env_cache(extra);
        else if (action == "SC_reply_env_cache") SC_reply_env_cache(data);
        else if (action == "SC_join_done") SC_join_done();
        else if (action == "SC_alert") SC_alert(data);
        else if (action == "SC_chat_event") SC_chat_event(JsonUtility.FromJson<NETData.ChatEvent>(data));
        else if (action == "SC_modify_object") SC_modify_object(JsonUtility.FromJson<LabObjectData>(data));
        else if (action == "SC_spawn_ghost") SC_spawn_ghost(JsonUtility.FromJson<NET_string>(data));
        else if (action == "SC_move_ghost") SC_move_ghost(JsonUtility.FromJson<Vector3>(data));
    }

    void SC_alert(string data)
    {
        LabHost.labDataManager.OnBasicNotify?.Invoke(data);
    }

    void SC_chat_event(NETData.ChatEvent chatEvent)
    {
        string eventName = chatEvent.eventName;
        string data = chatEvent.data;

        if (eventName == "join") ChatUIHost.AddEvent(ChatEventType.GREEN, data + " has joined the lab");
        if (eventName == "leave") ChatUIHost.AddEvent(ChatEventType.RED, data + " has left the lab");
        if (eventName == "play") ChatUIHost.AddEvent(ChatEventType.YELLOW, data + " has resumed simulation");
        if (eventName == "pause") ChatUIHost.AddEvent(ChatEventType.YELLOW, data + " has paused simulation");
        if (eventName == "message")
        {
            var msg = JsonUtility.FromJson<NETData.ChatMessage>(GameUtility.DecodeBase64(data));
            ChatUIHost.AddEvent(ChatEventType.BASIC, msg.message, msg.sender);
        }
        if (eventName == "self")
        {
            var msg = JsonUtility.FromJson<NETData.ChatMessage>(GameUtility.DecodeBase64(data));
            ChatUIHost.AddEvent(ChatEventType.SELF, msg.message, msg.sender);
        }
    }

    void SC_room_data(NETData.RoomDataExtended roomData)
    {
        LabHost.labDataManager.isSelfHost.SetData(roomData.hostUID == selfUID);
        LabNET.hostUID = roomData.hostUID;
        //LabHost.labDataManager.currentTool.TriggerCallback();
        LabHost.labUIManager.playerListUI.LoadPlayerDatas(roomData);
        LabHost.labEnvironmentManager.UpdateGravity(roomData.gravity, true, false);
    }

    void SC_sim_state_data(NETData.SimulationStateData simulationStateData)
    {
        /*int activeExptIndex = 0;
        if (simulationStateData.activeExperimenterUID == "") activeExptIndex = 0;
        else if (simulationStateData.activeExperimenterUID == selfUID) activeExptIndex = 1;
        else activeExptIndex = 2;*/

        if(simulationStateData.simulationPlaying == "True") GhostObjController.instance.ClearAll();

        LabNET.activeExperimenterUID = simulationStateData.activeExperimenterUID;

        //LabHost.labDataManager.activeExperimenterIndex.SetData(activeExptIndex);
        //LabHost.labDataManager.simulationRunningState.SetData(simulationStateData.simulationPlaying == "True");

        if (simulationStateData.activeExperimenterUID == "") LabStateHandler.labExptState = LabExptState.NOONE_EXPT;
        else if (simulationStateData.activeExperimenterUID == selfUID) LabStateHandler.labExptState = LabExptState.SELF_EXPT;
        else LabStateHandler.labExptState = LabExptState.SOMEONE_ELSE_EXPT;

        if (simulationStateData.simulationPlaying == "True") LabStateHandler.labPlayPauseState = LabPlayPauseState.PLAYING;
        else LabStateHandler.labPlayPauseState = LabPlayPauseState.PAUSED;

        LabHost.labDataManager.modifiedTick.SetData(simulationStateData.tick);
    }

    void SC_spawn_object(ObjectSpawnOrder spawnOrder)
    {
        LabHost.labEnvironmentManager.ExecuteSpawnOrder(spawnOrder);
    }
    void SC_modify_object(LabObjectData loData)
    {
        LabHost.labEnvironmentManager.ExecuteObjectModification(loData);
    }
    void SC_request_env_cache(string extra)
    {
        LabHost.labNET.SendToServer("CS_reply_env_cache", LabHost.labEnvironmentManager.GetSnapshot(), extra);
    }
    void SC_reply_env_cache(string data)
    {
        LabHost.labNET.roomJoinState.SetData(2);
        LabHost.labEnvironmentManager.LoadSnapshot(data);
        LabHost.labNET.roomJoinState.SetData(9);
    }

    void SC_join_done()
    {
        LabHost.labDataManager.OnBasicNotify("SC join done");
        if (LabHost.labNET.roomJoinMode == 0)
        {
            LabHost.labNET.roomJoinState.SetData(9);
            LabHost.labEnvironmentManager.envLoaded = true;
        }
        LabHost.labUIManager.LoadComplete();
    }

    void SC_spawn_ghost(NET_string dataObj)
    {
        SO_LabObject so = LabHost.labDataManager.GetObjectSOByNID(dataObj.data);
        /*Debug.Log("NID : " + nid);
        Debug.Log("so==null : " + (so==null));*/
        GhostObjController.instance.SpawnGhostObj(so);
    }
    void SC_move_ghost(Vector3 pos)
    {
        GhostObjController.instance.QuickMoveGhost(pos);
        DEV.instance.MoveGhostCount();
    }
}