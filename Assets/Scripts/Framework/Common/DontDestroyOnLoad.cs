using System;
using UnityEngine;

namespace Framework.Common
{
	public class DontDestroyOnLoad : MonoBehaviour
	{
		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (this.destroyDuplicates && UnityEngine.Object.FindObjectsOfType(base.GetType()).Length > 1)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		[SerializeField]
		private bool destroyDuplicates = true;
	}
}
