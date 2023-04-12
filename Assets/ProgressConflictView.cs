using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class ViewConflictData
{
    public string UserName;
    public int CampaignLevel;
    public long TotalGold;
}

public class ProgressConflictView : MonoBehaviour
{
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtGold;

    private ViewConflictData data;
    private Action<bool> callbackChoose;

    public void Initialize(ViewConflictData _data, Action<bool> ChooseThisData)
    {
        this.data = _data;
        callbackChoose = ChooseThisData;
        txtName.text = _data.UserName.ToString();
        txtLevel.text = $"LEVEL: { _data.CampaignLevel.ToString()}";
        txtGold.text = FBUtils.CurrencyConvert(_data.TotalGold);
    }

    public void OnButtonChoose()
    {
        callbackChoose?.Invoke(true);
    }
}
