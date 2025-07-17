using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using NETData;

public class PlayerListUI : MonoBehaviour
{
    public Transform listParent;
    public GameObject listEntryPiecePrefab;
    [Space]
    public TMPro.TextMeshProUGUI memberListTxt;

    Dictionary<string, PlayerListEntryUIPiece> playerListEntries = new Dictionary<string, PlayerListEntryUIPiece>();

    string hostUID;

    private void Awake()
    {
        foreach (Transform t in listParent) Destroy(t.gameObject);
    }

    private void Start()
    {
        if(!AppHost.isVirtualMode) LoadSelfOnly();
    }

    public void LoadSelfOnly()
    {
        var memData = new MemberData
        {
            dp = "",
            name = GameDB.selfName,
            role = 1,
            uid = GameDB.selfUID,
        };

        LoadPlayerDatas(new RoomDataExtended
        {
            clients = new MemberData[] { memData },
            hostUID = GameDB.selfUID,
            maxMember = 1,
        });
    }

    public void LoadPlayerDatas(NETData.RoomDataExtended roomData)
    {
        memberListTxt.text = "Lab Members ("+roomData.clients.Length+"/"+roomData.maxMember+")";
        hostUID = roomData.hostUID ;

        var tmp = new List<string>(playerListEntries.Keys);

        foreach (var client in roomData.clients)
        {
            //Debug.Log("ADD : " + client.uid);
            PlayerListEntryUIPiece piece;
            if(playerListEntries.TryGetValue(client.uid, out piece))
            {
                piece.UpdateData(client, hostUID);
            }
            else
            {
                AddEntry(client).UpdateData(client, hostUID); ;
            }
            tmp.Remove(client.uid);
        }

        foreach(var uid in tmp)
        {
            Debug.Log("RM : " + uid);
            PlayerListEntryUIPiece piece;
            if (playerListEntries.TryGetValue(uid, out piece))
            {
                Destroy(piece.gameObject);
                playerListEntries.Remove(uid);
            }
        }

        SortEntries();
    }

    void SortEntries()
    {
        foreach(var piece in playerListEntries.Values)
        {
            if(piece.memberData.uid == hostUID)
            {
                piece.gameObject.transform.SetSiblingIndex(0);
                //Debug.Log("--------------");
                //Debug.Log(piece.gameObject);
            }
        }
    }

    PlayerListEntryUIPiece AddEntry(NETData.MemberData memberData)
    {
        //Debug.Log("AddEntry");
        var go = Instantiate(listEntryPiecePrefab, listParent);
        PlayerListEntryUIPiece piece = go.GetComponent<PlayerListEntryUIPiece>();
        playerListEntries.Add(memberData.uid, piece);
        //Debug.Log(go);
        return piece;
    }
}
