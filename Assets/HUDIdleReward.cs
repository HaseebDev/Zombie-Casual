using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using QuickType;

[Serializable]
public struct IdleRewardData
{
    public float totalGoldEarn;
    public float goldPerHour;
    public long maxGold;
}

public class HUDIdleReward : BaseHUD
{
    public TextMeshProUGUI txtGoldEarn;
    public TextMeshProUGUI txtGoldInfo;
    public Button _btnEarn;
    public Slider _progressBar;
    public AttributeTooltip _attributeTooltip;

    private IdleRewardData Data;

    private long _totalEarns = 0;

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        this.Data = (IdleRewardData)args[0];
        _totalEarns = Mathf.RoundToInt(Data.totalGoldEarn);
        txtGoldEarn.text = $"{_totalEarns}/{Data.maxGold}";
        txtGoldInfo.text = $"{Data.goldPerHour}/h";
        _btnEarn.interactable = _totalEarns > 0;

        _progressBar.interactable = false;
        _progressBar.maxValue = Data.maxGold;
        _progressBar.value = _totalEarns;

    }

    public void OnButtonEarn()
    {
        var listReward = new List<RewardData>();
        listReward.Add(new RewardData()
        {
            _type = REWARD_TYPE.GOLD,
            _value = _totalEarns
        });
        MainMenuCanvas.instance.ShowRewardSimpleHUD(listReward, true);
        SaveManager.Instance.Data.Inventory.LastCollectIdleChestTS = TimeService.instance.GetCurrentTimeStamp();

        MissionManager.Instance.TriggerMission(MissionType.CLAIM_OFFLINE_REWARD);
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_HUD_HOME);
        Hide();
    }

    public void OnButtonInfo()
    {
        _attributeTooltip.Show();
    }
}
