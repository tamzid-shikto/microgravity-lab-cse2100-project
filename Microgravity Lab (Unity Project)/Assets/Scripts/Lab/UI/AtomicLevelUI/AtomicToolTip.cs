using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AtomicToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string message;
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUIController.ShowToolTip(message);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUIController.HideTooltip();
    }
}