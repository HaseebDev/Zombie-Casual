using System;
using UnityEngine;

public class DestroyAfterFewSecs : MonoBehaviour
{
	private void Update()
	{
		this.timeToDestroy -= Time.deltaTime;
		if (this.timeToDestroy <= 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	[SerializeField]
	private float timeToDestroy;
}
