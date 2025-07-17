using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cam;
    public Vector2 lookSensitivity = Vector2.one;
    public float moveSensitivity = 1f;

    public float yRotation = 17f, xRotation = 128f;

    static bool isLockedToCenter = true;
    static bool tempStatus = true;

    private void Start()
    {
        //yRotation = cam.eulerAngles.y;
        //xRotation = cam.eulerAngles.x+180f;
    }

    public void ApplyRotation(Quaternion rotation)
    {
        Vector3 eangles = rotation.eulerAngles;
        yRotation = eangles.y;
        xRotation = eangles.x;
    }

    public static void LockCursor()
    {
        isLockedToCenter = true;
        tempStatus = true;
        //Debug.Log("LockCursor");
    }

    public static void FreeCursor()
    {
        tempStatus = false;
        //Debug.Log("FreeCursor");
    }

    public static void RevertToLastState()
    {
        tempStatus = isLockedToCenter;
        //Debug.Log("RevertToLastState");
    }

    private void Awake()
    {
        LabHost.cameraController = this;
    }

    private void Update()
    {
        if (isLockedToCenter && tempStatus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Look();
            Movement();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Movement()
    {
        Vector3 movementDir = Vector3.zero;

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)))
        {
            movementDir.z = 1;
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)))
        {
            movementDir.z = -1;
        }

        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
        {
            movementDir.x = -1;
        }
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
        {
            movementDir.x = 1;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            movementDir.y = 1;
        } 
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            movementDir.y = -1;
        }

        movementDir *= moveSensitivity * Time.deltaTime;

        cam.Translate(movementDir);
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity.x * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity.y * Time.deltaTime;

        yRotation += mouseX; 
        xRotation -= mouseY; 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
