//using System;
//using Adic;
//using Framework.Interfaces;

//public class SettingPanelController : IDisposable, ITick, ITickLate
//{
//	public void Load()
//	{
//		this.settingPanelView.Load();
//		this.updateManager.AddTo(this);
//		this.VibrationSwitcherController.Load(this.settingPanelView.GetVibroBtnView(), this.updateManager);
//		this.SoundSwitcherController.Load(this.settingPanelView.GetSoundBtnView(), this.updateManager);
//	}

//	public void UnLoad()
//	{
//		this.updateManager.RemoveFrom(this);
//		this.VibrationSwitcherController.Unload();
//		this.SoundSwitcherController.Unload();
//		this.settingPanelView.Unload();
//	}

//	public void Tick()
//	{
//		if (this.settingPanelView.onCloseBtnPressed)
//		{
//			this.settingPanelView.Unload();
//		}
//	}

//	public void TickLate()
//	{
//		this.settingPanelView.TickLate();
//	}

//	public void Dispose()
//	{
//		this.updateManager = null;
//		this.settingPanelView = null;
//		this.VibrationSwitcherController = null;
//		this.SoundSwitcherController = null;
//	}

//	//[Inject]
//	//private SettingPanelView settingPanelView;

//	[Inject]
//	private VibroSwitcherController VibrationSwitcherController;

//	[Inject]
//	private SoundSwitcherController SoundSwitcherController;

//	[Inject]
//	private UpdateManager updateManager;
//}
