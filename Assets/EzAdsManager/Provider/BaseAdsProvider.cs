using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAdsProvider : MonoBehaviour, AdsProvider
{
    public virtual bool IsInterstitialReady => throw new NotImplementedException();

    public virtual bool IsRewardVideoReady => throw new NotImplementedException();

    public virtual bool IsBannerAdsReady => throw new NotImplementedException();

    public event Action<bool> OnInterstitialShowComplete;
    public virtual event Action<bool> OnInterstitialLoadComplete;
    public event Action<bool, float> OnRewardShowComplete;
    public event Action<bool> OnRewardLoadComplete;
    public event Action<bool> OnRewardOpened;

    public void DestroyAdsBanner(Action<bool> clearComplete = null)
    {
        throw new NotImplementedException();
    }

    public void HideAdsBanner(Action<bool> hideComplete = null)
    {
        throw new NotImplementedException();
    }

    public virtual void Init()
    {
    }

    public virtual void LoadInterstitial(Action<bool> loadComplete = null)
    {
    }

    public virtual void LoadRewardVideo(Action<bool> loadComplete = null)
    {
        
    }

    public void OnApplicationPause(bool pause)
    {

    }

    public virtual void RequestAdsBanner(Action<bool> loadComplete = null)
    {
       
    }

    public virtual void ShowAdsBanner(Action<bool> showComplete = null)
    {
       
    }

    public virtual void ShowInterstitial(Action<bool> showComplete = null)
    {
       
    }

    public virtual void ShowRewardVideo(Action<bool, float> showComplete = null, Action onShown = null)
    {
       
    }
}
