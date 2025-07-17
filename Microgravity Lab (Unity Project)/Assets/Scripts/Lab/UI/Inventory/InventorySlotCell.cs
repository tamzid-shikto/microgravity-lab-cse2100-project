using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Windows;

public class InventorySlotCell : MonoBehaviour
{
    public static List<InventorySlotCell> inventorySlotCells = new List<InventorySlotCell>();

    public Image blue;
    public Image icon;
    public AtomicToolTip tooltip;
    public TextMeshProUGUI nameText;
    public Button button;
    public SO_LabObject so;

    private void Awake()
    {
        inventorySlotCells.Add(this);
    }
    private void OnDestroy()
    {
        inventorySlotCells.Remove(this);
    }

    private void Start()
    {
        
    }

    public void SetData(SO_LabObject so_)
    {
        so = so_;
        icon.sprite = so.icon;
        tooltip.message = so.objectName;

        string cut = so.objectName.Length > 12 ? so.objectName.Substring(0, 12)+"..." : so.objectName;
        nameText.text = cut;

        button.onClick.AddListener(() =>
        {
            SelectCell(so);
        });
    }

    public void Unselect()
    {
        blue.enabled = false;
    }

    public void SelectCell(SO_LabObject so)
    {
        InventoryBackObjectPreviewController.instance.SetData(so);
        foreach(var cell in InventoryTool.inventorySlots) cell.Unselect();
        blue.enabled = true;
        InventoryFrontController.instance.SetData(so);
        InventoryTool.selectedSO = so;
    }
}
