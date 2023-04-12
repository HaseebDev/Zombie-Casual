using System;
using Framework.Interfaces;
using Framework.Managers.Ads;
using UnityEngine;
using UnityEngine.UI;

public abstract class BoosterEffect : CountdownEffect, IRewardedVideoAdListener
{
	protected void Init(BaseAdSystem _baseAdSystem)
	{
		this.baseAdSystem = _baseAdSystem;
		this.boosterIndicator.SetActive(false);
		BoosterButton boosterButton = this.boosterButton;
		boosterButton.onClickAction = (Action)Delegate.Combine(boosterButton.onClickAction, new Action(this.BoosterButton_OnClickAction));
	}

	public void BoosterButton_OnClickAction()
	{
		this.baseAdSystem.SetRewardedVideoCallbacks(this);
		this.boosterButton.gameObject.SetActive(false);
		this.baseAdSystem.ShowRewardedVideo(delegate(bool obj)
		{
		});
	}

	private void LateUpdate()
	{
		if (this.countdownIsStarted)
		{
			this.boosterTimeTxt.text = "00:" + this.countdownTime.ToString("00");
		}
	}

	public void OnRewardedVideoClosed(bool finished)
	{
		if (finished)
		{
			base.StartEffect(this.boosterTime);
		}
	}

	public void OnRewardedVideoFinished(double amount, string name)
	{
		base.StartEffect(this.boosterTime);
	}

	public void OnRewardedVideoLoaded()
	{
	}

	public void OnRewardedVideoShown()
	{
	}

	public void OnRewardedVideoFailedToLoad()
	{
	}

	public abstract override void ApplyEffect();

	public abstract override void ResetEffect();

	[SerializeField]
	protected float boosterTime;

	public GameObject boosterIndicator;

	public Text boosterTimeTxt;

	public BoosterButton boosterButton;

	private bool isActive;

	private BaseAdSystem baseAdSystem;
}
