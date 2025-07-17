using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MeinMenuRoomListEntryUIPiece : MonoBehaviour
{
    public Button joinButton;
    public TextMeshProUGUI roomHostNameText;
    public TextMeshProUGUI roomSizeText;

    public void SetData(string hostName, string size, Action joinCallBack)
    {
        roomHostNameText.text = hostName;
        roomSizeText.text = size;
        joinButton.onClick.AddListener(() =>
        {
            joinCallBack?.Invoke();
        });
    }
}
