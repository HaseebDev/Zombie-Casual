using System;
using Framework.Interfaces;

public class SwitcherController : IDisposable, ITick, ITickLate
{
	public SwitcherController(SwitchModel _switchModel)
	{
		this.SwitchModel = _switchModel;
	}

	public void Load(ButtonSwitcherView _buttonSwitcherView, UpdateManager _updateManager)
	{
		this.SwitcherView = _buttonSwitcherView;
		this.SwitchModel.LoadCurrentStateValue();
		this.updateManager = _updateManager;
		this.updateManager.AddTo(this);
	}

	public void Unload()
	{
		this.updateManager.RemoveFrom(this);
	}

	public void Tick()
	{
		if (this.SwitcherView.onStateChanged)
		{
			this.SwitchModel.StateChanged(this.SwitcherView.state);
		}
		if (this.SwitchModel.currentSwitchStateValueLoaded)
		{
			this.SwitcherView.Switch(this.SwitchModel.state);
		}
	}

	public void TickLate()
	{
		this.SwitchModel.MyLateUpdate();
		this.SwitcherView.MyLateUpdate();
	}

	public void Dispose()
	{
		this.SwitcherView = null;
		this.SwitchModel = null;
		this.updateManager = null;
	}

	private ButtonSwitcherView SwitcherView;

	private SwitchModel SwitchModel;

	private UpdateManager updateManager;
}
