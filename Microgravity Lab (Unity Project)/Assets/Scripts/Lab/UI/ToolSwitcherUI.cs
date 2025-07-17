using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolSwitcherUI : MonoBehaviour
{
    public List<Image> backgrounds;
    int currentIndex = 0;

    public List<GameObject> toolUIs;

    public static ToolSwitcherUI instance;

    int lastWorkedIndex = -2;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LabHost.labInputManager.OnToolSwitch += (int index) =>
        {
            currentIndex = index;
            UpdateUI();
        };
        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (Image go in backgrounds)
        {
            go.enabled = (false);
        }
        backgrounds[currentIndex].enabled = (true);

        //if (LabStateHandler.labExptState == LabExptState.NOONE_EXPT) SwitchToolUI(-1);
        //if (LabStateHandler.labExptState == LabExptState.SOMEONE_ELSE_EXPT) SwitchToolUI(-1);
        if (LabStateHandler.labToolState == LabToolState.ANALYSIS_TOOL || LabStateHandler.labExptState == LabExptState.SELF_EXPT) SwitchToolUI(currentIndex);
        else SwitchToolUI(-1);

        //int newIndex = LabHost.labDataManager.currentTool.GetData();
        //currentIndex = newIndex;

        /*foreach (Image go in backgrounds)
        {
            go.enabled = (false);
        }

        backgrounds[currentIndex].enabled = (true);
        if(!AppHost.isVirtualMode)
        {
            SwitchToolUI(currentIndex);
            return;
        }
        if (currentIndex == 2)
        {
            SwitchToolUI(2);
        }
        else if(LabHost.labDataManager.activeExperimenterIndex.GetData() == 1 
            && !LabHost.labDataManager.simulationRunningState.GetData())
        {
            SwitchToolUI(currentIndex);
        }
        else
        {
            SwitchToolUI(-1);
        }*/
    }

    void SwitchToolUI(int index)
    {
        if (lastWorkedIndex == index) return;
        lastWorkedIndex = index;

        foreach (GameObject obj in toolUIs) obj.SetActive(false);
        if(index != -1) toolUIs[index].SetActive(true);
    }

    /*
    private void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            currentIndex--;
            UpdateUI();
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            currentIndex++;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (currentIndex < 0)
        {
            currentIndex = 0;
            return;
        }

        if (currentIndex >= backgrounds.Count)
        {
            currentIndex = backgrounds.Count-1;
            return;
        }

        foreach (Image go in backgrounds)
        {
            go.enabled = (false);
        }
        backgrounds[currentIndex].enabled = (true);
        OnSwitch?.Invoke(currentIndex);
    }*/
}
