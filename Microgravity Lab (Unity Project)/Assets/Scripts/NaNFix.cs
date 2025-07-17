using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class NanFix : MonoBehaviour
{
    void Awake()
    {
        if (!Application.isPlaying)
        {
            CheckChildrenForNaN(transform);
        }
    }

    void CheckChildrenForNaN(Transform parent)
    {
        foreach (Transform child in parent)
        {
            RectTransform rect = child.GetComponent<RectTransform>();
            if (rect != null)
            {
                if (float.IsNaN(rect.anchoredPosition.y))
                {
                    Vector2 tmp = rect.anchoredPosition;
                    tmp.y = 0;
                    rect.anchoredPosition = tmp;
                    Debug.LogWarning($"NaN Y position found on: {child.name}", child.gameObject);
                }
                if (float.IsNaN(rect.anchoredPosition.x))
                {
                    Vector2 tmp = rect.anchoredPosition;
                    tmp.x = 0;
                    rect.anchoredPosition = tmp;
                    Debug.LogWarning($"NaN X position found on: {child.name}", child.gameObject);
                }
            }

            // Recursively check deeper children
            CheckChildrenForNaN(child);
        }
    }
}
