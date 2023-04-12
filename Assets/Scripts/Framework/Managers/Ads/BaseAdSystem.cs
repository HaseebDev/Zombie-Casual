using System;
using System.Collections.Generic;
using Framework.Interfaces;
using Framework.Security;
using Framework.Security.SafeTypes;

namespace Framework.Managers.Ads
{
	public abstract class BaseAdSystem
	{
		public abstract void Init();

        public abstract bool rewardVideoIsLoaded();

		public abstract void ShowInterstetial(Action<bool> IsLoaded);

		public abstract void ShowRewardedVideo(Action<bool> IsLoaded);

		public abstract KeyValuePair<string, double> GetRewardParameters();

		public abstract void ShowBanner();

		public abstract void HideBanner();

		protected BaseAdSystem()
		{
			this.adEnableStatus = new SafeInt(SafePlayerPrefs.GetInt("adEnableStatus", 1));
		}

		public bool AdIsEnable()
		{
			return false;// this.adEnableStatus == new SafeInt(1);
		}

		public void DisableAds()
		{
			this.adEnableStatus = new SafeInt(0);
			SafePlayerPrefs.SetInt("adEnableStatus", this.adEnableStatus.GetValue());
			this.HideBanner();
		}

		public void SetRewardedVideoCallbacks(IRewardedVideoAdListener listener)
		{
			this.rewardedVideoAdListener = listener;
		}

		public void SetInterstitialCallbacks(IInterstitialAdListener listener)
		{
			this.interstitialAdListener = listener;
		}

		private SafeInt adEnableStatus;

		protected IRewardedVideoAdListener rewardedVideoAdListener;

		protected IInterstitialAdListener interstitialAdListener;
	}
}
