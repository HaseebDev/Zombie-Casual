using System;
using Adic;
using Framework.Managers.Ads;
using UnityEngine;

public class MoneyBoosterEffect : BoosterEffect
{
	protected override void Start()
	{
		this.Inject();
		base.Init(this.baseAdSystem);
	}

	public override void ApplyEffect()
	{
		this.boosterIndicator.SetActive(true);
		for (int i = 0; i < this.countdownEffects.Length; i++)
		{
			this.countdownEffects[i].StartEffect(this.boosterTime);
		}
	}

	public override void ResetEffect()
	{
		this.boosterIndicator.SetActive(false);
		for (int i = 0; i < this.countdownEffects.Length; i++)
		{
			this.countdownEffects[i].ResetEffect();
		}
		this.particlesObject.SetActive(false);
	}

	[SerializeField]
	private CountdownEffect[] countdownEffects;

	[SerializeField]
	private GameObject particlesObject;

	[Inject]
	private BaseAdSystem baseAdSystem;
}
