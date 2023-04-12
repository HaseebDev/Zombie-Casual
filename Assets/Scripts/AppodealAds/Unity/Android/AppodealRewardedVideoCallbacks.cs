using System;
using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android
{
	public class AppodealRewardedVideoCallbacks : AndroidJavaProxy
	{
		internal AppodealRewardedVideoCallbacks(IRewardedVideoAdListener listener) : base("com.appodeal.ads.RewardedVideoCallbacks")
		{
			this.listener = listener;
		}

		private void onRewardedVideoLoaded(bool precache)
		{
			this.listener.onRewardedVideoLoaded(precache);
		}

		private void onRewardedVideoFailedToLoad()
		{
			this.listener.onRewardedVideoFailedToLoad();
		}

		private void onRewardedVideoShown()
		{
			this.listener.onRewardedVideoShown();
		}

		private void onRewardedVideoFinished(double amount, AndroidJavaObject name)
		{
			this.listener.onRewardedVideoFinished(amount, null);
		}

		private void onRewardedVideoFinished(double amount, string name)
		{
			this.listener.onRewardedVideoFinished(amount, name);
		}

		private void onRewardedVideoClosed(bool finished)
		{
			this.listener.onRewardedVideoClosed(finished);
		}

		private void onRewardedVideoExpired()
		{
			this.listener.onRewardedVideoExpired();
		}

		private IRewardedVideoAdListener listener;
	}
}
