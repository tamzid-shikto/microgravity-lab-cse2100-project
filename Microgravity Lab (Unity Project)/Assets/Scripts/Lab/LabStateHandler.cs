using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabStateHandler : MonoBehaviour
{
    public static LabExptState labExptState = LabExptState.NOONE_EXPT;
    public static LabToolState labToolState = LabToolState.HAND_TOOL;
    public static LabWindowState labWindowState = LabWindowState.ENV;
    public static LabPlayPauseState labPlayPauseState = LabPlayPauseState.PLAYING;

    public static List<LabWindowState> WINDOW_STACK = new List<LabWindowState>();

    private void Awake()
    {
        labExptState = LabExptState.NOONE_EXPT;
        labToolState = LabToolState.HAND_TOOL;
        labWindowState = LabWindowState.ENV;
        labPlayPauseState = LabPlayPauseState.PLAYING;
        WINDOW_STACK = new List<LabWindowState>();
}

    private void Update()
    {
        ShowSomeoneElseExpt(labToolState != LabToolState.ANALYSIS_TOOL && labExptState == LabExptState.SOMEONE_ELSE_EXPT);
        ShowNeedToPause(labToolState != LabToolState.ANALYSIS_TOOL && labExptState == LabExptState.NOONE_EXPT);

        PlayPauseSign(labPlayPauseState == LabPlayPauseState.PLAYING);

        LabEnvironmentManager.simulationRunning = labPlayPauseState == LabPlayPauseState.PLAYING;

        ToolSwitcherUI.instance.UpdateUI();

        if(WINDOW_STACK.Count > 0)
        {
            CameraController.FreeCursor();
        }
        else
        {
            CameraController.LockCursor();
        }
    }

    public static void PopWindowStack()
    {
        //Debug.Log("PopWindowStack()");
        WINDOW_STACK.RemoveAt(WINDOW_STACK.Count - 1);
    }

    public static LabWindowState TopWindow()
    {
        //Debug.Log("TOP WINDOW :: " + WINDOW_STACK[WINDOW_STACK.Count - 1].ToString());
        return WINDOW_STACK[WINDOW_STACK.Count - 1];
    }

    void ShowSomeoneElseExpt(bool show)
    {
        LabStatusUIHost.instance.otherDoingExptObj.SetActive(show);
    }
    void ShowNeedToPause(bool show)
    {
        LabStatusUIHost.instance.needToPauseObj.SetActive(show);
    }

    void PlayPauseSign(bool isPlaying)
    {
        LabStatusUIHost.instance.playSignObj.SetActive(isPlaying);
        LabStatusUIHost.instance.pauseSignObj.SetActive(!isPlaying);
    }
}

public enum LabExptState
{
    SOMEONE_ELSE_EXPT,
    NOONE_EXPT,
    SELF_EXPT,
}

public enum LabToolState
{
    HAND_TOOL,
    INVENTORY_TOOL,
    ANALYSIS_TOOL,
}

public enum LabWindowState
{
    LOADING,
    ENV,
    INVENTORY_BACK_WINDOW,
    ESC_WINDOW,
    SAVE_WINDOW,
    CHAT_WINDOW
}

public enum LabPlayPauseState
{
    PLAYING,
    PAUSED,
}