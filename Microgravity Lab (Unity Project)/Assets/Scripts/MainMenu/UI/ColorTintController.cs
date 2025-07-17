using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorTintController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image targetImage;
    [Space]
    public Color normalColor;
    public Color highlightedColor;

    private void Awake()
    {
        targetImage = GetComponent<Image>();
    }

    private void Start()
    {
        targetImage.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetImage.color = highlightedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetImage.color = normalColor;
    }

    private void OnDisable()
    {
        targetImage.color = normalColor;
    }
}