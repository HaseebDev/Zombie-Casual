using System;
using Adic;
using Framework.Interfaces;

public class TapticVibrationSystem : IVibrationSystem
{
	[Inject]
	public TapticVibrationSystem()
	{
		Taptic.tapticOn = true;
	}

	public void Vibrate()
	{
		if (GlobalVars.Vibration == 1 && this.HasVibrator())
		{
			Taptic.Light();
		}
	}

	public void Vibrate(long milliseconds)
	{
		if (GlobalVars.Vibration == 1 && this.HasVibrator())
		{
			Taptic.Light();
		}
	}

	public void Vibrate(long[] pattern, int repeat)
	{
		if (GlobalVars.Vibration == 1 && this.HasVibrator())
		{
			Taptic.Light();
		}
	}

	public void Cancel()
	{
	}

	private bool HasVibrator()
	{
		return true;
	}
}
