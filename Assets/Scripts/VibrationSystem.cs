using System;
using Adic;
using Framework.Interfaces;
using UnityEngine;

public class VibrationSystem : IVibrationSystem
{
	[Inject]
	public VibrationSystem()
	{
		this.unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		this.currentActivity = this.unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		this.sysService = this.currentActivity.Call<AndroidJavaObject>("getSystemService", new object[]
		{
			"vibrator"
		});
	}

	public void Vibrate()
	{
		if (GlobalVars.Vibration == 1 && this.HasVibrator())
		{
			this.sysService.Call("vibrate", Array.Empty<object>());
		}
	}

	public void Vibrate(long milliseconds)
	{
		if (GlobalVars.Vibration == 1 && this.HasVibrator())
		{
			this.sysService.Call("vibrate", new object[]
			{
				milliseconds
			});
		}
	}

	public void Vibrate(long[] pattern, int repeat)
	{
		if (GlobalVars.Vibration == 1 && this.HasVibrator())
		{
			this.sysService.Call("vibrate", new object[]
			{
				pattern,
				repeat
			});
		}
	}

	public void Cancel()
	{
		if (GlobalVars.Vibration == 1 && this.HasVibrator())
		{
			this.sysService.Call("cancel", Array.Empty<object>());
		}
	}

	private bool HasVibrator()
	{
		return this.isAndroid() && this.sysService.Call<bool>("hasVibrator", Array.Empty<object>());
	}

	private bool isAndroid()
	{
		return true;
	}

	public AndroidJavaClass unityPlayer;

	public AndroidJavaObject currentActivity;

	public AndroidJavaObject sysService;
}
