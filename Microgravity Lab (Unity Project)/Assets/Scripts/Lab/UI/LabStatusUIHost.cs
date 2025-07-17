using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class LabStatusUIHost : MonoBehaviour
{
    public List<IconTextSelectionController> toolSelectionList;

    [Space]
    public GameObject playSignObj;
    public GameObject pauseSignObj;

    [Space]
    public List<IconTextSelectionController> roleSelectionList;

    [Space]
    public GameObject otherDoingExptObj;
    public GameObject needToPauseObj;

    public static LabStatusUIHost instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        /*LabHost.labDataManager.simulationRunningState.OnChange += (bool state) =>
        {
            UpdatePlayPauseSign();
        };
        LabHost.labDataManager.currentTool.OnChange += (int _) =>
        {
            UpdatePlayPauseSign();
        };
        LabHost.labDataManager.currentTool.OnChange += (int index) =>
        {
            foreach (var tool in toolSelectionList) tool.UnSelected();
            toolSelectionList[index].Selected();
        };

        LabHost.labDataManager.activeExperimenterIndex.OnChange += (int _) =>
        {
            UpdatePlayPauseSign();
        };*/


        LabHost.labInputManager.OnToolSwitch += (int index) =>
        {
            foreach (var tool in toolSelectionList) tool.UnSelected();
            toolSelectionList[index].Selected();
        };
    }

    void UpdatePlayPauseSign()
    {
        return;
        /*bool isSimulationPlaying = LabHost.labDataManager.simulationRunningState.GetData();
        int activeExptIndex = LabHost.labDataManager.activeExperimenterIndex.GetData();
        bool isAnalysisMode = LabHost.labDataManager.currentTool.GetData() == 2;
        playSignObj.SetActive(isSimulationPlaying);
        pauseSignObj.SetActive(!isSimulationPlaying);

        otherDoingExptObj.SetActive(activeExptIndex==2 && AppHost.isVirtualMode && !isAnalysisMode);
        needToPauseObj.SetActive(isSimulationPlaying && activeExptIndex==0 && AppHost.isVirtualMode && !isAnalysisMode);*/
    }
}