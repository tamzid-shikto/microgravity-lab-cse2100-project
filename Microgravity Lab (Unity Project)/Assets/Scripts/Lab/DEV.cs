using System.Collections;
using UnityEngine;

public class DEV : MonoBehaviour
{
    public TMPro.TextMeshProUGUI tickText;
    public TMPro.TextMeshProUGUI data1text;
    public TMPro.TextMeshProUGUI data2text;

    int objectCount = 0;

    public static DEV instance;

    private void Awake()
    {
        instance = this;
    }

    int count = 0;
    public void MoveGhostCount()
    {
        count++;
        tickText.text = count.ToString();
    }

    private void Start()
    {
        foreach(Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }
    }

    void ShowSnapshotHash()
    {
        string dataRaw = LabHost.labEnvironmentManager.GetSnapshot();
        string hash = GameUtility.ComputeSha256Hash(dataRaw);
        data1text.text = hash;
        data2text.text = dataRaw;
    }

    private void Update()
    {
        tickText.text = LabHost.labEnvironmentManager.TICK_COUNT + "  |||  " + Application.targetFrameRate.ToString() + " fps";
        //ShowSnapshotHash();

        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
        {
            //LabHost.saveLabUI.SaveLab();
        }
    }
}