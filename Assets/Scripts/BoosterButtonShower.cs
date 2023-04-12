using System;
using System.Collections.Generic;
using Adic;
using Framework.Managers.Ads;
using UnityEngine;

public class BoosterButtonShower : MonoBehaviour
{
	private void Start()
	{
		this.Inject();
		this.HideAllBoosters();
	}

	private void ShowBooster()
	{
		//if (this.baseAdSystem.rewardVideoIsLoaded())
		//{
		//	this.boosterDatas[this.lastShowedBoosterIndex].boosterVisualButton.SetActive(true);
		//}
		this.boosterShowing = true;
		this.showTimeOut = 0f;
	}

	private void HideAllBoosters()
	{
		for (int i = 0; i < this.boosterDatas.Count; i++)
		{
			this.boosterDatas[i].boosterVisualButton.SetActive(false);
		}
	}

	private void HideBooster()
	{
		this.boosterShowTime = 0f;
		this.boosterShowing = false;
		this.boosterDatas[this.lastShowedBoosterIndex].boosterVisualButton.SetActive(false);
		this.lastShowedBoosterIndex++;
		if (this.lastShowedBoosterIndex > this.boosterDatas.Count - 1)
		{
			this.lastShowedBoosterIndex = 0;
		}
	}

	private void Update()
	{
		if (!this.boosterShowing)
		{
			this.showTimeOut += Time.deltaTime;
			if (this.showTimeOut >= this.showTimeOutMax)
			{
				this.ShowBooster();
			}
			return;
		}
		if (this.boosterShowTime < this.boosterShowMaxTime)
		{
			this.boosterShowTime += Time.deltaTime;
			this.boosterDatas[this.lastShowedBoosterIndex].boosterButton.SetLifetimeRatio(this.boosterShowTime / this.boosterShowMaxTime);
			return;
		}
		this.HideBooster();
	}

	[SerializeField]
	private List<BoosterData> boosterDatas = new List<BoosterData>();

	private float boosterShowTime;

	[SerializeField]
	private float boosterShowMaxTime = 30f;

	private float showTimeOut;

	[SerializeField]
	private float showTimeOutMax = 6f;

	private bool boosterShowing;

	private int lastShowedBoosterIndex;

	private bool adIsLoaded;

	[Inject]
	private BaseAdSystem baseAdSystem;
}
