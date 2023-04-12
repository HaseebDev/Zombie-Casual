using MEC;
using System;
using System.Collections.Generic;
using Doozy.Engine.Utils.ColorModels;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;

    public bool EnableInterstitialAds { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void OnDisable()
    {
        RemoveListener();
    }

    public List<AdsProvider> _listAdsProvider = new List<AdsProvider>();

    public AdsProvider AdsProvider {
        get { return (_listAdsProvider == null || _listAdsProvider.Count <= 0) ? null : _listAdsProvider[0]; }
    }

    public void Initialize()
    {
        //TO DO ADS ads manager
        _listAdsProvider.Clear();

        //#if UNITY_EDITOR
        //        AdsProvider fakeAdsProvider = new SimulateAdsController();
        //        fakeAdsProvider.Init();
        //        _listAdsProvider.Add(fakeAdsProvider);
        //#endif

#if ENABLE_ADMOB
        AdsProvider admobProvider = new AdmobAdsController();
        admobProvider.Init();
        _listAdsProvider.Add(admobProvider);
#elif ENABLE_IRONSOURCE
        //AdsProvider ironsourceProvider = new IronSourceController();
        //ironsourceProvider.Init();
        //_listAdsProvider.Add(ironsourceProvider);
        var ironsourceProvider = transform.GetComponentInChildren<AdsProvider>();
        if (ironsourceProvider != null)
        {
            ironsourceProvider.Init();
            _listAdsProvider.Add(ironsourceProvider);
        }
#endif
        RemoveListener();
        AdsProvider.OnInterstitialLoadComplete += AdsProvider_OnInterstitialLoadComplete;
        AdsProvider.OnInterstitialShowComplete += AdsProvider_OnInterstitialShowComplete;
        AdsProvider.OnRewardLoadComplete += AdsProvider_OnRewardLoadComplete;
        AdsProvider.OnRewardShowComplete += AdsProvider_OnRewardShowComplete;
        AdsProvider.OnRewardOpened += AdsProvider_OnRewardOpened;
        performCallWatchAds = false;

        EnableInterstitialAds = true;
        var enableInterstitial = FirebaseRemoteSystem.instance.GetConfigValue("ENABLE_INTERSTITIAL");
        if (enableInterstitial.Source == Firebase.RemoteConfig.ValueSource.RemoteValue)
        {
            EnableInterstitialAds = enableInterstitial.BooleanValue;
        }

        var _rewardsAdsInterval = FirebaseRemoteSystem.instance.GetConfigValue("REWARD_ADS_INTERVAL");
        if (_rewardsAdsInterval.Source == Firebase.RemoteConfig.ValueSource.RemoteValue)
        {
            REWARD_ADS_INTERVAL = (float)_rewardsAdsInterval.DoubleValue;
        }


        _timerWaitAds = 0f;
        _triggerWaitAds = false;
    }

    private void RemoveListener()
    {
        AdsProvider.OnInterstitialLoadComplete -= AdsProvider_OnInterstitialLoadComplete;
        AdsProvider.OnInterstitialShowComplete -= AdsProvider_OnInterstitialShowComplete;
        AdsProvider.OnRewardLoadComplete -= AdsProvider_OnRewardLoadComplete;
        AdsProvider.OnRewardShowComplete -= AdsProvider_OnRewardShowComplete;
        AdsProvider.OnRewardOpened -= AdsProvider_OnRewardOpened;
    }

    #region Event Handler

    private void AdsProvider_OnRewardLoadComplete(bool success)
    {
        Debug.Log($"[Ads] RewardLoadComplete {success}");
        //if (!success)
        //    EnableLoadingScreen(false);
    }

    private void AdsProvider_OnRewardShowComplete(bool success, float amount)
    {
        Debug.Log($"[Ads] RewardShowComplete {success} - {amount}");
        EnableLoadingScreen(false);
        _triggerWaitAds = false;
        _timerWaitAds = 0f;
    }

    private void AdsProvider_OnInterstitialLoadComplete(bool success)
    {
        Debug.Log($"[Ads] InterstitialLoadComplete {success}");
        //if (!success)
        //    EnableLoadingScreen(false);
    }

    private void AdsProvider_OnInterstitialShowComplete(bool success)
    {
        Debug.Log($"[Ads] InterstitialShowComplete {success}");
        EnableLoadingScreen(false);
    }

    private void AdsProvider_OnRewardOpened(bool isOpened)
    {
        if (isOpened)
        {
            Debug.Log($"[Ads] AdsProvider_OnRewardOpened {isOpened}");
        }
        EnableLoadingScreen(false);
        _triggerWaitAds = false;
        _timerWaitAds = 0f;
    }

    #endregion

    #region Intertitial

    public void LoadAdsInterstitial(Action<bool> loadComplete)
    {
        if (!EnableInterstitialAds)
        {
            loadComplete?.Invoke(false);
            return;
        }

        if (!AdsProvider.IsInterstitialReady)
        {
            AdsProvider.LoadInterstitial((success) =>
            {
                //TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
                loadComplete?.Invoke(success);
            });
        }
    }

    public void ShowAdsInterstitial(Action<bool> showComplete)
    {
        if (!EnableInterstitialAds)
        {
            showComplete?.Invoke(false);
            return;
        }

        if (SaveManager.Instance.Data.RemoveAds)
        {
            showComplete?.Invoke(true);
            return;
        }

        if (AdsProvider.IsInterstitialReady)
        {
            AdsProvider.ShowInterstitial((success) =>
            {
                //TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
                TopLayerCanvas.instance.ShowHUDLoading(false);
                showComplete?.Invoke(success);
            });
        }
        else
        {
            Timing.RunCoroutine(LoadAndShowInterstitialCoroutine(5f, (success) =>
            {
                //TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
                TopLayerCanvas.instance.ShowHUDLoading(false);
                showComplete?.Invoke(success);
            }));
        }
    }

    IEnumerator<float> LoadAndShowInterstitialCoroutine(float timeOut, Action<bool> showComplete)
    {
        if (!EnableInterstitialAds)
        {
            showComplete?.Invoke(false);
            yield break;
        }

        EnableLoadingScreen(true);
        bool waitCheckInternet = true;
        bool hasInternet = false;

        bool isShowingAds = false;

        if (AdsProvider.IsInterstitialReady)
        {
            AdsProvider.ShowInterstitial(showComplete);
            yield break;
        }

        NetworkDetector.instance.checkInternetConnection((reached) =>
        {
            waitCheckInternet = false;
            hasInternet = reached;
        }, true, false);

        while (waitCheckInternet)
            yield return Timing.WaitForOneFrame;

        if (!hasInternet)
        {
            if (!AdsProvider.IsInterstitialReady)
                AdsProvider.LoadInterstitial();
            showComplete?.Invoke(false);
            yield break;
        }

        bool waitingAds = true;
        float waitAds = timeOut;
        float timerAds = 0f;

        if (!AdsProvider.IsInterstitialReady)
            AdsProvider.LoadInterstitial();
        while (waitingAds && timerAds <= waitAds)
        {
            timerAds += Time.deltaTime;
            if (AdsProvider.IsInterstitialReady)
            {
                waitingAds = false;
                isShowingAds = true;
                AdsProvider.ShowInterstitial(showComplete);
                break;
            }

            yield return Timing.WaitForOneFrame;
        }

        if (waitingAds)
        {
            //time out!!!
            showComplete?.Invoke(false);
            yield break;
        }

        if (!isShowingAds)
            EnableLoadingScreen(false);
    }

    #endregion

    #region RewardAds
    public static float REWARD_ADS_INTERVAL = 5 * TimeService.MIN_SEC;
    public long LastWatchAds = 0;
    private bool performCallWatchAds = false;

    public void LoadAdsReward(Action<bool> loadComplete)
    {
        AdsProvider.LoadRewardVideo(loadComplete);
    }

    public void ShowAdsReward(Action<bool, float> showComplete, Action OnAdsShown = null, bool _forceShownAds = false)
    {
        long nextAds = SaveManager.Instance.Data.AdsTracker.LastTimeWatchedRewardAds + (long)REWARD_ADS_INTERVAL;
        if (!_forceShownAds && TimeService.instance.GetCurrentTimeStamp() < nextAds)
        {
            long diff = (nextAds) - TimeService.instance.GetCurrentTimeStamp();
            TopLayerCanvas.instance.ShowFloatingTextNotify(
              LocalizeController.GetText(LOCALIZE_ID_PREF.WATCH_ADS_FAIL_INTERVAL, TimeService.FormatTimeSpanShortly(diff)));
            OnAdsShown?.Invoke();
            return;
        }

        if (AdsProvider.IsRewardVideoReady)
        {
            _timerWaitAds = 0f;
            _triggerWaitAds = true;
            AdsProvider.ShowRewardVideo((success, amount) =>
            {
                Timing.RunCoroutine(FlowAfterWatchedAds(showComplete, success, amount));
            }, OnAdsShown);
        }
        else
        {
            Timing.RunCoroutine(LoadAndShowRewardCoroutine(5f, (success, amount) =>
            {
                Timing.RunCoroutine(FlowAfterWatchedAds(showComplete, success, amount));
            }, OnAdsShown));
        }
    }

    IEnumerator<float> FlowAfterWatchedAds(Action<bool, float> showComplete, bool success, float amount)
    {
        EnableLoadingScreen(true);
        yield return Timing.WaitForSeconds(0.5f);
        Resources.UnloadUnusedAssets();
        yield return Timing.WaitForOneFrame;
        GC.Collect();
        yield return Timing.WaitForOneFrame;
        EnableLoadingScreen(false);
        showComplete?.Invoke(success, amount);

        SaveManager.Instance.Data.AdsTracker.LastTimeWatchedRewardAds = TimeService.instance.GetCurrentTimeStamp();
        SaveManager.Instance.SetDataDirty();
    }

    public void ShowAdsRewardWithNotify(Action onSuccess, bool forceShowAds = false)
    {
        if (performCallWatchAds)
            return;
        performCallWatchAds = true;
        EnableLoadingScreen(true);
        ShowAdsReward((result, f) =>
        {
            Debug.Log($"ShowAdsRewardWithNotify result {result}");
            if (result)
            {
                onSuccess?.Invoke();
            }
            else
            {
                MasterCanvas.CurrentMasterCanvas.ShowNotifyHUD(LOCALIZE_ID_PREF.WATCH_ADS_FAIL.AsLocalizeString());
            }

            performCallWatchAds = false;
            EnableLoadingScreen(false);
        }, () =>
        {
            performCallWatchAds = false;
            EnableLoadingScreen(false);
        }, forceShowAds);
    }

    IEnumerator<float> LoadAndShowRewardCoroutine(float timeOut, Action<bool, float> showComplete, Action OnAdsShown)
    {
        float waitAds = timeOut;
        float timerAds = 0f;
        bool waitingAds = true;
        EnableLoadingScreen(true);

        bool waitCheckInternet = true;
        bool hasInternet = false;

        if (!AdsProvider.IsRewardVideoReady)
            AdsProvider.LoadRewardVideo();
        else
        {
            AdsProvider.ShowRewardVideo(showComplete, OnAdsShown);
            yield break;
        }

        NetworkDetector.instance.checkInternetConnection((reached) =>
        {
            waitCheckInternet = false;
            hasInternet = reached;
        }, true, false);

        while (waitCheckInternet)
            yield return Timing.WaitForOneFrame;

        if (!hasInternet)
        {
            showComplete?.Invoke(false, -1);
            EnableLoadingScreen(false);
            yield break;
        }
        else
        {
            if (!AdsProvider.IsRewardVideoReady)
                AdsProvider.LoadRewardVideo();
            while (waitingAds && timerAds <= waitAds)
            {
                timerAds += Time.deltaTime;
                if (AdsProvider.IsRewardVideoReady)
                {
                    waitingAds = false;
                    AdsProvider.ShowRewardVideo(showComplete, OnAdsShown);
                    break;
                }
                yield return Timing.WaitForOneFrame;
            }

            if (waitingAds)
            {
                showComplete?.Invoke(false, -1f);
            }
        }
    }

    #endregion

    #region ADS Banner

    public void RequestAdsBanner(Action<bool> loadComplete)
    {
        AdsProvider?.RequestAdsBanner(loadComplete);
    }

    public void ShowAdsBanner(Action<bool> showComplete = null)
    {
        if (!SaveManager.Instance.Data.RemoveAds)
            AdsProvider.ShowAdsBanner(showComplete);
    }

    public void HideAdsBanner(Action<bool> hideComplete)
    {
        AdsProvider.HideAdsBanner(hideComplete);
    }

    #endregion

    private float _timerWaitAds = 0f;
    private bool _triggerWaitAds = false;
    private void Update()
    {
        float _deltaTime = Time.deltaTime;
        if (_triggerWaitAds)
        {
            _timerWaitAds += _deltaTime;
            if (_timerWaitAds >= 5.0f)
            {
                EnableLoadingScreen(false);
                _triggerWaitAds = false;
                _timerWaitAds = 0f;
            }
        }
    }

    private void EnableLoadingScreen(bool enable)
    {
        //Debug.Log($"Called EnableLoadingScreen: {enable} ");
        //if (enable)
        //    TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_LOADING, false);
        //else
        //    TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING, null, true);

        TopLayerCanvas.instance.ShowHUDLoading(enable);
    }

    //private void OnApplicationPause(bool pause)
    //{
    //    if (AdsProvider != null)
    //        AdsProvider.OnApplicationPause(pause);
    //}
}