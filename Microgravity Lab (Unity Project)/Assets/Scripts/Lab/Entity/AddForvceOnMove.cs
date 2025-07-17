using System;
using UnityEngine;

public class AddForvceOnMove : MonoBehaviour
{
    Vector3 lastPos = Vector3.zero;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        this.enabled = false;
        Vector3 nowPos = transform.position;
        Vector3 dir = nowPos - lastPos;
        float diffMag = dir.magnitude;
        if(diffMag > 0)
        {
            float mass = GetComponent<ILabObject>().so.mass;
            //rb.mass = Mathf.Sqrt(mass);
            Vector3 force = dir * 75f;
            rb.AddForce(force);
            //Debug.Log(transform.name + force);
            this.enabled = false;
        }
        lastPos = transform.position;
    }
}
