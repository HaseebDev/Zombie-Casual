using System;
using Adic;
using Framework.Managers.Ads;
using Framework.Managers.Statistics;
using UnityEngine;

public class Starter : MonoBehaviour, IDisposable
{
	private void Awake()
	{
	}

	private void OnApplicationPause(bool pauseStatus)
	{
	}

	private void OnApplicationFocus(bool focus)
	{
		if (!focus)
		{
			this.mainController.SaveAllData();
		}
	}

	private void OnApplicationQuit()
	{
	}

	public void Init()
	{
		this.Inject();
		this.baseAdSystem.Init();
		this.mainController.Init();
		this.statisticSystem.AddStatisticSystem(new SS_FacebookAnalytics());
		this.statisticSystem.Init();
	}

	public void Dispose()
	{
		this.mainController = null;
	}

	[Inject]
	private MainController mainController;

	[Inject]
	private RewardedVideoButtonController rewardedVideoButtonController;

	[Inject]
	private BaseAdSystem baseAdSystem;

	[Inject]
	private StatisticSystemController statisticSystem;
}
