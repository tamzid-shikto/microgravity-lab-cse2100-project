using System.Collections;
using UnityEngine;

public class SinWaveEntity : MonoBehaviour
{
    public float rotateSpeed = 10f;
    public float upDownAmount = .1f;
    public float upDownSpeed = .1f;

    private void Start()
    {
        transform.Translate(Vector3.down * upDownAmount / 2f);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        transform.Translate(Vector3.up * Time.deltaTime * upDownAmount * Mathf.Sin(Time.timeSinceLevelLoad * upDownSpeed));
    }
}