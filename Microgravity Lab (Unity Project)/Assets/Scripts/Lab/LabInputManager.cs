using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabInputManager : MonoBehaviour
{
    public Action<int> OnToolSwitch;
    public Action<int> OnQuickInventoryScroll;
    public Action<int> OnHandToolScroll;
    public Action OnPlayPausePress;
    public Action OnChatOpenClose;
    public Action OnRoleSwitchPress;
    public Action OnEscMenuOpenClose;
    public Action OnInventoryOpenClose;

    private void Awake()
    {
        LabHost.labInputManager = this;
    }

    private void Start()
    {
        OnToolSwitch += (int toolIndex) => {
            if(toolIndex == 0) LabStateHandler.labToolState = LabToolState.HAND_TOOL;
            if(toolIndex == 1) LabStateHandler.labToolState = LabToolState.INVENTORY_TOOL;
            if(toolIndex == 2) LabStateHandler.labToolState = LabToolState.ANALYSIS_TOOL;
        };
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightShift))
        {
            for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
            {
                if (Input.GetKeyDown(key)) LabHost.labDataManager.DEVTOOLS_KEYDOWN.SetData(key);
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscMenuOpenClose?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnInventoryOpenClose?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            OnChatOpenClose?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.P)) OnPlayPausePress?.Invoke();
        if (Input.GetKeyDown(KeyCode.R)) OnRoleSwitchPress?.Invoke();

        if (Input.GetKeyDown(KeyCode.Alpha1)) OnToolSwitch?.Invoke(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) OnToolSwitch?.Invoke(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) OnToolSwitch?.Invoke(2);

        if (Input.mouseScrollDelta.y > 0)
        {
            OnQuickInventoryScroll?.Invoke(-1);
            OnHandToolScroll?.Invoke(-1);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            OnQuickInventoryScroll?.Invoke(1);
            OnHandToolScroll?.Invoke(1);
        }
    }
}
