using System;
using UnityEngine;

public abstract class MoveEffect : MonoBehaviour
{
	protected virtual void Start()
	{
		if (this.playOnStart)
		{
			this.StartEffect();
		}
	}

	public virtual void StartEffect()
	{
		this.effectIsOn = true;
	}

	public abstract void Reset();

	public virtual void StopEffect()
	{
		this.effectIsOn = false;
	}

	[SerializeField]
	private bool playOnStart;

	protected bool effectIsOn;
}
