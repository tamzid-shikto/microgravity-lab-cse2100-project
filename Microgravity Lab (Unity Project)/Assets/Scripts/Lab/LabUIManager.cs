using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class LabUIManager : MonoBehaviour
{
    public Transform notifyParent;
    public GameObject basicNotifyPrefab;
    public RectTransform canvasRect;

    public GameObject promptUIObj;
    public TextMeshProUGUI promptUITitleText;
    public TextMeshProUGUI promptInputText;
    public Button promptUIDoneButton;

    public PlayerListUI playerListUI;

    [Space]
    public GameObject loadingObj;

    private void Awake()
    {
        LabHost.labUIManager = this;
        loadingObj.SetActive(true);
    }

    public void LoadComplete()
    {
        loadingObj.SetActive(false);
        LabStateHandler.labWindowState = LabWindowState.ENV;
    }

    void Start()
    {
        LabHost.labDataManager.OnBasicNotify += BasicNotify;
    }

    void BasicNotify(string text)
    {
        GameObject go = Instantiate(basicNotifyPrefab, notifyParent);
        BasicNotifyController controller = go.GetComponent<BasicNotifyController>();
        controller.SetText(text);
    }

    public void Prompt(string title, Action<string> callback)
    {
        promptUITitleText.text = title;
        promptUIObj.SetActive(true);
        promptUIDoneButton.onClick.RemoveAllListeners();
        promptUIDoneButton.onClick.AddListener(() =>
        {
            string input = promptInputText.text.Trim();
            if(input.Length == 0)
            {
                LabHost.labDataManager.OnBasicNotify?.Invoke("Input must not be empty");
            }
            else if (!Regex.IsMatch(input, @"^[a-zA-Z0-9_ ]*$"))
            {
                LabHost.labDataManager.OnBasicNotify?.Invoke("Input can only contain letters, numbers & underscore");
            }
            else
            {
                promptUIObj.SetActive(false);
                callback?.Invoke(input);
            }
        });
    }
}
