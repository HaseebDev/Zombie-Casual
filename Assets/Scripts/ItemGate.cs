using System;
using Framework.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemGate : MonoBehaviour
{
	private void Start()
	{
		if (this.unlockBtn != null)
		{
			this.unlockBtn.onClick.AddListener(new UnityAction(this.UnlockBtnPressed));
		}
	}

	private void UnlockBtnPressed()
	{
		this.OnGateBtnPressed();
	}

	public void UpdateUnlockCellButton(long _unlockValue, long _currentValue)
	{
		if (_unlockValue > _currentValue)
		{
			this.unlockBtn.interactable = false;
			return;
		}
		this.unlockBtn.interactable = true;
	}

	public void UpdateBuyGate(long _unlockValue, bool isLocked)
	{
		if (isLocked)
		{
			this.unlockValueTxt.text = this.valuePrefix + Converter.CurrencyConvert(_unlockValue);
			this.visual.SetActive(true);
			return;
		}
		this.visual.SetActive(false);
	}

	[SerializeField]
	private Button unlockBtn;

	[SerializeField]
	private Text unlockValueTxt;

	[SerializeField]
	private GameObject visual;

	[SerializeField]
	private string valuePrefix;

	public Action OnGateBtnPressed;
}
