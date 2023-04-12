using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class AttributeUILocked : AttributeUI
{
    [SerializeField] private LocalizedTMPTextUI rankText;

    public void Load(string attributeID, double value, RankDefine rankDefine)
    {
        Load(attributeID, value);
        rankText.textName = rankDefine.name;
        rankText.targetTMPText.color = rankDefine.color;

        _realValue.gameObject.SetActive(false);
    }
}