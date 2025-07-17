using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryPreviewController : MonoBehaviour
{
    public static InventoryPreviewController instance;

    public Transform cam;
    public Transform spawnParent;

    private void Awake()
    {
        instance = this;
        Clear();
    }

    public void SpawnObject(SO_LabObject so)
    {
        Clear();
        GameObject go = Instantiate(so.spawnPrefab, spawnParent);
        RemoveChildrenComponent<Rigidbody>(go);
        RemoveChildrenComponent<Collider>(go);
        RemoveChildrenComponent<ILabObject>(go);
        RemoveChildrenComponent<OutlineController>(go);
        RemoveChildrenComponent<Outline>(go);

        go.transform.localPosition = Vector3.zero;
        float distance = so.GetSize().magnitude;

        cam.transform.localPosition = new Vector3(0, 0, distance);
    }

    void Clear()
    {
        foreach (Transform t in spawnParent) Destroy(t.gameObject);
    }

    void RemoveChildrenComponent<T>(GameObject go)
    {
        T[] rigidBodys = go.GetComponentsInChildren<T>(true);
        foreach (T c in rigidBodys) Destroy(c as Object);
    }
}
