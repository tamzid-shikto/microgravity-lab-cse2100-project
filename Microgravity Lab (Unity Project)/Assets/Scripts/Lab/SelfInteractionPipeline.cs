using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SelfInteractionPipeline : MonoBehaviour
{
    private void Awake()
    {
        LabHost.selfInteractionPipeline = this;
    }

    private void Start()
    {
        LabHost.labInputManager.OnPlayPausePress += () =>
        {
            //bool targetState = !LabHost.labDataManager.simulationRunningState.GetData();
            bool isPlaying = LabStateHandler.labPlayPauseState == LabPlayPauseState.PLAYING;
            if (!AppHost.isVirtualMode)
            {
                /*LabHost.labDataManager.simulationRunningState.SetData(targetState);
                LabHost.labDataManager.currentTool.TriggerCallback();*/
                if (isPlaying) LabStateHandler.labPlayPauseState = LabPlayPauseState.PAUSED;
                else LabStateHandler.labPlayPauseState = LabPlayPauseState.PLAYING;

                if (isPlaying) LabStateHandler.labExptState = LabExptState.SELF_EXPT;
                else LabStateHandler.labExptState = LabExptState.NOONE_EXPT;
            }
            else
            {
                LabHost.labNET.SendPlayPause(!isPlaying);
            }
        };
    }

    public void CameraMove()
    {

    }
    public void SpawnObject(SO_LabObject so, Vector3 forward, Vector3 point)
    {
        //Debug.Log("SpawnObject()");
        var spawnOrder = new ObjectSpawnOrder
        {
            tick = LabHost.labEnvironmentManager.TICK_COUNT,
            position = point,
            rotation = Quaternion.identity,
            prefabNID = so.objectNID,
            objectID = GameUtility.GenerateObjectSpawnID(GameDB.selfUID),
        };

        if (!AppHost.isVirtualMode) LabHost.labEnvironmentManager.ExecuteSpawnOrder(spawnOrder);
        else LabHost.labNET.SendObjectSpawnOrder(spawnOrder);
    }
}