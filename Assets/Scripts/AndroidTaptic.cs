using System;
using UnityEngine;

public class AndroidTaptic
{
	private void Vib()
	{
		Handheld.Vibrate();
	}

	public static void Vibrate()
	{
		AndroidTaptic.AndroidVibrate(AndroidTaptic.MediumDuration);
	}

	public static void Haptic(HapticTypes type)
	{
		switch (type)
		{
		case HapticTypes.Selection:
			AndroidTaptic.AndroidVibrate(AndroidTaptic.LightDuration, AndroidTaptic.LightAmplitude);
			return;
		case HapticTypes.Success:
			AndroidTaptic.AndroidVibrate(AndroidTaptic._successPattern, AndroidTaptic._successPatternAmplitude, -1);
			return;
		case HapticTypes.Warning:
			AndroidTaptic.AndroidVibrate(AndroidTaptic._warningPattern, AndroidTaptic._warningPatternAmplitude, -1);
			return;
		case HapticTypes.Failure:
			AndroidTaptic.AndroidVibrate(AndroidTaptic._failurePattern, AndroidTaptic._failurePatternAmplitude, -1);
			return;
		case HapticTypes.LightImpact:
			AndroidTaptic.AndroidVibrate(AndroidTaptic.LightDuration, AndroidTaptic.LightAmplitude);
			return;
		case HapticTypes.MediumImpact:
			AndroidTaptic.AndroidVibrate(AndroidTaptic.MediumDuration, AndroidTaptic.MediumAmplitude);
			return;
		case HapticTypes.HeavyImpact:
			AndroidTaptic.AndroidVibrate(AndroidTaptic.HeavyDuration, AndroidTaptic.HeavyAmplitude);
			return;
		default:
			return;
		}
	}

	public static void AndroidVibrate(long milliseconds)
	{
		AndroidTaptic.AndroidVibrator.Call("vibrate", new object[]
		{
			milliseconds
		});
	}

	public static void AndroidVibrate(long milliseconds, int amplitude)
	{
		if (AndroidTaptic.AndroidSDKVersion() < 26)
		{
			AndroidTaptic.AndroidVibrate(milliseconds);
			return;
		}
		AndroidTaptic.VibrationEffectClassInitialization();
		AndroidTaptic.VibrationEffect = AndroidTaptic.VibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", new object[]
		{
			milliseconds,
			amplitude
		});
		AndroidTaptic.AndroidVibrator.Call("vibrate", new object[]
		{
			AndroidTaptic.VibrationEffect
		});
	}

	public static void AndroidVibrate(long[] pattern, int repeat)
	{
		if (AndroidTaptic.AndroidSDKVersion() < 26)
		{
			AndroidTaptic.AndroidVibrator.Call("vibrate", new object[]
			{
				pattern,
				repeat
			});
			return;
		}
		AndroidTaptic.VibrationEffectClassInitialization();
		AndroidTaptic.VibrationEffect = AndroidTaptic.VibrationEffectClass.CallStatic<AndroidJavaObject>("createWaveform", new object[]
		{
			pattern,
			repeat
		});
		AndroidTaptic.AndroidVibrator.Call("vibrate", new object[]
		{
			AndroidTaptic.VibrationEffect
		});
	}

	public static void AndroidVibrate(long[] pattern, int[] amplitudes, int repeat)
	{
		if (AndroidTaptic.AndroidSDKVersion() < 26)
		{
			AndroidTaptic.AndroidVibrator.Call("vibrate", new object[]
			{
				pattern,
				repeat
			});
			return;
		}
		AndroidTaptic.VibrationEffectClassInitialization();
		AndroidTaptic.VibrationEffect = AndroidTaptic.VibrationEffectClass.CallStatic<AndroidJavaObject>("createWaveform", new object[]
		{
			pattern,
			amplitudes,
			repeat
		});
		AndroidTaptic.AndroidVibrator.Call("vibrate", new object[]
		{
			AndroidTaptic.VibrationEffect
		});
	}

	public static void AndroidCancelVibrations()
	{
		AndroidTaptic.AndroidVibrator.Call("cancel", Array.Empty<object>());
	}

	private static void VibrationEffectClassInitialization()
	{
		if (AndroidTaptic.VibrationEffectClass == null)
		{
			AndroidTaptic.VibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
		}
	}

	public static int AndroidSDKVersion()
	{
		if (AndroidTaptic._sdkVersion == -1)
		{
			return AndroidTaptic._sdkVersion = int.Parse(SystemInfo.operatingSystem.Substring(SystemInfo.operatingSystem.IndexOf("-") + 1, 3));
		}
		return AndroidTaptic._sdkVersion;
	}

	public static long LightDuration = 20L;

	public static long MediumDuration = 40L;

	public static long HeavyDuration = 80L;

	public static int LightAmplitude = 40;

	public static int MediumAmplitude = 120;

	public static int HeavyAmplitude = 255;

	private static int _sdkVersion = -1;

	private static long[] _successPattern = new long[]
	{
		0L,
		AndroidTaptic.LightDuration,
		AndroidTaptic.LightDuration,
		AndroidTaptic.HeavyDuration
	};

	private static int[] _successPatternAmplitude = new int[]
	{
		0,
		AndroidTaptic.LightAmplitude,
		0,
		AndroidTaptic.HeavyAmplitude
	};

	private static long[] _warningPattern = new long[]
	{
		0L,
		AndroidTaptic.HeavyDuration,
		AndroidTaptic.LightDuration,
		AndroidTaptic.MediumDuration
	};

	private static int[] _warningPatternAmplitude = new int[]
	{
		0,
		AndroidTaptic.HeavyAmplitude,
		0,
		AndroidTaptic.MediumAmplitude
	};

	private static long[] _failurePattern = new long[]
	{
		0L,
		AndroidTaptic.MediumDuration,
		AndroidTaptic.LightDuration,
		AndroidTaptic.MediumDuration,
		AndroidTaptic.LightDuration,
		AndroidTaptic.HeavyDuration,
		AndroidTaptic.LightDuration,
		AndroidTaptic.LightDuration
	};

	private static int[] _failurePatternAmplitude = new int[]
	{
		0,
		AndroidTaptic.MediumAmplitude,
		0,
		AndroidTaptic.MediumAmplitude,
		0,
		AndroidTaptic.HeavyAmplitude,
		0,
		AndroidTaptic.LightAmplitude
	};

	private static AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

	private static AndroidJavaObject CurrentActivity = AndroidTaptic.UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

	private static AndroidJavaObject AndroidVibrator = AndroidTaptic.CurrentActivity.Call<AndroidJavaObject>("getSystemService", new object[]
	{
		"vibrator"
	});

	private static AndroidJavaClass VibrationEffectClass;

	private static AndroidJavaObject VibrationEffect;

	private static int DefaultAmplitude;
}
