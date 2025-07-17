using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabHost : MonoBehaviour
{
    public static LabHost instance;

    public static LabUIManager labUIManager;
    public static LabDataManager labDataManager;
    public static LabEnvironmentManager labEnvironmentManager;
    public static LabInputManager labInputManager;
    public static CameraController cameraController;
    public static SelfInteractionPipeline selfInteractionPipeline;
    public static LabNET labNET;
    public static SaveLabUI saveLabUI;

    public Camera mainCamera;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Physics.gravity = new Vector3(0, -9.8f, 0);

        if (!AppHost.isVirtualMode)
        {
            //LabHost.labDataManager.activeExperimenterIndex.SetData(1);
            LabStateHandler.labExptState = LabExptState.NOONE_EXPT;
            LabHost.labUIManager.LoadComplete();
            LabHost.labEnvironmentManager.envLoaded = true;
        }

        CameraController.LockCursor();
    }
}

