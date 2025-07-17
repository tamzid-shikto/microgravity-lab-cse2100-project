using UnityEditor;
using System;
using UnityEngine;

[CustomEditor(typeof(SO_LabObject))]
public class MeshVolumeDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SO_LabObject data = (SO_LabObject)target;

        if (GUILayout.Button("Calculate Volume, Mass, and Dimensions"))
        {
            if (data.volumeMesh != null)
            {
                data.scaleFactor = CalculateScaleFactor(data);

                Vector3 size = MultipleV3(data.volumeMesh.bounds.size, data.scaleFactor);
                data.length = size.z;
                data.width = size.x;
                data.height = size.y;

                float sf = size.x / data.volumeMesh.bounds.size.x;

                data.volume = CalculateMeshVolume(data.volumeMesh) * sf;
                data.mass = data.density * data.volume;

                EditorUtility.SetDirty(data);
                Debug.Log("Auto-calculated volume, mass, and dimensions.");
            }
            else
            {
                Debug.LogWarning("No mesh assigned!");
            }
        }
    }

    private float CalculateMeshVolume(Mesh mesh)
    {
        float volume = 0f;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];

            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }

        return Mathf.Abs(volume);
    }

    private float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return Vector3.Dot(p1, Vector3.Cross(p2, p3)) / 6f;
    }

    private Vector3 CalculateScaleFactor(SO_LabObject so)
    {
        Vector3 result = Vector3.one;
        GameObject parent = so.spawnPrefab;
        GameObject p = null;
        var mr = parent.GetComponentInChildren<MeshRenderer>(true);

        if (mr == null) return result;
        else
        {
            while(p != parent)
            {
                result = MultipleV3(result, mr.transform.localScale);
                p = mr.transform.parent.gameObject;
            }
        }    

        return result;
    }

    Vector3 MultipleV3(Vector3 a, Vector3 b)
    {
        a.x *= b.x;
        a.y *= b.y;
        a.z *= b.z;
        return a;
    }
}
