using System;
using UnityEngine;

public class PositionPingPongChanger : MoveEffect
{
	private void ChangeDirection()
	{
		if (this.direction == 1)
		{
			this.direction = -1;
			return;
		}
		this.direction = 1;
	}

	public override void Reset()
	{
		base.transform.position = this.starPos;
	}

	public override void StartEffect()
	{
		base.StartEffect();
		this.starPos = base.transform.position;
		this.minPos = base.transform.position + this.minPosOffset;
		this.maxPos = base.transform.position + this.maxPosOffset;
	}

	private void Update()
	{
		if (this.effectIsOn)
		{
			if (this.minPosOffset.x != 0f || this.maxPosOffset.x != 0f)
			{
				if (base.transform.position.x <= this.minPos.x)
				{
					this.ChangeDirection();
				}
				else if (base.transform.position.x >= this.maxPos.x)
				{
					this.ChangeDirection();
				}
			}
			if (this.minPosOffset.y != 0f || this.maxPosOffset.y != 0f)
			{
				if (base.transform.position.y <= this.minPos.y)
				{
					this.ChangeDirection();
				}
				else if (base.transform.position.y >= this.maxPos.y)
				{
					this.ChangeDirection();
				}
			}
			if (this.minPosOffset.z != 0f || this.maxPosOffset.z != 0f)
			{
				if (base.transform.position.z <= this.minPos.z)
				{
					this.ChangeDirection();
				}
				else if (base.transform.position.z >= this.maxPos.z)
				{
					this.ChangeDirection();
				}
			}
			base.transform.Translate(0f, this.moveSpeed * (float)this.direction * Time.smoothDeltaTime, 0f);
		}
	}

	private Vector3 starPos;

	[SerializeField]
	private Vector3 minPosOffset;

	[SerializeField]
	private Vector3 maxPosOffset;

	private Vector3 minPos;

	private Vector3 maxPos;

	public float moveSpeed;

	private int direction = 1;
}
