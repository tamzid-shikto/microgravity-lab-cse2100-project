using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IconTextSelectionController : MonoBehaviour
{
    public GameObject blue;
    public GameObject blur;

    public void Selected()
    {
        blue.SetActive(true);
    }
    public void UnSelected()
    {
        blue.SetActive(false);
    }
    public void Blur()
    {
        blur.SetActive(true);
    }

    public void UnBlur()
    {
        blur.SetActive(false);
    }
}