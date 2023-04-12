using System;
using UnityEngine;

public class ZMKillRewardKUData : CountdownEffect, IUpgradableFloatValue
{
	public float GetUpgradableZMRewardK
	{
		get
		{
			return this.zMRewardK;
		}
	}

	public override void ApplyEffect()
	{
		this.doubleReward = 2;
	}

	public override void ResetEffect()
	{
		this.doubleReward = 1;
	}

	public void SetUpgradableFloatValue(float _value)
	{
		this.zMRewardK = _value * (float)this.doubleReward;
	}

	[SerializeField]
	private float zMRewardK = 1f;

	private int doubleReward = 1;
}
