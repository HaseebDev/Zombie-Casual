using System;
using Adic;
using Framework.Interfaces;
using Framework.Managers.Ads;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoosterShopButton : MonoBehaviour
{
	private void Start()
	{
		this.Inject();
		this.button.onClick.AddListener(new UnityAction(this.ButtonOnClick));
		this.button.interactable = false;
		this.Check();
	}

	private void ButtonOnClick()
	{
		this.vibrationSystem.Vibrate();
		this.boosterEffect.BoosterButton_OnClickAction();
	}

	private void Check()
	{
		if (!this.baseAdSystem.rewardVideoIsLoaded())
		{
			this.StartCheck();
			this.button.interactable = false;
			return;
		}
		this.StopCheck();
		this.button.interactable = true;
	}

	private void StartCheck()
	{
		this.startCheck = true;
		this.checkPeriod = this.defCheckPerdiod;
	}

	private void StopCheck()
	{
		this.startCheck = false;
	}

	private void Update()
	{
		if (this.startCheck)
		{
			this.checkPeriod -= Time.deltaTime;
			if (this.checkPeriod <= 0f)
			{
				this.Check();
			}
		}
	}

	[SerializeField]
	private Button button;

	[SerializeField]
	private BoosterEffect boosterEffect;

	[Inject]
	private BaseAdSystem baseAdSystem;

	[Inject]
	private IVibrationSystem vibrationSystem;

	private bool startCheck;

	private float defCheckPerdiod = 15f;

	private float checkPeriod;
}
