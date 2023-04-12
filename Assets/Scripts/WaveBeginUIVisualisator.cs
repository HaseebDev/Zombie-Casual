using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class WaveBeginUIVisualisator : MonoBehaviour
{
	public void Show()
	{
		base.transform.localScale = Vector3.zero;
		base.transform.DOScale(1.1f, 0.5f).SetEase(Ease.OutElastic, 1f, 3f).OnComplete(delegate
		{
		});
		base.gameObject.SetActive(true);
		base.Invoke("Hide", this.waveShowTime);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	[SerializeField]
	private float waveShowTime;
}
