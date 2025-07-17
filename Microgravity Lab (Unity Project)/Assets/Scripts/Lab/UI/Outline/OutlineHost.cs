using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngineInternal;

public class OutlineHost : MonoBehaviour
{
    Transform cam;
    private void Start()
    {
        cam = LabHost.cameraController.cam;
    }
    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit))
        {
            OutlineController oc = hit.transform.GetComponent<OutlineController>();
            if (oc != null) oc.Show();
            else
            {
                //Debug.Log("NULL :: " + hit.transform.name);
                OutlineController.HideLast();
            }
        }
        else OutlineController.HideLast();
    }

    private void OnDisable()
    {
        OutlineController.HideLast();
    }

    public static void KeepLastSelected(bool keep)
    {
        OutlineController.selectedOne?.ForceHide();
        OutlineController.selectedOne = keep ? OutlineController.last : null;
        OutlineController.selectedOne?.UpdateColor();
    }
}