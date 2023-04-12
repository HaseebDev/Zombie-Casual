using System;
using Adic;
using Framework.Interfaces;
using UnityEngine;

public class SwitchVibroModel : SwitchModel, IDisposable
{
	public override void LoadCurrentStateValue()
	{
		this.currentSwitchStateValueLoaded = true;
		GlobalVars.Vibration = PlayerPrefs.GetInt("vibro", 1);
		if (GlobalVars.Vibration == 1)
		{
			this.state = true;
			return;
		}
		this.state = false;
	}

	public override void StateChanged(bool _state)
	{
		if (_state)
		{
			GlobalVars.Vibration = 1;
			this.vibrationSystem.Vibrate(100L);
		}
		else
		{
			GlobalVars.Vibration = 0;
		}
		PlayerPrefs.SetInt("vibro", GlobalVars.Vibration);
		this.state = _state;
	}

	public override void MyLateUpdate()
	{
		base.MyLateUpdate();
	}

	public void Dispose()
	{
		this.vibrationSystem = null;
	}

	[Inject]
	private IVibrationSystem vibrationSystem;
}
