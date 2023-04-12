using System;
using UnityEngine;

public class PoolObjectWithLifeTime : PoolObject
{
	public override void OnAwake()
	{
		base.Done(this.lifeTime);
	}

	[SerializeField]
	private float lifeTime = 1f;
}
