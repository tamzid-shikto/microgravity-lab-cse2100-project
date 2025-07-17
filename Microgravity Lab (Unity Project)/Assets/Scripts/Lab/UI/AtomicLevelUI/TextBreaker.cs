using TMPro;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextBreaker : MonoBehaviour
{
    public int maxLength = 50;
    private TextMeshProUGUI tmp;
    private string rawText = "";
    private string lastFormatted = "";

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (tmp == null) return;

        if (tmp.text != lastFormatted)
        {
            rawText = tmp.text;
            lastFormatted = BreakText(rawText);
            tmp.text = lastFormatted;
        }
    }

    string BreakText(string input)
    {
        string[] words = input.Split(' ');
        string result = "";
        string currentLine = "";

        foreach (string word in words)
        {
            if ((currentLine + word).Length > maxLength)
            {
                result += currentLine.TrimEnd() + "\n";
                currentLine = word + " ";
            }
            else
            {
                currentLine += word + " ";
            }
        }

        result += currentLine.TrimEnd();
        return result;
    }
}
