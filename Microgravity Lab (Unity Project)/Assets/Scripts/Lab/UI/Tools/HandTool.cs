using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UIElements;

public class HandTool : MonoBehaviour
{
    public HorizontalScrollController horizontalScrollController;
    public TextMeshProUGUI toolModeShowText;
    public List<HandToolMode> toolModes = new List<HandToolMode>();
    [Space]
    public GameObject need_to_select_sign;
    public GameObject keybindings_sign;

    public HandToolMode activeToolMode;
    Transform cam;
    ILabObject target;
    public int pressedKey = -1;

    private void OnDisable()
    {
        target = null;
        OutlineHost.KeepLastSelected(false);
    }


    private void Awake()
    {
        horizontalScrollController.OnScroll += OnScroll;
        OnScroll(0);
    }

    private void Start()
    {
        cam = LabHost.instance.mainCamera.transform;

        LabHost.labInputManager.OnHandToolScroll += (int dir) =>
        {
            if (pressedKey != -1) return;
            if (transform.gameObject.activeInHierarchy)
            {
                horizontalScrollController.TryScroll(dir);
            }
        };
    }

    void OnScroll(int index)
    {
        foreach(var tm in toolModes)
        {
            tm.gfx_X.SetActive(false);
            tm.gfx_Y.SetActive(false);
            tm.gfx_Z.SetActive(false);
        }
        toolModeShowText.text = toolModes[index].title;
        activeToolMode = toolModes[index];
    }

    private void Update()
    {
        need_to_select_sign.SetActive(target == null);
        keybindings_sign.SetActive(target != null);

        if (Input.GetKey(KeyCode.X)) pressedKey = 0;
        else if (Input.GetKey(KeyCode.Y)) pressedKey = 1;
        else if (Input.GetKey(KeyCode.Z)) pressedKey = 2;
        else pressedKey = -1;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(
                cam.position,
                cam.forward,
                out hit))
            {
                ILabObject labObject = null;
                labObject = hit.transform.GetComponent<ILabObject>();
                target = labObject;
                OutlineHost.KeepLastSelected(target != null);
            }
        }

        AdjustIndicatorGFX();
        if (target == null) return;
        HandleInput();
    }

    void HandleInput()
    {
        float rotateSpeed = 5f;
        float scroll = Input.mouseScrollDelta.y;
        float scale = 0.05f * (target.transform.position - cam.transform.position).magnitude;
        if (activeToolMode.modeIndex == 0)
        {
            Vector3 pos = target.transform.position;
            Vector3 camDir = cam.forward;
            Vector3 camDir10 = new Vector3(
                    camDir.x < 0 ? -1f : 1f,
                    camDir.y < 0 ? -1f : 1f,
                    camDir.z < 0 ? -1f : 1f);
            pos.x += scale * scroll * (pressedKey == 0 ? 1f : 0) * camDir10.x;
            pos.y += scale * scroll * (pressedKey == 1 ? 1f : 0) * camDir10.y;
            pos.z += scale * scroll * (pressedKey == 2 ? 1f : 0) * camDir10.z;
            target.transform.position = pos;
            LabHost.labNET.SendModifyObject(target.GetLabObjectData());
        }
        if (activeToolMode.modeIndex == 1)
        {
            Vector3 rotationCache = target.transform.eulerAngles;
            Vector3 rot = new Vector3();
            rot.x = rotateSpeed * scroll * (pressedKey == 0 ? 1f : 0);
            rot.y = rotateSpeed * scroll * (pressedKey == 1 ? 1f : 0);
            rot.z = rotateSpeed * scroll * (pressedKey == 2 ? 1f : 0);
            target.transform.Rotate(rot, Space.World);
            LabHost.labNET.SendModifyObject(target.GetLabObjectData());

            /*Debug.Log("===========================");
            Debug.Log(target.transform.rotation);
            Debug.Log(rotationCache);*/
            /*if (pressedKey == 0)
            {

            }
            if (pressedKey == 1) rotationCache.y = futureRot.y;
            if (pressedKey == 2) rotationCache.z = futureRot.z;
            target.transform.eulerAngles = rotationCache;
            Debug.Log("----");
            Debug.Log(rotationCache);
            Debug.Log("===========================");*/
        }
    }

    void AdjustIndicatorGFX()
    {
        activeToolMode.gfx_PARENT.SetActive(target != null);
        if (target == null) return;
        activeToolMode.gfx_PARENT.transform.position = target.transform.position;
        float distance = Vector3.Distance(activeToolMode.gfx_PARENT.transform.position, cam.position);
        if(activeToolMode.modeIndex == 1) activeToolMode.gfx_PARENT.transform.localScale = distance * 0.1f * Vector3.one;

        activeToolMode.gfx_X.SetActive(pressedKey == 0 || pressedKey == -1);
        activeToolMode.gfx_Y.SetActive(pressedKey == 1 || pressedKey == -1);
        activeToolMode.gfx_Z.SetActive(pressedKey == 2 || pressedKey == -1);
    }
}

[Serializable]
public class HandToolMode
{
    public string title;
    public int modeIndex;
    public GameObject gfx_PARENT, gfx_X, gfx_Y, gfx_Z;
}
