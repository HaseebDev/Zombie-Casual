using System;
using Adic;
using Framework.Managers.Ads;
using UnityEngine;

public class DamageBoosterEffect : BoosterEffect
{
	protected override void Start()
	{
		this.Inject();
		base.Init(this.baseAdSystem);
	}

	public override void ApplyEffect()
	{
		CountdownEffect[] array = UnityEngine.Object.FindObjectsOfType<DamageBooster>();
		this.countdownEffects = array;
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
	}

	private CountdownEffect[] countdownEffects;

	[Inject]
	private BaseAdSystem baseAdSystem;
}
