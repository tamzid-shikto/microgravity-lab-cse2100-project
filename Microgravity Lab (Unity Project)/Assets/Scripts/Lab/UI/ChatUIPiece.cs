using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class ChatUIPiece : MonoBehaviour
{
    public _ChatUIPart basic, self, green, red, yellow;

    void HideAll()
    {
        basic.obj.SetActive(false);
        self.obj.SetActive(false);
        green.obj.SetActive(false);
        red.obj.SetActive(false);
        yellow.obj.SetActive(false);
    }

    public void SetData(ChatEventType chatEventType, string message, string sender)
    {
        HideAll();
        if (chatEventType == ChatEventType.BASIC)
        {
            basic.obj.SetActive(true);
            basic.text.text = message;
            basic.senderText.text = sender;
        }
        if (chatEventType == ChatEventType.SELF)
        {
            self.obj.SetActive(true);
            self.text.text = message;
        }
        else if (chatEventType == ChatEventType.GREEN)
        {
            green.obj.SetActive(true);
            green.text.text = message;
        }
        else if (chatEventType == ChatEventType.RED)
        {
            red.obj.SetActive(true);
            red.text.text = message;
        }
        else if (chatEventType == ChatEventType.YELLOW)
        {
            yellow.obj.SetActive(true);
            yellow.text.text = message;
        }
    }
}

[Serializable]
public class _ChatUIPart
{
    public GameObject obj;
    public TextMeshProUGUI text;
    public TextMeshProUGUI senderText;
    public TextMeshProUGUI timeText;
}