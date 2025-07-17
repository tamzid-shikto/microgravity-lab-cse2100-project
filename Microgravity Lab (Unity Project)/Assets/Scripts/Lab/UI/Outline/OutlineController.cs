using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public Outline outlineScript;

    public static OutlineController last;
    public static OutlineController selectedOne;

    private void Start()
    {
        UpdateColor();
    }

    public void Show()
    {
        if (outlineScript.enabled) return;
        HideLast();
        UpdateColor();
        outlineScript.enabled = true;
        last = this;
    }

    public void UpdateColor()
    {
        //Debug.Log("UpdateColor");
        outlineScript.OutlineColor = selectedOne == this ? Color.yellow : Color.white;
        outlineScript.OutlineWidth = selectedOne == this ? 10f : 7.5f;
    }

    public void Hide()
    {
        if (selectedOne == this) return;
        outlineScript.enabled = false;
    }

    public void ForceHide()
    {
        outlineScript.enabled = false;
    }

    public static void HideLast()
    {
        if (last != null)
        {
            last.Hide();
        }
    }
}
