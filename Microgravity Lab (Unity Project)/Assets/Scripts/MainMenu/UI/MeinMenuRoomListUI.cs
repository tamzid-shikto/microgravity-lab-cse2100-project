using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MeinMenuRoomListUI: MonoBehaviour
{
    public Transform listParent;
    public GameObject listEntryPiecePrefab;

    Dictionary<NETData.RoomData, GameObject> entryDictionary = new Dictionary<NETData.RoomData, GameObject>();

    private void Start()
    {
        //foreach (Transform t in listParent) Destroy(t.gameObject);
        //MainMenuHost.mainMenuDataManager.fetchedRooms.OnAdd += AddEntry;
        //MainMenuHost.mainMenuDataManager.fetchedRooms.OnRemove += RemoveEntry;
    }

    public void ClearAll()
    {
        foreach(Transform t in listParent) Destroy(t.gameObject);
    }

    public void AddEntry(NETData.RoomData roomData)
    {
        if (roomData == null) return;
        string hostName = roomData.hostName;
        string roomID = roomData.roomID;
        string size = roomData.size+"";

        GameObject go = Instantiate(listEntryPiecePrefab, listParent);
        MeinMenuRoomListEntryUIPiece piece = go.GetComponent<MeinMenuRoomListEntryUIPiece>();
        piece.SetData(hostName, size, () =>
        {
            AppHost.OnStartVirtualCall?.Invoke(roomID);
        });
        //entryDictionary.Add(roomData, go);
    }
/*
    void RemoveEntry(NETData.RoomData roomData)
    {
        if (roomData == null) return;
        GameObject go;
        if(entryDictionary.TryGetValue(roomData, out go))
        {
            Destroy(go);
            entryDictionary.Remove(roomData);
        }
    }*/
}
