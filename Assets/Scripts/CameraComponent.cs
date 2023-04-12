using System;
using UnityEngine;

public class CameraComponent : MonoBehaviour
{
	public void PlayEffects()
	{
		for (int i = 0; i < this.effects.Length; i++)
		{
			this.effects[i].StartEffect();
		}
	}

	public void ResetEffects()
	{
		for (int i = 0; i < this.effects.Length; i++)
		{
			this.effects[i].Reset();
		}
	}

	public void StopEffects()
	{
		for (int i = 0; i < this.effects.Length; i++)
		{
			this.effects[i].StopEffect();
		}
	}

	[SerializeField]
	private MoveEffect[] effects;
}
