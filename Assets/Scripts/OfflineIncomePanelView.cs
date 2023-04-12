using System;
using System.Collections.Generic;
using AppodealAds.Unity.Api;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Ez.Pooly;
using Framework.Interfaces;
using Framework.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class IdleOfflineRewardData
{
    //public int levelGained;
    public List<RewardData> _listRewards;

    public IdleOfflineRewardData()
    {

    }
}

public class OfflineIncomePanelView : BaseHUD, IState
{
    [SerializeField]
    private RectTransform _rectRewards;


    private Action<bool> CompleteDialog;

    public RectTransform _rectLevelGained;
    public TextMeshProUGUI _txtLevelGained;

    private IdleOfflineRewardData Data;
    public RewardItemView _rewardItemPrefab;
    private List<RewardItemView> _listRewardsItem;

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        Data = (IdleOfflineRewardData)args[0];
        CompleteDialog = (Action<bool>)args[1];

        //_rectLevelGained.gameObject.SetActiveIfNot(Data.levelGained > 0);
        //_txtLevelGained.text = Data.levelGained.ToString();
        _rectLevelGained.gameObject.SetActiveIfNot(false);
        if (_listRewardsItem == null)
            _listRewardsItem = new List<RewardItemView>();
        if (_listRewardsItem.Count > 0)
        {
            foreach (var item in _listRewardsItem)
            {
                Pooly.Despawn(item.transform);
            }
            _listRewardsItem.Clear();
        }


        foreach (var rwd in Data._listRewards)
        {
            var rwdItem = Pooly.Spawn<RewardItemView>(_rewardItemPrefab.transform, Vector3.zero, Quaternion.identity, _rectRewards);
            rwdItem.transform.localScale = Vector3.one;
            rwdItem.Initialize(rwd);
            _listRewardsItem.Add(rwdItem);
        }

    }

    public void Load()
    {
        base.gameObject.SetActive(true);

    }

    public void Unload()
    {
        base.gameObject.SetActive(false);
    }

    public void OnButtonClaim()
    {
        var task = SaveManager.Instance.Data.AddRewards(this.Data._listRewards);
        if (task)
        {
            TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_REWARD_SIMPLE, false, null, this.Data._listRewards, true, false);
        }
        Hide();
    }

    public void OnButtonClaimx2()
    {
        AdsManager.instance.ShowAdsReward((success, amount) =>
        {
            if (success)
            {
                foreach (var rwd in this.Data._listRewards)
                {
                    if (rwd._type == REWARD_TYPE.TOKEN)
                        rwd._value *= 2;
                }

                var task = SaveManager.Instance.Data.AddRewards(this.Data._listRewards);
                if (task)
                {
                    TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_REWARD_SIMPLE, false, null, this.Data._listRewards, true, false);
                }

            }
        });
        Hide();
    }

    public override void Hide(Action<bool> hideComplete = null)
    {
        base.Hide(hideComplete);
        CompleteDialog?.Invoke(true);
    }
}
