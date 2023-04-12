using System;
using Framework.Interfaces;
using UnityEngine;

namespace Framework.Views
{
	public class LoaderView : MonoBehaviour, IState, ILoadingProgressListener
	{
		private void Start()
		{
		}

		public void Load()
		{
			base.gameObject.SetActive(true);
		}

		public void SetLoadingProgress(float _progress)
		{
			for (int i = 0; i < this.loadingIndicators.Length; i++)
			{
				this.loadingIndicators[i].SetNormalizedValue(_progress);
			}
		}

		public void Unload()
		{
			base.gameObject.SetActive(false);
		}

		[SerializeField]
		private BaseFloatValueView[] loadingIndicators;
	}
}
