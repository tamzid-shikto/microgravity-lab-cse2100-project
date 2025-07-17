using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDataManager : MonoBehaviour
{
    public IChangableList<NETData.RoomData> fetchedRooms = new IChangableList<NETData.RoomData>();
    public IChangableData<int> currentPickedLabMode = new IChangableData<int>();
    public IChangableData<int> roomFetchingState = new IChangableData<int>();

    private void Awake()
    {
        MainMenuHost.mainMenuDataManager = this;
    }

    private void Start()
    {
        currentPickedLabMode.SetData(0);
        roomFetchingState.SetData(0);
    }
}
