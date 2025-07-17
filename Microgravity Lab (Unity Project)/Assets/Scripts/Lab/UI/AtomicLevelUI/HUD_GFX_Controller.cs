using System;
using UnityEngine;

public class HUD_GFX_Controller : MonoBehaviour
{
    public float scale = 1f;
    [Space]
    public Transform X, Y, Z;
    public Transform GFX;
    public Action<int> OnInput;

    Camera camera;
    Transform cam;
    bool isMouseDown = false;
    Vector3 initPos;
    Vector3 dragOffset;
    Vector3 dirVector;
    Vector3 targetStartPos;
    Transform target;


    private void Start()
    {
        camera = LabHost.instance.mainCamera;
        cam = camera.transform;
    }

    public void SetTarget(GameObject targetObj)
    {
        target = targetObj.transform;
    }

    private void Update()
    {
        GFX.gameObject.SetActive(target != null);

        if (target != null)
        {
            AdjustIndicatorGFX();
            HandleInput();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("----" + hit.transform.name);
                float x = hit.transform == X ? 1f : 0;
                float y = hit.transform == Y ? 1f : 0;
                float z = hit.transform == Z ? 1f : 0;

                if (x + y + z != 0)
                {
                    isMouseDown = true;
                    initPos = Input.mousePosition;
                    targetStartPos = target.position;
                    dirVector = new Vector3(x, y, z);

                    // Calculate initial drag offset
                    Vector3 worldMousePos = camera.ScreenToWorldPoint(new Vector3(initPos.x, initPos.y, camera.WorldToScreenPoint(target.position).z));
                    dragOffset = target.position - worldMousePos;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            X.gameObject.SetActive(false);
            Y.gameObject.SetActive(false);
            Z.gameObject.SetActive(false);

            X.gameObject.SetActive(true);
            Y.gameObject.SetActive(true);
            Z.gameObject.SetActive(true);
        }
    }

    void HandleInput()
    {
        if (isMouseDown)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = camera.WorldToScreenPoint(target.position).z; // Maintain depth

            Vector3 newPosition = camera.ScreenToWorldPoint(mousePos) + dragOffset;

            // Preserve non-selected axes
            newPosition.x = targetStartPos.x * (1 - dirVector.x) + newPosition.x * dirVector.x;
            newPosition.y = targetStartPos.y * (1 - dirVector.y) + newPosition.y * dirVector.y;
            newPosition.z = targetStartPos.z * (1 - dirVector.z) + newPosition.z * dirVector.z;

            target.position = newPosition;
        }
    }

    void AdjustIndicatorGFX()
    {
        GFX.position = target.position;
        float distance = Vector3.Distance(GFX.position, cam.position);
        GFX.localScale = distance * scale * Vector3.one;
    }
}
