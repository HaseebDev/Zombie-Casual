using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AttackStatusPanelView : MonoBehaviour
{
	public void Show()
	{
		this.img1.DOFade(1f, 0.5f);
		this.img2.DOFade(1f, 0.5f);
		this.img3.DOFade(1f, 0.5f);
		this.img4.DOFade(1f, 0.5f);
	}

	public void Hide()
	{
		this.img1.DOFade(0f, 0.5f);
		this.img2.DOFade(0f, 0.5f);
		this.img3.DOFade(0f, 0.5f);
		this.img4.DOFade(0f, 0.5f);
	}

	[SerializeField]
	private Image img1;

	[SerializeField]
	private Image img2;

	[SerializeField]
	private Image img3;

	[SerializeField]
	private Image img4;
}
