using System;
using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class ESC_MenuUI : MonoBehaviour
{
    public static ESC_MenuUI instance;

    public GameObject obj;
    [Space]
    public Button saveButton;
    public TextMeshProUGUI saveBtnText;
    public Button quitButton;

    [Space]
    public GameObject settingsPanelLockObj;
    public GameObject onlyHostSign;
    public GameObject needToDoExptSign;

    [Space]
    public MultiButtonController gravityMultiButtonController;
    public GameObject customGravityObj;
    public Scrollbar gravityScrollBar;
    public TMP_InputField gravityInputField;

    [Space]
    public AutoBlinkController autoBlinkController;

    static bool scrollCallbackEnabled = true;
    public static int selectedGravityOption = 0;
    bool isOpen = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitRightSide();

        //===============================================================

        obj.SetActive(false);
        if (AppHost.savedLabData != null && AppHost.savedLabData.saveID != "") saveBtnText.text = "Overwrite saved lab";
        ShowHide();
        LabHost.labInputManager.OnEscMenuOpenClose += () =>
        {
            bool futureState = !isOpen;
            if (futureState == false
                && LabStateHandler.TopWindow() != LabWindowState.ESC_WINDOW)
            {
                return;
            }
            isOpen = futureState;
            if (isOpen) LabStateHandler.WINDOW_STACK.Add(LabWindowState.ESC_WINDOW);
            else LabStateHandler.PopWindowStack();
            ShowHide();
        };

        saveButton.onClick.AddListener(LabHost.saveLabUI.SaveLab);
        quitButton.onClick.AddListener(Quit);
    }

    public void SetGravityUI(float gravity, bool forceUpdateScroll)
    {
        int index = 3;
        if (gravity == GravityValues.ZERO_GRAVITY) index = 0;
        if (gravity == GravityValues.EARTH) index = 1;
        if (gravity == GravityValues.MOON) index = 2;
        
        customGravityObj.SetActive(index == 3);
        autoBlinkController.Blink();

        gravityMultiButtonController.SilentSelect(index);
        gravityInputField.text = gravity.ToString("F2");
        scrollCallbackEnabled = false;
        if(!LabNET.IsSelfHost() || forceUpdateScroll) gravityScrollBar.value = GravityToScroll(gravity);
        scrollCallbackEnabled = true;
    }

    private void Update()
    {
        UpdateSettingsPanelStuff();
    }

    void InitRightSide()
    {
        customGravityObj.SetActive(false);
        gravityMultiButtonController.OnButtonClick += (int index) =>
        {
            selectedGravityOption = index;
            customGravityObj.SetActive(index == 3);

            autoBlinkController.Blink();

            //scrollCallbackEnabled = false;
            if (index == 0) gravityScrollBar.value = GravityToScroll(GravityValues.ZERO_GRAVITY);
            if (index == 1) gravityScrollBar.value = GravityToScroll(GravityValues.EARTH);
            if (index == 2) gravityScrollBar.value = GravityToScroll(GravityValues.MOON);
            //scrollCallbackEnabled = true;

            //LabHost.labNET.SendGravity(ScrollToGravity(gravityScrollBar.value));
        };
        gravityScrollBar.onValueChanged.AddListener((float value) =>
        {
            if (!scrollCallbackEnabled) return;

            float gravityValue = ScrollToGravity(value);

            gravityInputField.text = gravityValue.ToString("F2");
            if (AppHost.isVirtualMode) LabHost.labNET.SendGravity(gravityValue);
            else LabHost.labEnvironmentManager.UpdateGravity(gravityValue, false, false);
        });
    }

    float ScrollToGravity(float v)
    {
        return (v - 0.5f) * 20f;
    }
    float GravityToScroll(float g)
    {
        return (g + 10f) / 20f;
    }

    void Quit()
    {
        LabHost.labNET.Disconnect();
        AppHost.instance.LoadMainMenuScene();
    }
    void ShowHide()
    {
        obj.SetActive(isOpen);
        if(isOpen) CameraController.FreeCursor();
        else CameraController.RevertToLastState();
    }

    public void UpdateSettingsPanelStuff()
    {
        settingsPanelLockObj.SetActive(!LabNET.IsSelfHost() || !LabNET.IsSelfExpt());
        needToDoExptSign.SetActive(AppHost.isVirtualMode && LabNET.IsSelfHost() && LabStateHandler.labExptState != LabExptState.SELF_EXPT);
        onlyHostSign.SetActive(AppHost.isVirtualMode && !LabNET.IsSelfHost());
    }
}

static class GravityValues
{
    public static float ZERO_GRAVITY = 0;
    public static float EARTH = 9.8f;
    public static float MOON = -9.8f;//9.8f / 6f;
}