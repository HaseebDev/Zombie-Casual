using System;
using Adic;
using Framework.Interfaces;
using Framework.Managers.Event;

public class LevelCheatPanelController : IState, IDisposable, ITick, ITickLate
{
	public void Load()
	{
		this.updateManager.AddTo(this);
		this.LevelCheatPanelView.Load();
	}

	public void Unload()
	{
		this.updateManager.RemoveFrom(this);
		this.LevelCheatPanelView.Unload();
	}

	public void Tick()
	{
	}

	public void TickLate()
	{
		this.LevelCheatPanelView.TickLate();
	}

	public void Dispose()
	{
		this.updateManager = null;
		this.LevelCheatPanelView = null;
		
	}

	[Inject]
	private LevelCheatPanelView LevelCheatPanelView;




	[Inject]
	private UpdateManager updateManager;
}
