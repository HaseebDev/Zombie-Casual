using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SaveGameHelper;
using UnityEngine.UI;
using TMPro;
using Ez.Pooly;
using System;
using DG.Tweening;
using QuickType;
using MEC;

[Serializable]
public class RewardData
{
    public REWARD_TYPE _type;
    public long _value;
    public object _extends;

    public RewardData(REWARD_TYPE type, long value)
    {
        this._type = type;
        this._value = value;
        this._extends = null;
    }

    public RewardData(RewardData deepClone)
    {
        this._type = deepClone._type;
        this._value = deepClone._value;
        this._extends = deepClone._extends;
    }

    public RewardData(string rewardID, long value)
    {
        if (rewardID.Contains("SCROLL_HERO"))
        {
            this._type = REWARD_TYPE.SCROLL_HERO;
            this._extends = rewardID.Replace("SCROLL_", "");
        }
        else
        {
            this._type = DesignHelper.ConvertToRewardType(rewardID);
            this._extends = rewardID;
        }

        this._value = value;
    }

    public RewardData(REWARD_TYPE type, long value, object extends)
    {
        this._type = type;
        this._value = value;
        this._extends = extends;
    }

    public RewardData()
    {
    }
}

public class HUDCompleteLevelReward : BaseHUD
{
    [SerializeField] private StarHelper _starHelper;

    public GameObject betweenLine;
    public GridLayoutGroup _gridGroup;
    public RectTransform _rectRewards;
    public RectTransform _rectBonusRewards;
    public TextMeshProUGUI _txtBonusRewards;
    public Button _btnAds;
    public GameObject btnHome;
    public Button btnNext;
    public GameObject focusArrowBtnHome;
    public GameObject lockBtnNext;

    private List<Reward> listRewards;
    private List<Reward> listBonusRewards;
    private List<RewardUI> _listRwdViews;

    private int CurrentFinishedLevel;

    private Action ClaimComplete;

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);


        btnHome.SetActive(true);
        listRewards = (List<Reward>) args[0];
        listBonusRewards = (List<Reward>) args[1];

        ClaimComplete = (Action) args[2];
        CurrentFinishedLevel = (int) args[3];
        int star = (int) args[4];

        CheckToDisableNextButtonWhileTutorial();
        SaveGameHelper.SaveStar(star, CurrentFinishedLevel);
        _starHelper.LoadWithAnim(star);

        if (_listRwdViews != null && _listRwdViews.Count > 0)
        {
            foreach (var item in _listRwdViews)
            {
                Pooly.Despawn(item.transform);
            }
        }

        _gridGroup.gameObject.SetActiveIfNot(true);
        _listRwdViews = new List<RewardUI>();
        foreach (var data in listRewards)
        {
            RewardUI rwdView = Pooly.Spawn<RewardUI>(POOLY_PREF.REWARD_UI_CAMPAIGN, Vector3.zero,
                Quaternion.identity, _rectRewards);
            rwdView.ShowShining(false);
            rwdView.transform.localScale = Vector3.one;
            rwdView.Load(new RewardData(data.RewardId, data.Value));
            _listRwdViews.Add(rwdView);
        }

        //bonus rewards
        var enableBonus = listBonusRewards != null && listBonusRewards.Count > 0;
        _rectBonusRewards.gameObject.SetActiveIfNot(enableBonus);
        _txtBonusRewards.gameObject.SetActiveIfNot(enableBonus);
        betweenLine.gameObject.SetActiveIfNot(enableBonus);

        foreach (var data in listBonusRewards)
        {
            RewardUI rwdView = Pooly.Spawn<RewardUI>(POOLY_PREF.REWARD_UI_CAMPAIGN, Vector3.zero,
                Quaternion.identity, _rectBonusRewards);
            rwdView.ShowShining(false);
            rwdView.transform.localScale = Vector3.one;
            rwdView.Load(new RewardData(data.RewardId, data.Value));
            _listRwdViews.Add(rwdView);
        }


        //_gridGroup.CalculateLayoutInputHorizontal();
        //_gridGroup.CalculateLayoutInputVertical();
        //_gridGroup.SetLayoutHorizontal();
        //_gridGroup.SetLayoutVertical();

        int maxDouble = (int) DesignHelper.GetConfigDesign(GameConstant.MAX_COMPLETE_LEVEL_X2_REWARD_PER_DAY).Value;
        if (SaveManager.Instance.Data.DayTrackingData.TodayDoubleCompleteLevelReward >= maxDouble)
            _btnAds.gameObject.SetActiveIfNot(false);
        else
            _btnAds.gameObject.SetActiveIfNot(true);


        if (SROptions.Current.AutoNextLevel)
        {
            DOVirtual.DelayedCall(0.5f, () => { OnButtonClaim(); });
        }
        
        SaveGameHelper.CheckToUnlockHero();
    }

    private void CheckToDisableNextButtonWhileTutorial()
    {
        int[] tutorial = new[] {5, 30};

        int maxLevel = SaveGameHelper.GetMaxCampaignLevel();
        //Debug.LogError($"MAX LEVEL {maxLevel}, CURRENT {CurrentFinishedLevel}");
        bool isTutorialStep = false;

        foreach (var level in tutorial)
        {
            if (CurrentFinishedLevel == level && maxLevel == level + 1)
            {
                isTutorialStep = true;
                break;
            }
        }

        btnNext.interactable = !isTutorialStep;
        focusArrowBtnHome.SetActive(isTutorialStep);
        lockBtnNext.SetActive(isTutorialStep);
    }

    public override void Show(Action<bool> showComplete = null, bool addStack = true)
    {
        base.Show((complete) =>
        {
            showComplete?.Invoke(complete);
            if (complete)
            {
                this._gridGroup.enabled = false;
                this._gridGroup.enabled = true;
            }
        }, addStack);

        AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_WIN);
    }

    public void OnButtonClaim()
    {
        //do nothing
        Hide();
        ClaimComplete?.Invoke();
    }

    public override void Hide(Action<bool> hideComplete = null)
    {
        btnHome.SetActive(false);
        base.Hide(hideComplete);
    }

    public void OnButtonX2Reward()
    {
        //TODO:SHOW ads
        //add one time more!!!!
        AdsManager.instance.ShowAdsReward((complete, amount) =>
        {
            if (complete)
            {
                DoubleGoldOnly();
                SaveManager.Instance.Data.DayTrackingData.TodayDoubleCompleteLevelReward++;
                SaveManager.Instance.SetDataDirty();
            }
        });
    }

    private void DoubleGoldOnly()
    {
        foreach (var rwd in listRewards)
        {
            var type = DesignHelper.ConvertToRewardType(rwd.RewardId);
            if (type == REWARD_TYPE.GOLD)
                SaveManager.Instance.Data.AddReward(new RewardData(rwd.RewardId, rwd.Value));
        }

        foreach (var rwd in _listRwdViews)
        {
            if (rwd.Data._type == REWARD_TYPE.GOLD)
            {
                var currentValue = rwd.Data._value;
                rwd.Data._value = currentValue * 2;
                rwd.Load(rwd.Data);
                rwd.ShowShining(true);
            }
        }

        _btnAds.gameObject.SetActiveIfNot(false);
    }

    public override void OnButtonBack()
    {
        base.OnButtonBack();
        //GamePlayController.instance.GamePlayAdsController?.CheckAndShowInterstitialAds(LevelModel.instance.CurrentLevel, (callback) =>
        // {

        // });

        GameMaster.instance.InterstitialController?.SetTriggerCheck(true);
        GameMaster.instance.BackToMenuScene();
    }
}