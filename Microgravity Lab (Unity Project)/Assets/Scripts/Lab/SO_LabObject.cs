using UnityEngine;

[CreateAssetMenu(fileName = "SO_LabObject", menuName = "SO/SO_LabObject")]
public class SO_LabObject : ScriptableObject
{
    public string objectName;
    public string objectNID;
    public LabObjectType labObjectType = LabObjectType.OTHER;
    [Space]
    public float length;
    public float width;
    public float height;
    [Space]
    public float density;
    public Vector3 scaleFactor = Vector3.one;
    public float mass;
    public float volume;
    [Space]
    public float selfMagnetism;
    public float towardsMagnetAttraction;
    public float elasticity;
    public float bounciness;
    public float friction;
    [Space]
    public Sprite icon;
    public Mesh volumeMesh;
    public GameObject spawnPrefab;

    public Vector3 GetSize()
    {
        return new Vector3 (width, height, length);
    }
}

public enum LabObjectType
{
    OTHER,
    NATURE,
    SPORT,
    SOLID,
    METAL,
    FORCE_FIELD,
}