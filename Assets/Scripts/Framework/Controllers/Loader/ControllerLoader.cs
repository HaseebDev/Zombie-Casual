using System;
using Adic;
using Framework.Common;
using Framework.Managers.Event;
using Framework.Views;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Controllers.Loader
{
	public class ControllerLoader : BaseBehavior, ISceneLoader, IDisposable
	{
		protected override void Start()
		{
			this.Inject();
			base.Start();
		}

		public virtual void StartLoad(string _nameOfTheLoadedScene, LoadSceneMode _loadSceneMode)
		{
			this.loadingView.Load();
			Time.timeScale = 0f;
			this.loadSceneMode = _loadSceneMode;
			this.nameOfTheLoadedScene = _nameOfTheLoadedScene;
			if (!this.waitingToStartLoading)
			{
				this.LoadNextLevel();
			}
		}

		protected virtual void LoadNextLevel()
		{
			UnityEngine.Debug.Log("Загрузка началась");
			Time.timeScale = 1f;
			SceneManager.LoadScene(this.nameOfTheLoadedScene, this.loadSceneMode);
			this.loadingView.Unload();
			UnityEngine.Debug.Log("Загрузка завершилась");
			
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.LOADING_FINISHED);
        }

		public void Dispose()
		{
			this.loadingView = null;
		}

		[Inject]
		protected LoaderView loadingView;

		private string nameOfTheLoadedScene;

		protected bool waitingToStartLoading;

		private LoadSceneMode loadSceneMode;
	}
}
