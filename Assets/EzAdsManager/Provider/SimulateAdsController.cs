using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;


public class SimulateAdsController : AdsProvider
{
    private bool _isInterstitialReady = true;
    private bool _isRewardReady = true;
    private float _delayLoadAds = 30f;

    public bool IsInterstitialReady {
        get {
            if (SROptions.Current.NetState == DEBUG_NET_STATE.NO_NETWORK)
                return false;
            else
                return _isInterstitialReady;
        }
    }

    public bool IsRewardVideoReady {
        get {
            if (SROptions.Current.NetState == DEBUG_NET_STATE.NO_NETWORK)
                return false;
            else
                return _isRewardReady;
        }
    }

    public bool IsBannerAdsReady => throw new NotImplementedException();

    public event Action<bool> OnInterstitialShowComplete;
    public event Action<bool> OnInterstitialLoadComplete;
    public event Action<bool, float> OnRewardShowComplete;
    public event Action<bool> OnRewardLoadComplete;
    public event Action<bool> OnRewardOpened;

    public void Init()
    {
        //DO NOTHING
    }

    public void LoadInterstitial(Action<bool> loadComplete = null)
    {
        Debug.Log("[SimulateAds] LoadInterstitial");
        Timing.CallDelayed(_delayLoadAds, () =>
        {
            loadComplete?.Invoke(true);
        });
    }

    public void LoadRewardVideo(Action<bool> loadComplete = null)
    {
        Debug.Log("[SimulateAds] LoadRewardVideo");
        Timing.CallDelayed(_delayLoadAds, () =>
        {
            loadComplete?.Invoke(_isInterstitialReady);
        });

    }

    public void ShowInterstitial(Action<bool> showComplete = null)
    {

        if (IsInterstitialReady)
        {
            Debug.Log("[SimulateAds] ShowInterstitial");
            TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_SIMULATE_ADS, false, null, ADSTYPE.ADS_INTERSTITIAL);
            Timing.CallDelayed(2.0f, () =>
            {
                TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_SIMULATE_ADS);
                showComplete?.Invoke(_isRewardReady);
            });
        }

    }

    public void ShowRewardVideo(Action<bool, float> showComplete = null, Action onShown = null)
    {
        if (IsRewardVideoReady)
        {
            Debug.Log("[SimulateAds] ShowRewardVideo");
            TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_SIMULATE_ADS, false, null, ADSTYPE.ADS_REWARD);
            Timing.CallDelayed(3.0f, () =>
            {
                TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_SIMULATE_ADS);
                showComplete?.Invoke(true, 100f);
            });
        }

    }

    public void RequestAdsBanner(Action<bool> loadComplete = null)
    {
        loadComplete?.Invoke(true);
    }

    public void ShowAdsBanner(Action<bool> showComplete = null)
    {
        showComplete?.Invoke(true);

        TopLayerCanvas.instance.EnableSimuateAdsBanner(true);
    }

    public void DestroyAdsBanner(Action<bool> clearComplete = null)
    {
        clearComplete?.Invoke(true);
        TopLayerCanvas.instance.EnableSimuateAdsBanner(false);
    }

    public void HideAdsBanner(Action<bool> hideComplete = null)
    {
        hideComplete?.Invoke(true);
        TopLayerCanvas.instance.EnableSimuateAdsBanner(false);
    }

    public void OnApplicationPause(bool pause)
    {
        Debug.Log($"[ADS] OnApplicationPause {pause}");
    }
}
