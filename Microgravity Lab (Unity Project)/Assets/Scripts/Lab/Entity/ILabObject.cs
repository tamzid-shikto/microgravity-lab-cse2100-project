using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ILabObject : MonoBehaviour
{
    public static List<ILabObject> labObjects = new List<ILabObject>();

    Vector3 acceleration, lastVelocity;
    public static ILabObject FindByOID(string iod)
    {
        foreach (ILabObject obj in labObjects) if(obj.objectID == iod) return obj;
        return null;
    }

    protected string objectID;
    [SerializeField] public SO_LabObject so;
    public Rigidbody rb;
    [HideInInspector] public List<Collider> colliders;

    protected int[] i_array = new int[0];
    protected float[] f_array = new float[0];
    protected string[] s_array = new string[0];

    private void OnEnable()
    {
        labObjects.Add(this);
    }
    private void OnDisable()
    {
        labObjects.Remove(this);
    }

    private void Start()
    {
        colliders = new List<Collider>(gameObject.GetComponentsInChildren<Collider>());
    }

    public LabObjectData GetLabObjectData()
    {
        var loData = new LabObjectData();
        loData.oid = objectID;
        loData.nid = so.objectNID;

        loData.position = transform.position;
        loData.rotation = transform.rotation;
        loData.localScale = transform.localScale;
        loData.rbData = new RBData(rb);

        loData.i_array = (int[]) i_array.Clone();
        loData.f_array = (float[]) f_array.Clone();
        loData.s_array = (string[]) s_array.Clone();

        return loData;
    }

    public void LoadLabObjectData(LabObjectData loData)
    {
        objectID = loData.oid;
        so = LabHost.labDataManager.GetObjectSOByNID(loData.nid);

        transform.position = loData.position;
        transform.rotation = loData.rotation;
        transform.localScale = loData.localScale;
        loData.rbData.ApplyToRB(rb);
        
        i_array = (int[]) loData.i_array.Clone();
        f_array = (float[]) loData.f_array.Clone();
        s_array = (string[]) loData.s_array.Clone();

        OnObjectDataLoad();
    }

    public void SetBasicData(string oid, Vector3 pos, Quaternion rot)
    {
        objectID= oid;
        transform.position = pos;
        transform.rotation = rot;
    }

    protected void OnObjectDataLoad()
    {

    }

    private void FixedUpdate()
    {
        if (!LabEnvironmentManager.simulationRunning) return;
        acceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = rb.velocity;
    }

    public Vector3 GetAcceleration()
    {
        return acceleration;
    }
}

[Serializable]
public struct LabObjectData
{
    public string oid;
    public string nid;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;
    public RBData rbData;
    public int[] i_array;
    public float[] f_array;
    public string[] s_array;
}

[Serializable]
public struct RBData
{
    public Vector3 velocity;
    public Vector3 angularVelocity;
    public float mass;
    public float drag;
    public float angularDrag;

    public RBData(Rigidbody rb)
    {
        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;
        mass = rb.mass;
        drag = rb.drag;
        angularDrag = rb.angularDrag;
    }

    public void ApplyToRB(Rigidbody rb)
    {
        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
    }
}
