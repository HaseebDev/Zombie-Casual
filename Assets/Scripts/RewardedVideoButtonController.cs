using System;
using Adic;
using Framework.Interfaces;
using Framework.Managers.Event;

public class RewardedVideoButtonController : IDisposable, ITick, ITickLate
{
	public void Load(RewardedVideoButtonView _rewardedVideoButtonView)
	{
		this.RewardedVideoButtonView = _rewardedVideoButtonView;
		this.RewardedVideoButtonModel.Init();
	}

	public void Unload()
	{
		this.RewardedVideoButtonView.Unload();
		this.RewardedVideoButtonModel.Unload();
		this.OnRewardedVideoFinishedStarted = false;
		this.OnRewardedVideoFinishedSucces = false;
		this.OnRewardedVideoFinishedFailled = false;
	}

	public void Tick()
	{
		if (this.RewardedVideoButtonView.rewardVideoBtnClicked)
		{
			this.RewardedVideoButtonModel.ButtonClicked();
		}
		if (this.RewardedVideoButtonModel.OnRewardedVideoShowSucces)
		{
			this.OnRewardedVideoFinishedSucces = true;
		}
		if (this.RewardedVideoButtonModel.OnRewardedVideoShowError)
		{
			this.OnRewardedVideoFinishedFailled = true;
		}
		if (this.RewardedVideoButtonModel.OnRewardedVideoStarted)
		{
			this.OnRewardedVideoFinishedStarted = true;
		}
	}

	public void TickLate()
	{
		this.OnRewardedVideoFinishedStarted = false;
		this.OnRewardedVideoFinishedSucces = false;
		this.OnRewardedVideoFinishedFailled = false;
		this.RewardedVideoButtonModel.MyLateUpdate();
		this.RewardedVideoButtonView.MyLateUpdate();
	}

	public void Dispose()
	{
		this.RewardedVideoButtonView = null;
		this.RewardedVideoButtonModel = null;
	}

	private RewardedVideoButtonView RewardedVideoButtonView;

	[Inject]
	private RewardedVideoButtonModel RewardedVideoButtonModel;


	public bool OnRewardedVideoFinishedStarted;

	public bool OnRewardedVideoFinishedSucces;

	public bool OnRewardedVideoFinishedFailled;
}
