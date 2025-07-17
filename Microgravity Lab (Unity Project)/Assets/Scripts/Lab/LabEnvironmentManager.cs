using LabData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class LabEnvironmentManager : MonoBehaviour
{
    public Transform labObjectParent;
    public FixedSizeList<LabObjectStateData> labObjectCache = new FixedSizeList<LabObjectStateData>(50 * 10);
    public int TICK_COUNT = 0;

    public static bool simulationRunning = true;
    public bool envLoaded = false;

    public static float gravity;
    private void Awake()
    {
        LabHost.labEnvironmentManager = this;
        Physics.simulationMode = SimulationMode.Script;
    }

    private void Start()
    {
        /*LabHost.labDataManager.simulationRunningState.OnChange += (bool playing) =>
        {
            simulationRunning = playing;
        };*/
        LabHost.labDataManager.modifiedTick.OnChange += (int tick) =>
        {
            if (tick != TICK_COUNT) AdjustTick(tick);
        };

        if (AppHost.savedLabData != null)
        {
            //Debug.Log("AppHost.savedLabData.camPosition :: " + JsonUtility.ToJson(AppHost.savedLabData.camPosition));
            //Debug.Log("AppHost.savedLabData.camRotation :: " + JsonUtility.ToJson(AppHost.savedLabData.camRotation));
            LabHost.instance.mainCamera.transform.position = AppHost.savedLabData.camPosition;
            LabHost.cameraController.ApplyRotation(AppHost.savedLabData.camRotation);
            LoadSnapshot(AppHost.savedLabData.snapshot);
        }
    }

    private void FixedUpdate()
    {
        if (simulationRunning && envLoaded)
        {
            //Debug.Log("SIM :: " + TICK_COUNT);
            QuickAdvanceTicks(1);
            SaveTick();
            TICK_COUNT++;
        }
    }

    public void UpdateGravity(float g, bool updateESC, bool forceUpdateScroll)
    {
        gravity = g;
        Physics.gravity = Vector3.up * gravity * -1f;
        if(updateESC)
        {
            GameUtility.DoAtNextFrame(() =>
            {
                GameUtility.DoAtNextFrame(() =>
                {
                    ESC_MenuUI.instance.SetGravityUI(g, forceUpdateScroll);
                });
            });
        }
        Debug.Log("Gravity Updated :: " + gravity);
    }

    public string GetSnapshot()
    {
        return JsonUtility.ToJson(labObjectCache.GetLast());
    }

    public void LoadSnapshot(string snapshotData)
    {
        if (snapshotData == "") return;
        LabObjectStateData stateData = JsonUtility.FromJson<LabObjectStateData>(snapshotData);
        ApplySnapshot(stateData);
    }

    void ApplySnapshot(LabObjectStateData stateData)
    {
        foreach(var loData in stateData.labObjectDatas)
        {
            var lo = ILabObject.FindByOID(loData.oid);
            if(lo == null)
            {
                lo = SpawnSingleObject(loData.nid, loData.oid);
            }
            lo.LoadLabObjectData(loData);
        }
        TICK_COUNT = stateData.tick;
        UpdateGravity(stateData.gravity, true, true);
        envLoaded = true;
    }

    public void ExecuteSpawnOrder(ObjectSpawnOrder spawnOrder)
    {
        if(spawnOrder.tick < TICK_COUNT)
        {
            AdjustTick(spawnOrder.tick);
        }

        var lo = SpawnSingleObject(spawnOrder.prefabNID, spawnOrder.objectID);
        lo.SetBasicData(spawnOrder.objectID, spawnOrder.position, spawnOrder.rotation);
    }

    public void ExecuteObjectModification(LabObjectData loData)
    {
        ILabObject.FindByOID(loData.oid)?.LoadLabObjectData(loData);
    }

    ILabObject SpawnSingleObject(string prefabNID, string objectID)
    {
        //Debug.Log("prefabNID :: " + prefabNID);
        SO_LabObject so = LabHost.labDataManager.GetObjectSOByNID(prefabNID);
        var go = Instantiate(LabHost.labDataManager.GetObjectSOByNID(prefabNID).spawnPrefab, labObjectParent);
        var lo = go.GetComponent<ILabObject>();
        var rb = go.GetComponent<Rigidbody>();

        rb.mass = so.mass;

        PhysicMaterial bouncyMat = new PhysicMaterial("RuntimeBouncy");
        bouncyMat.bounciness = so.bounciness;
        bouncyMat.dynamicFriction = so.friction;
        bouncyMat.staticFriction = so.friction;
        bouncyMat.bounceCombine = PhysicMaterialCombine.Maximum;
        bouncyMat.frictionCombine = PhysicMaterialCombine.Minimum;

        MeshCollider[] meshColliders = go.GetComponentsInChildren<MeshCollider>(true);
        foreach (MeshCollider c in meshColliders) c.material = bouncyMat;
        BoxCollider[] boxColliders = go.GetComponentsInChildren<BoxCollider>(true);
        foreach (BoxCollider c in boxColliders) c.material = bouncyMat;
        SphereCollider[] sphereColliders = go.GetComponentsInChildren<SphereCollider>(true);
        foreach (SphereCollider c in sphereColliders) c.material = bouncyMat;

        InitializePhysicsForObject(go.transform);

        SaveTick();
        return lo;
    }

    void InitializePhysicsForObject(Transform target)
    {
        ILabObject targetLabO = target.GetComponent<ILabObject>();  
        foreach(var ilabobj in ILabObject.labObjects)
        {
            ilabobj.rb.isKinematic = ilabobj != targetLabO;
            foreach(var c in ilabobj.colliders) c.enabled = ilabobj == targetLabO;
        }

        /*List<GameObject> tmp = new List<GameObject>();
        foreach(Transform t in labObjectParent)
        {
            var go = t.gameObject;
            tmp.Add(go);
            go.SetActive(t == target);
        }*/

        AppHost.instance.SetUnlimitedFramerate();
        GameUtility.DoAtNextFrame(() =>
        {
            QuickAdvanceTicks(1);
            /*foreach (GameObject go in tmp)
            {
                go.SetActive(true);
            }*/
            foreach (var ilabobj in ILabObject.labObjects)
            {
                ilabobj.rb.isKinematic = false;
                foreach (var c in ilabobj.colliders) c.enabled = true;
            }
            AppHost.instance.Set60Framerate();
        });
    }

    void SaveTick()
    {
        List<LabObjectData> loDatas = new List<LabObjectData>();
        foreach(var lo in ILabObject.labObjects){
            loDatas.Add(lo.GetLabObjectData());
        }
        var objectState = new LabObjectStateData
        {
            tick = TICK_COUNT + 0,
            gravity = gravity,
            labObjectDatas = loDatas.ToArray(),
        };

        labObjectCache.Add(objectState);
    }

    void AdjustTick(int targetTick)
    {
        if (targetTick <= 0 || !envLoaded) return;
        //Debug.Log("ADJUST FROM: " + TICK_COUNT + " TO: " + targetTick);
        //LabHost.labDataManager.OnBasicNotify?.Invoke("ADJUST FROM: " + TICK_COUNT + " TO: " + targetTick);

        var arr = labObjectCache.ToArray();
        var lastTick = arr[arr.Length-1].tick;
        int diff = (lastTick - targetTick);
        if (diff == 0) return;
        else if(diff < 0)
        {
            QuickAdvanceTicks(-diff);
            TICK_COUNT = targetTick;
            return;
        }

        var targetCache = arr[arr.Length - 1 - diff];
        ApplySnapshot(targetCache);

        labObjectCache.RemoveNFromLast(diff);

        TICK_COUNT = targetTick;
    }

    void QuickAdvanceTicks(int tickCount)
    {
        float newFDT = Time.fixedDeltaTime;// / 50f;
        for (int i = 0; i < tickCount; i++) Physics.Simulate(newFDT);
        /*for (int x = 0; x < 50; x++)
        {
        }*/
    }
}

[Serializable]
public class LabObjectStateData
{
    public int tick;
    public float gravity;
    public LabObjectData[] labObjectDatas;
}
public struct ObjectSpawnOrder
{
    public string objectID;
    public string prefabNID;
    public int tick;
    public Vector3 position;
    public Quaternion rotation;
}


/*
[Serializable]
public struct _RigidbodyState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
    public float mass;
    public float drag;
    public float angularDrag;
    public string prefabNID;
    public string objectID;

    public RigidbodyState(Rigidbody rb, string _prefabNID, string _objectID)
    {
        position = rb.position;
        rotation = rb.rotation;
        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;
        mass = rb.mass;
        drag = rb.drag;
        angularDrag = rb.angularDrag;
        prefabNID = _prefabNID;
        objectID = _objectID;
    }

    public void ApplyToRigidbody(Rigidbody rb)
    {
        rb.position = position;
        rb.rotation = rotation;
        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
    }
}*/