using System;
using System.Collections.Generic;
using Adic;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine;

namespace Framework.Managers.Ads
{
    public class AppodealAdSystem : BaseAdSystem, IInterstitialAdListener, IRewardedVideoAdListener
    {
        public override void Init()
        {
            if (!base.AdIsEnable())
            {
                return;
            }
            UnityEngine.Debug.Log("APPODEAL: Инициализация началась");
            string androidAppodealKey = this.gameConfig.androidAppodealKey;
            string iOSAppodealKey = this.gameConfig.iOSAppodealKey;
            this.adPlacementEnum = new AdPlacementEnum();
            this.appodealKey = androidAppodealKey;
            Appodeal.initialize(this.appodealKey, 135);
            Appodeal.setInterstitialCallbacks(this);
            Appodeal.setRewardedVideoCallbacks(this);
            UnityEngine.Debug.Log("APPODEAL: Инициализация завершена  " + this.appodealKey);
        }

        public override KeyValuePair<string, double> GetRewardParameters()
        {
            return Appodeal.getRewardParameters(this.adPlacementEnum.GetPlacment("rewarded_video"));
        }

        public override void ShowBanner()
        {
            if (!base.AdIsEnable())
            {
                return;
            }
            if (Appodeal.isLoaded(8))
            {
                Appodeal.show(8);
            }
            UnityEngine.Debug.Log("APPODEAL: Показать баннер");
        }

        public override void HideBanner()
        {
            if (!base.AdIsEnable())
            {
                return;
            }
            Appodeal.hide(8);
            UnityEngine.Debug.Log("APPODEAL: Скрыть баннер");
        }

        public override void ShowInterstetial(Action<bool> IsLoaded)
        {
            if (!base.AdIsEnable())
            {
                IsLoaded(false);
                return;
            }
            if (Appodeal.isLoaded(3))
            {
                IsLoaded(true);
                Appodeal.show(3);
            }
            else
            {
                IsLoaded(false);
            }
            UnityEngine.Debug.Log("APPODEAL: Показать межстраничку");
        }

        public override void ShowRewardedVideo(Action<bool> IsLoaded)
        {
            //if (Appodeal.isLoaded(128))
            //{
            //	IsLoaded(true);
            //	Appodeal.show(128);
            //}
            //else
            //{
            //	IsLoaded(false);
            //}
            //UnityEngine.Debug.Log("APPODEAL: Показать видео с наградой");

            IsLoaded?.Invoke(true);
            onRewardedVideoShown();
            onRewardedVideoClosed(true);
        }

        public void onInterstitialShown()
        {
            if (this.interstitialAdListener != null)
            {
                this.interstitialAdListener.OnInterstitialShown();
            }
        }

        public void onInterstitialClosed()
        {
            if (this.interstitialAdListener != null)
            {
                this.interstitialAdListener.OnInterstitialClosed();
            }
        }

        public void onInterstitialClicked()
        {
            if (this.interstitialAdListener != null)
            {
                this.interstitialAdListener.OnInterstitialClicked();
            }
        }

        public void onInterstitialLoaded(bool isPrecache)
        {
        }

        public void onInterstitialFailedToLoad()
        {
        }

        public void onInterstitialExpired()
        {
        }

        public void onRewardedVideoLoaded(bool precache)
        {
            if (this.rewardedVideoAdListener != null)
            {
                this.rewardedVideoAdListener.OnRewardedVideoLoaded();
            }
        }

        public void onRewardedVideoFailedToLoad()
        {
            if (this.rewardedVideoAdListener != null)
            {
                this.rewardedVideoAdListener.OnRewardedVideoFailedToLoad();
            }
        }

        public void onRewardedVideoShown()
        {
            if (this.rewardedVideoAdListener != null)
            {
                this.rewardedVideoAdListener.OnRewardedVideoShown();
            }
        }

        public void onRewardedVideoFinished(double amount, string name)
        {
            if (this.rewardedVideoAdListener != null)
            {
                this.rewardedVideoAdListener.OnRewardedVideoFinished(amount, name);
            }
        }

        public void onRewardedVideoClosed(bool finished)
        {
            if (this.rewardedVideoAdListener != null)
            {
                this.rewardedVideoAdListener.OnRewardedVideoClosed(finished);
            }
        }

        public void onRewardedVideoExpired()
        {
        }

        public override bool rewardVideoIsLoaded()
        {
            //return Appodeal.isLoaded(128);

            return true;
        }

        private AdPlacementEnum adPlacementEnum;

        [Inject]
        private GameConfig gameConfig;

        private string appodealKey;
    }
}
