#if ENABLE_ADMOB
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.Mediation.IronSource;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAdsMediationTestSuite.Api;

public class AdmobAdsController : AdsProvider
{

    //banner test id: ca-app-pub-3940256099942544/6300978111

#if UNITY_ANDROID
    public const string STATIC_INTERSTITIAL_ID = "ca-app-pub-4527239098434445/7645589482";
    public const string REWARD_ID = "ca-app-pub-4527239098434445/1623106610";
    public const string ADS_BANNER = "ca-app-pub-4527239098434445/3372384121";
    //public const string ADS_BANNER = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IOS
    public const string STATIC_INTERSTITIAL_ID = "";
    public const string REWARD_ID = "";
    public const string ADS_BANNER = "";
#else
    public const string STATIC_INTERSTITIAL_ID = "";
    public const string REWARD_ID = "";
     public const string ADS_BANNER = "";
#endif

    public bool IsInterstitialReady => interstitial != null ? interstitial.IsLoaded() : false;

    private bool _isRewardReady;
    public bool IsRewardVideoReady => rewardBasedVideo != null ? rewardBasedVideo.IsLoaded() : false;

    public bool IsBannerAdsReady => _isBannerAdsReady;

    public event Action<bool> OnInterstitialShowComplete;
    public event Action<bool> OnInterstitialLoadComplete;
    public event Action<bool, float> OnRewardShowComplete;
    public event Action<bool> OnRewardLoadComplete;
    public event Action<bool> OnRewardOpened;
    public Action OnRewardAdsShown;

    public bool _isCloseAds;
    private InterstitialAd interstitial = null;
    private RewardedAd rewardBasedVideo = null;
    private BannerView bannerView = null;

    private bool _isBannerAdsReady = false;

    private List<string> _testDeviceIds = new List<string>();

    public void Init()
    {
#if UNITY_ANDROID
        string appId = "ca-app-pub-4527239098434445~9142503980";
        // string appId = "ca-app-pub-6305464951596521~9794824305";
#elif UNITY_IOS
            string appId = "";
#else
            string appId = "unexpected_platform";
#endif
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((statusInit) =>
        {
            Debug.Log($"[Admob] Init status {statusInit}");
        });

        InitInterstitial();
        InitReward();
        InitializeAdsBanner();

        LoadRewardVideo();
        LoadInterstitial();
        MoreSteps();

        _testDeviceIds = new List<string>();

        var remoteTestDevice = FirebaseRemoteSystem.instance.GetConfigValue("ADMOB_TEST_DEVICE_ID");
        if (remoteTestDevice.Source == Firebase.RemoteConfig.ValueSource.RemoteValue)
        {
            var testDevices = remoteTestDevice.StringValue;
            var split = testDevices.Split('/');
            if (split != null && split.Length > 0)
            {
                _testDeviceIds.AddRange(split);
            }
        }

        MediationTestSuite.OnMediationTestSuiteDismissed += this.HandleMediationTestSuiteDismissed;
    }


    public void MoreSteps()
    {
        IronSource.SetConsent(true);
    }

    #region Interstitial

    private Action<bool> onShowAdComplete = null;
    private Action<bool> onLoadInterstitialComplete = null;

    private void InitInterstitial()
    {
        Debug.Log("[Admob] called InitInterstitial!");
        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(STATIC_INTERSTITIAL_ID);
        // Called when an ad is shown.
        interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        interstitial.OnAdClosed += HandleOnAdClosed;

        interstitial.OnAdLoaded += Interstitial_OnAdLoaded;

        interstitial.OnAdFailedToLoad += Interstitial_OnAdFailedToLoad;

        //this.interstitial.on += HandleOnAdLeavingApplication;

    }

    private void Interstitial_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        MonoBehaviour.print($"Interstitial_OnAdFailedToLoad {e}");
        onLoadInterstitialComplete?.Invoke(false);
    }

    private void Interstitial_OnAdLoaded(object sender, EventArgs e)
    {
        MonoBehaviour.print("Interstitial_OnAdLoaded");
        onLoadInterstitialComplete?.Invoke(true);
    }

    public void LoadInterstitial(Action<bool> loadComplete = null)
    {
        Debug.Log("[Admob] called LoadInterstitial!");
        // Load the interstitial with the request.
        interstitial.LoadAd(GetNewRequest());
        onLoadInterstitialComplete = loadComplete;
    }

    public void ShowInterstitial(Action<bool> onComplete)
    {
        Debug.Log("[Admob] called LoadInterstitial!");
        if (IsInterstitialReady)
        {
            MonoBehaviour.print("[Admob] Interstitial is Ready. Start show");
            interstitial.Show();
            onShowAdComplete = onComplete;
        }
        else
        {
            MonoBehaviour.print("[Admob] Interstitial Not Ready. on fail");
            onShowAdComplete(false);
            OnInterstitialShowComplete?.Invoke(false);
        }

    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        _isCloseAds = true;
        MonoBehaviour.print("HandleAdClosed event received");
        if (onShowAdComplete != null)
        {
            onShowAdComplete(true);
        }

        OnInterstitialShowComplete?.Invoke(true);
        //reset
        onShowAdComplete = null;
        MonoBehaviour.print("HandleAdClosed started load new ads");
        LoadInterstitial();
    }


    #endregion

    #region Reward Video

    // Callback when player has watched video
    private System.Action<bool, float> _OnRewardVideoValidated = null;
    private Action<bool> _OnRewardVideoLoadComplete = null;

    private bool _IsRewardValidated = false;
    public void InitReward()
    {
        // Get singleton reward based video ad reference.
        this.rewardBasedVideo = new RewardedAd(REWARD_ID);// RewardBasedVideoAd.Instance;
        rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
        // Called when the user should be rewarded for watching a video.
        rewardBasedVideo.OnUserEarnedReward += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;

        //rewardBasedVideo.On += HandleRewardBasedVideoOnAdsComplete;

        rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoStarted;

        rewardBasedVideo.OnAdLoaded += RewardBasedVideo_OnAdLoaded;

        rewardBasedVideo.OnAdFailedToLoad += RewardBasedVideo_OnAdFailedToLoad;
    }

    public void LoadRewardVideo(Action<bool> loadComplete = null)
    {
        Debug.Log("[Admob] called LoadRewardVideo");
        this.rewardBasedVideo.LoadAd(GetNewRequest());
        //this.rewardBasedVideo.LoadAd(GetNewRequest(), REWARD_ID);
        _OnRewardVideoLoadComplete = loadComplete;
    }

    public void ShowRewardVideo(System.Action<bool, float> onRewardVideoValidated, Action onShown)
    {
        Debug.Log("[Admob] called ShowRewardVideo!");
        OnRewardAdsShown = onShown;
        if (IsRewardVideoReady)
        {
            Debug.Log("[Admob] reward ads ready ==>");
            _OnRewardVideoValidated = onRewardVideoValidated;
            _IsRewardValidated = false;
            _isCloseAds = false;
            rewardBasedVideo.Show();

        }
        else
        {
            Debug.Log("<>== reward ads not ready ==>");
            onRewardVideoValidated(false, -1f);
            OnRewardShowComplete?.Invoke(false, -1f);
        }

    }

    private void RewardBasedVideo_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        //MonoBehaviour.print($"RewardBasedVideo_OnAdFailedToLoad event received {e.Message}");
        _OnRewardVideoLoadComplete?.Invoke(false);
    }

    private void RewardBasedVideo_OnAdLoaded(object sender, EventArgs e)
    {
        MonoBehaviour.print("RewardBasedVideo_OnAdLoaded event received");
        _OnRewardVideoLoadComplete?.Invoke(true);
    }

    public void HandleRewardBasedVideoOnAdsComplete(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoOnAdsComplete event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
        //Invoke("ForceCloseRewardAds", 35f);

        // LoadingMini.Instance._isTrackingAds = true;

    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
#if UNITY_ANDROID
        // CancelInvoke("ForceCloseRewardAds");
#endif

        OnRewardAdsShown?.Invoke();
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        _isCloseAds = true;

        MonoBehaviour.print("HandleRewardBasedVideoClosed event received");

        if (_OnRewardVideoValidated != null)
        {
            if (_IsRewardValidated)
            {
                _OnRewardVideoValidated(true, 100f);
                OnRewardShowComplete?.Invoke(true, 100f);

            }
        }

        LoadRewardVideo((s) =>
        {

        });

        //if (_OnRewardVideoValidated != null)
        //{
        //    MonoBehaviour.print("HandleRewardBasedVideoClosed callback");

        //    if (_IsRewardValidated)
        //    {
        //        if (_OnRewardVideoValidated != null)
        //        {
        //            _OnRewardVideoValidated(true, 100f);
        //            OnRewardShowComplete?.Invoke(true, 100f);
        //        }
        //    }
        //    else
        //    {
        //        if (_OnRewardVideoValidated != null)
        //        {
        //            _OnRewardVideoValidated(false, -1f);
        //            OnRewardShowComplete?.Invoke(false, -1f);
        //        }

        //    }

        //    _OnRewardVideoValidated = null;
        //}
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardBasedVideoRewarded event received for "
                        + amount.ToString() + " " + type);


        _IsRewardValidated = true;
        if (_isCloseAds && _OnRewardVideoValidated != null)
        {
            MonoBehaviour.print("HandleRewardBasedVideoClosed callback");

            if (_IsRewardValidated)
            {
                if (_OnRewardVideoValidated != null)
                {
                    _OnRewardVideoValidated(true, 100f);
                    OnRewardShowComplete?.Invoke(true, 100f);
                }
            }
            else
            {
                if (_OnRewardVideoValidated != null)
                {
                    _OnRewardVideoValidated(false, -1f);
                    OnRewardShowComplete?.Invoke(false, -1f);
                }

            }

            _OnRewardVideoValidated = null;
        }
    }

    #endregion

    #region Banner Ads

    private Action<bool> OnLoadedAdsBanner;
    private Action<bool> OnShowedAdsBanner;


    public void InitializeAdsBanner()
    {
        this.bannerView = new BannerView(ADS_BANNER, AdSize.Banner, AdPosition.Top);
        // Called when an ad request has successfully loaded.
        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        this.bannerView.OnAdOpening += this.HandleOnAdBannerOpened;
        // Called when the user returned from the app after an ad click.
        this.bannerView.OnAdClosed += this.HandleOnAdBannerClosed;
        //Called when the ad click caused the user to leave the application.
        //this.bannerView.OnAdLeavingApplication += this.HandleOnAdLeavingApplication;
    }


    public void RequestAdsBanner(Action<bool> loadComplete = null)
    {
        if (this.bannerView == null)
        {
            InitializeAdsBanner();
        }

        OnLoadedAdsBanner = loadComplete;
        this.bannerView.LoadAd(GetNewRequest());
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
        this._isBannerAdsReady = true;
        OnLoadedAdsBanner?.Invoke(true);
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
        //                    + args.Message);
        this._isBannerAdsReady = false;
        OnLoadedAdsBanner?.Invoke(false);
    }

    public void HandleOnAdBannerOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
        OnLoadedAdsBanner?.Invoke(true);
    }

    public void HandleOnAdBannerClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

    public void ShowAdsBanner(Action<bool> showComplete = null)
    {
        if (this.bannerView != null)
            this.bannerView.Show();
        OnShowedAdsBanner = showComplete;

    }

    public void ClearAdsBanner(Action<bool> clearComplete = null)
    {
        bannerView?.Destroy();
        clearComplete?.Invoke(true);
    }


    #endregion

    private AdRequest GetNewRequest()
    {
        //AdRequest.Builder builder = ;
        //foreach (var device in _testDeviceIds)
        //{
        //    builder.AddTestDevice(device);
        //}

        return new AdRequest.Builder().Build();
    }

    public void DestroyAdsBanner(Action<bool> clearComplete = null)
    {
        bannerView?.Destroy();
        clearComplete?.Invoke(true);
    }

    public void HideAdsBanner(Action<bool> hideComplete = null)
    {
        bannerView?.Hide();
        hideComplete?.Invoke(true);
    }

    public void OnApplicationPause(bool pause)
    {

    }

    #region Test Suite
    public void ShowMediationTestSuite()
    {
        Debug.Log("Called ShowMediationTestSuite!");
        MediationTestSuite.Show();
    }

    public void HandleMediationTestSuiteDismissed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleMediationTestSuiteDismissed event received");
    }

    #endregion

}
#endif