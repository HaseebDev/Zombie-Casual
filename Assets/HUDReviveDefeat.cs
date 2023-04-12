using Ez.Pooly;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;
using MEC;

public class HUDReviveDefeat : BaseHUD
{
    public static string NUM_DAY_REVIVE_ADS = "NUM_DAY_REVIVE_ADS";
    public static string NUM_DAY_REVIVE_DIAMOND = "NUM_DAY_REVIVE_DIAMOND";

    public enum Tab
    {
        NONE,
        REVIVE,
        DEFEAT
    }

    [Header("Revive")] public static float REVIVE_COUNTDOWN_DURATION = 10f;
    public long REVIVE_GEM_PRICE = 99;
    public CanvasGroup _rectRevive;
    public Button _btnReviveAds;
    public Button _btnReviveGem;
    public TextMeshProUGUI _txtReviveCountDown;
    public LocalizedTMPTextUI _txtGemPrice;
    public AutoUpdateLayoutGroup _rectDefeatVerticalLayoutGroup;
    
    [Header("Defeat")] public CanvasGroup _rectDefeat;
    public RectTransform _rectRewards;

    //private var
    private Tab currentTab = Tab.NONE;
    private float timerCountDown = 0f;
    private float timerText = 0f;
    private List<RewardItemView> _listRwdViews;

    public void SwitchTab(Tab _tab)
    {
        if (currentTab == _tab)
            return;

        _rectRevive.gameObject.SetActiveIfNot(false);
        _rectDefeat.gameObject.SetActiveIfNot(false);
        switch (_tab)
        {
            case Tab.NONE:
                break;
            case Tab.REVIVE:
                _rectRevive.gameObject.SetActiveIfNot(true);
                _rectRevive.alpha = 0;
                _rectRevive.DOFade(1.0f, 0.3f).SetEase(Ease.Linear);
                InitTabRevive();
                break;
            case Tab.DEFEAT:
                _rectDefeat.gameObject.SetActiveIfNot(true);
                _rectDefeat.alpha = 0;
                _rectDefeat.DOFade(1.0f, 0.3f).SetEase(Ease.Linear);
                InitTabDefeat();
                break;
            default:
                break;
        }

        currentTab = _tab;
    }

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        timerCountDown = 10f;
        _txtReviveCountDown.text = Mathf.RoundToInt(timerCountDown).ToString();

        _rectRevive.gameObject.SetActiveIfNot(false);
        _rectDefeat.gameObject.SetActiveIfNot(false);

        REVIVE_GEM_PRICE = DesignHelper.GetConfigDesign(GameConstant.REVIVE_GEM_PRICE).Value;
    }

    private bool CheckCouldRevive()
    {
        bool cond1 = SaveManager.Instance.Data.DayTrackingData.TodayReviveAds <
                     DesignHelper.GetConfigDesign(NUM_DAY_REVIVE_ADS).Value;
        _btnReviveAds.gameObject.SetActiveIfNot(cond1);

        bool cond2 = SaveManager.Instance.Data.DayTrackingData.TodayReviveDiamond <
                     DesignHelper.GetConfigDesign(NUM_DAY_REVIVE_DIAMOND).Value;
        _btnReviveGem.gameObject.SetActiveIfNot(cond2);
        return (cond1 || cond2) && !GamePlayController.instance.IsRevivedOneTime;
    }

    public override void Show(Action<bool> showComplete = null, bool addStack = true)
    {
        base.Show(showComplete, addStack);
        //AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_DEFEAT);
        AudioSystem.instance.PlayBgMusicWithDuration(BGM_ENUM.BGM_DEFEAT);

        Timing.RunCoroutine(ShowPopupCoroutine());
    }

    IEnumerator<float> ShowPopupCoroutine()
    {
        yield return Timing.WaitForSeconds(1.0f);
        if (CheckCouldRevive())
        {
            SwitchTab(Tab.REVIVE);
        }
        else
        {
            SwitchTab(Tab.DEFEAT);
        }
    }

    #region REVIVE

    public void SkipReviveTime()
    {
        timerCountDown -= 1f;
        if (timerCountDown < 0)
            timerCountDown = 0;
        _txtReviveCountDown.text = Mathf.RoundToInt(timerCountDown).ToString();
    }

    public void InitTabRevive()
    {
        _rectRevive.gameObject.SetActiveIfNot(true);
        timerCountDown = REVIVE_COUNTDOWN_DURATION;
        _txtGemPrice.text = REVIVE_GEM_PRICE.ToString();
    }

    public void OnButtonReviveAds()
    {
        //NetworkDetector.instance.checkInternetConnection((reached) =>
        //{
        //    if (reached)
        //    {
        //        // Show ads
        //        AdsManager.instance.ShowAdsRewardWithNotify(() =>
        //        {
        //            EventSystemServiceStatic.DispatchAll(EVENT_NAME.REVIVE_CAMPAIGN_BATTLE);
        //            SaveManager.Instance.Data.DayTrackingData.TodayReviveAds++;
        //            SaveManager.Instance.SetDataDirty();
        //            Hide();
        //        });
        //    }
        //});

        // Show ads
        AdsManager.instance.ShowAdsRewardWithNotify(() =>
        {
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.REVIVE_CAMPAIGN_BATTLE);
            SaveManager.Instance.Data.DayTrackingData.TodayReviveAds++;
            SaveManager.Instance.SetDataDirty();
            Hide();
        }, true);

    }

    public void OnButtonReviveGem()
    {
        var data = SaveManager.Instance.Data;
        if (CurrencyModels.instance.Diamonds >= REVIVE_GEM_PRICE)
        {
            if (data.DecreaseDiamond(REVIVE_GEM_PRICE))
            {
                SaveManager.Instance.Data.DayTrackingData.TodayReviveDiamond++;
                SaveManager.Instance.SetDataDirty();
                EventSystemServiceStatic.DispatchAll(EVENT_NAME.REVIVE_CAMPAIGN_BATTLE);
                Hide();
                AudioSystem.instance.PlayBgMusic(BGM_ENUM.BGM_BATTLE_01);
            }
        }
        else
        {
            MasterCanvas.CurrentMasterCanvas.ShowNotEnoughHUD(CurrencyType.DIAMOND, REVIVE_GEM_PRICE);
            // InGameCanvas.instance.ShowFloatingText(new Vector2(Screen.width / 2, Screen.height / 2), "Not Enough Money!!!", 50, 200, Color.red, 1.0f);
        }
    }

    #endregion

    #region DEFEAT

    public void InitTabDefeat()
    {
        _rectDefeat.gameObject.SetActiveIfNot(true);

        if (_listRwdViews != null && _listRwdViews.Count > 0)
        {
            foreach (var item in _listRwdViews)
            {
                Pooly.Despawn(item.transform);
            }
        }

        _listRwdViews = new List<RewardItemView>();

        RewardItemView rwdView = Pooly.Spawn<RewardItemView>(POOLY_PREF.REWARD_ITEM_VIEW, Vector3.zero,
            Quaternion.identity, _rectRewards);
        rwdView.transform.localScale = Vector3.one;
        rwdView.Initialize(new RewardData()
        {
            _type = DesignHelper.ConvertToRewardType(GamePlayController.instance.currentLevelEarn.type.ToString()),
            _value = GamePlayController.instance.currentLevelEarn.value
        });

        var verticalLayoutGroup = _rectDefeat.GetComponent<VerticalLayoutGroup>();
        verticalLayoutGroup.enabled = false;
        DOVirtual.DelayedCall(0.05f, () => { verticalLayoutGroup.enabled = true; });

        _listRwdViews.Add(rwdView);
        _rectDefeatVerticalLayoutGroup.Reload();
    }

    public void OnButtonHome()
    {
        GameMaster.instance.BackToMenuScene();
        HideWithOutContinueGame();
    }

    public void OnButtonReplayLevel()
    {
        Hide();
        GamePlayController.instance.RestartCampaignMode();
        AudioSystem.instance.PlayBgMusic(BGM_ENUM.BGM_BATTLE_01);
    }

    #endregion

    private void Update()
    {
        GamePlayController.instance.SetPauseGameplay(true);
        float _deltaTime = Time.deltaTime;
        switch (currentTab)
        {
            case Tab.NONE:
                break;
            case Tab.REVIVE:
                var totalChildCount = transform.parent.childCount;
                int currentChildIndex = transform.GetSiblingIndex();
                bool isLast = true;
                for (int i = currentChildIndex + 1; i < totalChildCount; i++)
                {
                    if (transform.parent.GetChild(i).gameObject.activeInHierarchy)
                    {
                        isLast = false;
                        break;
                    }
                }
                
                if (isLast && !InGameCanvas.instance.shop.gameObject.activeInHierarchy)
                {
                    timerCountDown -= _deltaTime;
                    timerText += _deltaTime;

                    if (timerText >= 1f)
                    {
                        _txtReviveCountDown.text = Mathf.RoundToInt(timerCountDown).ToString();
                        timerText = 0f;
                    }

                    if (timerCountDown <= 0f)
                    {
                        SwitchTab(Tab.DEFEAT);
                    } 
                }
                break;
            case Tab.DEFEAT:
                break;
            default:
                break;
        }
    }

    public override void Hide(Action<bool> hideComplete = null)
    {
        base.Hide(hideComplete);
        DOVirtual.DelayedCall(1.5f, () => { GamePlayController.instance.SetPauseGameplay(false); });
        currentTab = Tab.NONE;
    }

    private void HideWithOutContinueGame()
    {
        base.Hide();
        currentTab = Tab.NONE;
    }
}