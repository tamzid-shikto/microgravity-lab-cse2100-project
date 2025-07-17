using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InspectTool : MonoBehaviour
{
    public GameObject needToSelectSignObj;
    public GameObject tipSignObj;
    public List<GameObject> UIObjs;
    [Space]
    public Transform velocityGFX;

    [Header("Transform")]
    [Space]
    public TMP_InputField posX;
    public TMP_InputField posY;
    public TMP_InputField posZ;
    [Space]
    public TMP_InputField rotX;
    public TMP_InputField rotY;
    public TMP_InputField rotZ;

    [Header("Motion")]
    [Space]
    public TMP_InputField velX;
    public TMP_InputField velY;
    public TMP_InputField velZ;
    [Space]
    public TMP_InputField accX;
    public TMP_InputField accY;
    public TMP_InputField accZ;

    public static InspectTool instance;
    static Transform target;
    static ILabObject labObj;
    static Rigidbody rb;
    static Transform cam;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cam = LabHost.instance.mainCamera.transform;
    }

    private void OnDisable()
    {
        target = null;
        OutlineHost.KeepLastSelected(false);
    }

    private void Update()
    {
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
                if (labObject == null)
                {
                    target = null;
                    OutlineHost.KeepLastSelected(false);
                }
                else
                {
                    target = labObject.transform;
                    rb = labObject.rb;
                    labObj = labObject;
                    OutlineHost.KeepLastSelected(true);
                }
            }
            else
            {
                target = null;
                OutlineHost.KeepLastSelected(false);
            }
        }

        needToSelectSignObj.SetActive(target == null);
        tipSignObj.SetActive(target != null);
        foreach(var go in UIObjs) go.SetActive(target != null);

        if (target == null) return;

        posX.text = target.position.x.ToString("F2");
        posY.text = target.position.y.ToString("F2");
        posZ.text = target.position.z.ToString("F2");

        rotX.text = (target.eulerAngles.x % 360f).ToString("F1");
        rotY.text = (target.eulerAngles.y % 360f).ToString("F1");
        rotZ.text = (target.eulerAngles.z % 360f).ToString("F1");

        velX.text = rb.velocity.x.ToString("F2");
        velY.text = rb.velocity.y.ToString("F2");
        velZ.text = rb.velocity.z.ToString("F2");

        Vector3 acceleration = labObj.GetAcceleration();
        accX.text = acceleration.x.ToString("F2");
        accY.text = acceleration.y.ToString("F2");
        accZ.text = acceleration.z.ToString("F2");

        velocityGFX.position = target.position;
        velocityGFX.forward = rb.velocity.normalized;
        float distance = Vector3.Distance(velocityGFX.position, cam.position);
        velocityGFX.localScale = distance * 0.125f * Vector3.one;
    }
}

