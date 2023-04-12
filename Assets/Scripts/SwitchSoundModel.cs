using System;
using Adic;
using UnityEngine;

public class SwitchSoundModel : SwitchModel, IDisposable
{
	public override void LoadCurrentStateValue()
	{
		this.currentSwitchStateValueLoaded = true;
		GlobalVars.Sound = PlayerPrefs.GetInt("sound", 1);
		if (GlobalVars.Sound == 1)
		{
			this.state = true;
		}
		else
		{
			this.state = false;
		}
		this.UpdateSoundSettings(this.state);
	}

	private void UpdateSoundSettings(bool _state)
	{
		if (_state)
		{
			GlobalVars.Sound = 1;
			AudioSystem.instance.Unmute();
			return;
		}
		GlobalVars.Sound = 0;
		AudioSystem.instance.Mute();
	}

	public override void StateChanged(bool _state)
	{
		this.UpdateSoundSettings(_state);
		PlayerPrefs.SetInt("sound", GlobalVars.Sound);
		this.state = _state;
	}

	public override void MyLateUpdate()
	{
		base.MyLateUpdate();
	}

	public void Dispose()
	{
	}

}
