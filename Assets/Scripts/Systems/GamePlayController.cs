using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using com.datld.data;
using com.datld.talent;
using Framework.Components.Input.TouchDetector;
using MEC;
using Ez.Pooly;
using DG.Tweening;
using QuickType;
using static AnalyticsConstant;
using UnityEngine.AddressableAssets;

public enum GameMode
{
    NONE,
    CAMPAIGN_MODE,
    IDLE_MODE
}


public class BattleEarnTracker
{
    public CurrencyType type;
    public long value;

    public BattleEarnTracker(CurrencyType _type)
    {
        type = _type;
        value = 0;
    }

    public void ResetValue()
    {
        value = 0;
    }

    public void AddValue(long _value)
    {
        value += _value;
    }
}

public class GamePlayController : BaseSystem<GamePlayController>
{
    public static string levelFolderName = "[LEVELS]";
    public static GamePlayController instance;

    public bool CheatNeverEndBattle { get; private set; }

    public void SetCheatNeverEndBattle(bool _value)
    {
        CheatNeverEndBattle = _value;
    }

    public GameLevel gameLevel { get; private set; }
    public BattleEarnTracker currentLevelEarn { get; private set; }

    public bool IsPausedGame { get; private set; }
    public bool IsRevivedOneTime { get; private set; }
    public bool IsFinishedLevel { get; private set; }

    public void SetFinishLevel(bool _value)
    {
        IsFinishedLevel = _value;
    }

    public Camera MainCamera { get; private set; }
    public MyCameraShake cameraShake { get; private set; }

    public Camera GetMainCamera()
    {
        return MainCamera != null ? MainCamera : Camera.main;
    }

    public TouchDetector3D touchDetector;

    public KillingZombiesAwarder KillingZombieAward;

    private GameMode _mode;

    public long StartGameTS { get; private set; }

    public Action OnStartGame;
    public Action OnFinishLevel;

    public bool IsPlayingGame { get; private set; }

    private float _lastTimeScale = 1.0f;

    public void SetPlayingGame(bool _value)
    {
        this.IsPlayingGame = _value;
        //if (!this.IsPlayingGame)
        //{

        //    _lastTimeScale = Time.timeScale;
        //    Time.timeScale = 0.2f;
        //}
        //else
        //{
        //    Time.timeScale = _lastTimeScale;
        //}
    }

    //public GamePlayInterstitial GamePlayAdsController { get; private set; }

    #region Game Mode

    public GameMode gameMode { get; private set; }

    public void SetGameMode(GameMode mode)
    {
        switch (mode)
        {
            case GameMode.NONE:
                break;
            case GameMode.CAMPAIGN_MODE:
                InitCampaignMode();
                break;
            case GameMode.IDLE_MODE:
                InitIdleMode();
                break;
            default:
                break;
        }

        gameMode = mode;
    }

    #endregion

    public WaweController _waveController;
    public ManualShootBehaviour manualShootBehaviour;

    private List<Transform> _listSpawnedOstacles = new List<Transform>();

    private CastleHealth _castleHP;
    public int CurrentLevel = 0;

    public CastleHealth CastleHP {
        get {
            if (_castleHP == null)
            {
                if (_castleHP == null)
                    _castleHP = GameObject.FindObjectOfType<CastleHealth>();
            }

            return _castleHP;
        }
    }

    public void AddObstacle(Transform trf)
    {
        if (_listSpawnedOstacles == null)
            _listSpawnedOstacles = new List<Transform>();
        _listSpawnedOstacles.Add(trf);
    }

    public void RemoveAllObstacles()
    {
        if (_listSpawnedOstacles != null)
        {
            foreach (var obs in _listSpawnedOstacles)
            {
                Pooly.Despawn(obs);
            }

            _listSpawnedOstacles.Clear();
        }

        //var zomObjects = GameObject.FindObjectsOfType<BaseZombieObject>();
        var zomObjects = GameObject.FindGameObjectsWithTag(TagConstant.TAG_ZOM_OBJECT);
        if (zomObjects != null && zomObjects.Length > 0)
        {
            foreach (var item in zomObjects)
            {
                var zomObject = item.GetComponent<BaseZombieObject>();
                if (zomObject != null)
                    zomObject.CleanUp();
            }
        }



    }

    private void Awake()
    {
        instance = this;
        AudioSystem.instance.PlayBgMusic(BGM_ENUM.BGM_BATTLE_01);
    }

    public override void Initialize(params object[] pars)
    {
        CheatNeverEndBattle = false;
        base.Initialize(pars);
        Timing.RunCoroutine(InitalizeAsync(pars));
    }

    IEnumerator<float> InitalizeAsync(params object[] pars)
    {
        _mode = (GameMode)pars[0];
        int level = (int)pars[1];

        if (FloatingTextQueue == null)
        {
            FloatingTextQueue = new FloatingTextQueue();
            FloatingTextQueue.Initialize();
        }

        FloatingTextQueue.StartLoop();

        LoadModels(level);

        KillingZombieAward.PreInit(_mode);

        GameMaster.instance.SetPowerSavingMode(false);

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(InitLevelMap(level)));
        SetGameMode(_mode);
        CurrentLevel = SaveManager.Instance.Data.GameData.CampaignProgress.CurrentLevel = level;

        InGameCanvas.instance._gamePannelView.InitByGameMode(_mode);

        manualShootBehaviour.Initialize(SaveManager.Instance.Data.Inventory.ManualHero);
        manualShootBehaviour.ApplyBehavior();

        int startWave = 1;
        if (_mode == GameMode.IDLE_MODE)
        {
            startWave = SaveManager.Instance.Data.GetPlayProgress(GameMode.IDLE_MODE).CurrentWave;
            startWave = Mathf.Clamp(startWave - 1, 1, int.MaxValue);
        }

        _waveController._actionLevelReadyUp = new Action(OnLevelReadyUp);
        _waveController.OnLevelLoaded = new Action(OnNewLevelLoaded);

        _waveController.ApplyBehaviour();
        _waveController.RestartLevel(startWave);
        FindWallPos();
        touchDetector.SetEnableTouch(true);

        StartGame();

        SetPauseGameplay(false);
        IsRevivedOneTime = false;

        InitSpawnFlyingAirDrop();

        EventSystemServiceStatic.AddListener(this, EVENT_NAME.ON_TOUCH_GROUND, new Action<Vector3, bool>(OnTapGround));

        //preload interstial
        AdsManager.instance.LoadAdsInterstitial((success) => { });

        InitializeHintTap();

        InitializeZombieAngrySys();
        IsInited = true;
        yield break;
    }

    public void StartGame()
    {
        if (campaignData != null)
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_CASTLE_HP, 1, campaignData.castleHealth.startHp);
        IsFinishedLevel = false;
        MainCamera = this.gameLevel._camera;
        cameraShake = this.gameLevel._camera.GetComponent<MyCameraShake>();

        OnStartGame?.Invoke();
        StartGameTS = TimeService.instance.GetCurrentTimeStamp();
        SetPlayingGame(true);

    }

    public override void UpdateSystem(float _deltaTime)
    {
        Logwin.Log("Is Playing Game:", IsPlayingGame, "Game");
        if (!IsInited || IsPausedGame)
        {
            //Debug.LogError("GamePlayController not update!!!");
            return;
        }


        foreach (var hero in gameLevel._dictCharacter)
        {
            hero.Value.UpdateCharacter(_deltaTime);
        }

        if (gameLevel.ManualHero != null)
        {
            gameLevel.ManualHero.UpdateCharacter(_deltaTime);
        }

        this._waveController?.UpdateWaveController(_deltaTime);
        this.KillingZombieAward?.UpdateSystem(_deltaTime);


        UpdateAddOn(_deltaTime);

        UpdateSpawnFlyingAirDrop(_deltaTime);

        UpdateHintTap(_deltaTime);

        if (gameMode == GameMode.IDLE_MODE)
        {
            UpdateIdleMode(_deltaTime);
        }

    }

    private IEnumerator<float> InitLevelMap(int level)
    {
        string levelName = _mode == GameMode.CAMPAIGN_MODE ? "CampaignMap" : "IdleMap";
        //string levelName = "GameSkeleton";
        var op = Addressables.LoadAssetAsync<GameObject>(levelName);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        gameLevel = UnityEngine.Object.Instantiate(op.Result).GetComponent<GameLevel>();

        MAP_NAME mapName = _mode == GameMode.CAMPAIGN_MODE ? MAP_NAME.MAP_DESERT_DAY : MAP_NAME.MAP_CITY_NIGHT;
        if (SROptions.Current.Map != MAP_NAME.NONE)
        {
            mapName = SROptions.Current.Map;
        }

        if (_mode == GameMode.CAMPAIGN_MODE)
        {
            mapName = DesignHelper.GetMapByLevel(level);
        }

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(gameLevel.InitializeCoroutine(_mode, mapName)));
        gameLevel.ResetCastleHealth();

        //TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
        TopLayerCanvas.instance.ShowHUDLoading(false);
    }

    private void LoadModels(int level)
    {
        CurrencyModels.instance.Load();
        LevelModel.instance.Init(SaveManager.Instance.Data.GetPlayProgress(_mode), level);
        LevelModel.instance.Load();
    }

    private void OnApplicationQuit()
    {
        CurrencyModels.instance.Save();
        LevelModel.instance.Save();

        GameMaster.instance.ReturnFromBattle();
    }

    private void OnDestroy()
    {
        manualShootBehaviour.StopBehavior();
    }

    #region Campaign Mode

    public CampaignModel campaignData { get; private set; }
    private Action claimRewardComplete;

    private void InitCampaignMode()
    {
        campaignData = new CampaignModel();
        campaignData.castleHealth = gameLevel.castleHealth;
        campaignData.ResetData();

        EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_CASTLE_HP, 1, campaignData.castleHealth.startHp);
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.CASTLE_DEFEATED, new Action(OnCampaignModeDefeat));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.REVIVE_CAMPAIGN_BATTLE, new Action(ReviveCampaignMode));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.RESET_CAMPAIGN_DATA, new Action(ResetCampaignMode));

        GameMaster.instance.InterstitialController.IncreaseCountGamePlay();
    }

    public void ResetCampaignMode()
    {
        campaignData?.ResetData();
    }

    private float _lastDefeatTime = -1.5f;

    public void OnCampaignModeDefeat()
    {
        if (Time.time - _lastDefeatTime > 1.5f)
        {
            SetPauseGameplay(true);
            InGameCanvas.instance.ShowHUD(EnumHUD.HUD_REVIVE_DEFEAT, false);
            InGameCanvas.instance._hintTapOnScreen.SetEnable(false);
            SetPlayingGame(false);
            OnFinishLevel?.Invoke();
        }

        _lastDefeatTime = Time.time;
    }

    public void RestartCampaignMode()
    {
        Timing.RunCoroutine(RestartCampaignModeCoroutine());
    }

    IEnumerator<float> RestartCampaignModeCoroutine()
    {
        TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_LOADING, false, null, true);
        //TopLayerCanvas.instance.ShowHUDLoading(true);
        FloatingTextQueue.CleanUp();
        campaignData.ResetData();
        yield return Timing.WaitForSeconds(0.5f);

        KillingZombieAward.CollectAllCoins();

        yield return Timing.WaitForOneFrame;
        ResourceManager.instance.LimitAllPrefabs();
        yield return Timing.WaitForSeconds(0.3f);
        RemoveAllObstacles();

        Resources.UnloadUnusedAssets();
        yield return Timing.WaitForOneFrame;
        GC.Collect();
        yield return Timing.WaitForOneFrame;

        _waveController.RestartLevel();
        gameLevel.ResetGameLevel();

        TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
        //TopLayerCanvas.instance.ShowHUDLoading(false);

        SetPlayingGame(true);
    }

    public void ReviveCampaignMode()
    {
        campaignData.ResetData(0.5f);
        gameLevel.ResetGameLevel();
        IsRevivedOneTime = true;

        SetPlayingGame(true);
    }

    public void ShowDialogCompleteLevelReward()
    {
        // Minus one because level up was call before call this function
        int currentLevel = LevelModel.instance.CurrentLevel - 1;

        //if tutorial level, force fullhp to 3 stars
        if (currentLevel == 3)
        {
            CastleHP.SetCurrentHp(CastleHP.startHp);
        }
        int stars = GetStar();

        var tuppleRewards = SaveManager.Instance.Data.AddCampaignRewards(currentLevel, stars);

        claimRewardComplete = OnClaimRewardComplete;
        InGameCanvas.instance.ShowHUD(EnumHUD.HUD_COMPLETE_LEVEL_REWWARD, false, null, tuppleRewards.Item1,
            tuppleRewards.Item2,
            claimRewardComplete, currentLevel, stars);
        
        MissionManager.Instance.TriggerMission(MissionType.FINISH_LEVEL,gameMode);
        SetPlayingGame(false);
        OnFinishLevel?.Invoke();
    }

    private int GetStar()
    {
        float percent = CastleHP.Percent * 100f;
        if (percent >= 90)
        {
            return 3;
        }

        if (percent >= 50)
        {
            return 2;
        }

        return 1;
    }

    /// <summary>
    /// Campaign Mode Level up
    /// </summary>
    public void OnClaimRewardComplete()
    {
        GameMaster.instance.InterstitialController?.SetTriggerCheck(true);
        GameMaster.instance.InterstitialController?.CheckAndShowInterstitialAds(SaveGameHelper.GetCurrentCampaignLevel(), (complete) =>
        {
            // TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING, null, true);
            TopLayerCanvas.instance.ShowHUDLoading(false);
            Timing.RunCoroutine(DoLevelUpCampaignModeCoroutine());
        });
    }

    IEnumerator<float> DoLevelUpCampaignModeCoroutine()
    {
        TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_LOADING, false, null, true);
        //TopLayerCanvas.instance.ShowHUDLoading(true);
        yield return Timing.WaitForSeconds(0.5f);
        FloatingTextQueue.CleanUp();
        AnalyticsManager.instance.LogEventDayLoginVsCampaignLevel(SaveManager.Instance.Data.MetaData.DayLogin,
            SaveGameHelper.GetCurrentCampaignLevel());
        yield return Timing.WaitForOneFrame;

        KillingZombieAward.CollectAllCoins();
        yield return Timing.WaitForOneFrame;

        gameLevel.CleanObstacles();

        MAP_NAME mapName = MAP_NAME.MAP_DESERT_DAY;
        mapName = DesignHelper.GetMapByLevel(SaveGameHelper.GetCurrentCampaignLevel());
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(gameLevel.ResetMapDecor(mapName)));

        _waveController.zombiesOnTheWawe.Clear();
        ResourceManager.instance.LimitAllPrefabs();
        yield return Timing.WaitForSeconds(0.3f);
        RemoveAllObstacles();
        Resources.UnloadUnusedAssets();
        yield return Timing.WaitForOneFrame;
        GC.Collect();
        yield return Timing.WaitForOneFrame;
        gameLevel.ResetGameLevel();

        yield return Timing.WaitForOneFrame;
        _waveController.LevelUp();

        yield return Timing.WaitForSeconds(0.5f);
        TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
        //TopLayerCanvas.instance.ShowHUDLoading(false);
        GameMaster.instance.InterstitialController.IncreaseCountGamePlay();

    }

    #endregion

    #region Idle Mode

    public static float OFFLINE_GOLD_MIN = 5.0f / TimeService.MIN_SEC * 1.0f;
    public static float OFFLINE_LEVEL_GAIN = 2.0f / TimeService.DAY_SEC * 1.0f;
    public bool BusyIdleTask { get; private set; }

    private bool CheckedSkipLevel = false;

    public void InitIdleMode()
    {
        BusyIdleTask = false;
        campaignData = new CampaignModel();
        campaignData.castleHealth = gameLevel.castleHealth;
        campaignData.ResetData();
        InitializePowerSavingMode();
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_CASTLE_HP, 1, campaignData.castleHealth.startHp);
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.CASTLE_DEFEATED, new Action(RestartIdleMode));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.REVIVE_CAMPAIGN_BATTLE, new Action(ReviveCampaignMode));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.RESET_CAMPAIGN_DATA, new Action(ResetCampaignMode));
        CheckedSkipLevel = false;
        var offlineReward = CheckOfflineIdle();
        if (offlineReward != null)
        {
            ShowDialogOfflineReward(offlineReward, (success) =>
             {
                 Timing.CallDelayed(2.0f, () =>
                 {
                     CheckAndShowDialogSkipLevel();
                 });
             });
        }
        else
        {
            CheckAndShowDialogSkipLevel();
        }

        SaveManager.Instance.Data.MetaData.IsUnlockIdleMode = true;

    }

    public IdleOfflineRewardData CheckOfflineIdle()
    {
        IdleOfflineRewardData result = null;

        List<RewardData> listRewards = new List<RewardData>();
        var currentTs = TimeService.instance.GetCurrentTimeStamp();
        var lastLogout = SaveManager.Instance.Data.GameData.IdleProgress.LastLogoutTS;

        var diff = currentTs - lastLogout;
        if (diff >= 60 && SaveManager.Instance.Data.GameData.IdleProgress.TotalTimePlayed > 0)
        {
            result = new IdleOfflineRewardData();

            //var levelGained = GetMaxIdleLevelSkipable();
            var idleRwdDesign =
                DesignHelper.GetIdleChestDesign(SaveManager.Instance.Data.GameData.IdleProgress.CurrentLevel);
            if (idleRwdDesign != null)
            {
                float tokenGained = (float)(idleRwdDesign.TokenPerSec * diff);
                float maxToken = (float)(idleRwdDesign.MaxTokenTime * idleRwdDesign.TokenPerSec);

                tokenGained = Mathf.Clamp(tokenGained, 0, maxToken);
                listRewards.Add(new RewardData()
                {
                    _type = REWARD_TYPE.TOKEN,
                    _value = Mathf.RoundToInt(tokenGained)
                });
            }

            //if (levelGained >= 1)
            //{
            //    result.levelGained = Mathf.RoundToInt(levelGained);
            //}

            result._listRewards = listRewards;
        }

        return result;
    }

    private long GetMaxIdleLevelSkipable(int curIdleLevel)
    {
        long targetLevel = 0;
        float TargetSkipStep = DesignHelper.GetConfigDesign(GameConstant.STEP_TO_SKIP_IDLE_LEVEL).Value;
        var teamDPS = SaveManager.Instance.Data.CalcCurrentTeamDPS(GameMode.IDLE_MODE);
        var level = DesignManager.instance.LevelIdleDesign.LastOrDefault(x =>
            teamDPS >= TargetSkipStep * x.DPSToWin && x.Level > curIdleLevel);
        if (level != null)
        {
            targetLevel = level.Level;
        }

        Debug.Log($"curDPS:{teamDPS} - TARGET LEVEL: {level}");

        return targetLevel;
    }

    public void ShowDialogOfflineReward(IdleOfflineRewardData data, Action<bool> complete)
    {
        InGameCanvas.instance.ShowHUDIdleOfflineReward(data, complete);
    }

    public void CheckAndShowDialogSkipLevel()
    {
        var progressIdle = SaveManager.Instance.Data.GetPlayProgress(GameMode.IDLE_MODE);
        if (progressIdle != null && progressIdle.TotalTimePlayed <= 0)
            return;

        Debug.Log("called CheckAndShowDialogSkipLevel!!!");
        Timing.RunCoroutine(CheckAndShowDialogSkipLevelCoroutine());
    }

    IEnumerator<float> CheckAndShowDialogSkipLevelCoroutine()
    {
        if (CheckedSkipLevel)
            yield break;

        //load design!!!
        if (DesignManager.instance.skipIdleLeveDesign == null)
        {
            bool waitTask = true;
            Timing.RunCoroutine(DesignManager.instance.LoadSkipIdleLevelDesignCoroutine((callback) =>
            {
                waitTask = false;
            }));

            while (waitTask)
                yield return Timing.WaitForOneFrame;
        }


        var idleProgress = SaveManager.Instance.Data.GetPlayProgress(GameMode.IDLE_MODE);
        var ds = DesignHelper.GetSkipIdleLevelDesign(idleProgress.CurrentLevel);
        var maxLevelToSkip = GetMaxIdleLevelSkipable(idleProgress.CurrentLevel);

        var lvlGap = maxLevelToSkip - idleProgress.CurrentLevel;
        if (lvlGap > 0)
        {
            int maxLevelDiff = (int)lvlGap;
            int freeLevelDiff = Mathf.RoundToInt(maxLevelDiff * 0.3f);
            freeLevelDiff = Mathf.Clamp(freeLevelDiff, 1, int.MaxValue);

            int levelDiff = (int)(idleProgress.CurrentLevel - ds.Level);
            IdleSkipLevelData data = new IdleSkipLevelData()
            {
                FreeSkipLevel = freeLevelDiff,
                FreeSkipToken = ds.BaseAdsToken + (levelDiff) * ds.StepAdsToken,
                MaxSkipLevel = maxLevelDiff,
                MaxSkipToken = ds.BaseDiamondToken + (levelDiff) * ds.StepDiamondToken,
                PriceMaxSkip = ds.PriceDiamond
            };
            InGameCanvas.instance.ShowHUD(EnumHUD.HUD_IDLE_SKIP_LEVEL, false, null, data);
        }

        CheckedSkipLevel = true;
    }


    private float timerLog = 0f;

    private void UpdateIdleMode(float deltaTime)
    {
#if UNITY_EDITOR
        timerLog += deltaTime;
        if (timerLog >= 2.0f)
        {
            var curTeamDPS = SaveManager.Instance.Data.CalcCurrentTeamDPS(GameMode.IDLE_MODE);
            Logwin.Log("[Team DPS]", curTeamDPS, "[IDLE MODE]");
            timerLog = 0f;
        }
#endif
        UpdatePowerSavingMode(deltaTime);

    }

    public void RestartIdleMode()
    {
        if (BusyIdleTask)
            return;
        BusyIdleTask = true;
        Timing.RunCoroutine(RestartIdleModeCoroutine());
    }

    IEnumerator<float> RestartIdleModeCoroutine()
    {
        TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_LOADING, false, null, true);
        //TopLayerCanvas.instance.EnableSplashScreen(true);

        FloatingTextQueue.CleanUp();
        yield return Timing.WaitForSeconds(0.3f);

        KillingZombieAward.CollectAllCoins();

        gameLevel.ResetLevel();

        _waveController.zombiesOnTheWawe.Clear();
        ResourceManager.instance.LimitAllPrefabs();
        yield return Timing.WaitForSeconds(0.3f);
        RemoveAllObstacles();
        Resources.UnloadUnusedAssets();
        yield return Timing.WaitForOneFrame;
        GC.Collect();
        yield return Timing.WaitForOneFrame;

        PlayProgress playprogress = SaveManager.Instance.Data.GetPlayProgress(GameMode.IDLE_MODE);
        var targetWave = Mathf.Clamp(playprogress.CurrentWave - 1, 1, int.MaxValue);
        _waveController.RestartLevel(targetWave);
        gameLevel.ResetGameLevel();

        yield return Timing.WaitForSeconds(0.5f);
        campaignData.ResetData();
        //TopLayerCanvas.instance.EnableSplashScreen(false);
        TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);

        BusyIdleTask = false;
    }

    #endregion

    #region Callback handler

    public void AnimSlowDownGamePlay(Action callback)
    {
        Timing.RunCoroutine(AnimSlowDownGamePlayCoroutine(callback));
    }

    IEnumerator<float> AnimSlowDownGamePlayCoroutine(Action callback)
    {
        var oldTimeScale = Time.timeScale;
        var targetTimeScale = oldTimeScale * 0.25f;
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, targetTimeScale, 0.1f).SetEase(Ease.Linear);
        yield return Timing.WaitForSeconds(0.3f);
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, oldTimeScale, 0.1f).SetEase(Ease.Linear);
        callback?.Invoke();
    }

    private void OnLevelReadyUp()
    {
        if (gameMode == GameMode.CAMPAIGN_MODE)
        {
            //GamePlayAdsController?.CheckAndShowInterstitialAds(LevelModel.instance.CurrentLevel);
            Timing.RunCoroutine(AnimLevelUpCoroutine());

            var currentCampaignLevel = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CurrentLevel;
            //AnalyticsManager.instance.LogEventDayLoginVsCampaignLevel(SaveManager.Instance.Data.MetaData.DayLogin,
            //    SaveManager.Instance.Data.GetCurrentCampaignLevel());
        }
        else if (gameMode == GameMode.IDLE_MODE)
        {
            LevelModel.instance.LevelUp();
        }
        //castle do net deal more dmg
    }

    IEnumerator<float> AnimLevelUpCoroutine()
    {
        Debug.Log("start AnimLevelUpCoroutine!");
        AdsManager.instance.LoadAdsInterstitial((success) =>
        {
            Debug.Log($"[ADS] Finish battle load interstitial callback {success}");
        });

        InGameCanvas.instance._hintTapOnScreen.SetEnable(false);
        InGameCanvas.instance._gamePannelView.CleanZomHpBar();

        KillingZombieAward.CollectAllCoins();

        yield return Timing.WaitForOneFrame;
        campaignData.castleHealth.IsTargetable = false;
        campaignData.castleHealth.IsTargetable = true;
        //var currentTimeScale = Time.timeScale;
        //Time.timeScale = currentTimeScale * 0.5f;
        //yield return Timing.WaitForSeconds(2.0f);
        //Time.timeScale = currentTimeScale;
        //yield return Timing.WaitForOneFrame; 

        //preload ads here!!!
        yield return Timing.WaitForSeconds(1.0f);
        ShowDialogCompleteLevelReward();
        IsFinishedLevel = true;

    }

    private void OnNewLevelLoaded()
    {
        currentLevelEarn =
            new BattleEarnTracker(gameMode == GameMode.CAMPAIGN_MODE ? CurrencyType.GOLD : CurrencyType.TOKEN);
        if (gameMode == GameMode.CAMPAIGN_MODE)
            campaignData.ResetData();

        timerFlyingAirDrop = 0f;
        gameLevel.GenerateObstacle();
    }

    #endregion

    public void CleanUp()
    {
        RemoveListeners();
        gameLevel.CleanUp();
        _waveController.CleanUp();

        KillingZombieAward.CollectAllCoins();
        OnSaveAddOn();
        //_listSpawnedOstacles.Clear();
        CleanAddOn();
        FloatingTextQueue.StopLoop();

        GameObject.Destroy(gameLevel.gameObject);

        InGameCanvas.instance.CleanUp();

        foreach (var airDrop in _listSpawnedAirdrop)
        {
            if (airDrop != null)
                Destroy(airDrop.gameObject);
        }

        _listSpawnedAirdrop.Clear();

        RemoveAllObstacles();
    }

    #region Game Utils

    public Vector3 wallPosition { get; private set; }

    public void FindWallPos()
    {
        var wall = GameObject.Find("wall");
        if (wall != null)
            wallPosition = wall.transform.position;
        else
            Debug.LogError("cant find wall!!!");
    }

    public Character GetCharacter(string heroId)
    {
        Character result = null;
        gameLevel._dictCharacter.TryGetValue(heroId, out result);
        //if (result == null)
        //{
        //    Debug.LogError($"Cant find char wit {heroId}");
        //}

        return result;
    }

    //public static AutoDespawnParticles PlayEffect(AutoDespawnParticles parPrefab, Vector3 position, Quaternion rotation, Transform parent = null, float scale = 1.0f)
    //{
    //    if (parPrefab == null)
    //        return null;
    //    var scaleTrf = Vector3.one * scale;

    //    if (scaleTrf.x <= 0)
    //        scaleTrf = Vector3.one;

    //    AutoDespawnParticles _par =
    //        Pooly.Spawn<AutoDespawnParticles>(parPrefab.transform, position, rotation, null);
    //    _par.transform.SetParent(parent);
    //    _par.transform.localScale = Vector3.one;

    //    _par.transform.localScale = scaleTrf;
    //    _par.ResetPar();
    //    _par.PlayEffect();
    //    return _par;
    //}

    #endregion

    public void RemoveListeners()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.CASTLE_DEFEATED,
            new Action(OnCampaignModeDefeat));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.REVIVE_CAMPAIGN_BATTLE,
            new Action(ReviveCampaignMode));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.ON_TOUCH_GROUND,
            new Action<Vector3, bool>(OnTapGround));
    }

    public void SetPauseGameplay(bool value)
    {
        IsPausedGame = value;
    }

    public void UpdateWaveController(float _deltaTime)
    {
    }

    #region ADD ON

    [Header("Add On")] public Transform AddOnParent;

    private List<BaseCharacterUltimate> _listAddOn = new List<BaseCharacterUltimate>();

    public IEnumerator<float> SpawnAddOnSkill(string AddOnID, Action<BaseCharacterUltimate> callback)
    {
        BaseCharacterUltimate skillInstance = null;

        var exists = _listAddOn.Find(x => x.SkillID == AddOnID);
        if (exists == null)
        {
            //var path = $"GameData/AddOn/{AddOnID}";
            //var skillRes = Resources.Load<BaseCharacterUltimate>(path);

            //if (skillRes != null)
            //{
            //    skillInstance = GameObject.Instantiate(skillRes, transform);
            //    skillInstance.PreInit(AddOnID, false);
            //}

            var op = Addressables.LoadAssetAsync<GameObject>(AddOnID);
            while (!op.IsDone)
                yield return Timing.WaitForOneFrame;
            if (op.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                callback?.Invoke(skillInstance);
                Debug.LogError($"SpawnAddOnSkill failed!!! {AddOnID}");
            }

            skillInstance = GameObject.Instantiate(op.Result, transform).GetComponent<BaseCharacterUltimate>();
            skillInstance.SetReduceCountDown(0);
            skillInstance.PreInit(AddOnID, false);

            _listAddOn.Add(skillInstance);
        }

        callback?.Invoke(skillInstance);
    }

    public void RemoveAllAddOns()
    {
        foreach (var addOn in _listAddOn)
        {
            GameObject.Destroy(addOn);
        }

        _listAddOn.Clear();
    }

    public void UpdateAddOn(float _deltaTime)
    {
        if (_listAddOn == null || _listAddOn.Count <= 0)
            return;
        for (int i = _listAddOn.Count - 1; i >= 0; i--)
        {
            _listAddOn[i].UpdateSkill(_deltaTime);
        }
    }

    public void OnSaveAddOn()
    {
        for (int i = _listAddOn.Count - 1; i >= 0; i--)
        {
            _listAddOn[i].OnSaveGame();
        }
    }

    public void CleanAddOn()
    {
        for (int i = _listAddOn.Count - 1; i >= 0; i--)
        {
            _listAddOn[i].CleanUp();
        }
    }

    #endregion

    #region AirDrop

    private float timerFlyingAirDrop = 0f;
    public static float MIN_SPAWN_FLYING_AIDROP = 40f;
    public static float MAX_SPAWN_FLYING_AIDROP = 90f;
    public float FlyingAirDropDuration = 0f;

    private FlyingAirDrop _flyingAirDropPrefab;

    private List<Transform> _listSpawnedAirdrop = new List<Transform>();

    public bool EnableFlyingAirdrop {
        get { return true; }
    }

    public void SpawnAirDrop()
    {
        Timing.RunCoroutine(SpawnAirDropCoroutine());
    }

    public IEnumerator<float> SpawnAirDropCoroutine()
    {
        Vector3 randPos = gameLevel._skillZoneMarker.centerSkillMarker.position;
        randPos.x += UnityEngine.Random.Range(-0.5f, 0.5f);
        randPos.z += UnityEngine.Random.Range(-0.5f, 0.5f);
        //var airDrop = Pooly.Spawn<AirDrop>(POOLY_PREF.AIR_DROP, Vector3.zero, Quaternion.identity, transform);
        var op = Addressables.LoadAssetAsync<GameObject>(POOLY_PREF.AIR_DROP);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        var airDrop = GameObject.Instantiate(op.Result).GetComponent<AirDrop>();
        airDrop.Initialize();
        airDrop.SpawnAirDrop(randPos);

        _listSpawnedAirdrop.Add(airDrop.transform);
    }

    public void SpawnFlyingAirDrop()
    {
        Timing.RunCoroutine(SpawnFlyingAirDropCoroutine());
    }


    public IEnumerator<float> SpawnFlyingAirDropCoroutine()
    {
        var op = Addressables.LoadAssetAsync<GameObject>(POOLY_PREF.FLYING_AIR_DROP);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        _flyingAirDropPrefab = op.Result.GetComponent<FlyingAirDrop>();
        var airDrop = GameObject.Instantiate(op.Result).GetComponent<FlyingAirDrop>();
        airDrop.Initialize(15f);
        airDrop.TravelAroundMap(gameLevel._skillZoneMarker.startSkillMarker.position,
            gameLevel._skillZoneMarker.endSkillMarker.position);
        _listSpawnedAirdrop.Add(airDrop.transform);
    }

    public void InitSpawnFlyingAirDrop()
    {
        MIN_SPAWN_FLYING_AIDROP = DesignHelper.GetConfigDesign("MIN_SPAWN_FLYING_AIDROP").Value;
        MAX_SPAWN_FLYING_AIDROP = DesignHelper.GetConfigDesign("MAX_SPAWN_FLYING_AIDROP").Value;
        FlyingAirDropDuration = UnityEngine.Random.Range(MIN_SPAWN_FLYING_AIDROP, MAX_SPAWN_FLYING_AIDROP);
    }

    public void UpdateSpawnFlyingAirDrop(float _deltaTime)
    {
        if (!EnableFlyingAirdrop)
            return;

        timerFlyingAirDrop += _deltaTime;
        if (timerFlyingAirDrop >= FlyingAirDropDuration)
        {
            FlyingAirDropDuration = UnityEngine.Random.Range(MIN_SPAWN_FLYING_AIDROP, MAX_SPAWN_FLYING_AIDROP);
            SpawnFlyingAirDrop();
            timerFlyingAirDrop = 0f;
        }
    }

    public void DestroyAirdrop(Transform instance)
    {
        var toDestroy = _listSpawnedAirdrop.Find(x => x == instance);
        if (toDestroy != null)
        {
            //Pooly.Despawn(toDestroy.transform);
            _listSpawnedAirdrop.Remove(instance);
            GameObject.Destroy(instance.gameObject);
        }
    }

    #endregion

    public void OnSaveGame()
    {
        OnSaveAddOn();
    }

    public bool IsValidRefillHP()
    {
        bool result = false;

        if (gameMode == GameMode.CAMPAIGN_MODE || gameMode == GameMode.IDLE_MODE)
        {
            if (CastleHP.CurrentHp <= CastleHP.GetHPWithCoeff() * 0.98f)
                return true;
        }
        //else
        //{
        //    foreach (var hero in gameLevel._dictCharacter)
        //    {
        //        if (hero.Value._health.CurrentHp <= hero.Value._health.GetHPWithCoeff() * 0.98f)
        //            return true;
        //    }
        //}

        return result;
    }

    public void RefillHPGamePlay(float hp)
    {
        if (hp == 0)
            return;

        if (gameMode == GameMode.CAMPAIGN_MODE || gameMode == GameMode.IDLE_MODE)
        {
            CastleHP.RefillHP(hp);
            // campaignData.ResetData();
        }

        //else
        //{
        //    foreach (var hero in gameLevel._dictCharacter)
        //    {
        //        hero.Value._health.RefillHP(hp);
        //    }
        //}
    }

    public void RefillHPPercentGamePlay(float Percentage)
    {
        if (gameMode == GameMode.CAMPAIGN_MODE ||
            gameMode == GameMode.IDLE_MODE)
        {
            CastleHP.RefillHPPercent(Percentage);
        }

        //else
        //{
        //    foreach (var hero in gameLevel._dictCharacter)
        //    {
        //        hero.Value._health.RefillHPPercent(Percentage);
        //    }
        //}
    }

    public void RefillHPHero(string heroId, float hpRecover)
    {
        gameLevel._dictCharacter[heroId]._health.RefillHP(hpRecover);
    }

    public void ResetAllHeroData()
    {
        foreach (var hero in this.gameLevel._dictCharacter)
        {
            hero.Value.ResetHeroData();
        }
    }

    #region HintTap

    public bool ShouldShowHintTap()
    {
        return this.CurrentLevel <= 2;
    }

    public const float HINT_TAP_DELAY = 5f;
    public const float HINT_TAP_SHOWTIME = 3f;

    private float timerShowHintTap = 0f;
    private float showingTime = 0f;
    private bool showingHintTap = false;

    public void InitializeHintTap()
    {
        showingHintTap = false;
        timerShowHintTap = 0f;
    }

    public void OnTapGround(Vector3 tapPoint, bool forceShoot)
    {
        timerShowHintTap = 0f;
        showingHintTap = false;
        showingTime = 0f;
        InGameCanvas.instance._hintTapOnScreen.SetEnable(false);
    }

    public void UpdateHintTap(float _deltaTime)
    {
        if (!ShouldShowHintTap())
            return;

        if (!showingHintTap)
        {
            timerShowHintTap += _deltaTime;
            if (timerShowHintTap >= HINT_TAP_DELAY)
            {
                timerShowHintTap = 0f;
                InGameCanvas.instance._hintTapOnScreen.PlayAnimLoop();
                showingHintTap = true;
                showingTime = 0f;
            }
        }
        else
        {
            showingTime += _deltaTime;
            if (showingTime >= HINT_TAP_SHOWTIME)
            {
                showingTime = 0f;
                showingHintTap = false;
                InGameCanvas.instance._hintTapOnScreen.SetEnable(false);
            }
        }
    }

    #endregion

    #region Queue Floating text

    public FloatingTextQueue FloatingTextQueue { get; private set; }

    public void EnqueueFloatingText(FloatingTextData _data)
    {
        FloatingTextQueue.EnqueueProcess(_data);
    }

    #endregion

    public void SetMultiplierSpeedForSubwave(int wave, int subwave, float speed)
    {
        this._waveController.SetMultiplierSpeedForSubwave(wave, subwave, speed);
    }

    #region ZombieAngry

    public int targetZombieAngry = 0;
    private int CountZombieHit = 0;

    private int maxZomAngry;
    private int minZomAngry;

    public void InitializeZombieAngrySys()
    {
        this.minZomAngry = (int)DesignHelper.GetConfigDesign(GameConstant.MIN_COUNT_ZOMBIE_ANGRY).Value;
        this.maxZomAngry = (int)DesignHelper.GetConfigDesign(GameConstant.MAX_COUNT_ZOMBIE_ANGRY).Value;

        this.targetZombieAngry = UnityEngine.Random.Range(minZomAngry, maxZomAngry + 1);

    }
    public bool CheckIsAngryZombie()
    {
        ++CountZombieHit;
        if (CountZombieHit >= targetZombieAngry)
        {
            CountZombieHit = 0;
            this.targetZombieAngry = UnityEngine.Random.Range(minZomAngry, maxZomAngry + 1);
            return true;
        }

        return false;
    }

    #endregion

    #region Power Saving Mode

    public bool IsPowerSaving { get; private set; }

    private float _timerSavingMode = 0f;
    public float SAVING_MODE_DURATION = 120f;

    private Action<bool> OnDisablePowerSavingMode;

    public void InitializePowerSavingMode()
    {
        OnDisablePowerSavingMode = DisablePowerSavingMode;
        SAVING_MODE_DURATION = DesignHelper.GetConfigDesign(GameConstant.POWER_SAVING_IDLE_DURATION).Value;
    }

    public void UpdatePowerSavingMode(float _deltaTime)
    {
        if (!GameMaster.instance.IsPowerSaving)
        {
#if !UNITY_EDITOR
            if (Input.touchCount > 0)
            {
                _timerSavingMode = 0f;
            }
#else
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                _timerSavingMode = 0f;
            }
#endif

            _timerSavingMode += _deltaTime;
            if (_timerSavingMode >= SAVING_MODE_DURATION)
            {
                IsPowerSaving = true;
                Timing.RunCoroutine(EnablePowerSaving());
            }
        }
    }

    IEnumerator<float> EnablePowerSaving()
    {
        SetPauseGameplay(true);
        TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_POWER_SAVING_MODE, false, null, OnDisablePowerSavingMode);
        yield return Timing.WaitForSeconds(0.3f);
        yield return Timing.WaitForOneFrame;

        //ResourceManager.instance.LimitAllPrefabs();
        //yield return Timing.WaitForOneFrame;

        Resources.UnloadUnusedAssets();
        yield return Timing.WaitForOneFrame;
        GC.Collect();
        yield return Timing.WaitForOneFrame;
        HUDPowerSaving _hudPowerSaving = (HUDPowerSaving)TopLayerCanvas.instance.GetHUD(EnumHUD.HUD_POWER_SAVING_MODE);
        if (_hudPowerSaving != null)
        {
            _hudPowerSaving.FadeIn();
        }
        yield return Timing.WaitForOneFrame;
        GameMaster.instance.SetPowerSavingMode(true);
        SetPauseGameplay(false);

    }

    public void DisablePowerSavingMode(bool isDisable)
    {
        _timerSavingMode = 0;
        GameMaster.instance.SetPowerSavingMode(false);
    }

    #endregion

    #region CameraShake
    public void DoCameraShake()
    {
        this.cameraShake.DoShake();
    }

    #endregion

    #region Cheat!!!!

    public void CheatFinishLevel(int stars)
    {
        float multiplier = 0.5f;
        if (stars >= 3)
            multiplier = 0.91f;
        else if (stars == 2)
            multiplier = 0.51f;
        else
            multiplier = 0.2f;

        if (this._mode == GameMode.CAMPAIGN_MODE)
        {
            this.CastleHP.SetCurrentHp(multiplier * this._castleHP.startHp);
            _waveController.DoLevelUp();
        }
        else
        {
            // do nothing
        }
    }

    #endregion
}
