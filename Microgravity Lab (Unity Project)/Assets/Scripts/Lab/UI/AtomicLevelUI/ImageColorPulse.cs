using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageColorPulse : MonoBehaviour
{
    public Image image;
    public float timePeriod = 2f;

    [Space]
    public Gradient colorRange;

    private void Update()
    {
        Color col1 = colorRange.colorKeys[0].color;
        Color col2 = colorRange.colorKeys[1].color;

        float t = (Mathf.Sin(Time.time * (2 * Mathf.PI / timePeriod)) + 1) / 2;
        image.color = Color.Lerp(col1, col2, t);
    }
}