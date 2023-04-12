using System;
using UnityEngine;

public class UnityFrameAnimationController : AnimationController
{
	public override void SetSpeed(float speed)
	{
		this.animator.speed = speed;
	}

	[SerializeField]
	private Animator animator;
}
