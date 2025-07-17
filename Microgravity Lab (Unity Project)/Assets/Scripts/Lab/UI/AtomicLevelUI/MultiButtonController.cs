using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiButtonController : MonoBehaviour
{
    public int autoSelectedIndex;
    [Space]
    public Color normalColor;
    public Color selectedColor;

    List<Button> buttons;
    public Action<int> OnButtonClick;

    private void Awake()
    {
        buttons = new List<Button>(GetComponentsInChildren<Button>());
        SetSelected(autoSelectedIndex >= 0 ? buttons[autoSelectedIndex] : null);

        int count = 0;
        foreach (var button in buttons)
        {
            int _ = count + 0;
            count++;
            button.onClick.AddListener(() =>
            {
                SetSelected(button);
                OnButtonClick?.Invoke(_);
            });
        }
    }

    public void SilentSelect(int index)
    {
        autoSelectedIndex = index;
        try
        {
            SetSelected(buttons[index]);
        }catch(Exception ex) { }
    }
    
    void SetSelected(Button target)
    {
        foreach(var button in buttons)
        {
            button.GetComponent<Image>().color = (target == button) ? selectedColor : normalColor;
        }
    }
}