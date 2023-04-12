using System;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android
{
	public class AndroidAppodealClient : IAppodealAdsClient
	{
		private int nativeAdTypesForType(int adTypes)
		{
			int num = 0;
			if ((adTypes & 3) > 0)
			{
				num |= 3;
			}
			if ((adTypes & 4) > 0)
			{
				num |= 4;
			}
			if ((adTypes & 64) > 0)
			{
				num |= 64;
			}
			if ((adTypes & 16) > 0)
			{
				num |= 16;
			}
			if ((adTypes & 8) > 0)
			{
				num |= 8;
			}
			if ((adTypes & 512) > 0)
			{
				num |= 256;
			}
			if ((adTypes & 128) > 0)
			{
				num |= 128;
			}
			if ((adTypes & 128) > 0)
			{
				num |= 128;
			}
			return num;
		}

		public AndroidJavaClass getAppodealClass()
		{
			if (this.appodealClass == null)
			{
				this.appodealClass = new AndroidJavaClass("com.appodeal.ads.Appodeal");
			}
			return this.appodealClass;
		}

		public AndroidJavaClass getAppodealUnityClass()
		{
			if (this.appodealUnityClass == null)
			{
				this.appodealUnityClass = new AndroidJavaClass("com.appodeal.unity.AppodealUnity");
			}
			return this.appodealUnityClass;
		}

		public AndroidJavaObject getAppodealBannerInstance()
		{
			if (this.appodealBannerInstance == null)
			{
				this.appodealBannerClass = new AndroidJavaClass("com.appodeal.unity.AppodealUnityBannerView");
				this.appodealBannerInstance = this.appodealBannerClass.CallStatic<AndroidJavaObject>("getInstance", Array.Empty<object>());
			}
			return this.appodealBannerInstance;
		}

		public AndroidJavaObject getActivity()
		{
			if (this.activity == null)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				this.activity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			return this.activity;
		}

		public void initialize(string appKey, int adTypes)
		{
			this.initialize(appKey, adTypes, true);
		}

		public void initialize(string appKey, int adTypes, bool hasConsent)
		{
			this.getAppodealClass().CallStatic("setFramework", new object[]
			{
				"unity",
				Appodeal.getPluginVersion(),
				Application.unityVersion
			});
			if ((adTypes & 64) > 0 || (adTypes & 512) > 0)
			{
				this.getAppodealClass().CallStatic("setFramework", new object[]
				{
					"unity",
					Appodeal.getPluginVersion(),
					Application.unityVersion,
					false,
					false
				});
				this.getAppodealClass().CallStatic("disableNetwork", new object[]
				{
					this.getActivity(),
					"amazon_ads",
					4
				});
			}
			this.getAppodealClass().CallStatic("initialize", new object[]
			{
				this.getActivity(),
				appKey,
				this.nativeAdTypesForType(adTypes),
				hasConsent
			});
		}

		public bool isInitialized(int adType)
		{
			return this.getAppodealClass().CallStatic<bool>("isInitialized", new object[]
			{
				this.nativeAdTypesForType(adType)
			});
		}

		public bool show(int adTypes)
		{
			return this.getAppodealClass().CallStatic<bool>("show", new object[]
			{
				this.getActivity(),
				this.nativeAdTypesForType(adTypes)
			});
		}

		public bool show(int adTypes, string placement)
		{
			return this.getAppodealClass().CallStatic<bool>("show", new object[]
			{
				this.getActivity(),
				this.nativeAdTypesForType(adTypes),
				placement
			});
		}

		public bool showBannerView(int YAxis, int XAxis, string Placement)
		{
			return this.getAppodealBannerInstance().Call<bool>("showBannerView", new object[]
			{
				this.getActivity(),
				XAxis,
				YAxis,
				Placement
			});
		}

		public bool showMrecView(int YAxis, int XAxis, string Placement)
		{
			return this.getAppodealBannerInstance().Call<bool>("showMrecView", new object[]
			{
				this.getActivity(),
				XAxis,
				YAxis,
				Placement
			});
		}

		public bool isLoaded(int adTypes)
		{
			return this.getAppodealClass().CallStatic<bool>("isLoaded", new object[]
			{
				this.nativeAdTypesForType(adTypes)
			});
		}

		public void cache(int adTypes)
		{
			this.getAppodealClass().CallStatic("cache", new object[]
			{
				this.getActivity(),
				this.nativeAdTypesForType(adTypes)
			});
		}

		public void hide(int adTypes)
		{
			this.getAppodealClass().CallStatic("hide", new object[]
			{
				this.getActivity(),
				this.nativeAdTypesForType(adTypes)
			});
		}

		public void hideBannerView()
		{
			this.getAppodealBannerInstance().Call("hideBannerView", new object[]
			{
				this.getActivity()
			});
		}

		public void hideMrecView()
		{
			this.getAppodealBannerInstance().Call("hideMrecView", new object[]
			{
				this.getActivity()
			});
		}

		public bool isPrecache(int adTypes)
		{
			return this.getAppodealClass().CallStatic<bool>("isPrecache", new object[]
			{
				this.nativeAdTypesForType(adTypes)
			});
		}

		public void setAutoCache(int adTypes, bool autoCache)
		{
			this.getAppodealClass().CallStatic("setAutoCache", new object[]
			{
				this.nativeAdTypesForType(adTypes),
				autoCache
			});
		}

		public void onResume()
		{
			this.getAppodealClass().CallStatic("onResume", new object[]
			{
				this.getActivity(),
				4
			});
		}

		public void setSmartBanners(bool value)
		{
			this.getAppodealClass().CallStatic("setSmartBanners", new object[]
			{
				value
			});
			this.getAppodealBannerInstance().Call("setSmartBanners", new object[]
			{
				value
			});
		}

		public void setBannerAnimation(bool value)
		{
			this.getAppodealClass().CallStatic("setBannerAnimation", new object[]
			{
				value
			});
		}

		public void setBannerBackground(bool value)
		{
			UnityEngine.Debug.LogWarning("Not Supported by Android SDK");
		}

		public void setTabletBanners(bool value)
		{
			this.getAppodealClass().CallStatic("set728x90Banners", new object[]
			{
				value
			});
		}

		public void setTesting(bool test)
		{
			this.getAppodealClass().CallStatic("setTesting", new object[]
			{
				test
			});
		}

		public void setLogLevel(Appodeal.LogLevel logging)
		{
			switch (logging)
			{
			case Appodeal.LogLevel.None:
				this.getAppodealClass().CallStatic("setLogLevel", new object[]
				{
					new AndroidJavaClass("com.appodeal.ads.utils.Log$LogLevel").GetStatic<AndroidJavaObject>("none")
				});
				return;
			case Appodeal.LogLevel.Debug:
				this.getAppodealClass().CallStatic("setLogLevel", new object[]
				{
					new AndroidJavaClass("com.appodeal.ads.utils.Log$LogLevel").GetStatic<AndroidJavaObject>("debug")
				});
				return;
			case Appodeal.LogLevel.Verbose:
				this.getAppodealClass().CallStatic("setLogLevel", new object[]
				{
					new AndroidJavaClass("com.appodeal.ads.utils.Log$LogLevel").GetStatic<AndroidJavaObject>("verbose")
				});
				return;
			default:
				return;
			}
		}

		public void setChildDirectedTreatment(bool value)
		{
			this.getAppodealClass().CallStatic("setChildDirectedTreatment", new object[]
			{
				value
			});
		}

		public void disableNetwork(string network)
		{
			this.getAppodealClass().CallStatic("disableNetwork", new object[]
			{
				this.getActivity(),
				network
			});
		}

		public void disableNetwork(string network, int adTypes)
		{
			this.getAppodealClass().CallStatic("disableNetwork", new object[]
			{
				this.getActivity(),
				network,
				this.nativeAdTypesForType(adTypes)
			});
		}

		public void disableLocationPermissionCheck()
		{
			this.getAppodealClass().CallStatic("disableLocationPermissionCheck", Array.Empty<object>());
		}

		public void disableWriteExternalStoragePermissionCheck()
		{
			this.getAppodealClass().CallStatic("disableWriteExternalStoragePermissionCheck", Array.Empty<object>());
		}

		public void setTriggerOnLoadedOnPrecache(int adTypes, bool onLoadedTriggerBoth)
		{
			this.getAppodealClass().CallStatic("setTriggerOnLoadedOnPrecache", new object[]
			{
				this.nativeAdTypesForType(adTypes),
				onLoadedTriggerBoth
			});
		}

		public void muteVideosIfCallsMuted(bool value)
		{
			this.getAppodealClass().CallStatic("muteVideosIfCallsMuted", new object[]
			{
				value
			});
		}

		public void showTestScreen()
		{
			this.getAppodealClass().CallStatic("startTestActivity", new object[]
			{
				this.getActivity()
			});
		}

		public string getVersion()
		{
			return this.getAppodealClass().CallStatic<string>("getVersion", Array.Empty<object>());
		}

		public bool canShow(int adTypes)
		{
			return this.getAppodealClass().CallStatic<bool>("canShow", new object[]
			{
				this.nativeAdTypesForType(adTypes)
			});
		}

		public bool canShow(int adTypes, string placement)
		{
			return this.getAppodealClass().CallStatic<bool>("canShow", new object[]
			{
				this.nativeAdTypesForType(adTypes),
				placement
			});
		}

		public void setSegmentFilter(string name, bool value)
		{
			this.getAppodealClass().CallStatic("setSegmentFilter", new object[]
			{
				name,
				value
			});
		}

		public void setSegmentFilter(string name, int value)
		{
			this.getAppodealClass().CallStatic("setSegmentFilter", new object[]
			{
				name,
				value
			});
		}

		public void setSegmentFilter(string name, double value)
		{
			this.getAppodealClass().CallStatic("setSegmentFilter", new object[]
			{
				name,
				value
			});
		}

		public void setSegmentFilter(string name, string value)
		{
			this.getAppodealClass().CallStatic("setSegmentFilter", new object[]
			{
				name,
				value
			});
		}

		public void setExtraData(string key, bool value)
		{
			this.getAppodealClass().CallStatic("setExtraData", new object[]
			{
				key,
				value
			});
		}

		public void setExtraData(string key, int value)
		{
			this.getAppodealClass().CallStatic("setExtraData", new object[]
			{
				key,
				value
			});
		}

		public void setExtraData(string key, double value)
		{
			this.getAppodealClass().CallStatic("setExtraData", new object[]
			{
				key,
				value
			});
		}

		public void setExtraData(string key, string value)
		{
			this.getAppodealClass().CallStatic("setExtraData", new object[]
			{
				key,
				value
			});
		}

		public void trackInAppPurchase(double amount, string currency)
		{
			this.getAppodealClass().CallStatic("trackInAppPurchase", new object[]
			{
				this.getActivity(),
				amount,
				currency
			});
		}

		public string getRewardCurrency(string placement)
		{
			return this.getAppodealClass().CallStatic<AndroidJavaObject>("getRewardParameters", new object[]
			{
				placement
			}).Get<string>("second");
		}

		public double getRewardAmount(string placement)
		{
			return this.getAppodealClass().CallStatic<AndroidJavaObject>("getRewardParameters", new object[]
			{
				placement
			}).Get<AndroidJavaObject>("first").Call<double>("doubleValue", Array.Empty<object>());
		}

		public string getRewardCurrency()
		{
			return this.getAppodealClass().CallStatic<AndroidJavaObject>("getRewardParameters", Array.Empty<object>()).Get<string>("second");
		}

		public double getRewardAmount()
		{
			return this.getAppodealClass().CallStatic<AndroidJavaObject>("getRewardParameters", Array.Empty<object>()).Get<AndroidJavaObject>("first").Call<double>("doubleValue", Array.Empty<object>());
		}

		public double getPredictedEcpm(int adType)
		{
			return this.getAppodealClass().CallStatic<double>("getPredictedEcpm", new object[]
			{
				adType
			});
		}

		public void destroy(int adTypes)
		{
			this.getAppodealClass().CallStatic("destroy", new object[]
			{
				this.nativeAdTypesForType(adTypes)
			});
		}

		public void getUserSettings()
		{
			this.userSettings = this.getAppodealClass().CallStatic<AndroidJavaObject>("getUserSettings", new object[]
			{
				this.getActivity()
			});
		}

		public void setUserId(string id)
		{
			this.userSettings.Call<AndroidJavaObject>("setUserId", new object[]
			{
				id
			});
		}

		public void setAge(int age)
		{
			this.userSettings.Call<AndroidJavaObject>("setAge", new object[]
			{
				age
			});
		}

		public void setGender(UserSettings.Gender gender)
		{
			switch (gender)
			{
			case UserSettings.Gender.OTHER:
				this.userSettings.Call<AndroidJavaObject>("setGender", new object[]
				{
					new AndroidJavaClass("com.appodeal.ads.UserSettings$Gender").GetStatic<AndroidJavaObject>("OTHER")
				});
				return;
			case UserSettings.Gender.MALE:
				this.userSettings.Call<AndroidJavaObject>("setGender", new object[]
				{
					new AndroidJavaClass("com.appodeal.ads.UserSettings$Gender").GetStatic<AndroidJavaObject>("MALE")
				});
				return;
			case UserSettings.Gender.FEMALE:
				this.userSettings.Call<AndroidJavaObject>("setGender", new object[]
				{
					new AndroidJavaClass("com.appodeal.ads.UserSettings$Gender").GetStatic<AndroidJavaObject>("FEMALE")
				});
				return;
			default:
				return;
			}
		}

		public void setInterstitialCallbacks(IInterstitialAdListener listener)
		{
			this.getAppodealClass().CallStatic("setInterstitialCallbacks", new object[]
			{
				new AppodealInterstitialCallbacks(listener)
			});
		}

		public void setNonSkippableVideoCallbacks(INonSkippableVideoAdListener listener)
		{
			this.getAppodealClass().CallStatic("setNonSkippableVideoCallbacks", new object[]
			{
				new AppodealNonSkippableVideoCallbacks(listener)
			});
		}

		public void setRewardedVideoCallbacks(IRewardedVideoAdListener listener)
		{
			this.getAppodealClass().CallStatic("setRewardedVideoCallbacks", new object[]
			{
				new AppodealRewardedVideoCallbacks(listener)
			});
		}

		public void setBannerCallbacks(IBannerAdListener listener)
		{
			this.getAppodealClass().CallStatic("setBannerCallbacks", new object[]
			{
				new AppodealBannerCallbacks(listener)
			});
		}

		public void setMrecCallbacks(IMrecAdListener listener)
		{
			this.getAppodealClass().CallStatic("setMrecCallbacks", new object[]
			{
				new AppodealMrecCallbacks(listener)
			});
		}

		public void requestAndroidMPermissions(IPermissionGrantedListener listener)
		{
			this.getAppodealClass().CallStatic("requestAndroidMPermissions", new object[]
			{
				this.getActivity(),
				new AppodealPermissionCallbacks(listener)
			});
		}

		private bool isShow;

		private AndroidJavaClass appodealClass;

		private AndroidJavaClass appodealUnityClass;

		private AndroidJavaClass appodealBannerClass;

		private AndroidJavaObject appodealBannerInstance;

		private AndroidJavaObject userSettings;

		private AndroidJavaObject activity;

		private AndroidJavaObject popupWindow;

		private AndroidJavaObject resources;

		private AndroidJavaObject displayMetrics;

		private AndroidJavaObject window;

		private AndroidJavaObject decorView;

		private AndroidJavaObject attributes;

		private AndroidJavaObject rootView;

		public const int NONE = 0;

		public const int INTERSTITIAL = 3;

		public const int BANNER = 4;

		public const int BANNER_BOTTOM = 8;

		public const int BANNER_TOP = 16;

		public const int BANNER_VIEW = 64;

		public const int MREC = 256;

		public const int REWARDED_VIDEO = 128;
	}
}
