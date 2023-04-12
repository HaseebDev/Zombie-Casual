using System;
using UnityEngine;

public class Taptic : MonoBehaviour
{
	public static void Warning()
	{
		if (!Taptic.tapticOn || Application.isEditor)
		{
			return;
		}
		AndroidTaptic.Haptic(HapticTypes.Warning);
	}

	public static void Failure()
	{
		if (!Taptic.tapticOn || Application.isEditor)
		{
			return;
		}
		AndroidTaptic.Haptic(HapticTypes.Failure);
	}

	public static void Success()
	{
		if (!Taptic.tapticOn || Application.isEditor)
		{
			return;
		}
		AndroidTaptic.Haptic(HapticTypes.Success);
	}

	public static void Light()
	{
		if (!Taptic.tapticOn || Application.isEditor)
		{
			return;
		}
		AndroidTaptic.Haptic(HapticTypes.LightImpact);
	}

	public static void Medium()
	{
		if (!Taptic.tapticOn || Application.isEditor)
		{
			return;
		}
		AndroidTaptic.Haptic(HapticTypes.MediumImpact);
	}

	public static void Heavy()
	{
		if (!Taptic.tapticOn || Application.isEditor)
		{
			return;
		}
		AndroidTaptic.Haptic(HapticTypes.HeavyImpact);
	}

	public static void Default()
	{
		if (!Taptic.tapticOn || Application.isEditor)
		{
			return;
		}
		Handheld.Vibrate();
	}

	public static void Vibrate()
	{
		if (!Taptic.tapticOn || Application.isEditor)
		{
			return;
		}
		AndroidTaptic.Vibrate();
	}

	public static void Selection()
	{
		if (!Taptic.tapticOn || Application.isEditor)
		{
			return;
		}
		AndroidTaptic.Haptic(HapticTypes.Selection);
	}

	private static bool iPhone6s()
	{
		return SystemInfo.deviceModel == "iPhone8,1" || SystemInfo.deviceModel == "iPhone8,2";
	}

	public static bool tapticOn = true;
}
