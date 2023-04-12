using System;
using Adic;
using Framework.Common;
using Framework.Interfaces;
using UnityEngine;

public class SmoothRotation : BaseBehavior, ITick
{
	private new void Start()
	{
		base.Start();
		this.updateManager.AddTo(this);
	}

	public void Tick()
	{
		base.CachedTransform.Rotate(this.rotationVector, Time.deltaTime);
	}

	private void OnDestroy()
	{
		this.updateManager.RemoveFrom(this);
		this.updateManager = null;
	}

	[SerializeField]
	private Vector3 rotationVector;

	[Inject]
	private UpdateManager updateManager;
}
