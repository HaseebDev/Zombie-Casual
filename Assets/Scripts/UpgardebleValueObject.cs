using System;
using Adic;
using Framework.Interfaces;
using UnityEngine;

public class UpgardebleValueObject : UpgradeableObject
{
	protected override void Start()
	{
		base.Start();
		this.Inject();
		this.SuccesedUprade = (Action)Delegate.Combine(this.SuccesedUprade, new Action(this.UpgardebleValueObject_SuccesedUprade));
	}

	protected override void LoadUpgradeableObject()
	{
		base.LoadUpgradeableObject();
		this.upgradableFloatValue = this.upgardebleValueGameObject.GetComponent<IUpgradableFloatValue>();
		this.upgradableFloatValue.SetUpgradableFloatValue((float)base.Value);
	}

	public override void CalculatePrice()
	{
		base.Price = Mathf.CeilToInt(10f * Mathf.Pow((float)base.Level, 1.92f));
	}

	private void UpgardebleValueObject_SuccesedUprade()
	{
		this.vibrationSystem.Vibrate();
		this.upgradableFloatValue.SetUpgradableFloatValue((float)base.Value);
	}

	public override void CalculateValue()
	{
		base.Value = base.Level;
	}

	private void OnDestroy()
	{
		this.SuccesedUprade = (Action)Delegate.Remove(this.SuccesedUprade, new Action(this.UpgardebleValueObject_SuccesedUprade));
	}

	private IUpgradableFloatValue upgradableFloatValue;

	[Inject]
	private IVibrationSystem vibrationSystem;

	[SerializeField]
	private int upgradeNumber = 1;
}
