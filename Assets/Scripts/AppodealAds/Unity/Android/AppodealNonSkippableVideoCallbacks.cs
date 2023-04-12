using System;
using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android
{
	public class AppodealNonSkippableVideoCallbacks : AndroidJavaProxy
	{
		internal AppodealNonSkippableVideoCallbacks(INonSkippableVideoAdListener listener) : base("com.appodeal.ads.NonSkippableVideoCallbacks")
		{
			this.listener = listener;
		}

		private void onNonSkippableVideoLoaded(bool isPrecache)
		{
			this.listener.onNonSkippableVideoLoaded(isPrecache);
		}

		private void onNonSkippableVideoFailedToLoad()
		{
			this.listener.onNonSkippableVideoFailedToLoad();
		}

		private void onNonSkippableVideoShown()
		{
			this.listener.onNonSkippableVideoShown();
		}

		private void onNonSkippableVideoFinished()
		{
			this.listener.onNonSkippableVideoFinished();
		}

		private void onNonSkippableVideoClosed(bool finished)
		{
			this.listener.onNonSkippableVideoClosed(finished);
		}

		private void onNonSkippableVideoExpired()
		{
			this.listener.onNonSkippableVideoExpired();
		}

		private INonSkippableVideoAdListener listener;
	}
}
