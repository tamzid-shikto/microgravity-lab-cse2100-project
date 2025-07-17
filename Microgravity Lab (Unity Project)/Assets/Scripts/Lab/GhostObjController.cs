using System.Collections;
using UnityEngine;

public class GhostObjController : MonoBehaviour
{
    public Material ghostMat;
    public int ghostLayer = 7;
    public float stepOffset = 0.1f;

    public static Vector3 possibleSpawnPosition;

    public static GhostObjController instance;
    static Transform target;
    static Vector3 halfExtends;
    Transform cam;

    public static bool selfMoveGhost = false;

    static float intervalTime = 1f/15f;
    static float time = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cam = LabHost.instance.mainCamera.transform;        
    }

    private void Update()
    {
        if(target != null) target.position = Vector3.Lerp(target.position, possibleSpawnPosition, Time.deltaTime * 12.5f);
        if (target == null || !selfMoveGhost) return;

        RaycastHit hit;
        if (Physics.Raycast(
            cam.position,
            cam.forward,
            out hit))
        {
            possibleSpawnPosition = hit.point;
            for(int i=1; i<=100; i++)
            {
                possibleSpawnPosition -= cam.forward * stepOffset;
                Collider[] cs = Physics.OverlapBox(possibleSpawnPosition, halfExtends);
                if(cs.Length == 0)
                {
                    break;
                }
            }
        }

        target.position = possibleSpawnPosition;

        time += Time.deltaTime;
        if (time >= intervalTime)
        {
            time = 0;
            LabHost.labNET.SendMoveGhost(possibleSpawnPosition);
        }
    }

    public void ClearAll()
    {
        if(target != null) { 
            Destroy(target.gameObject);
            target = null;
            selfMoveGhost = false;
        }
    }

    public void QuickMoveGhost(Vector3 pos)
    {
        if (target == null) return;
        possibleSpawnPosition = pos;
    }

    public void SpawnGhostObj(SO_LabObject so)
    {
        Debug.Log(so == null);
        ClearAll();

        var go = Instantiate(so.spawnPrefab, transform);

        RemoveChildrenComponent<ILabObject>(go);
        RemoveChildrenComponent<Rigidbody>(go);
        RemoveChildrenComponent<SphereCollider>(go);
        RemoveChildrenComponent<BoxCollider>(go);
        RemoveChildrenComponent<MeshCollider>(go);
        RemoveChildrenComponent<OutlineController>(go);

        target = go.transform ;
        go.layer = ghostLayer;

        MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer mr in meshRenderers) mr.sharedMaterial = ghostMat;

        //Mesh m = go.GetComponentInChildren<MeshFilter>(true).mesh;
        halfExtends = new Vector3(so.length, so.height, so.width) / 2f;
    }

    void RemoveChildrenComponent<T>(GameObject go)
    {
        T[] rigidBodys = go.GetComponentsInChildren<T>(true);
        foreach (T c in rigidBodys) Destroy(c as Object);
    }
}