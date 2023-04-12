//using System;
//using Adic;
//using Framework.Interfaces;

//public class GamePlayController : IDisposable, ITick, ITickLate, ITickSec, ISave, ILoad
//{

//	public void Init()
//	{
//		this.offlineIncomePanelController.Load();
//		this.gamePanelController.Load();
//		this.locationModel.Init();
//		this.locationModel.LoadLastLocation();
//		this.updateManager.AddTo(this);
//		this.audioSystem.PlayBgMusic(SoundEnum.BG_MUSIC);
//	}

//	public void Save()
//	{
//		this.levelModel.Save();
//		this.likeModel.Save();
//		this.starModel.Save();
//		this.offlineIncomePanelController.Save();
//	}

//	public void Load()
//	{
//		this.levelModel.Load();
//		this.likeModel.Load();
//		this.starModel.Load();
//	}

//	public void UnLoad()
//	{
//		this.updateManager.RemoveFrom(this);
//		this.gamePanelController.UnLoad();
//	}

//	public void Tick()
//	{
//		if (this.gamePlayView.OnTap)
//		{
//			this.starModel.Stars += this.likeModel.LikesPerTap;
//		}
//	}

//	public void TickLate()
//	{
//		this.likeModel.TickLate();
//		this.gamePlayView.TickLate();
//	}

//	public void Dispose()
//	{
//		this.gamePlayView = null;
//		this.gamePanelController = null;
//		this.levelModel = null;
//		this.locationModel = null;
//		this.starModel = null;
//		this.likeModel = null;
//		this.updateManager = null;
//		this.offlineIncomePanelController = null;
//	}

//	public void TickSec()
//	{
//	}

//	[Inject]
//	private GamePlayView gamePlayView;

//	[Inject]
//	private GamePanelController gamePanelController;

//	[Inject]
//	private LevelModel levelModel;

//	[Inject]
//	private OfflineIncomePanelController offlineIncomePanelController;

//	[Inject]
//	private LocationModel locationModel;

//	[Inject]
//	private CoinsModel starModel;

//	[Inject]
//	private LikeModel likeModel;

//	[Inject]
//	private UpdateManager updateManager;

//	[Inject]
//	private IAudioSystem audioSystem;
//}
