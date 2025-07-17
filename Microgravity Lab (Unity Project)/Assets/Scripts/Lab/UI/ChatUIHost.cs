using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUIHost : MonoBehaviour
{
    public static ChatUIHost instance;

    public Transform parent;
    public GameObject prefab;
    public GameObject uiObj;
    public AutoBlinkController autoBlinkController;
    [Space]
    public GameObject chatNotificationObj;
    public TextMeshProUGUI notificationCountTxt;
    public ScrollRect scrollRect;
    [Space]
    public TMP_InputField textBox;
    public Button sendButton;
    [Space]
    public Button closeButton;

    static bool isShowing = false;
    static int notificationCount = 0;

    private void Awake()
    {
        instance = this;
        sendButton.onClick.AddListener(() =>
        {
            string text = textBox.text.Trim();
            LabHost.labNET.SendChatMessage(text);
            textBox.text = "";
        });
        closeButton.onClick.AddListener(() =>
        {
            Show(false);
        });

        ShowHideGFX(false);
    }

    private void Start()
    {
        LabHost.labInputManager.OnChatOpenClose += ToggleShowHide;
        ClearChat();
    }

    private void Update()
    {
        chatNotificationObj.SetActive(notificationCount > 0);
        notificationCountTxt.text = notificationCount.ToString();
        string text = textBox.text.Trim();
        sendButton.interactable = text.Length > 0 && text.Length <= 100;
    }

    static void ScrollDown()
    {
        Canvas.ForceUpdateCanvases(); // Make sure layout is up-to-date
        instance.scrollRect.verticalNormalizedPosition = 0f;
    }

    public static void Show(bool show)
    {
        isShowing = show;
        ShowHideGFX(show);
        if (show) LabStateHandler.WINDOW_STACK.Add(LabWindowState.CHAT_WINDOW);
        else LabStateHandler.PopWindowStack();

        if(show) UpdateChatScroll();
        notificationCount = 0;

    }
    static void ShowHideGFX(bool show)
    {
        instance.uiObj.SetActive(show);
    }
    public static void ToggleShowHide()
    {
        if (isShowing) return;
        Show(true);
    }

    public static void ClearChat()
    {
        foreach(Transform t in instance.parent) Destroy(t.gameObject);
    }

    public static void AddEvent(ChatEventType chatEventType, string message, string sender)
    {
        notificationCount++;
        GameObject go = Instantiate(instance.prefab, instance.parent);
        var uip = go.GetComponent<ChatUIPiece>();
        uip.SetData(chatEventType, message, sender);

        UpdateChatScroll();
    }

    static void UpdateChatScroll()
    {
        GameUtility.DoAtNextFrame(() =>
        {
            GameUtility.ForceLayout(instance.parent.GetComponent<RectTransform>());
            GameUtility.DoAtNextFrame(() =>
            {
                GameUtility.ForceLayout(instance.parent.GetComponent<RectTransform>());
                GameUtility.DoAtNextFrame(() =>
                {
                    GameUtility.ForceLayout(instance.parent.GetComponent<RectTransform>());
                    GameUtility.DoAtNextFrame(() =>
                    {
                        GameUtility.ForceLayout(instance.parent.GetComponent<RectTransform>());
                        GameUtility.DoAtNextFrame(() =>
                        {
                            GameUtility.ForceLayout(instance.parent.GetComponent<RectTransform>());
                            GameUtility.DoAtNextFrame(() =>
                            {
                                GameUtility.ForceLayout(instance.parent.GetComponent<RectTransform>());

                                ScrollDown();
                            });
                        });
                    });
                });
            });
        });
    }
    public static void AddEvent(ChatEventType chatEventType, string message)
    {
        AddEvent(chatEventType, message, "");
    }
}

public enum ChatEventType
{
    BASIC,
    SELF,
    GREEN,
    RED,
    YELLOW,
}