using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCatalogueChip : MonoBehaviour
{
    public static List<InventoryCatalogueChip> inventoryCatalogueChips = new List<InventoryCatalogueChip>();

    public GameObject blue;
    public Button button;
    public bool autoSelect;
    [Space]
    public bool showAll;
    public LabObjectType targetType;

    private void Awake()
    {
        Show(autoSelect);
        inventoryCatalogueChips.Add(this);

        button.onClick.AddListener(() =>
        {
            LoadType(targetType, showAll);
            SetSelected();
        });
    }

    public void SetSelected()
    {
        foreach (var item in inventoryCatalogueChips) item.Show(false);
        Show(true);
    }

    void Show(bool show)
    {
        blue.SetActive(show);
    }

    void LoadType(LabObjectType labObjectType, bool loadAll)
    {
        foreach (var x in InventorySlotCell.inventorySlotCells)
        {
            x.gameObject.SetActive(loadAll || x.so.labObjectType == labObjectType);
        }
    }
}