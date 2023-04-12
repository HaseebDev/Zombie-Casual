using System;
using UnityEngine;

public class RotateAround : MoveEffect
{
	public override void Reset()
	{
		base.transform.position = this.startPos;
		base.transform.rotation = this.startRot;
	}

	public override void StartEffect()
	{
		base.StartEffect();
		this.startPos = base.transform.position;
		this.startRot = base.transform.rotation;
	}

	private void Update()
	{
		if (this.effectIsOn)
		{
			base.transform.LookAt(this.aroundPoint);
			base.transform.Translate(this.direction * Time.smoothDeltaTime * this.rotateSpeeed);
		}
	}

	[SerializeField]
	private float rotateSpeeed;

	[SerializeField]
	private Transform aroundPoint;

	[SerializeField]
	private Vector3 direction;

	private Vector3 startPos;

	private Quaternion startRot;
}
