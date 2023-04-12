using Coffee.UIEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using CustomListView.Weapon;
using UIScripts.Main_Menu;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI;
using UnityExtensions.Localization;
using Random = System.Random;
using MEC;
using QuickType.Ascend;
using TMPro;
using UnityEngine.Experimental.Rendering;

public enum HOME_TAB
{
    HEROES,
    EVENT,
    HOME,
    RESEARCH,
    SHOP
}

public class HUDHomeMenu : CanExitHUD
{
    public LocalizedTMPTextParam _txtCampaignLevel;
    public LocalizedTMPTextParam _txtIdleLevel;
    public TextMeshProUGUI _txtIdleEarnedPill;


    [Header("idle mode")]
    public Slider _idleModeProgress;

    //private RenderTexture _minigameTexture;
    [SerializeField] public RawImage _minigameRawImg;

    // [Header("Reference")] [SerializeField] private PromotionShop _promotionShop;
    [SerializeField] private LocationPackItem _locationPackItemPrefab;
    // [SerializeField] private SelectLevelPanel _selectLevel;
    [SerializeField] private StarChangeAnimation _starChangeAnimation;
    [SerializeField] private HeroPromoManager _heroPromoManager;
    [SerializeField] private ReminderUI _starRewardReminder;

    [SerializeField] private Transform _locationPackHolder;

    [SerializeField] private HomeLevelProgress _homeLevelProgress;

    [Header("Buttons")] [SerializeField] private ButtonIdle _buttonIdle;
    [SerializeField] private ButtonCampaign _buttonCampaignSelect;
    // [SerializeField] private Button _promotionButton;
    [SerializeField] private Button _freeStuffShopButton;
    [SerializeField] private ButtonMission _buttonMission;
    [SerializeField] private ReminderUI _freeStuffShopReminderUi;
    [SerializeField] private Animator _freeStuffAnimator;


    // [SerializeField] private Button _x2Button;

    [Header("Idle Rewards")] public GameObject _shinningIdleReward;
    public Button btnIdleGold;
    public UIEffect idleRewardIcon;
    private bool EnableTimeReward = false;
    public GameObject idleRewardLock;

    [Header("Debug")] [SerializeField] private DevDebugManager _debugManager;
    public UserInfoView userInfoView;

    private UserInfoViewData _userInfoData;

    public override void Init()
    {
        if (!isInit)
        {
            // #if UNITY_EDITOR
            //             if (!_debugManager.IsDebug() || _debugManager.IsDebug() && !_debugManager.IsHidePromotion())
            //             {
            //                 _promotionShop.SetOnPurchaseCallback(hasPromo => { _promotionButton.gameObject.SetActive(hasPromo); });
            //                 _promotionShop.Load();
            //             }
            // #else

            //_promotionShop.SetOnPurchaseCallback(hasPromo => { _promotionButton.gameObject.SetActive(hasPromo); });
            //_promotionShop.Load();

            // #endif

            // _selectLevel.Init();
            _heroPromoManager.Init();
            SaveGameHelper.CheckToUnlockHero();

            // Debug.LogError(SaveManager.Instance.Data.TutorialData.LastTutorialStep);
            // if (SaveManager.Instance.Data.TutorialData.LastTutorialStep <= 1)
            TutorialManager.instance.StartLastTutorial();

            // Create render texture
            //_minigameTexture = new RenderTexture(1242, 2688, 24);
            //MiniGameController.instance.camera.targetTexture = _minigameTexture;
            Debug.LogError("COMPLETE CREATE RENDER TEXTURE");
        }

        base.Init();
    }

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);

        var progressLevel = SaveManager.Instance.Data.GetUserCurrentLevelProgress();
        _userInfoData = new UserInfoViewData()
        {
            name = SaveManager.Instance.Data.MetaData.UserName,
            currentLevel = SaveManager.Instance.Data.GetUserCurrentLevel(),
            currentProgress = progressLevel.Item1,
            maxProgress = progressLevel.Item2,
        };

        userInfoView?.UpdateHUD(_userInfoData);
        EnableTimeReward =
            DesignHelper.IsRequirementAvailable(EnumHUD.HUD_IDLE_OFFLINE_REWARD
                .ToString()); //UnlockHUDManager.IsUnlockIdleReward();

        _shinningIdleReward.gameObject.SetActiveIfNot(EnableTimeReward && GameMaster.instance.ReachMaxIdleGold());
        idleRewardIcon.effectMode = EnableTimeReward ? EffectMode.None : EffectMode.Grayscale;

        bool enableIdleMode = DesignHelper.IsRequirementAvailable(GameMode.IDLE_MODE.ToString());
        _buttonIdle.SetUnlock(enableIdleMode);

        MiniGameController.instance.gameObject.SetActiveIfNot(true);
        MiniGameController.instance.SetPauseMiniGame(false);
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.RESET_HUD_HOME, new Action(ResetHUDHome));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.CLAIM_STAR_REWARD, new Action(OnClaimStarReward));
        //_minigameTexture.DiscardContents();
        //_minigameTexture.Release();
    }

    private void ResetHUDHome()
    {
        _shinningIdleReward.gameObject.SetActiveIfNot(GameMaster.instance.ReachMaxIdleGold());
    }

    private void Start()
    {
        AudioSystem.instance.PlayBgMusic(BGM_ENUM.BGM_MENU);
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.RESET_HUD_HOME, new Action(ResetHUDHome));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.CLAIM_STAR_REWARD, new Action(OnClaimStarReward));
    }

    private void OnClaimStarReward()
    {
        _starRewardReminder.Load(HUDStarReward.GetCanClaimRewardCount());
    }

    public override void ResetLayers()
    {
        Init();
        CheckToShowLocationPack();

        _starRewardReminder.Load(HUDStarReward.GetCanClaimRewardCount());
        _heroPromoManager.ResetLayer();
        _buttonIdle.Load();
        _buttonCampaignSelect.Load();

        UpdateFreeStuffIcon();

        MainMenuCanvas.instance.MainMenuTab.ResetLayer();
        // _x2Button.gameObject.SetActive(!AchieveManager.HasPurchasedDiamond());
        _txtCampaignLevel.UpdateParams(SaveGameHelper.GetMaxCampaignLevel());
        _txtIdleLevel.UpdateParams(SaveManager.Instance.Data.GameData.IdleProgress.CurrentLevel);

        _idleModeProgress.interactable = false;
        _idleModeProgress.maxValue = DesignHelper.GetMaxLevelIdleMode();
        _idleModeProgress.value = SaveManager.Instance.Data.GameData.IdleProgress.CurrentLevel;

        _txtIdleEarnedPill.gameObject.SetActiveIfNot(false);

        Timing.CallDelayed(0.5f, () =>
        {
            Timing.RunCoroutine(DesignManager.instance.LoadBaseAscendDesign((design) =>
            {
                long pillToEarned = GetIdleModePillToEarn();
                _txtIdleEarnedPill.text = FBUtils.CurrencyConvert(pillToEarned);
                _txtIdleEarnedPill.gameObject.SetActiveIfNot(true);
            }));
        });


        _buttonMission.ResetLayer();
        _homeLevelProgress.ResetLayer();
        // _promotionShop.Load();

        // idleRewardLock.SetActive(!UnlockHUDManager.IsUnlockIdleReward());
        _freeStuffShopButton.gameObject.SetActive(
            DesignHelper.IsRequirementAvailable(EnumHUD.HUD_SHOP.ToString())); // UnlockHUDManager.IsUnlockShop());
        _starChangeAnimation.Load();
        // _minigameRawImg.texture = _minigameTexture;


        bool enableIdleMode = DesignHelper.IsRequirementAvailable(GameMode.IDLE_MODE.ToString());
        _buttonIdle.SetUnlock(enableIdleMode);

        base.ResetLayers();
        // Canvas.ForceUpdateCanvases();
    }

    private void UpdateFreeStuffIcon()
    {
        _freeStuffShopButton.gameObject.SetActive(HUDDailyFreeStuff.HasAvailableItem());
        int freeStuffCount = HUDDailyFreeStuff.GetAvailableFreeStuffCount();
        _freeStuffShopReminderUi.Load(freeStuffCount);

        if (_freeStuffAnimator != null)
            _freeStuffAnimator.enabled = freeStuffCount != 0;
    }

    private void CheckToShowLocationPack()
    {
        // if (!_debugManager.IsDebug() || _debugManager.IsDebug() && !_debugManager.IsHideLocationPack())
        // {
        var data = SaveManager.Instance.Data;
        for (int i = 1; i <= data.GameData.CampaignProgress.CurrentLevel; i++)
        {
            string locationId = DesignHelper.GetLocationIdByLevel(i);
            var locationPackElement = DesignHelper.GetLocationPackDesign(locationId);

            if (locationPackElement != null &&
                !data.LocationPackSave.ShowedLocation.Contains(locationId) &&
                !data.LocationPackSave.BoughPack.Contains(locationPackElement.Id))
            {
                if (SaveGameHelper.GetMaxCampaignLevel() >= locationPackElement.UnlockLevel)
                {
                    // Show
                    var locationPackItem = Instantiate(_locationPackItemPrefab, _locationPackHolder);
                    locationPackItem.Load(locationId);
                    locationPackItem.SetOnCloseCallback(() =>
                    {
                        if (_locationPackHolder.childCount == 1)
                        {
                            _locationPackHolder.gameObject.SetActive(false);
                        }
                    });

                    data.LocationPackSave.ShowedLocation.Add(locationId);
                    SaveManager.Instance.SaveData();
                    _locationPackHolder.gameObject.SetActive(true);
                }
            }
        }

        // }
    }

    public void OnButtonStarReward()
    {
        MainMenuCanvas.instance.ShowHUD(EnumHUD.HUD_STAR_REWARD,false);
    }

    public override void Hide(Action<bool> hideComplete = null)
    {
        base.Hide(hideComplete);
        MiniGameController.instance.gameObject.SetActiveIfNot(false);
    }

    public void OnButtonIdleMode()
    {
        CheckAndShowDialogAscend();
        //GameMaster.instance.GoToSceneGame(GameMode.IDLE_MODE);
    }

    private float _lastClickCaimpain = 0;
    public void OnButtonCampaignMode()
    {
        if(Time.time - _lastClickCaimpain < 0.1f)
            return;

        _lastClickCaimpain = Time.time;
        GameMaster.instance.GoToSceneGame(GameMode.CAMPAIGN_MODE, SaveGameHelper.GetMaxCampaignLevel());
    }

    public void ShowDiamondShop()
    {
        Action callback = () => { HUDShop.Instance.SnapToShopType(ShopType.DIAMOND); };
        MainMenuCanvas.instance.ShowHUD(EnumHUD.HUD_SHOP, true, null, callback);
    }

    public void ShowDailyShowFreeStuffShop()
    {
        TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_DAILY_FREE_STUFF, false);
        // _freeStuffShop.Init();
    }

    #region Controller

    public void CheckAndShowDialogAscend()
    {
        if (!_buttonIdle.IsUnlocked)
        {
            int levelUnlock = DesignHelper.GetUnlockRequirementLevel(GameMode.IDLE_MODE.ToString()).Item2;
            TopLayerCanvas.instance.ShowFloatingTextNotify(LocalizeController.GetText(LOCALIZE_ID_PREF.UNLOCK_IN_LV, levelUnlock));
            return;
        }

        if (DesignManager.instance.baseAscendRewardDesign == null)
        {
            //TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_LOADING);
            TopLayerCanvas.instance.ShowHUDLoading(true);
            Timing.RunCoroutine(DesignManager.instance.LoadBaseAscendDesign((design) =>
            {
                //TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
                TopLayerCanvas.instance.ShowHUDLoading(false);
                DoShowDialogAscend();
            }));
        }
        else
        {
            DoShowDialogAscend();
        }

    }

    public void DoShowDialogAscend()
    {
        HUDAscendData data = default(HUDAscendData);
        var progressData = SaveManager.Instance.Data.GameData.IdleProgress;
        int currentLevel = progressData.CurrentLevel;

        BaseAscendReward ds = DesignHelper.GetBaseAscendRewardsByLevel(currentLevel);
        //long pillEarn = ds.BasePill + (currentLevel - ds.StartLevel) * ds.StepPill;
        long tokenEarn = ds.BaseTokenReturn + (currentLevel - ds.StartLevel) * ds.StepTokenReturn;
        data.currentLevel = currentLevel;
        data.maxLevel = progressData.MaxLevel;
        data.totalTimePlayed = progressData.TotalTimePlayed;
        data.totalPillEarn = GetIdleModePillToEarn();
        data.totalTokenReturn = tokenEarn;
        data.resetTimeStamp = progressData.ResetModeTS;
        MainMenuCanvas.instance.ShowHUD(EnumHUD.HUD_ASCEND, false, null, data);
    }


    public long GetIdleModePillToEarn()
    {
        var progressData = SaveManager.Instance.Data.GameData.IdleProgress;
        int currentLevel = progressData.CurrentLevel;
        BaseAscendReward ds = DesignHelper.GetBaseAscendRewardsByLevel(currentLevel);
        long pillEarn = ds.BasePill + (currentLevel - ds.StartLevel) * ds.StepPill;

        return pillEarn;
    }

    public void SwitchLanguage()
    {
        LocalizeController.Instance.SwitchLanguage();
    }

    #endregion

    private void Update()
    {
        float timer = Time.deltaTime;
        UpdateIdleChest(timer);

        // if (Input.GetKeyUp("z"))
        // {
        //     OnButtonStarReward();
        //     ShowDailyShowFreeStuffShop();
        // }
    }

    #region Idle chest

    float timerDuration = 0f;

    private static float IDLE_CHEST_CHECK = 30;

    public void UpdateIdleChest(float _deltaTime)
    {
        timerDuration += _deltaTime;
        if (timerDuration >= IDLE_CHEST_CHECK)
        {
            timerDuration = 0f;
            _shinningIdleReward.gameObject.SetActiveIfNot(GameMaster.instance.ReachMaxIdleGold());
        }
    }

    public void OnButtonIdleChest()
    {
        var unlockDesign = DesignHelper.GetUnlockRequirementLevel(EnumHUD.HUD_IDLE_OFFLINE_REWARD.ToString());
        if (!EnableTimeReward)
        {
            TopLayerCanvas.instance.ShowFloatingTextNotify(
                LocalizeController.GetText(LOCALIZE_ID_PREF.UNLOCK_IN_LV, unlockDesign.Item2));
        }
        else
            GameMaster.instance.OnCollectIdleReward();
    }

    #endregion

    public void ShowPromotionShop()
    {
        // _promotionShop.Load();
        // _promotionShop.gameObject.SetActive(true);
    }

    public void ShowSetting()
    {
        TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_SETTING, false, null, false);
    }

    public void ResetMiniGame()
    {
        //  _minigameRawImg.texture = _minigameTexture;
        // _minigameRawImg.SetAllDirty();
    }

    public override void CleanUp()
    {
        base.CleanUp();
        _homeLevelProgress.CleanUp();
    }

    private float _lastOpenMapTime = 0;
    public void OpenMap()
    {
        if(Time.time - _lastOpenMapTime < 0.7f)
            return;

        _lastOpenMapTime = Time.time;
        //FindObjectOfType<MiniGameController>().gameObject.SetActive(false);
        MasterCanvas.CurrentMasterCanvas.ShowHUD(EnumHUD.HUD_SELECT_CAMPAGIN_LEVEL);
    }
}