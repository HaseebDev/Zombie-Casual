using System;
using Framework.Interfaces;
using UnityEngine;

public class GamePlayView : MonoBehaviour, ITickLate
{
	public void OnTouchMaskPress()
	{
		this.currentStepPos = UnityEngine.Input.mousePosition;
	}

	private void CreateIndi(Vector3 _cords)
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint(_cords);
		pos.z = 1f;
		this.tapEffectPoolManager.CreatePoolObjectOnPos(pos);
	}

	public void OnTouchMaskDown()
	{
		if (Vector3.Distance(this.currentStepPos, UnityEngine.Input.mousePosition) > this.tapDetectDistance)
		{
			this.currentStepPos = UnityEngine.Input.mousePosition;
		}
	}

	public void TickLate()
	{
	}

	[SerializeField]
	private float tapDetectDistance;

	private Vector3 currentStepPos;

	[SerializeField]
	private PoolObjectManager tapEffectPoolManager;

	public bool OnTap;
}
