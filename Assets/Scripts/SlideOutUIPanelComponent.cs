using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class SlideOutUIPanelComponent : MonoBehaviour
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
		this.MovePanel(this.getPosVecor(), this.openSpeed, this.openEase);
	}

	public void ClosePanel()
	{
		this.isOpened = false;
		this.MovePanel(this.getPosVecor(), this.closeSpeed, this.closeEase);
	}

	private Vector3 getPosVecor()
	{
		float x = this.defPos.x;
		float y = this.defPos.y;
		if (!this.isOpened)
		{
			if (this.horizontalSide)
			{
				x = -this.defPos.x;
			}
			if (this.verticalSide)
			{
				y = -this.defPos.y;
			}
		}
		return new Vector3(x, y);
	}

	private void MovePanel(Vector3 pos, float speed, Ease ease)
	{
		this.rectTransform.DOAnchorPos(pos, speed, false).SetEase(ease);
	}

	[SerializeField]
	private bool horizontalSide;

	[SerializeField]
	private bool verticalSide;

	[SerializeField]
	private float openSpeed;

	[SerializeField]
	private float closeSpeed;

	[SerializeField]
	private Ease openEase;

	[SerializeField]
	private Ease closeEase;

	[SerializeField]
	private bool isOpened;

	[SerializeField]
	private Vector3 defPos;

	private RectTransform rectTransform;
}
