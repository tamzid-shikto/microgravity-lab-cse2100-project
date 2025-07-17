using System.Collections;
using UnityEngine;
using TMPro;

public class TooltipUIController : MonoBehaviour
{
    public static TooltipUIController instance;

    public RectTransform obj;
    public TextMeshProUGUI tipText;
    public bool showTip = false;

    private void Awake()
    {
        instance = this;
        HideTooltip();
    }

    private void Update()
    {
        Vector3 offset = new Vector3(10f, -20f, 0);
        Vector3 mousePos = Input.mousePosition + offset;
        obj.position = mousePos;
        instance.obj.localScale = showTip ? Vector3.one : Vector3.zero;
    }

    IEnumerator SetText(string text)
    {
        tipText.text = text;
        yield return null;
        obj.gameObject.SetActive(false);
        yield return null;
        obj.gameObject.SetActive(true);
    }

    public static void ShowToolTip(string text)
    {
        instance.StartCoroutine(instance.SetText(text));
        instance.showTip = true;
    }
    public static void HideTooltip()
    {
        instance.showTip = false;
    }
}