using System;
using Adic;
using Framework.Interfaces;
using UnityEngine;

public class UpgardebleDancerController : UpgradeableObject
{
	protected override void Start()
	{
		base.Start();
		this.Inject();
		this.likeModel = GameObject.Find("LikeModel").GetComponent<LikeModel>();
		this.SuccesedUprade = (Action)Delegate.Combine(this.SuccesedUprade, new Action(this.OnSuccesedUprade));
		this.dancersUpgrader.Init(this);
		this.dancersUpgrader.LoadAllUpgrades();
	}

	private void OnSuccesedUprade()
	{
		this.vibrationSystem.Vibrate();
		this.dancersUpgrader.UpdateVisual();
		if (this.dancersUpgrader.progressIsFilled())
		{
			this.likeModel.IncraseLikesPerSec(base.Value);
		}
		if (this.tutorialController != null)
		{
			if (this.tutorialController.tutStepNumb == 4)
			{
				this.tutorialController.SetStep(5);
				return;
			}
			if (this.tutorialController.tutStepNumb == 5)
			{
				this.tutorialController.SetStep(6);
				this.tutorialController.HideCurrentStep();
			}
		}
	}

	public override void CalculatePrice()
	{
        base.Price = 99;
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
	private DancersUpgrader dancersUpgrader;

	[SerializeField]
	private TutorialController tutorialController;

	[Inject]
	private IVibrationSystem vibrationSystem;
}
