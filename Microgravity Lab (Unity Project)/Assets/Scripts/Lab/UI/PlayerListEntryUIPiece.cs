using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerListEntryUIPiece : MonoBehaviour
{
    public Image dp;
    public Button optionButton;
    public TextMeshProUGUI memberName;
    [Space]
    public Image selfSign;
    public GameObject hostSign;
    public GameObject spectatorSign;
    public GameObject exptSign;

    [HideInInspector] public NETData.MemberData memberData;

    public void UpdateData(NETData.MemberData _memberData, string hostUID)
    {
        memberData = _memberData;
        //Debug.Log("UpdateData");

        selfSign.enabled = (memberData.uid == GameDB.selfUID);
        hostSign.SetActive(memberData.uid == hostUID);
        spectatorSign.SetActive(memberData.role == 0);
        exptSign.SetActive(memberData.role == 1);

        optionButton.gameObject.SetActive(hostUID == GameDB.selfUID && memberData.uid != GameDB.selfUID);
        memberName.text = memberData.name;
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
    }
}
