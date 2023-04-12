using System;

public abstract class SwitchModel
{
	public virtual void LoadCurrentStateValue()
	{
		this.currentSwitchStateValueLoaded = true;
	}

	public abstract void StateChanged(bool _state);

	public virtual void MyLateUpdate()
	{
		this.currentSwitchStateValueLoaded = false;
	}

	public bool currentSwitchStateValueLoaded;

	public bool state;
}
