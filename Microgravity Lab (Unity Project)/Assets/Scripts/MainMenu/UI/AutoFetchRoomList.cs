using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class AutoFetchRoomList : MonoBehaviour
{
    public GameObject noRoomObj;
    public GameObject failFetchObj;
    public GameObject fetchingObj;
    [Space]
    public MeinMenuRoomListUI meinMenuRoomListUI;

    static string lastResult = "--";
    static float intervalTime = 0.25f;
    static float time = intervalTime;

    static bool fetchDone = true;

    private void Start()
    {
        meinMenuRoomListUI.ClearAll();
    }

    private void Update()
    {
        if (!fetchDone) return;
        time += Time.deltaTime;
        if(time >= intervalTime)
        {
            time -= intervalTime;
            TryFetch();
        }
    }

    void TryFetch()
    {
        fetchDone = false;
        failFetchObj.SetActive(false);
        fetchingObj.SetActive(lastResult == "");
        GameUtility.HTTP_GET(AppHost.serverHTTP() + "/roomlist", (string text) =>
        {
            fetchDone = true;
            if (text == lastResult) return;
            failFetchObj.SetActive(false);

            bool noRoom = text.Length == 0;
            noRoomObj.SetActive(noRoom);
            if (noRoom) return;

            if (lastResult != text)
            {
                fetchingObj.SetActive(false);
                lastResult = text;
                string[] roomDataPiece = text.Split('*');
                meinMenuRoomListUI.ClearAll();
                foreach (string piece in roomDataPiece)
                {
                    meinMenuRoomListUI.AddEntry(JsonUtility.FromJson<NETData.RoomData>(piece));
                }
            }
        }, () =>
        {
            lastResult = "";
            if(failFetchObj != null) failFetchObj.SetActive(true);
            if(fetchingObj != null) fetchingObj.SetActive(false);
            meinMenuRoomListUI.ClearAll();
            fetchDone = true;
        });
    }
}