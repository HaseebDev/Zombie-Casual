using System;
using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android
{
	public class AppodealMrecCallbacks : AndroidJavaProxy
	{
		internal AppodealMrecCallbacks(IMrecAdListener listener) : base("com.appodeal.ads.MrecCallbacks")
		{
			this.listener = listener;
		}

		private void onMrecLoaded(bool isPrecache)
		{
			this.listener.onMrecLoaded(isPrecache);
		}

		private void onMrecFailedToLoad()
		{
			this.listener.onMrecFailedToLoad();
		}

		private void onMrecShown()
		{
			this.listener.onMrecShown();
		}

		private void onMrecClicked()
		{
			this.listener.onMrecClicked();
		}

		private void onMrecExpired()
		{
			this.listener.onMrecExpired();
		}

		private IMrecAdListener listener;
	}
}
