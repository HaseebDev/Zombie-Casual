using System;
using UnityEngine;

public class ScalePingPongHitEffect : MonoBehaviour, IHitEffect
{
	private void Start()
	{
		this.defScale = base.transform.localScale;
	}

	public void Hit()
	{
		if (!this.state)
		{
			base.transform.localScale += this.force;
		}
		else
		{
			base.transform.localScale -= this.force;
		}
		this.state = !this.state;
	}

	[SerializeField]
	private Vector3 force;

	private bool state;

	private Vector3 defScale;
}
