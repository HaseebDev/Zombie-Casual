using System;
using UnityEngine;

public abstract class CountdownEffect : MonoBehaviour
{
	public void StartEffect(float _time)
	{
		this.effectTime = _time;
		if (_time > this.countdownTime)
		{
			this.countdownTime = _time;
		}
		if (!this.countdownIsStarted)
		{
			this.ApplyEffect();
			this.countdownIsStarted = true;
		}
	}

	protected virtual void Start()
	{
	}

	public abstract void ApplyEffect();

	public abstract void ResetEffect();

	public void CountDownEnd()
	{
		this.countdownIsStarted = false;
		this.ResetEffect();
	}

	private void Update()
	{
		if (this.countdownIsStarted)
		{
			this.countdownTime -= Time.deltaTime;
			if (this.countdownTime <= 0f)
			{
				this.CountDownEnd();
			}
		}
	}

	protected bool countdownIsStarted;

	public float countdownTime;

	protected float effectTime;
}
