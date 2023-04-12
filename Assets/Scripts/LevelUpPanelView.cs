using System;
using AppodealAds.Unity.Api;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Framework.Interfaces;
using Framework.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelUpPanelView : MonoBehaviour, ITickLate, IState
{
	public void Load()
	{
		this.movePlatform.transform.localScale = Vector3.zero;
		this.movePlatform.DOScale(1.1f, 1.5f).SetEase(Ease.OutElastic, 1f, 2f);
		base.gameObject.SetActive(true);
		if (!Appodeal.isLoaded(128))
		{
			this.collectDoubleWithAdBtn.interactable = false;
			return;
		}
		this.collectDoubleWithAdBtn.interactable = true;
	}

	public RewardedVideoButtonView GetRewardedVideoButtonView()
	{
		return this.rewardedVideoButtonView;
	}

	public void Unload()
	{
		base.gameObject.SetActive(false);
		this.OnCollectBtnClicked = false;
		this.OnDoubleCollectBtnClicked = false;
	}

	private void Start()
	{
		this.collectBtn.onClick.AddListener(new UnityAction(this.CollectBtnClickHandler));
		this.collectDoubleWithAdBtn.onClick.AddListener(new UnityAction(this.DoubleCollectBtnClickHandler));
	}

	private void CollectBtnClickHandler()
	{
		this.OnCollectBtnClicked = true;
	}

	private void DoubleCollectBtnClickHandler()
	{
		this.OnDoubleCollectBtnClicked = true;
	}

	public void setBonusLikesTxt(long _likes)
	{
		this.bonusLikesTxt.text = Converter.CurrencyConvert(_likes);
	}

	public void TickLate()
	{
		this.OnCollectBtnClicked = false;
		this.OnDoubleCollectBtnClicked = false;
	}

	private void OnDestroy()
	{
		this.collectBtn.onClick.RemoveListener(new UnityAction(this.CollectBtnClickHandler));
		this.collectDoubleWithAdBtn.onClick.RemoveListener(new UnityAction(this.DoubleCollectBtnClickHandler));
	}

	[SerializeField]
	private Button collectBtn;

	[SerializeField]
	private Button collectDoubleWithAdBtn;

	[SerializeField]
	private Text bonusLikesTxt;

	[SerializeField]
	private RewardedVideoButtonView rewardedVideoButtonView;

	[SerializeField]
	private Transform movePlatform;

	public bool OnCollectBtnClicked;

	public bool OnDoubleCollectBtnClicked;
}
