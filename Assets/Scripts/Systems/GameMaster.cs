using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.talent;
using DG.Tweening;
using UnityEngine;
using MEC;
using UnityEngine.SceneManagement;
using Ez.Pooly;
using UnityEngine.AddressableAssets;
using Facebook.Unity;
using QuickType;


[Serializable]
public struct SimpleLightSetting
{
    public Color _mapGroundColor;
    public Color _sourceColor;
    public Color _equatorColor;
    public Color _groundColor;
    public float IntensityMultiplie;
    public int Bounces;
}


public class GameMaster : BaseSystem<GameMaster>
{
    public static GameMaster instance;
    public ZombieWalking zombieWalking;

    [Header("RunTimeAssets")] public AssetReference ResourcesManagerAddress;
    public AssetReference PoolyAddress;
    public AssetReferencePoolySO gamePlayPooly;

    public OptimizationController OptmizationController { get; private set; }

    public static bool IsSpeedUp {
        get {
            return Time.timeScale != 1;
        }
    }

    public static bool ENABLE_ARMOUR_FEATURE {
        get { return false; }
    }

    public Camera menuCamera;
    public GameMode currentMode { get; private set; }

    public static bool ENABLE_IDLE_MODE {
        get { return true; }
    }

    private int initSleepTimeOut = 0;

    private long startGameTs = 0;
    private float lastTimeScale = 1.0f;

    private void Awake()
    {
        instance = this;
        _dictParticlesDelay = new Dictionary<string, float>();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        // Application.backgroundLoadingPriority = ThreadPriority.High;

        StartLoadGame();
    }

    #region Load Game

    public SimpleLightSetting loadingSceneRenderSetting;

    public void StartLoadGame()
    {
        Timing.RunCoroutine(StartLoadGameCoroutine(OnLoadingPercentChanged));
    }

    private IEnumerator<float> StartLoadGameCoroutine(Action<float> loadingPercent)
    {
        DOTween.SetTweensCapacity(1000, 100);
        AnalyticsManager.instance.Initialize();
        // init facebook sdk
        FB.Init(OnInitFBComplete, OnFBHideUnity);

        FirebaseRemoteSystem.instance.Initialize();

        initSleepTimeOut = Screen.sleepTimeout;

        OptmizationController = new OptimizationController();
        OptmizationController.Initialize();
        //loading resources
        var timeCounter = Time.time;
        var op = ResourcesManagerAddress.InstantiateAsync(transform);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;

        Debug.Log($"Time Load ResourceManager: {Time.time - timeCounter}");

        //loading pooly
        timeCounter = Time.time;
        op = PoolyAddress.InstantiateAsync(transform);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        Debug.Log($"Time Load Pooly: {Time.time - timeCounter}");

        loadingPercent?.Invoke(10);
        bool waitingTask = true;
        //init design
        DesignManager.instance.InitializeCoroutine((complete) =>
        {
            if (complete)
                waitingTask = false;
        });


        while (waitingTask)
        {
            yield return Timing.WaitForOneFrame;
        }


        if (InterstitialController == null)
        {
            InterstitialController = new GamePlayInterstitial();
            InterstitialController.Initialize();
        }

        loadingPercent?.Invoke(15);
        NetworkDetector.instance.Initialize();
        TimeService.instance.Initialize();
        startGameTs = TimeService.instance.GetCurrentTimeStamp();

        //game system
        AudioSystem.instance.Initialize();
        loadingPercent?.Invoke(17);
        yield return Timing.WaitForOneFrame;



        //login social here!!!
        loadingPercent?.Invoke(20);

        if (SocialLogin.instance.IsEnable())
        {
            SocialLogin.instance.Initialize();
            FirestoreSystem.instance.Initialize();

            yield return Timing.WaitForOneFrame;
            waitingTask = true;
            SocialLogin.instance.LoginFirebase((complete, authID) =>
            {
                Debug.Log($"LoginFirebase RESULT complete:{complete} FirebaseID: {authID}");

                waitingTask = false;
                //TopLayerCanvas.instance._hudGameLoading?.UpdateUserInfoID(authID);
            });
            while (waitingTask)
            {
                yield return Timing.WaitForOneFrame;
            }

            if (SocialLogin.instance.IsSignedInFireBase())
                EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_USER_INFO_ID,
                    SocialLogin.instance.FirebaseAuthID);
        }


        //init save
        loadingPercent?.Invoke(30);
        waitingTask = true;
        SaveManager.Instance.InitializeCoroutine((complete) =>
        {
            if (complete)
                waitingTask = false;
        });
        while (waitingTask)
        {
            yield return Timing.WaitForOneFrame;
        }

        // init localize
        waitingTask = true;
        LocalizeController.Instance.InitializeCoroutine((complete) =>
        {
            if (complete)
            {
                waitingTask = false;
            }
        });
        while (waitingTask)
        {
            yield return Timing.WaitForOneFrame;
        }

        //init third party


        IAPManager.instance.Initialize();
        yield return Timing.WaitForOneFrame;


        AdsManager.instance.Initialize();
        yield return Timing.WaitForOneFrame;
        loadingPercent?.Invoke(25);

        // Init talent
        TalentManager.Init();
        TalentManager.UpdateFromSave();

        MissionManager.Instance.Init();

        // Init reminder controller
        ReminderManager.Init();

        // Init battle pass
        BattlePassManager.Init();


        if (SaveManager.Instance.Data.MetaData.FirstTimeJoinGame)
        {
            SaveManager.Instance.PostFirstTimeJoinGame();
        }

        loadingPercent?.Invoke(100);
        yield return Timing.WaitForOneFrame;

        Application.backgroundLoadingPriority = ThreadPriority.Low;
        FinishLoadingProcess();

        if (SaveManager.Instance.Data.MetaData.FirstTimeJoinGame)
            SaveManager.Instance.Data.MetaData.FirstTimeJoinGame = false;
    }

    private void OnLoadingPercentChanged(float curPercent)
    {
        if (TopLayerCanvas.instance)
        {
            TopLayerCanvas.instance._hudGameLoading?.SetLoadingPercent(curPercent);
        }
    }

    private void BackupLightSetting()
    {
        loadingSceneRenderSetting = new SimpleLightSetting();
        loadingSceneRenderSetting._groundColor = RenderSettings.ambientGroundColor;
        loadingSceneRenderSetting._equatorColor = RenderSettings.ambientEquatorColor;
        loadingSceneRenderSetting._sourceColor = RenderSettings.ambientSkyColor;
        loadingSceneRenderSetting.IntensityMultiplie = RenderSettings.ambientIntensity;
        loadingSceneRenderSetting.Bounces = RenderSettings.reflectionBounces;
    }

    private void ReverseLoadingLightSetting()
    {
        RenderSettings.ambientGroundColor = loadingSceneRenderSetting._groundColor;
        RenderSettings.ambientEquatorColor = loadingSceneRenderSetting._equatorColor;
        RenderSettings.ambientSkyColor = loadingSceneRenderSetting._sourceColor;
        RenderSettings.ambientIntensity = loadingSceneRenderSetting.IntensityMultiplie;
        RenderSettings.reflectionBounces = loadingSceneRenderSetting.Bounces;
    }

    private void FinishLoadingProcess()
    {
        BackupLightSetting();
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.GAME_LOADED);
        
        if (SaveManager.Instance.Data.MetaData.FirstTimeJoinGame)
        {
            GoToSceneGame(GameMode.CAMPAIGN_MODE, 1, true);
        }
        else
        {

            LoadScene(GameConstant.SCENE_MAIN_MENU, (complete) =>
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    //TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
                    TopLayerCanvas.instance.ShowHUDLoading(false);
                    TopLayerCanvas.instance._hudGameLoading?.Hide();
                });

                MainMenuCanvas.instance.Initialize();
                MiniGameController.instance.InitializeCoroutine((success) => { });
            });
        }

    }

    #endregion

    #region Scene Utils

    public void LoadScene(string sceneName, Action<bool> callback)
    {
        StartCoroutine(LoadSceneAsync(sceneName, callback));
    }

    IEnumerator LoadSceneAsync(string sceneName, Action<bool> callback)
    {
        yield return null;

        //Begin to load the Scene you specify
        if (sceneName == GameConstant.SCENE_GAME_PLAY)
        {
            if (SceneManager.GetSceneByName(GameConstant.SCENE_MAIN_MENU).isLoaded)
                SceneManager.UnloadSceneAsync(GameConstant.SCENE_MAIN_MENU);
        }
        else if (sceneName == GameConstant.SCENE_MAIN_MENU)
        {
            if (SceneManager.GetSceneByName(GameConstant.SCENE_GAME_PLAY).isLoaded)
                SceneManager.UnloadSceneAsync(GameConstant.SCENE_GAME_PLAY);
        }

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
        Debug.Log("Pro :" + asyncOperation.progress);
        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (asyncOperation.isDone)
            callback?.Invoke(true);
    }

    #endregion

    public void QuickPlayCampaignLevel(int level)
    {
        SaveManager.Instance.Data.OverwritePlayerProgress(GameMode.CAMPAIGN_MODE, level);
        GameMaster.instance.GoToSceneGame(GameMode.CAMPAIGN_MODE, level);
    }

    public void GoToSceneGame(GameMode gameMode, int level = 0, bool FirstTimeJoinGame = false)
    {
        currentMode = gameMode;
        Timing.RunCoroutine(GoToSceneGameCoroutine(level, FirstTimeJoinGame));
    }

    IEnumerator<float> GoToSceneGameCoroutine(int level, bool firstTimeJoinGame = false)
    {
        float timerWait = 0f;
        TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_LOADING, false, null, true);
        //TopLayerCanvas.instance.ShowHUDLoading(true);
        yield return Timing.WaitForSeconds(0.5f);

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(DesignManager.instance.LoadDesignsGotoGame()));

        PrepareGoToSceneGame();

        // Load map obstacle
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ResourceManager.instance.LoadMapObstacleModel()));

        // Load gameplay pooly
        foreach (var VARIABLE in gamePlayPooly.assetReferences)
        {
            if (!VARIABLE.IsValid())
            {
                var op = VARIABLE.LoadAssetAsync<GameObject>();

                while (!op.IsDone)
                {
                    yield return Timing.WaitForOneFrame;
                }

                Pooly.CreateMissingPooledItem(op.Result.transform, op.Result.name, false);
            }
        }

        GC.Collect();
        yield return Timing.WaitForOneFrame;
        Resources.UnloadUnusedAssets();
        yield return Timing.WaitForOneFrame;
        GC.Collect();
        yield return Timing.WaitForOneFrame;
        bool waitTask = true;

        LoadScene(GameConstant.SCENE_GAME_PLAY, (loadcomplete) =>
        {
            if (loadcomplete)
                waitTask = false;
        });

        if (waitTask)
            yield return Timing.WaitForOneFrame;


        while (MainGameContext.instance == null)
        {
            yield return Timing.WaitForOneFrame;
            timerWait += Time.deltaTime;
            if (timerWait >= 5.0f)
            {
                Debug.LogError("Load Scene Game TIMEOUT!!!!");
                break;
            }
        }

        if (MainGameContext.instance != null)
            MainGameContext.instance.Initialize(level);
        else
            Debug.LogError("MainGameContext is null!!!!");

        menuCamera?.gameObject.SetActive(false);
        yield return Timing.WaitForSeconds(0.7f);
        TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);

        if (firstTimeJoinGame)
        {
            yield return Timing.WaitForSeconds(0.3f);
            TopLayerCanvas.instance._hudGameLoading.Hide();
        }
    }

    private void PrepareGoToSceneGame()
    {
        CurrencyModels.instance.RemoveAllCallback();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (MiniGameController.instance != null)
            MiniGameController.instance.CleanUp();

        MasterCanvas.CurrentMasterCanvas?.CleanUp();

        DesignManager.instance.LoadAirdropDesign();

        ResetEffectController();

        if (GameMaster.instance.currentMode == GameMode.IDLE_MODE)
        {
            AddressableManager.instance.PreloadFromAddressable<BaseHUD>(POOLY_PREF.GetHUDPrefabByType(EnumHUD.HUD_POWER_SAVING_MODE), null);
        }

        AddressableManager.instance.PreloadFromAddressable<FlyingAirDrop>(POOLY_PREF.FLYING_AIR_DROP, null);


    }

    public void BackToMenuScene()
    {
        FindObjectOfType<MainGameContext>().enabled = false;
        Timing.RunCoroutine(BackToMenuSceneCoroutine());
        Screen.sleepTimeout = initSleepTimeOut;
    }

    IEnumerator<float> BackToMenuSceneCoroutine()
    {
        TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_LOADING, false, null, true);
        //TopLayerCanvas.instance.ShowHUDLoading(true);
        yield return Timing.WaitForSeconds(0.5f);

        bool waitTask = true;
        ReturnFromBattle();
        GamePlayController.instance.CleanUp();

        yield return Timing.WaitForOneFrame;

        GamePlayController.instance._waveController.zombiesOnTheWawe.Clear();
        ResourceManager.instance.LimitAllPrefabs();
        yield return Timing.WaitForSeconds(0.3f);

        yield return Timing.WaitForOneFrame;
        Resources.UnloadUnusedAssets();
        yield return Timing.WaitForOneFrame;
        GC.Collect();
        yield return Timing.WaitForOneFrame;
        ReverseLoadingLightSetting();
        LoadScene(GameConstant.SCENE_MAIN_MENU, (loadcomplete) =>
        {
            if (loadcomplete)
                waitTask = false;
        });

        while (waitTask)
            yield return Timing.WaitForOneFrame;

        menuCamera.gameObject.SetActive(true);
        AdsManager.instance.HideAdsBanner((complete) => { });

        TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
        //TopLayerCanvas.instance.ShowHUDLoading(false);

        MainMenuCanvas.instance.Initialize();
        MiniGameController.instance.InitializeCoroutine((success) => { });

        yield return Timing.WaitForSeconds(0.2f);
        InterstitialController?.SetTriggerCheck(true);
        InterstitialController?.CheckAndShowInterstitialAds(SaveGameHelper.GetCurrentCampaignLevel(), null);
    }

    public void ReturnFromBattle()
    {
        if (currentMode == GameMode.CAMPAIGN_MODE)
            SaveManager.Instance.Data.GameData.CampaignProgress.LastLogoutTS =
                TimeService.instance.GetCurrentTimeStamp();
        else if (currentMode == GameMode.IDLE_MODE)
        {
            var currentTS = TimeService.instance.GetCurrentTimeStamp();
            var timeDiff = currentTS - GamePlayController.instance.StartGameTS;

            SaveManager.Instance.Data.GameData.IdleProgress.TotalTimePlayed += timeDiff;
            SaveManager.Instance.Data.GameData.IdleProgress.LastLogoutTS = currentTS;
        }


        SaveManager.Instance.SetDataDirty();
    }

    private void Update()
    {
        float _deltaTime = Time.deltaTime;
        NetworkDetector.instance?.UpdateSystem(_deltaTime);
        TimeService.instance?.UpdateSystem(_deltaTime);

        //if (SaveManager.Instance != null && SaveManager.Instance.Data != null)
        //    Logwin.Log("Count GamePlay:", SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CountPlayGame);
    }

    #region PlayEffect

    public Dictionary<string, float> _dictParticlesDelay { get; private set; }

    public void ResetEffectController()
    {
        if (_dictParticlesDelay != null)
            _dictParticlesDelay.Clear();
        else
            _dictParticlesDelay = new Dictionary<string, float>();
    }

    public AutoDespawnParticles PlayEffect(AutoDespawnParticles parPrefab, Vector3 position, Quaternion rotation,
        Transform parent = null, float scale = 1.0f)
    {
        if (parPrefab == null)
            return null;

        if (parPrefab.ThresHoldDuration > 0)
        {
            var key = CombineParticleName(parPrefab.name, parent);
            if (_dictParticlesDelay.ContainsKey(key))
            {
                if (Time.time < _dictParticlesDelay[key])
                {
                    //Debug.LogError("Blocked threshold");
                    return null;
                }


                _dictParticlesDelay[key] = Time.time + parPrefab.ThresHoldDuration;
            }
            else
            {
                _dictParticlesDelay.Add(key, Time.time + parPrefab.ThresHoldDuration);
            }

        }

        var scaleTrf = Vector3.one * scale;

        if (scaleTrf.x <= 0)
            scaleTrf = Vector3.one;

        AutoDespawnParticles _par =
            Pooly.Spawn<AutoDespawnParticles>(parPrefab.transform, position, rotation, null);

        _par.transform.SetParent(parent);
        _par.transform.localScale = Vector3.one;

        _par.transform.localScale = scaleTrf;
        _par.ResetPar();
        _par.PlayEffect();

        return _par;
    }

    public static AutoDespawnParticles PlayEffect(string effectName, Vector3 position, Quaternion rotation,
        Transform parent = null, float scale = 1.0f)
    {
        var scaleTrf = Vector3.one * scale;

        if (scaleTrf.x <= 0)
            scaleTrf = Vector3.one;

        AutoDespawnParticles _par =
            Pooly.Spawn<AutoDespawnParticles>(effectName, position, rotation, null);

        _par.transform.SetParent(parent);
        _par.transform.localScale = Vector3.one;

        _par.transform.localScale = scaleTrf;
        _par.ResetPar();
        _par.PlayEffect();

        return _par;
    }

    public static AutoDespawnParticles PlayEffect(COMMON_FX fx, Vector3 position, Quaternion rotation,
        Transform parent = null, float scale = 1.0f)
    {
        var scaleTrf = Vector3.one * scale;

        if (scaleTrf.x <= 0)
            scaleTrf = Vector3.one;

        AutoDespawnParticles _par =
            Pooly.Spawn<AutoDespawnParticles>(ResourceManager.instance._effectResources.GetEffect(fx).transform,
                position, rotation, null);

        _par.transform.SetParent(parent);
        _par.transform.localScale = Vector3.one;

        _par.transform.localScale = scaleTrf;
        _par.ResetPar();
        _par.PlayEffect();

        return _par;
    }

    public static string CombineParticleName(string parName, Transform parent)
    {
        if (parent == null)
            return $"0_{parName}";
        else
            return $"{parent.GetInstanceID()}_{parName}";
    }

    #endregion

    #region Idle Chest Rewards

    public bool EnableIdleChestReward {
        get { return true; }
    }

    public long GetIdleGoldReward()
    {
        long result = 0;
        var diff = TimeService.instance.GetCurrentTimeStamp() -
                   SaveManager.Instance.Data.Inventory.LastCollectIdleChestTS;
        if (diff > 0)
        {
            var currentLevel = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CurrentLevel;
            var des = DesignHelper.GetIdleChestDesign(currentLevel);
            if (des != null)
            {
                var diffMin = diff;
                result = (long)(diffMin * des.GoldPerSec);
                if (result >= des.MaxTime * des.GoldPerSec)
                {
                    result = (long)(des.MaxTime * des.GoldPerSec);
                }
            }
        }


        return result;
    }

    public bool ReachMaxIdleGold()
    {
        bool result = false;
        var currentLevel = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CurrentLevel;
        var des = DesignHelper.GetIdleChestDesign(currentLevel);
        result = (GetIdleGoldReward() >= (long)(des.MaxTime * des.GoldPerSec));
        return result;
    }

    public void OnCollectIdleReward()
    {
        var totalGold = GetIdleGoldReward();
        if (totalGold >= 0)
        {
            var currentLevel = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CurrentLevel;
            var des = DesignHelper.GetIdleChestDesign(currentLevel);
            ShowHUDEarnIdleChest(totalGold, (float)(des.GoldPerSec * 3600), (long)(des.GoldPerSec * des.MaxTime));
        }
        else
        {
            MainMenuCanvas.instance.ShowFloatingTextNotify("Idle Reward Currently not available!");
        }
    }

    private void ShowHUDEarnIdleChest(float _totalGold, float _goldPerHour, long _maxGold)
    {
        var data = new IdleRewardData()
        {
            goldPerHour = _goldPerHour,
            totalGoldEarn = _totalGold,
            maxGold = _maxGold
        };

        MainMenuCanvas.instance.ShowHUD(EnumHUD.HUD_IDLE_REWARD, false, null, data);
    }

    #endregion

    #region Check and show interstitial

    public GamePlayInterstitial InterstitialController { get; private set; }

    #endregion

    private void OnApplicationQuit()
    {
        var currentTs = TimeService.instance.GetCurrentTimeStamp();
        var totalSec = currentTs - startGameTs;
        if (totalSec > 0)
        {
            AnalyticsManager.instance.LogEvent(AnalyticsConstant.ANALYTICS_ENUM.AVG_PLAY_TIME,
                new LogEventParam("time-sec", totalSec));
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        //#if !UNITY_EDITOR
        //        if (!focus)
        //        {
        //            lastTimeScale = Time.timeScale;
        //            Time.timeScale = 0f;
        //        }
        //        else
        //        {
        //            Time.timeScale = lastTimeScale;
        //        }
        //#endif
    }

    #region Utils
    public static bool IsZombieTag(string tag)
    {
        if (tag.Equals(TagConstant.TAG_ZOMBIE) ||
            tag.Equals(TagConstant.TAG_BOSS))
            return true;

        return false;
    }

    #endregion

    public bool IsPowerSaving { get; private set; }

    public void SetPowerSavingMode(bool _value)
    {
        IsPowerSaving = _value;
        if (IsPowerSaving)
        {
            OptmizationController.DisableAllFX();
        }
        else
        {
            OptmizationController.ResumeLastProfile();
        }
    }

    #region FB SDK
    private void OnInitFBComplete()
    {
        Debug.Log("[FB SDK] OnInitFBComplete!!!");
    }

    private void OnFBHideUnity(bool isGameShown)
    {
        Debug.Log($"[FB SDK] OnFBHideUnity!!! {isGameShown}");
    }
    #endregion

   
}




[Serializable]
public class GamePlayInterstitial
{
    public int ShowAdsDuration;
    public int FirstTimeAdsLevel;


    private bool _triggerCheck = false;

    public void Initialize()
    {
        var remoteFirstInterstitial = FirebaseRemoteSystem.instance.GetConfigValue("FIRST_TIME_SHOW_INTERSTITIAL_LEVEL");
        if (remoteFirstInterstitial.Source == Firebase.RemoteConfig.ValueSource.RemoteValue)
            FirstTimeAdsLevel = (int)remoteFirstInterstitial.LongValue;
        else
            FirstTimeAdsLevel = (int)DesignHelper.GetConfigDesign("FIRST_TIME_SHOW_INTERSTITIAL_LEVEL").Value;


        var remoteDuration = FirebaseRemoteSystem.instance.GetConfigValue("DURATION_SHOW_INTERSTITIAL_LEVEL");
        if (remoteDuration.Source == Firebase.RemoteConfig.ValueSource.RemoteValue)
        {
            ShowAdsDuration = (int)remoteDuration.LongValue;
        }
        else
        {
            ShowAdsDuration = (int)DesignHelper.GetConfigDesign("DURATION_SHOW_INTERSTITIAL_LEVEL").Value;
        }



        _triggerCheck = false;
    }

    public void IncreaseCountGamePlay()
    {
        if (SaveManager.Instance.Data.DayTrackingData.IsPassedFirstTimeShowInterstitial)
            SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CountPlayGame++;
    }

    public bool CheckAndShowInterstitialAds(int currentFinishedLevel, Action<bool> callback)
    {
        if (SROptions.Current.EveryLeveLShowInterstitial)
        {
            AdsManager.instance.ShowAdsInterstitial((complete) => { callback?.Invoke(true); });
            return true;
        }

        if (!_triggerCheck)
        {
            callback?.Invoke(false);
            return false;
        }

        //first time show ads
        if (!SaveManager.Instance.Data.DayTrackingData.IsPassedFirstTimeShowInterstitial)
        {
            if (currentFinishedLevel < FirstTimeAdsLevel)
            {
                callback?.Invoke(false);
                return false;
            }
            else
            {
                AdsManager.instance.ShowAdsInterstitial((complete) =>
                {
                    SaveManager.Instance.Data.DayTrackingData.IsPassedFirstTimeShowInterstitial = true;
                    SaveManager.Instance.SetDataDirty();
                    callback?.Invoke(true);

                });
                _triggerCheck = false;
                return true;

            }
        }



        if (SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CountPlayGame % ShowAdsDuration == 0)
        {
            AdsManager.instance.ShowAdsInterstitial((complete) => { callback?.Invoke(true); });
            _triggerCheck = false;
            return true;
        }
        else
        {
            callback?.Invoke(false);
        }

        return false;
    }

    public void SetTriggerCheck(bool shouldCheck)
    {
        _triggerCheck = shouldCheck;
    }
}