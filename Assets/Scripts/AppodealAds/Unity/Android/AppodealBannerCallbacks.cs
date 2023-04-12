using System;
using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android
{
	public class AppodealBannerCallbacks : AndroidJavaProxy
	{
		internal AppodealBannerCallbacks(IBannerAdListener listener) : base("com.appodeal.ads.BannerCallbacks")
		{
			this.listener = listener;
		}

		private void onBannerLoaded(int height, bool isPrecache)
		{
			this.listener.onBannerLoaded(isPrecache);
		}

		private void onBannerFailedToLoad()
		{
			this.listener.onBannerFailedToLoad();
		}

		private void onBannerShown()
		{
			this.listener.onBannerShown();
		}

		private void onBannerClicked()
		{
			this.listener.onBannerClicked();
		}

		private void onBannerExpired()
		{
			this.listener.onBannerExpired();
		}

		private IBannerAdListener listener;
	}
}
