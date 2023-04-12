using System;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine;

public class AppodealDemo : MonoBehaviour, IPermissionGrantedListener, IInterstitialAdListener, IBannerAdListener, IMrecAdListener, INonSkippableVideoAdListener, IRewardedVideoAdListener
{
	public void Awake()
	{
		Appodeal.requestAndroidMPermissions(this);
	}

	public void Init()
	{
		if (this.loggingToggle)
		{
			Appodeal.setLogLevel(Appodeal.LogLevel.Verbose);
		}
		else
		{
			Appodeal.setLogLevel(Appodeal.LogLevel.None);
		}
		Appodeal.setTesting(this.testingToggle);
		new UserSettings().setAge(25).setGender(UserSettings.Gender.OTHER).setUserId("best_user_ever");
		Appodeal.disableNetwork("appnext");
		Appodeal.disableNetwork("amazon_ads", 4);
		Appodeal.disableLocationPermissionCheck();
		Appodeal.disableWriteExternalStoragePermissionCheck();
		Appodeal.setTriggerOnLoadedOnPrecache(3, true);
		Appodeal.setSmartBanners(true);
		Appodeal.setBannerAnimation(false);
		Appodeal.setTabletBanners(false);
		Appodeal.setBannerBackground(false);
		Appodeal.setChildDirectedTreatment(false);
		Appodeal.muteVideosIfCallsMuted(true);
		Appodeal.setAutoCache(3, false);
		Appodeal.setExtraData(ExtraData.APPSFLYER_ID, "1527256526604-2129416");
		int @int = PlayerPrefs.GetInt("result_gdpr_sdk", 0);
		UnityEngine.Debug.Log("result_gdpr_sdk: " + @int);
		Appodeal.initialize(this.appKey, 707, @int == 1);
		Appodeal.setBannerCallbacks(this);
		Appodeal.setInterstitialCallbacks(this);
		Appodeal.setRewardedVideoCallbacks(this);
		Appodeal.setMrecCallbacks(this);
		Appodeal.setSegmentFilter("newBoolean", true);
		Appodeal.setSegmentFilter("newInt", 1234567890);
		Appodeal.setSegmentFilter("newDouble", 123.123456789);
		Appodeal.setSegmentFilter("newString", "newStringFromSDK");
	}

	private void OnGUI()
	{
		this.InitStyles();
		if (GUI.Toggle(new Rect((float)this.widthScale, (float)(this.heightScale - Screen.height / 18), (float)(this.toggleSize * 3), (float)this.toggleSize), this.testingToggle, new GUIContent("Testing")))
		{
			this.testingToggle = true;
		}
		else
		{
			this.testingToggle = false;
		}
		if (GUI.Toggle(new Rect((float)(Screen.width / 2), (float)(this.heightScale - Screen.height / 18), (float)(this.toggleSize * 3), (float)this.toggleSize), this.loggingToggle, new GUIContent("Logging")))
		{
			this.loggingToggle = true;
		}
		else
		{
			this.loggingToggle = false;
		}
		if (GUI.Button(new Rect((float)this.widthScale, (float)this.heightScale, (float)this.buttonWidth, (float)this.buttonHeight), "INITIALIZE", this.buttonStyle))
		{
			this.Init();
		}
		if (GUI.Button(new Rect((float)this.widthScale, (float)(this.heightScale + this.heightScale), (float)this.buttonWidth, (float)this.buttonHeight), this.interstitialLabel, this.buttonStyle))
		{
			this.showInterstitial();
		}
		if (GUI.Button(new Rect((float)this.widthScale, (float)(this.heightScale + 2 * this.heightScale), (float)this.buttonWidth, (float)this.buttonHeight), this.rewardedLabel, this.buttonStyle))
		{
			this.showRewardedVideo();
		}
		if (GUI.Button(new Rect((float)this.widthScale, (float)(this.heightScale + 3 * this.heightScale), (float)this.buttonWidth, (float)this.buttonHeight), "SHOW BANNER", this.buttonStyle))
		{
			this.showBanner();
		}
		if (GUI.Button(new Rect((float)this.widthScale, (float)(this.heightScale + 4 * this.heightScale), (float)this.buttonWidth, (float)this.buttonHeight), "HIDE BANNER", this.buttonStyle))
		{
			this.hideBanner();
		}
		if (GUI.Button(new Rect((float)this.widthScale, (float)(this.heightScale + 5 * this.heightScale), (float)this.buttonWidth, (float)this.buttonHeight), "SHOW BANNER VIEW", this.buttonStyle))
		{
			this.showBannerView();
		}
		if (GUI.Button(new Rect((float)this.widthScale, (float)(this.heightScale + 6 * this.heightScale), (float)this.buttonWidth, (float)this.buttonHeight), "HIDE BANNER VIEW", this.buttonStyle))
		{
			this.hideBannerView();
		}
		if (GUI.Button(new Rect((float)this.widthScale, (float)(this.heightScale + 7 * this.heightScale), (float)this.buttonWidth, (float)this.buttonHeight), "SHOW MREC VIEW", this.buttonStyle))
		{
			this.showMrecView();
		}
		if (GUI.Button(new Rect((float)this.widthScale, (float)(this.heightScale + 8 * this.heightScale), (float)this.buttonWidth, (float)this.buttonHeight), "HIDE MREC VIEW", this.buttonStyle))
		{
			this.hideMrecView();
		}
		if (GUI.Button(new Rect((float)this.widthScale, (float)(this.heightScale + 9 * this.heightScale), (float)this.buttonWidth, (float)this.buttonHeight), "SHOW TEST SCREEN", this.buttonStyle))
		{
			Appodeal.showTestScreen();
		}
	}

	private void InitStyles()
	{
		if (this.buttonStyle == null)
		{
			this.buttonWidth = Screen.width - Screen.width / 5;
			this.buttonHeight = Screen.height / 18;
			this.heightScale = Screen.height / 15;
			this.widthScale = Screen.width / 10;
			this.toggleSize = Screen.height / 20;
			this.buttonStyle = new GUIStyle(GUI.skin.button);
			this.buttonStyle.fontSize = this.buttonHeight / 2;
			this.buttonStyle.normal.textColor = Color.red;
			this.buttonStyle.hover.textColor = Color.red;
			this.buttonStyle.active.textColor = Color.red;
			this.buttonStyle.focused.textColor = Color.red;
			this.buttonStyle.active.background = this.MakeTexure(this.buttonWidth, this.buttonHeight, Color.grey);
			this.buttonStyle.focused.background = this.MakeTexure(this.buttonWidth, this.buttonHeight, Color.grey);
			this.buttonStyle.normal.background = this.MakeTexure(this.buttonWidth, this.buttonHeight, Color.white);
			this.buttonStyle.hover.background = this.MakeTexure(this.buttonWidth, this.buttonHeight, Color.white);
			GUI.skin.toggle = this.buttonStyle;
		}
	}

	private Texture2D MakeTexure(int width, int height, Color color)
	{
		Color[] array = new Color[width * height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = color;
		}
		Texture2D texture2D = new Texture2D(width, height);
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	public void showInterstitial()
	{
		if (Appodeal.isLoaded(3) && !Appodeal.isPrecache(3))
		{
			Appodeal.show(3);
			return;
		}
		Appodeal.cache(3);
	}

	public void showRewardedVideo()
	{
		UnityEngine.Debug.Log("Predicted eCPM for Rewarded Video: " + Appodeal.getPredictedEcpm(128));
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"Reward currency: ",
			Appodeal.getRewardParameters().Key,
			", amount: ",
			Appodeal.getRewardParameters().Value
		}));
		if (Appodeal.canShow(128))
		{
			Appodeal.show(128);
		}
	}

	public void showBanner()
	{
		Appodeal.show(8, "banner_button_click");
	}

	public void showBannerView()
	{
		Appodeal.showBannerView(Screen.currentResolution.height - Screen.currentResolution.height / 10, -2, "banner_view");
	}

	public void showMrecView()
	{
		Appodeal.showMrecView(Screen.currentResolution.height - Screen.currentResolution.height / 10, -2, "mrec_view");
	}

	public void hideBanner()
	{
		Appodeal.hide(4);
	}

	public void hideBannerView()
	{
		Appodeal.hideBannerView();
	}

	public void hideMrecView()
	{
		Appodeal.hideMrecView();
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			Appodeal.onResume();
         
        }
        
	}

	public void onBannerLoaded(bool precache)
	{
		MonoBehaviour.print("banner loaded");
	}

	public void onBannerFailedToLoad()
	{
		MonoBehaviour.print("banner failed");
	}

	public void onBannerShown()
	{
		MonoBehaviour.print("banner opened");
	}

	public void onBannerClicked()
	{
		MonoBehaviour.print("banner clicked");
	}

	public void onBannerExpired()
	{
		MonoBehaviour.print("banner expired");
	}

	public void onMrecLoaded(bool precache)
	{
		MonoBehaviour.print("mrec loaded");
	}

	public void onMrecFailedToLoad()
	{
		MonoBehaviour.print("mrec failed");
	}

	public void onMrecShown()
	{
		MonoBehaviour.print("mrec opened");
	}

	public void onMrecClicked()
	{
		MonoBehaviour.print("mrec clicked");
	}

	public void onMrecExpired()
	{
		MonoBehaviour.print("mrec expired");
	}

	public void onInterstitialLoaded(bool isPrecache)
	{
		this.interstitialLabel = "SHOW INTERSTITIAL";
		MonoBehaviour.print("Appodeal. Interstitial loaded");
	}

	public void onInterstitialFailedToLoad()
	{
		MonoBehaviour.print("Appodeal. Interstitial failed");
	}

	public void onInterstitialShown()
	{
		this.interstitialLabel = "CACHE INTERSTITIAL";
		MonoBehaviour.print("Appodeal. Interstitial opened");
	}

	public void onInterstitialClosed()
	{
		MonoBehaviour.print("Appodeal. Interstitial closed");
	}

	public void onInterstitialClicked()
	{
		MonoBehaviour.print("Appodeal. Interstitial clicked");
	}

	public void onInterstitialExpired()
	{
		MonoBehaviour.print("Appodeal. Interstitial expired");
	}

	public void onNonSkippableVideoLoaded(bool isPrecache)
	{
		UnityEngine.Debug.Log("NonSkippable Video loaded");
	}

	public void onNonSkippableVideoFailedToLoad()
	{
		UnityEngine.Debug.Log("NonSkippable Video failed to load");
	}

	public void onNonSkippableVideoShown()
	{
		UnityEngine.Debug.Log("NonSkippable Video opened");
	}

	public void onNonSkippableVideoClosed(bool isFinished)
	{
		UnityEngine.Debug.Log("NonSkippable Video, finished:" + isFinished.ToString());
	}

	public void onNonSkippableVideoFinished()
	{
		UnityEngine.Debug.Log("NonSkippable Video finished");
	}

	public void onNonSkippableVideoExpired()
	{
		UnityEngine.Debug.Log("NonSkippable Video expired");
	}

	public void onRewardedVideoLoaded(bool isPrecache)
	{
		this.rewardedLabel = "SHOW REWARDED";
		MonoBehaviour.print("Appodeal. Video loaded");
	}

	public void onRewardedVideoFailedToLoad()
	{
		MonoBehaviour.print("Appodeal. Video failed");
	}

	public void onRewardedVideoShown()
	{
		this.rewardedLabel = "Loading";
		MonoBehaviour.print("Appodeal. Video shown");
	}

	public void onRewardedVideoClosed(bool finished)
	{
		MonoBehaviour.print("Appodeal. Video closed");
	}

	public void onRewardedVideoFinished(double amount, string name)
	{
		MonoBehaviour.print(string.Concat(new object[]
		{
			"Appodeal. Reward: ",
			amount,
			" ",
			name
		}));
	}

	public void onRewardedVideoExpired()
	{
		MonoBehaviour.print("Appodeal. Video expired");
	}

	public void writeExternalStorageResponse(int result)
	{
		if (result == 0)
		{
			UnityEngine.Debug.Log("WRITE_EXTERNAL_STORAGE permission granted");
			return;
		}
		UnityEngine.Debug.Log("WRITE_EXTERNAL_STORAGE permission grant refused");
	}

	public void accessCoarseLocationResponse(int result)
	{
		if (result == 0)
		{
			UnityEngine.Debug.Log("ACCESS_COARSE_LOCATION permission granted");
			return;
		}
		UnityEngine.Debug.Log("ACCESS_COARSE_LOCATION permission grant refused");
	}

	private string appKey = "fee50c333ff3825fd6ad6d38cff78154de3025546d47a84f";

	private string interstitialLabel = "CACHE INTERSTITIAL";

	private string rewardedLabel = "Loading";

	private int buttonWidth;

	private int buttonHeight;

	private int heightScale;

	private int widthScale;

	private int toggleSize;

	private GUIStyle buttonStyle;

	private bool testingToggle;

	private bool loggingToggle;
}
