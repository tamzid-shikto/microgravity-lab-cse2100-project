using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MainMenuNET : MonoBehaviour
{

    private void Awake()
    {
        MainMenuHost.mainMenuNET = this;
    }
}

namespace NETData
{
    [Serializable]
    public class RoomData
    {
        public string roomID;
        public string hostName;
        public string dp;
        public string size;
    }
}