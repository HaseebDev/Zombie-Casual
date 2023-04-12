using System;
using Adic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemCellBase : MonoBehaviour
{
	private void Awake()
	{
		this.unlockCost = (long)Mathf.CeilToInt(50f * Mathf.Pow(1.8f, (float)this.unlockLevel));
	}

	public virtual void Start()
	{
		this.Inject();
		ItemGate itemGate = this.payGate;
		itemGate.OnGateBtnPressed = (Action)Delegate.Combine(itemGate.OnGateBtnPressed, new Action(this.BuyCell));
		this.UpdateValues();
		this.isInitiated = true;
	}

	private void OnEnable()
	{
		if (this.isInitiated)
		{
			this.UpdateValues();
		}
	}

	public virtual void OnStarChanged()
	{
		this.SetupPayGateButton();
	}

	public void OnLevelChanged()
	{
		this.UpdateGates();
	}

	protected virtual void SetupLevelGate()
	{
		if (this.unlockLevel > LevelModel.instance.CurrentLevel)
		{
			this.lvlGate.UpdateBuyGate((long)this.unlockLevel, true);
			return;
		}
		this.lvlGate.UpdateBuyGate((long)this.unlockLevel, false);
	}

	public abstract void BuyCell();

	protected virtual void UpdatePayGate()
	{
		this.payGate.UpdateBuyGate(this.unlockCost, !this.isBuyed);
	}

	public abstract void UpdateValues();

	protected virtual void SetupPayGateButton()
	{
		if (!this.isBuyed)
		{
			this.payGate.UpdateUnlockCellButton(this.unlockCost, this.coinsModel.Golds);
		}
	}

	protected virtual void UpdateGates()
	{
		this.SetupLevelGate();
		if (this.unlockLevel <= LevelModel.instance.CurrentLevel && this.unlockCost > 0L)
		{
			this.UpdatePayGate();
		}
	}

	private void OnDestroy()
	{
		this.coinsModel = null;
		this.likeModel = null;
		
	}

	[SerializeField]
	protected Text itemNameTxt;

	[SerializeField]
	protected Text itemDescTxt;

	[SerializeField]
	protected ItemGate lvlGate;

	[SerializeField]
	protected ItemGate payGate;

	[Inject]
	protected CurrencyModels coinsModel;

	[Inject]
	protected LikeModel likeModel;


	[SerializeField]
	protected int unlockLevel;

	protected long unlockCost;

	protected bool isBuyed;

	private bool isInitiated;
}
