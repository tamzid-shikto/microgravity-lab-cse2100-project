using UnityEngine;

[ExecuteInEditMode]
public class GridArrangerWatcher : MonoBehaviour
{
    [SerializeField] private bool autoArrange = true;
    private int lastChildCount = -1;

    private void Update()
    {
        if (!autoArrange || Application.isPlaying) return;

        int currentCount = transform.childCount;
        if (currentCount != lastChildCount)
        {
            lastChildCount = currentCount;
            ArrangeChildren();
        }
    }

    private void OnEnable()
    {
        ArrangeChildren();
    }

    private void ArrangeChildren()
    {
        int count = transform.childCount;
        int columns = Mathf.CeilToInt(Mathf.Sqrt(count));

        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            int row = i / columns;
            int col = i % columns;

            Vector3 newLocalPos = new Vector3(col * 2f, 0, row * 2f);
            child.localPosition = newLocalPos;
        }
    }
}
