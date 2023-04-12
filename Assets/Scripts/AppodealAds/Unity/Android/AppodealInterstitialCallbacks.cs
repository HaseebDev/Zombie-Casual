using System;
using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android
{
	public class AppodealInterstitialCallbacks : AndroidJavaProxy
	{
		internal AppodealInterstitialCallbacks(IInterstitialAdListener listener) : base("com.appodeal.ads.InterstitialCallbacks")
		{
			this.listener = listener;
		}

		private void onInterstitialLoaded(bool isPrecache)
		{
			this.listener.onInterstitialLoaded(isPrecache);
		}

		private void onInterstitialFailedToLoad()
		{
			this.listener.onInterstitialFailedToLoad();
		}

		private void onInterstitialShown()
		{
			this.listener.onInterstitialShown();
		}

		private void onInterstitialClicked()
		{
			this.listener.onInterstitialClicked();
		}

		private void onInterstitialClosed()
		{
			this.listener.onInterstitialClosed();
		}

		private void onInterstitialExpired()
		{
			this.listener.onInterstitialExpired();
		}

		private IInterstitialAdListener listener;
	}
}
