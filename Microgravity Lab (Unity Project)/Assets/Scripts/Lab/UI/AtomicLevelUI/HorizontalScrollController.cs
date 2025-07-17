using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalScrollController : MonoBehaviour
{
    public int perScrollShiftAmount = 0;
    public int spacing;
    public Action<int> OnScroll;

    List<GameObject> elements = new List<GameObject>();
    float smoothFactor = 0.125f*100;
    int index = 0;

    private void Awake()
    {
        foreach(Transform t in transform)
        {
            elements.Add(t.gameObject);
        }
    }

    private void Start()
    {
        TryScroll(0);
    }

    private void Update()
    {
        Vector3 target = transform.localPosition;
        target.x = -1 * index * (perScrollShiftAmount+spacing);
        transform.localPosition = Vector3.Lerp(transform.localPosition, target, smoothFactor*Time.deltaTime);
    }

    public void TryScroll(int dir)
    {
        int newIndex = index + dir;
        newIndex = Mathf.Clamp(newIndex, 0, elements.Count-1);
        if (newIndex == index) return;
        index = newIndex;
        OnScroll?.Invoke(index);
    }
}