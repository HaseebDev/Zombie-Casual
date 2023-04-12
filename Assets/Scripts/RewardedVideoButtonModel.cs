using System;
using Adic;
using Framework.Interfaces;
using Framework.Managers.Ads;

public class RewardedVideoButtonModel : IDisposable, IRewardedVideoAdListener
{
	public void Init()
	{
		this.baseAdSystem.SetRewardedVideoCallbacks(this);
	}

	public void ButtonClicked()
	{
		this.baseAdSystem.ShowRewardedVideo(delegate(bool onLoaded)
		{
			this.OnRewardedVideoStarted = true;
		});
	}

	public void Unload()
	{
		this.OnRewardedVideoStarted = false;
		this.OnRewardedVideoShowSucces = false;
		this.OnRewardedVideoShowError = false;
	}

	public void MyLateUpdate()
	{
		this.OnRewardedVideoStarted = false;
		this.OnRewardedVideoShowSucces = false;
		this.OnRewardedVideoShowError = false;
	}

	public void OnRewardedVideoClosed(bool finished)
	{
		if (finished)
		{
			this.OnRewardedVideoShowSucces = true;
			return;
		}
		this.OnRewardedVideoShowError = true;
	}

	public void OnRewardedVideoFinished(double amount, string name)
	{
		this.OnRewardedVideoShowSucces = true;
	}

	public void Dispose()
	{
		this.baseAdSystem.SetRewardedVideoCallbacks(null);
		this.baseAdSystem = null;
	}

	public void OnRewardedVideoFailedToLoad()
	{
	}

	public void OnRewardedVideoLoaded()
	{
	}

	public void OnRewardedVideoShown()
	{
	}

	[Inject]
	private BaseAdSystem baseAdSystem;

	public bool OnRewardedVideoStarted;

	public bool OnRewardedVideoShowSucces;

	public bool OnRewardedVideoShowError;
}
