using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetHost : MonoBehaviour
{
    public static List<MagnetController> magnetControllers = new List<MagnetController>();

    private void FixedUpdate()
    {
        if (!LabEnvironmentManager.simulationRunning) return;
        if(magnetControllers.Count == 0) return;

        List<MagnetController> onlyMagnets = new List<MagnetController>();
        foreach(var c in magnetControllers)
        {
            if(c.isSelfMagnet && c.rb != null) onlyMagnets.Add(c);
        }

        Vector3 centerOfMagnets = CalculateMagneticCenter(onlyMagnets);
        ApplyForceTowardCenter(centerOfMagnets);
    }

    Vector3 CalculateMagneticCenter(List<MagnetController> magnets)
    {
        Vector3 weightedSum = Vector3.zero;
        float totalIntensity = 0f;

        foreach (var magnet in magnets)
        {
            weightedSum += magnet.transform.position * magnet.magnetIntensity;
            totalIntensity += magnet.magnetIntensity;
        }

        return totalIntensity > 0 ? weightedSum / totalIntensity : Vector3.zero;
    }

    void ApplyForceTowardCenter(Vector3 center)
    {
        //Debug.Log("ApplyForceTowardCenter() :: " + magnetControllers.Count);
        foreach (var m in magnetControllers)
        {
            if(m.rb == null) continue;
            Vector3 dir = center - m.transform.position;
            float distanceSqr = dir.sqrMagnitude + 0.01f; // prevent div by zero
            float intensity = m.magnetIntensity;

            // Inverse square falloff
            float attraction = intensity / distanceSqr;

            Vector3 force = dir.normalized * attraction;
            m.rb.AddForce(force);
        }
    }

}