using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;

public class MainMenuUIManager : MonoBehaviour
{
    public GameObject homeUIObj;
    public GameObject prelabUIObj;
    [Space]
    public GameObject changeNameUIObj;
    public TextMeshProUGUI playerNameTxt;
    public Button changeNameButton;
    [Space]
    public Button cancelButton;
    public Button saveButton;
    public TMP_InputField nameInput;
    [Space]
    public GameObject notificationPrefab;

    public void Notify(string text)
    {
        GameObject go = Instantiate(notificationPrefab);
        BasicNotifyController controller = go.GetComponent<BasicNotifyController>();
        controller.SetText(text);
    }

    private void Awake()
    {
        MainMenuHost.mainMenuUIManager = this;
        changeNameUIObj.SetActive(false);

        cancelButton.onClick.AddListener(() =>
        {
            changeNameUIObj.SetActive(false);
        });
        changeNameButton.onClick.AddListener(() =>
        {
            nameInput.text = GameDB.selfName;
            changeNameUIObj.SetActive(true);
        });

        saveButton.onClick.AddListener(() =>
        {
            string name = nameInput.text;
            if (name.Length == 0)
            {
                Notify("Name must not be empty");
                return;
            }
            else if (!Regex.IsMatch(name, @"^[a-zA-Z0-9_ ]*$"))
            {
                Notify("Name can only contain letters, numbers & underscore");
                return;
            }

            GameDB.ChangeName(name);

            Notify("Name was saved successfully");
            changeNameUIObj.SetActive(false);
            playerNameTxt.text = GameDB.selfName;
        });
    }

    private void Start()
    {
        playerNameTxt.text = GameDB.selfName;

        MainMenuHost.mainMenuDataManager.currentPickedLabMode.OnChange += (int data) =>
        {
            homeUIObj.SetActive(data == 0);
            prelabUIObj.SetActive(data != 0);
        };

        GameUtility.DoAtNextFrame(() =>
        {
            MainMenuHost.mainMenuDataManager.currentPickedLabMode.SetData(0);
        });
    }
}
