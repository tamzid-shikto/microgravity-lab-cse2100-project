using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuPreLabUI : MonoBehaviour
{
    public Button createRoomButton;
    public Button loadSavedRoomButton;
    public GameObject offlineSign;
    public Button returnHomeButton;

    [Space]
    public TextMeshProUGUI uiTitleText;
    public TextMeshProUGUI searchTextBox;
    public Button searchButton;
    public Button refreshButton;

    [Space]
    public GameObject roomListObj;
    public GameObject fetchingStateTextObj;
    public GameObject fetchingFailedTextObj;
    public GameObject fetchingNoRoomObj;

    [Space]
    public Transform savedLabParent;
    public Transform templatesParent;
    public GameObject savedLabPrefab;

    public static MainMenuPreLabUI instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadTemplates();
        LoadSavedLabsToUI();

        createRoomButton.onClick.AddListener(() =>
        {
            AppHost.savedLabData = SavedLabBannerController.selectedBanner.labData;
            bool isOfflineMode = MainMenuHost.mainMenuDataManager.currentPickedLabMode.GetData() == 1;
            if(isOfflineMode) AppHost.OnStartOfflineCall?.Invoke(0);
            else AppHost.OnStartVirtualCall?.Invoke("");
        });
        loadSavedRoomButton.onClick = createRoomButton.onClick;

        returnHomeButton.onClick.AddListener(() =>
        {
            MainMenuHost.mainMenuDataManager.currentPickedLabMode.SetData(0);
        });
        refreshButton.onClick.AddListener(() =>
        {
            //MainMenuHost.mainMenuNET.FetchRoomList();
        });

        MainMenuHost.mainMenuDataManager.currentPickedLabMode.OnChange += (int data) =>
        {
            Debug.Log("MainMenuHost.mainMenuDataManager.currentPickedLabMode.OnChange :: " + data);
            if (data == 1)
            {
                uiTitleText.text = "OFFLINE MODE";
                roomListObj.SetActive(false);
            }
            else if(data == 2)
            {
                uiTitleText.text = "ONLINE MODE";
                roomListObj.SetActive(true);
            }

            bool isOffline = AppHost.serverName.Length == 0 && data == 2;
            createRoomButton.gameObject.SetActive(!isOffline);
            loadSavedRoomButton.gameObject.SetActive(!isOffline);
            offlineSign.SetActive(isOffline);
        };
        MainMenuHost.mainMenuDataManager.roomFetchingState.OnChange += (int data) =>
        {
            fetchingStateTextObj.SetActive(data == 1);
            fetchingFailedTextObj.SetActive(data == 2);
            fetchingNoRoomObj.SetActive(data == 3);
            refreshButton.gameObject.SetActive(data >= 2);
        };
    }

    public void LabLoadMode()
    {
        loadSavedRoomButton.interactable = (true);
        createRoomButton.interactable = (false);
    }
    public void LabCreateMode()
    {
        loadSavedRoomButton.interactable = (false);
        createRoomButton.interactable = (true);
    }

    void LoadTemplates()
    {
        bool isFirst = true;
        foreach (Transform t in templatesParent) Destroy(t.gameObject);

        foreach (var labData in GameDB.labTemplates)
        {
            GameObject go = Instantiate(savedLabPrefab, templatesParent);
            var bannerController = go.GetComponent<SavedLabBannerController>();
            bannerController.SetData(labData);
            bannerController.isPreSaved = false;
            if (isFirst)
            {
                isFirst = false;
                bannerController.isAutoSelected = true;
            }
        }
    }

    void LoadSavedLabsToUI()
    {
        foreach (Transform t in savedLabParent) Destroy(t.gameObject);

        foreach(var labData in GameDB.savedLabDatas)
        {
            GameObject go = Instantiate(savedLabPrefab, savedLabParent);
            var bannerController = go.GetComponent<SavedLabBannerController>();
            bannerController.SetData(labData);
            bannerController.isPreSaved = true;
        }
    }
}
