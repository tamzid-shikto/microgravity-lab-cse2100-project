using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryBackObjectPreviewController: MonoBehaviour
{
    public TextMeshProUGUI nameText;
    [Space]
    public TMP_InputField length, width, height;
    [Space]
    public TMP_InputField mass, density;
    [Space]
    public TMP_InputField magnetism, magneticAttraction;
    [Space]
    public TMP_InputField elasticity, friction;

    public static InventoryBackObjectPreviewController instance;

    private void Awake()
    {
        instance = this;
    }

    public void SetData(SO_LabObject so)
    {
        nameText.text = so.objectName;

        length.text = so.length.ToString("F2");
        width.text = so.width.ToString("F2");
        height.text = so.height.ToString("F2");

        mass.text = so.mass.ToString("F2");
        density.text = so.density.ToString("F2");

        if (so.selfMagnetism > 0) magnetism.text = so.selfMagnetism.ToString();
        else magnetism.text = "---";

        if (so.towardsMagnetAttraction > 0) magneticAttraction.text = so.towardsMagnetAttraction.ToString();
        else magneticAttraction.text = "---";

        if (so.elasticity > 0) elasticity.text = so.elasticity.ToString();
        else elasticity.text = "---";

        if (so.friction > 0) friction.text = so.friction.ToString();
        else friction.text = "---";
    }
}
