using System;
using Adic;
using Framework.Interfaces;
using UnityEngine;

public class RepUpgadeableObject : UpgradeableObject
{
	protected override void Start()
	{
		base.Start();
		this.Inject();
		this.likeModel = GameObject.Find("LikeModel").GetComponent<LikeModel>();
		this.SuccesedUprade = (Action)Delegate.Combine(this.SuccesedUprade, new Action(this.OnSuccesedUprade));
		this.locationUpgrader.Init(this);
		this.locationUpgrader.UpdateVisualUprages();
	}

	private void OnSuccesedUprade()
	{
		this.vibrationSystem.Vibrate();
		this.likeModel.IncraseMultipler();
		this.locationUpgrader.UpdateVisualUprages();
	}

	public override void CalculatePrice()
	{
        base.Price = 10;
	}

	public override void CalculateValue()
	{
	}

	private void OnDestroy()
	{
		this.vibrationSystem = null;
		this.SuccesedUprade = (Action)Delegate.Remove(this.SuccesedUprade, new Action(this.OnSuccesedUprade));
	}

	private LikeModel likeModel;

	[SerializeField]
	private LocationUpgrader locationUpgrader;

	[Inject]
	private IVibrationSystem vibrationSystem;
}
