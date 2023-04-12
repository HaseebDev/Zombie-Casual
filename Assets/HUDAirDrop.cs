using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Ez.Pooly;
using TMPro;
using UnityEngine.UI;
using MEC;

[Serializable]
public class HUDAirDropData
{
    public List<RewardData> _listRewardNormal;
    public List<RewardData> _listRewardExtra;
}
public class HUDAirDrop : BaseHUD
{
    public RectTransform _rectSupply;
    public RectTransform _rectMultiSupply;
    public VerticalLayoutGroup _masterVerticalGroup;
    public RewardItemView RewardPrefab;

    private HUDAirDropData _data;
    private List<RewardItemView> _listRewardsItems = new List<RewardItemView>();

    public Button btnClaimNormal;
    public Button btnClaimExtra;
    public TextMeshProUGUI txtClaimExtra;

    private bool forceClaimExtra = false;

    private List<RewardData> _listMergedReward;

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        this._data = (HUDAirDropData)(args[0]);
        this.forceClaimExtra = (bool)(args[1]);

        if (forceClaimExtra)
        {
            btnClaimExtra.gameObject.SetActiveIfNot(true);
            btnClaimNormal.gameObject.SetActiveIfNot(false);
            txtClaimExtra.text = "Claim";
        }
        else
        {
            btnClaimExtra.gameObject.SetActiveIfNot(true);
            btnClaimNormal.gameObject.SetActiveIfNot(true);
            txtClaimExtra.text = "Claim Extra";
        }

        if (_listRewardsItems != null && _listRewardsItems.Count >= 0)
        {
            foreach (var item in _listRewardsItems)
            {
                Pooly.Despawn(item.transform);
            }
        }

        _listRewardsItems.Clear();
        if (this._data != null)
        {
            foreach (var rwd in _data._listRewardNormal)
            {
                var rwdItem = Pooly.Spawn<RewardItemView>(POOLY_PREF.REWARD_ITEM_VIEW, Vector3.zero,
                    Quaternion.identity, _rectSupply);
                rwdItem.transform.localScale = Vector3.one;
                rwdItem.Initialize(rwd);
                _listRewardsItems.Add(rwdItem);
            }

            _listMergedReward = SaveGameHelper.MergeReward(_data._listRewardNormal, _data._listRewardExtra);
            if (_listMergedReward != null && _listMergedReward.Count > 0)
            {
                foreach (var rwd in _listMergedReward)
                {
                    var rwdItem = Pooly.Spawn<RewardItemView>(POOLY_PREF.REWARD_ITEM_VIEW, Vector3.zero,
                        Quaternion.identity, _rectMultiSupply);
                    rwdItem.transform.localScale = Vector3.one;
                    rwdItem.Initialize(rwd);
                    _listRewardsItems.Add(rwdItem);
                }
            }

        }

        // _masterVerticalGroup.enabled = false;
        // DOVirtual.DelayedCall(0.05f, () =>
        // {
        //     
        // });
        Canvas.ForceUpdateCanvases();
    }

    public void OnButtonClaim()
    {
        Timing.RunCoroutine(HideCoroutine());
    }

    IEnumerator<float> HideCoroutine()
    {
        Hide();
        yield return Timing.WaitForSeconds(0.3f);
        SaveManager.Instance.Data.AddRewards(_data._listRewardNormal);
        TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_REWARD_SIMPLE, false, null, _data._listRewardNormal, false, false);
    }

    public void OnButtonClaimExtra()
    {
        AdsManager.instance.ShowAdsReward((complete, amount) =>
        {
            if (complete && _listMergedReward != null)
            {
                //TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);

                TopLayerCanvas.instance.ShowHUDLoading(false);
                Hide();
                Timing.CallDelayed(0.3f, () =>
                {
                    SaveManager.Instance.Data.AddRewards(_listMergedReward);
                    EventSystemServiceStatic.DispatchAll(EVENT_NAME.ADD_ADON);
                    TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_REWARD_SIMPLE, false, null, _listMergedReward, false, false);
                });
            }
        });
    }

    public override void Show(Action<bool> showComplete = null, bool addStack = true)
    {
        base.Show(showComplete, addStack);
        GamePlayController.instance.SetPauseGameplay(true);
    }

    public override void Hide(Action<bool> hideComplete = null)
    {
        base.Hide(hideComplete);
        InGameCanvas.instance._gamePannelView?.ResetAddOnPannel();
        GamePlayController.instance.SetPauseGameplay(false);
    }

    public override void OnButtonBack()
    {
        //DO NOTHING
    }
}