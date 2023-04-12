using System;
using Adic;
using Framework.Interfaces;
using UnityEngine;

public class UnitUpgadeableObject : UpgradeableObject
{
	protected override void Start()
	{
		base.Start();
		this.Inject();
		this.SuccesedUprade = (Action)Delegate.Combine(this.SuccesedUprade, new Action(this.UnitUpgadeableObject_SuccesedUprade));
	}

	protected override void LoadUpgradeableObject()
	{
		base.LoadUpgradeableObject();
		this.UpdateSolider();
	}

	private void FindObjectByUpgadebleId()
	{
	}

	private void UpdateSolider()
	{
		Character component = this.upgardebleValueGameObject.GetComponent<Character>();
		if (base.Level > 1)
		{
			this.upgardebleValueGameObject.SetActive(true);
			component.WeaponDamage = (float)base.Value;
			component.Level = base.Level;
		}
	}

	private void UnitUpgadeableObject_SuccesedUprade()
	{
		this.UpdateSolider();
		this.vibrationSystem.Vibrate();
		if (this.soliderNumber == 1)
		{
			this.tutorialController.t1TFinger.OnBuy();
		}
	}

	public override void CalculatePrice()
	{
		base.Price = Mathf.CeilToInt(Mathf.Pow(4f, (float)this.soliderNumber) * 35f * Mathf.Pow((float)base.Level, 1.5f));
	}

	public override void CalculateValue()
	{
		base.Value = Mathf.CeilToInt(Mathf.Pow((float)this.soliderNumber, 3f) * Mathf.Pow((float)base.Level, 1.5f));
	}

	private void OnDestroy()
	{
		this.vibrationSystem = null;
		this.SuccesedUprade = (Action)Delegate.Remove(this.SuccesedUprade, new Action(this.UnitUpgadeableObject_SuccesedUprade));
	}

	[SerializeField]
	private int soliderNumber;

	[Inject]
	private IVibrationSystem vibrationSystem;

	[SerializeField]
	private TutorialController tutorialController;
}
