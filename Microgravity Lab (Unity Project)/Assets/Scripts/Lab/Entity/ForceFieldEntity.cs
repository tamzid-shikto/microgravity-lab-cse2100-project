using System.Collections;
using UnityEngine;

public class ForceFieldEntity : MonoBehaviour
{
    //public Vector3 forceDir = Vector3.up;
    public float forceAmount = 10f;
    public float maxDistance = 10f;
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay :: " + other.transform.name);
        var rb = other.GetComponent<Rigidbody>();
        if (rb == null) rb = other.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            float forceFactor = 1 -(Vector3.Distance(transform.position, rb.position)/maxDistance);
            if(forceFactor  < 0) forceFactor = 0;
            rb.AddForce(transform.up * forceAmount * forceFactor);
            Debug.Log("AddForce :: " + other.transform.name);
        }
    }
}