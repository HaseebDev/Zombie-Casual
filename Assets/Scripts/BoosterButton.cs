using System;
using Framework.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoosterButton : MonoBehaviour
{
	private void Start()
	{
		this.clickBtn.onClick.AddListener(new UnityAction(this.BtnClickHandler));
	}

	private void BtnClickHandler()
	{
		Action action = this.onClickAction;
		if (action == null)
		{
			return;
		}
		action();
	}

	public void SetLifetimeRatio(float _normalizedValue)
	{
		this.buttonLifeTimeProgressBar.SetNormalizedValue(_normalizedValue);
	}

	private void OnDestroy()
	{
		this.clickBtn.onClick.RemoveListener(new UnityAction(this.BtnClickHandler));
	}

	[SerializeField]
	private FillingImageView buttonLifeTimeProgressBar;

	[SerializeField]
	private Button clickBtn;

	public Action onClickAction;
}
