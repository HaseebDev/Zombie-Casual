using System;
using System.Collections.Generic;
using AppodealAds.Unity.Common;

namespace AppodealAds.Unity.Api
{
	public class Appodeal
	{
		private static IAppodealAdsClient getInstance()
		{
			if (Appodeal.client == null)
			{
				Appodeal.client = AppodealAdsClientFactory.GetAppodealAdsClient();
			}
			return Appodeal.client;
		}

		public static void initialize(string appKey, int adTypes)
		{
			Appodeal.getInstance().initialize(appKey, adTypes);
		}

		public static void initialize(string appKey, int adTypes, bool hasConsent)
		{
			Appodeal.getInstance().initialize(appKey, adTypes, hasConsent);
		}

		public static bool show(int adTypes)
		{
			return Appodeal.getInstance().show(adTypes);
		}

		public static bool show(int adTypes, string placement)
		{
			return Appodeal.getInstance().show(adTypes, placement);
		}

		public static bool showBannerView(int YAxis, int XGravity, string placement)
		{
			return Appodeal.getInstance().showBannerView(YAxis, XGravity, placement);
		}

		public static bool showMrecView(int YAxis, int XGravity, string placement)
		{
			return Appodeal.getInstance().showMrecView(YAxis, XGravity, placement);
		}

		public static bool isLoaded(int adTypes)
		{
			return Appodeal.getInstance().isLoaded(adTypes);
		}

		public static void cache(int adTypes)
		{
			Appodeal.getInstance().cache(adTypes);
		}

		public static void hide(int adTypes)
		{
			Appodeal.getInstance().hide(adTypes);
		}

		public static void hideBannerView()
		{
			Appodeal.getInstance().hideBannerView();
		}

		public static void hideMrecView()
		{
			Appodeal.getInstance().hideMrecView();
		}

		public static void setAutoCache(int adTypes, bool autoCache)
		{
			Appodeal.getInstance().setAutoCache(adTypes, autoCache);
		}

		public static bool isPrecache(int adTypes)
		{
			return Appodeal.getInstance().isPrecache(adTypes);
		}

		public static void onResume()
		{
			Appodeal.getInstance().onResume();
		}

		public static void setSmartBanners(bool value)
		{
			Appodeal.getInstance().setSmartBanners(value);
		}

		public static void setBannerBackground(bool value)
		{
			Appodeal.getInstance().setBannerBackground(value);
		}

		public static void setBannerAnimation(bool value)
		{
			Appodeal.getInstance().setBannerAnimation(value);
		}

		public static void setTabletBanners(bool value)
		{
			Appodeal.getInstance().setTabletBanners(value);
		}

		public static void setTesting(bool test)
		{
			Appodeal.getInstance().setTesting(test);
		}

		public static void setLogLevel(Appodeal.LogLevel log)
		{
			Appodeal.getInstance().setLogLevel(log);
		}

		public static void setChildDirectedTreatment(bool value)
		{
			Appodeal.getInstance().setChildDirectedTreatment(value);
		}

		public static void disableNetwork(string network)
		{
			Appodeal.getInstance().disableNetwork(network);
		}

		public static void disableNetwork(string network, int adType)
		{
			Appodeal.getInstance().disableNetwork(network, adType);
		}

		public static void disableLocationPermissionCheck()
		{
			Appodeal.getInstance().disableLocationPermissionCheck();
		}

		public static void disableWriteExternalStoragePermissionCheck()
		{
			Appodeal.getInstance().disableWriteExternalStoragePermissionCheck();
		}

		public static void setTriggerOnLoadedOnPrecache(int adTypes, bool onLoadedTriggerBoth)
		{
			Appodeal.getInstance().setTriggerOnLoadedOnPrecache(adTypes, onLoadedTriggerBoth);
		}

		public static void muteVideosIfCallsMuted(bool value)
		{
			Appodeal.getInstance().muteVideosIfCallsMuted(value);
		}

		public static void showTestScreen()
		{
			Appodeal.getInstance().showTestScreen();
		}

		public static bool canShow(int adTypes)
		{
			return Appodeal.getInstance().canShow(adTypes);
		}

		public static bool canShow(int adTypes, string placement)
		{
			return Appodeal.getInstance().canShow(adTypes, placement);
		}

		public static void setSegmentFilter(string name, bool value)
		{
			Appodeal.getInstance().setSegmentFilter(name, value);
		}

		public static void setSegmentFilter(string name, int value)
		{
			Appodeal.getInstance().setSegmentFilter(name, value);
		}

		public static void setSegmentFilter(string name, double value)
		{
			Appodeal.getInstance().setSegmentFilter(name, value);
		}

		public static void setSegmentFilter(string name, string value)
		{
			Appodeal.getInstance().setSegmentFilter(name, value);
		}

		public static void setExtraData(string key, bool value)
		{
			Appodeal.getInstance().setExtraData(key, value);
		}

		public static void setExtraData(string key, int value)
		{
			Appodeal.getInstance().setExtraData(key, value);
		}

		public static void setExtraData(string key, double value)
		{
			Appodeal.getInstance().setExtraData(key, value);
		}

		public static void setExtraData(string key, string value)
		{
			Appodeal.getInstance().setExtraData(key, value);
		}

		public static void trackInAppPurchase(double amount, string currency)
		{
			Appodeal.getInstance().trackInAppPurchase(amount, currency);
		}

		public static string getNativeSDKVersion()
		{
			return Appodeal.getInstance().getVersion();
		}

		public static string getPluginVersion()
		{
			return "2.8.49.1";
		}

		public static void setInterstitialCallbacks(IInterstitialAdListener listener)
		{
			Appodeal.getInstance().setInterstitialCallbacks(listener);
		}

		public static void setNonSkippableVideoCallbacks(INonSkippableVideoAdListener listener)
		{
			Appodeal.getInstance().setNonSkippableVideoCallbacks(listener);
		}

		public static void setRewardedVideoCallbacks(IRewardedVideoAdListener listener)
		{
			Appodeal.getInstance().setRewardedVideoCallbacks(listener);
		}

		public static void setBannerCallbacks(IBannerAdListener listener)
		{
			Appodeal.getInstance().setBannerCallbacks(listener);
		}

		public static void setMrecCallbacks(IMrecAdListener listener)
		{
			Appodeal.getInstance().setMrecCallbacks(listener);
		}

		public static void requestAndroidMPermissions(IPermissionGrantedListener listener)
		{
			Appodeal.getInstance().requestAndroidMPermissions(listener);
		}

		public static KeyValuePair<string, double> getRewardParameters()
		{
			return new KeyValuePair<string, double>(Appodeal.getInstance().getRewardCurrency(), Appodeal.getInstance().getRewardAmount());
		}

		public static KeyValuePair<string, double> getRewardParameters(string placement)
		{
			return new KeyValuePair<string, double>(Appodeal.getInstance().getRewardCurrency(placement), Appodeal.getInstance().getRewardAmount(placement));
		}

		public static double getPredictedEcpm(int adType)
		{
			return Appodeal.getInstance().getPredictedEcpm(adType);
		}

		public static void destroy(int adTypes)
		{
			Appodeal.getInstance().destroy(adTypes);
		}

		public const int NONE = 0;

		public const int INTERSTITIAL = 3;

		public const int BANNER = 4;

		public const int BANNER_BOTTOM = 8;

		public const int BANNER_TOP = 16;

		public const int BANNER_VIEW = 64;

		public const int MREC = 512;

		public const int REWARDED_VIDEO = 128;

		public const int NON_SKIPPABLE_VIDEO = 128;

		public const int BANNER_HORIZONTAL_SMART = -1;

		public const int BANNER_HORIZONTAL_CENTER = -2;

		public const int BANNER_HORIZONTAL_RIGHT = -3;

		public const int BANNER_HORIZONTAL_LEFT = -4;

		public const string APPODEAL_PLUGIN_VERSION = "2.8.49.1";

		private static IAppodealAdsClient client;

		public enum LogLevel
		{
			None,
			Debug,
			Verbose
		}
	}
}
