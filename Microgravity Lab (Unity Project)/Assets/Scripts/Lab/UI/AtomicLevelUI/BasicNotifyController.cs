using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BasicNotifyController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;

    float time = 3f;
    RectTransform rectTransform;
    Vector3 targetPos;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, 0, rectTransform.localPosition.z);
        targetPos = rectTransform.localPosition;
        //targetPos.y = LabHost.labUIManager.canvasRect.rect.height;
        targetPos.y = Screen.height / 2;

        Destroy(gameObject, time);
    }

    private void Update()
    {
        rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, targetPos, 0.125f*Time.deltaTime/time);
    }

    public void SetText(string _text)
    {
        text.text = _text;
    }
}