using System.Collections;
using UnityEngine;

public class MagnetController : MonoBehaviour
{
    public bool isSelfMagnet = false;
    public float magnetIntensity = 0f;
    public Rigidbody rb;

    private void OnEnable()
    {
        MagnetHost.magnetControllers.Add(this);
    }

    private void OnDisable()
    {
        MagnetHost.magnetControllers.Remove(this);
    }
}