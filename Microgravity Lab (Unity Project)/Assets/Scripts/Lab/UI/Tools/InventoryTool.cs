using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InventoryTool : MonoBehaviour
{
    public GameObject inventorySlotPrefab;
    public Transform catalogueGrid;
    [Space]
    public GameObject backObj;
    public GameObject frontObj;

    static Transform cam;
    public static List<InventorySlotCell> inventorySlots = new List<InventorySlotCell>();
    public static SO_LabObject selectedSO;

    static bool isBackUIActive = false;

    private void Start()
    {
        backObj.SetActive(false);
        LoadInventoryCatalogue();
        cam = LabHost.instance.mainCamera.transform;

        LabHost.labInputManager.OnInventoryOpenClose += () =>
        {
            if (!frontObj.activeInHierarchy) return;
            bool futureActiveState = !isBackUIActive;
            if (futureActiveState == false 
                && LabStateHandler.TopWindow() != LabWindowState.INVENTORY_BACK_WINDOW)
            {
                return;
            }else if(futureActiveState == true && LabStateHandler.WINDOW_STACK.Count > 0)
            {
                return;
            }

            backObj.SetActive(futureActiveState);
            if (futureActiveState) LabStateHandler.WINDOW_STACK.Add(LabWindowState.INVENTORY_BACK_WINDOW);
            else LabStateHandler.PopWindowStack();

            isBackUIActive = futureActiveState;
            //Debug.Log("TRY OnInventoryOpenClose :: " + LabStateHandler.WINDOW_STACK.Count);

            //if (backObj.activeInHierarchy) CameraController.FreeCursor();
            //else CameraController.LockCursor();
            TooltipUIController.HideTooltip();
        };
    }

    void LoadInventoryCatalogue()
    {
        foreach(Transform t in catalogueGrid) Destroy(t.gameObject);

        foreach(var so in LabHost.labDataManager.so_LabObjects)
        {
            GameObject go = Instantiate(inventorySlotPrefab, catalogueGrid);
            InventorySlotCell cell = go.GetComponent<InventorySlotCell>();
            cell.SetData(so);
            if (inventorySlots.Count == 0) cell.SelectCell(so);
            inventorySlots.Add(cell);
        }
    }

    private void OnEnable()
    {
        //Debug.Log("ENABLE");
    }

    private void OnDisable()
    {
        GhostObjController.instance.ClearAll();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && frontObj.activeInHierarchy && !backObj.activeInHierarchy)
        {
            LabHost.selfInteractionPipeline.SpawnObject(selectedSO, cam.forward, GhostObjController.possibleSpawnPosition);
        }
        /*if(!backObj.activeInHierarchy)
        {
            if(Input.GetMouseButtonDown(0))
            {
                LabHost.selfInteractionPipeline.SpawnObject(selectedSO, cam.forward, GhostObjController.possibleSpawnPosition);
                RaycastHit hit;
                if (Physics.Raycast(
                    cam.position,
                    cam.forward,
                    out hit))
                {
                    //LabHost.selfInteractionPipeline.SpawnObject(selectedSO, cam.forward, hit.point);
                }
            }
            else
            {
                //Debug.Log("FAIL SPAWN");
            }
        }*/
    }
}


/*
public class InventoryTool : MonoBehaviour
{
    static int selectedBottomCellIndex = 0;
    Transform cam;

    private void Awake()
    {
        cam = LabHost.cameraController.cam;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SpawnObject();
        }
    }

    public static void SelectBottomCell(int index)
    {
        selectedBottomCellIndex = index;
    }

    public void SpawnObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(
            cam.position, 
            cam.forward, 
            out hit))
        {
            LabHost.selfInteractionPipeline.SpawnObject(LabHost.labDataManager.so_LabObjects[selectedBottomCellIndex], cam.forward, hit.point);
        }
    }
}
*/