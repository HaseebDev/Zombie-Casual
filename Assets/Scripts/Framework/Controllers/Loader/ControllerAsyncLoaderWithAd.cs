using System;
using Adic;
using Framework.Interfaces;
using Framework.Managers.Ads;
using UnityEngine.SceneManagement;

namespace Framework.Controllers.Loader
{
	public class ControllerAsyncLoaderWithAd : ControllerLoader, IInterstitialAdListener
	{
		public override void StartLoad(string _nameOfTheLoadedScene, LoadSceneMode _loadSceneMode)
		{
			this.waitingToStartLoading = true;
			base.StartLoad(_nameOfTheLoadedScene, _loadSceneMode);
			this.adSystem.HideBanner();
			this.adSystem.SetInterstitialCallbacks(this);
			this.adSystem.ShowInterstetial(delegate(bool isLoaded)
			{
				if (isLoaded)
				{
					this.waitingToFinishAdLoading = true;
					return;
				}
				this.LoadNextLevel();
			});
		}

		public void OnInterstitialClicked()
		{
		}

		public void OnInterstitialClosed()
		{
			if (this.waitingToFinishAdLoading)
			{
				this.LoadNextLevel();
			}
		}

		public void OnInterstitialLoaded(bool isPrecache)
		{
		}

		public void OnInterstitialShown()
		{
			if (this.waitingToFinishAdLoading)
			{
				this.LoadNextLevel();
			}
		}

		protected override void LoadNextLevel()
		{
			this.waitingToFinishAdLoading = false;
			base.LoadNextLevel();
		}

		[Inject]
		private BaseAdSystem adSystem;

		private bool waitingToFinishAdLoading;
	}
}
