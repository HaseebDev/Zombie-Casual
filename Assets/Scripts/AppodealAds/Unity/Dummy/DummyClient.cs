using System;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Dummy
{
	public class DummyClient : IAppodealAdsClient
	{
		public void initialize(string appKey, int adTypes)
		{
			UnityEngine.Debug.Log("Call to Appodeal.initialize on not supported platform");
		}

		public void initialize(string appKey, int adTypes, bool hasConsent)
		{
			UnityEngine.Debug.Log("Call to Appodeal.initialize on not supported platform");
		}

		public bool isInitialized(int adType)
		{
			UnityEngine.Debug.Log("Call Appodeal.isInitialized on not supported platform");
			return false;
		}

		public bool show(int adTypes)
		{
			UnityEngine.Debug.Log("Call to Appodeal.show on not supported platform");
			return false;
		}

		public bool show(int adTypes, string placement)
		{
			UnityEngine.Debug.Log("Call to Appodeal.show on not supported platform");
			return false;
		}

		public bool showBannerView(int YAxis, int XAxis, string Placement)
		{
			UnityEngine.Debug.Log("Call to Appodeal.showBannerView on not supported platform");
			return false;
		}

		public bool showMrecView(int YAxis, int XGravity, string Placement)
		{
			UnityEngine.Debug.Log("Call to Appodeal.showMrecView on not supported platform");
			return false;
		}

		public bool isLoaded(int adTypes)
		{
			UnityEngine.Debug.Log("Call to Appodeal.showBannerView on not supported platform");
			return false;
		}

		public void cache(int adTypes)
		{
			UnityEngine.Debug.Log("Call to Appodeal.cache on not supported platform");
		}

		public void hide(int adTypes)
		{
			UnityEngine.Debug.Log("Call to Appodeal.hide on not supported platform");
		}

		public void hideBannerView()
		{
			UnityEngine.Debug.Log("Call to Appodeal.hideBannerView on not supported platform");
		}

		public void hideMrecView()
		{
			UnityEngine.Debug.Log("Call to Appodeal.hideMrecView on not supported platform");
		}

		public bool isPrecache(int adTypes)
		{
			UnityEngine.Debug.Log("Call to Appodeal.isPrecache on not supported platform");
			return false;
		}

		public void setAutoCache(int adTypes, bool autoCache)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setAutoCache on not supported platform");
		}

		public void onResume()
		{
			UnityEngine.Debug.Log("Call to Appodeal.onResume on not supported platform");
		}

		public void setSmartBanners(bool value)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setSmartBanners on not supported platform");
		}

		public void setBannerAnimation(bool value)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setBannerAnimation on not supported platform");
		}

		public void setBannerBackground(bool value)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setBannerBackground on not supported platform");
		}

		public void setTabletBanners(bool value)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setTabletBanners on not supported platform");
		}

		public void setTesting(bool test)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setTesting on not supported platform");
		}

		public void setLogLevel(Appodeal.LogLevel logging)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setLogLevel on not supported platform");
		}

		public void setChildDirectedTreatment(bool value)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setChildDirectedTreatment on not supported platform");
		}

		public void disableNetwork(string network)
		{
			UnityEngine.Debug.Log("Call to Appodeal.disableNetwork on not supported platform");
		}

		public void disableNetwork(string network, int adTypes)
		{
			UnityEngine.Debug.Log("Call to Appodeal.disableNetwork on not supported platform");
		}

		public void disableLocationPermissionCheck()
		{
			UnityEngine.Debug.Log("Call to Appodeal.disableLocationPermissionCheck on not supported platform");
		}

		public void disableWriteExternalStoragePermissionCheck()
		{
			UnityEngine.Debug.Log("Call to Appodeal.disableWriteExternalStoragePermissionCheck on not supported platform");
		}

		public void setTriggerOnLoadedOnPrecache(int adTypes, bool onLoadedTriggerBoth)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setTriggerOnLoadedOnPrecache on not supported platform");
		}

		public void muteVideosIfCallsMuted(bool value)
		{
			UnityEngine.Debug.Log("Call to Appodeal.muteVideosIfCallsMuted on not supported platform");
		}

		public void showTestScreen()
		{
			UnityEngine.Debug.Log("Call to Appodeal.showTestScreen on not supported platform");
		}

		public string getVersion()
		{
			return "2.8.49.1";
		}

		public bool canShow(int adTypes)
		{
			UnityEngine.Debug.Log("Call to Appodeal.canShow on not supported platform");
			return false;
		}

		public bool canShow(int adTypes, string placement)
		{
			UnityEngine.Debug.Log("Call to Appodeal.canShow on not supported platform");
			return false;
		}

		public void setSegmentFilter(string name, bool value)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setSegmentFilter on not supported platform");
		}

		public void setSegmentFilter(string name, int value)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setSegmentFilter on not supported platform");
		}

		public void setSegmentFilter(string name, double value)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setSegmentFilter on not supported platform");
		}

		public void setSegmentFilter(string name, string value)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setSegmentFilter on not supported platform");
		}

		public void trackInAppPurchase(double amount, string currency)
		{
			UnityEngine.Debug.Log("Call to Appodeal.trackInAppPurchase on not supported platform");
		}

		public string getRewardCurrency(string placement)
		{
			UnityEngine.Debug.Log("Call to Appodeal.getRewardCurrency on not supported platform");
			return "USD";
		}

		public double getRewardAmount(string placement)
		{
			UnityEngine.Debug.Log("Call to Appodeal.getRewardAmount on not supported platform");
			return 0.0;
		}

		public string getRewardCurrency()
		{
			UnityEngine.Debug.Log("Call to Appodeal.getRewardCurrency on not supported platform");
			return "USD";
		}

		public double getRewardAmount()
		{
			UnityEngine.Debug.Log("Call to Appodeal.getRewardAmount on not supported platform");
			return 0.0;
		}

		public double getPredictedEcpm(int adType)
		{
			UnityEngine.Debug.Log("Call to Appodeal.getPredictedEcpm on not supported platform");
			return 0.0;
		}

		public void setExtraData(string key, bool value)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setExtraData(string key, bool value) on not supported platform");
		}

		public void setExtraData(string key, int value)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setExtraData(string key, int value) on not supported platform");
		}

		public void setExtraData(string key, double value)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setExtraDatastring key, double value) on not supported platform");
		}

		public void setExtraData(string key, string value)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setExtraData(string key, string value) on not supported platform");
		}

		public void setInterstitialCallbacks(IInterstitialAdListener listener)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setInterstitialCallbacks on not supported platform");
		}

		public void setNonSkippableVideoCallbacks(INonSkippableVideoAdListener listener)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setNonSkippableVideoCallbacks on not supported platform");
		}

		public void setRewardedVideoCallbacks(IRewardedVideoAdListener listener)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setRewardedVideoCallbacks on not supported platform");
		}

		public void setBannerCallbacks(IBannerAdListener listener)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setBannerCallbacks on not supported platform");
		}

		public void setMrecCallbacks(IMrecAdListener listener)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setMrecCallbacks on not supported platform");
		}

		public void requestAndroidMPermissions(IPermissionGrantedListener listener)
		{
			UnityEngine.Debug.Log("Call to Appodeal.requestAndroidMPermissions on not supported platform");
		}

		public void getUserSettings()
		{
		}

		public void setUserId(string id)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setUserId on not supported platform");
		}

		public void setAge(int age)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setAge on not supported platform");
		}

		public void setGender(UserSettings.Gender gender)
		{
			UnityEngine.Debug.Log("Call to Appodeal.setGender on not supported platform");
		}

		public void requestAndroidMPermissions()
		{
			UnityEngine.Debug.Log("Call to Appodeal.requestAndroidMPermissions on not supported platform");
		}

		public void destroy(int adTypes)
		{
			UnityEngine.Debug.Log("Call to Appodeal.destroy on not supported platform");
		}
	}
}
