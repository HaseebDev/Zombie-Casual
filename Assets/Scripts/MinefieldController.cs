using System;
using UnityEngine;

public class MinefieldController : CountdownEffect
{
	public override void ApplyEffect()
	{
		for (int i = 0; i < this.mines.Length; i++)
		{
			this.mines[i].Activate();
		}
	}

	public override void ResetEffect()
	{
	}

	[SerializeField]
	private Mine[] mines;
}
