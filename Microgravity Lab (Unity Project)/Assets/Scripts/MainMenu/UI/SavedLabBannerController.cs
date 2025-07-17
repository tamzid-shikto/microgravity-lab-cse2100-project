using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SavedLabBannerController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static SavedLabBannerController selectedBanner;
    public static List<SavedLabBannerController> allBanners = new List<SavedLabBannerController>();

    public Image bannerImage;
    public GameObject selectedSignObj;
    public Button button;
    public bool isAutoSelected;
    public bool isPreSaved = false;

    public SavedLabData labData;

    private void Awake()
    {
        selectedSignObj.SetActive(false);
        allBanners.Add(this);
        button.onClick.AddListener(MakeSelected);
    }

    private void Start()
    {
        if (isAutoSelected) MakeSelected();
    }

    private void OnDestroy()
    {
        allBanners.Remove(this);
    }


    public void SetData(SavedLabData _labData)
    {
        labData = _labData;
        Sprite bannerSprite = labData.GetBannerSprite();
        string name = labData.name;
        string modifiedTimestamp = labData.modifiedTimeStamp;

        bannerImage.sprite = bannerSprite;
    }

    public void MakeSelected()
    {
        foreach(var b in allBanners)
        {
            b.selectedSignObj.SetActive(false);
        }
        selectedSignObj.SetActive(true);
        selectedBanner = this;
        if (isPreSaved) MainMenuPreLabUI.instance.LabLoadMode();
        else MainMenuPreLabUI.instance.LabCreateMode();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUIController.ShowToolTip(labData.name + "\n<i>" + labData.modifiedTimeStamp + "</i>");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUIController.HideTooltip();
    }
}
