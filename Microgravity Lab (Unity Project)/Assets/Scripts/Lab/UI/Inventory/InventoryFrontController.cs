using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryFrontController : MonoBehaviour
{
    public Image icon;
    [Space]
    public TextMeshProUGUI title;
    public TextMeshProUGUI subTitle;

    public static InventoryFrontController instance;
    static SO_LabObject lastSO;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        if(lastSO != null)
        {
            SetData(lastSO);
        }   
    }

    public void SetData(SO_LabObject so)
    {
        lastSO = so;
        title.text = so.objectName;
        subTitle.text = $"Size: {so.length.ToString("F2")} x {so.width.ToString("F2")} x {so.height.ToString("F2")} meter\nMass: {so.mass.ToString("F2")} kg";

        icon.sprite = so.icon;
        GhostObjController.instance.SpawnGhostObj(so);
        GhostObjController.selfMoveGhost = true;
        LabHost.labNET.SendSpawnGhost(so.objectNID);
        InventoryPreviewController.instance.SpawnObject(so);
    }
}
