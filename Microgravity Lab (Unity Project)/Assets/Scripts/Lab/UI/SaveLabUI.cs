using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text.RegularExpressions;
using Unity.VisualScripting.Antlr3.Runtime;

public class SaveLabUI : MonoBehaviour
{
    public GameObject obj;
    public Image bannerImage;
    public TMP_InputField nameText;
    public TMP_InputField createTimestampText;
    public TMP_InputField modifyTimestampText;

    public Button saveButton;
    public Button cancelButton;

    [Space]
    public LayerMask screenshotLayer;

    bool shouldOverwrite;

    private void Awake()
    {
        LabHost.saveLabUI = this;
        obj.SetActive(false);

        cancelButton.onClick.AddListener(() => {
            if (LabStateHandler.TopWindow() != LabWindowState.SAVE_WINDOW)
            {
                return;
            }
            obj.SetActive(false);
            LabStateHandler.PopWindowStack();
        });
    }

    private void Start()
    {
        shouldOverwrite = AppHost.savedLabData != null && AppHost.savedLabData.saveID != "";
    }

    public void SaveLab()
    {
        CameraController.FreeCursor();
        string snapshot = LabHost.labEnvironmentManager.GetSnapshot();
        StartCoroutine(CaptureScreenshotAsBase64((string bannerImageData) =>
        {
            if (!shouldOverwrite)
            {
                obj.SetActive(true);
                LabStateHandler.WINDOW_STACK.Add(LabWindowState.SAVE_WINDOW);
            }

            string modificationTimestamp = GetTimeStampText();
            modifyTimestampText.text = modificationTimestamp;
            bannerImage.sprite = GameUtility.ImageDataB64ToSprite(bannerImageData);
            saveButton.onClick.RemoveAllListeners();
            saveButton.onClick.AddListener(() =>
            {
                FinishSaving(bannerImageData, snapshot, modificationTimestamp);
            });
            if(shouldOverwrite) FinishSaving(bannerImageData, snapshot, modificationTimestamp);
        }));
    }

    void FinishSaving(string bannerImageData, string snapshot, string modificationTimestamp)
    {
        string name = nameText.text.Trim();
        if (shouldOverwrite) name = AppHost.savedLabData.name;
        nameText.text = name;
        Debug.Log("CLK");
        Debug.Log(name);
        Debug.Log(name.Length);

        if (name.Length == 0)
        {
            Debug.Log("E1");
            LabHost.labDataManager.OnBasicNotify?.Invoke("Name must not be empty");
        }
        else if (!Regex.IsMatch(name, @"^[a-zA-Z0-9_ ]*$"))
        {
            Debug.Log("E2");
            LabHost.labDataManager.OnBasicNotify?.Invoke("Name can only contain letters, numbers & underscore");
        }
        else
        {
            Debug.Log("E--");
            obj.SetActive(false);
            GameDB.SaveLabData(new SavedLabData
            {
                saveID = shouldOverwrite ? AppHost.savedLabData.saveID : GameUtility.GetMillisecondsFromUTC().ToString(),
                bannerData = bannerImageData,
                snapshot = snapshot,
                name = name,
                modifiedTimeStamp = modificationTimestamp,
                camPosition = LabHost.instance.mainCamera.transform.position,
                camRotation = LabHost.instance.mainCamera.transform.rotation,
            });

            LabHost.labDataManager.OnBasicNotify?.Invoke("Lab was saved successfully");
        }
    }

    string GetTimeStampText()
    {
        return DateTime.Now.ToString();
    }

    IEnumerator CaptureScreenshotAsBase64(Action<string> callback)
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;

        RenderTexture rt = new RenderTexture(width, height, 24);
        Camera cam = new GameObject("TempCam").AddComponent<Camera>();
        cam.CopyFrom(LabHost.instance.mainCamera);
        cam.targetTexture = rt;
        cam.cullingMask = screenshotLayer;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.clear;

        cam.Render();

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        RenderTexture.active = null;
        cam.targetTexture = null;
        Destroy(rt);
        Destroy(cam.gameObject);

        byte[] imageBytes = tex.EncodeToPNG();
        string base64Image = Convert.ToBase64String(imageBytes);
        Debug.Log(base64Image);
        Destroy(tex);
        callback?.Invoke(base64Image);
    }
}


[Serializable]
public class SavedLabData
{
    public string saveID;
    public string bannerData;
    public string name;
    public string snapshot;
    public string createdTimeStamp;
    public string modifiedTimeStamp;
    public Vector3 camPosition;
    public Quaternion camRotation;

    public Sprite GetBannerSprite()
    {
        return GameUtility.ImageDataB64ToSprite(bannerData);
    }
}