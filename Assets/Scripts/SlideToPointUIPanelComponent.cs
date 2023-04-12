using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class SlideToPointUIPanelComponent : MonoBehaviour
{
	private void Start()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		this.defPos = this.rectTransform.anchoredPosition;
		this.ClosePanel();
	}

	public void SwitchState()
	{
		if (this.isOpened)
		{
			this.ClosePanel();
			return;
		}
		this.OpenPanel();
	}

	public void OpenPanel()
	{
		this.isOpened = true;
		this.MovePanel(new Vector3(this.targetPoint.anchoredPosition.x, this.targetPoint.anchoredPosition.y), this.openSpeed, this.openEase);
	}

	public void ClosePanel()
	{
		this.isOpened = false;
		this.MovePanel(new Vector3(this.defPos.x, this.defPos.y), this.closeSpeed, this.closeEase);
	}

	private void MovePanel(Vector3 pos, float speed, Ease ease)
	{
		this.rectTransform.DOAnchorPos(pos, speed, false).SetEase(ease);
	}

	[SerializeField]
	private float openSpeed;

	[SerializeField]
	private float closeSpeed;

	[SerializeField]
	private Ease openEase;

	[SerializeField]
	private Ease closeEase;

	private bool isOpened;

	[Tooltip(" Точка для состояния открытия ")]
	[SerializeField]
	private RectTransform targetPoint;

	private Vector3 defPos;

	private RectTransform rectTransform;
}
