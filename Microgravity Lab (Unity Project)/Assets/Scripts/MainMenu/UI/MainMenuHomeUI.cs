using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHomeUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI serverNameText;
    public GameObject onlineObj;
    public GameObject offlineObj;
    public AtomicToolTip serverNameToolTip;
    [Space]
    public Button offlineButton;
    public Button onlineButton;

    [Space]
    public Button exitAppButton;
    public Button toggleFullscreenButton;

    private void Awake()
    {
        offlineButton.onClick.AddListener(() =>
        {
            MainMenuHost.mainMenuDataManager.currentPickedLabMode.SetData(1);
        });
        onlineButton.onClick.AddListener(() =>
        {
            MainMenuHost.mainMenuDataManager.currentPickedLabMode.SetData(2);
        });

        exitAppButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        toggleFullscreenButton.onClick.AddListener(() =>
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Screen.fullScreen = !Screen.fullScreen;
        });

        //titleText.text = Application.targetFrameRate.ToString() + " FPS";
    }

    private void Start()
    {
        bool online = AppHost.serverName.Length > 0;
        serverNameText.text = online ? AppHost.serverName : "Offline";
        onlineObj.SetActive(online);
        offlineObj.SetActive(!online);
        serverNameToolTip.message = online ? 
            "Connected to server : " + AppHost.serverName : 
            "You are currently offline, so online mode won't be available";
    }
}
