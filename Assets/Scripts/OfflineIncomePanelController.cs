//using System;
//using Adic;
//using Framework.Interfaces;

//public class OfflineIncomePanelController : IState, IDisposable, ITick, ITickLate, ISave
//{
//	public void Load()
//	{
//		this.offlineIncomePanelModel.SetLikesPerSecValue(this.likeModel.LikesPerSec);
//		this.offlineIncomePanelModel.CountMinutesSinceLastExit();
//		this.offlineIncomStarValue = this.offlineIncomePanelModel.GetRewardValue();
//		if (this.offlineIncomStarValue <= 0L)
//		{
//			return;
//		}
//		this.offlineIncomePanelView.Load();
//		this.SetLevelUpRewardTextField();
//		this.rewardedVideoButtonController.Load(this.offlineIncomePanelView.GetRewardedVideoButtonView());
//		this.updateManager.AddTo(this);
//	}

//	public void Save()
//	{
//		this.offlineIncomePanelModel.SaveExitDate();
//	}

//	public void Unload()
//	{
//		this.rewardedVideoButtonController.Unload();
//		this.offlineIncomePanelView.Unload();
//		this.updateManager.RemoveFrom(this);
//	}

//	private void SetLevelUpRewardTextField()
//	{
//		this.offlineIncomePanelView.setBonusLikesTxt(this.offlineIncomStarValue);
//	}

//	public void Tick()
//	{
//		this.rewardedVideoButtonController.Tick();
//		if (this.rewardedVideoButtonController.OnRewardedVideoFinishedFailled)
//		{
//			this.Unload();
//		}
//		if (this.rewardedVideoButtonController.OnRewardedVideoFinishedSucces)
//		{
//			CoinsModel.instance.Golds += this.offlineIncomePanelModel.GetExtraRewardValue();
//			this.Unload();
//		}
//		if (this.offlineIncomePanelView.OnCollectBtnClicked)
//		{
//			//this.vibrationSystem.Vibrate();
//   //         CoinsModel.instance.Golds += this.offlineIncomePanelModel.GetRewardValue();
//			//this.Unload();
            
//		}
//		if (this.offlineIncomePanelView.OnDoubleCollectBtnClicked)
//		{
//			this.vibrationSystem.Vibrate();
//		}
//	}

//	public void TickLate()
//	{
//		this.rewardedVideoButtonController.TickLate();
//		this.offlineIncomePanelView.TickLate();
//	}

//	public void Dispose()
//	{
//		this.offlineIncomePanelView = null;
		
//		this.likeModel = null;
//		this.rewardedVideoButtonController = null;
//		this.offlineIncomePanelModel = null;
//		this.updateManager = null;
//		this.vibrationSystem = null;
//	}

//	[Inject]
//	private OfflineIncomePanelView offlineIncomePanelView;

//	[Inject]
//	private OfflineIncomePanelModel offlineIncomePanelModel;


//	[Inject]
//	private LikeModel likeModel;

//	[Inject]
//	private UpdateManager updateManager;

//	[Inject]
//	private RewardedVideoButtonController rewardedVideoButtonController;

//	[Inject]
//	private IVibrationSystem vibrationSystem;

//	private long offlineIncomStarValue;
//}
