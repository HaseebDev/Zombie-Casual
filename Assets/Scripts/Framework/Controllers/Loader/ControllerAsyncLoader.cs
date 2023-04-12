using System;
using System.Collections;
using Adic;
using Framework.Common;
using Framework.Interfaces;
using Framework.Managers.Event;
using Framework.Views;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Controllers.Loader
{
	public class ControllerAsyncLoader : BaseBehavior, ISceneLoader, IDisposable
	{
		protected override void Start()
		{
			base.Start();
			this.loadingProgressListener = this.loadingView.GetComponent<ILoadingProgressListener>();
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
			base.StartCoroutine(this.LoadAsync(this.nameOfTheLoadedScene));
		}

		private IEnumerator LoadAsync(string sceneName)
		{
			UnityEngine.Debug.Log("Загрузка началась");
			Time.timeScale = 1f;
			AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, this.loadSceneMode);
			while (!operation.isDone)
			{
				float loadingProgress = Mathf.Clamp01(operation.progress / 0.9f);
				if (this.loadingProgressListener != null)
				{
					this.loadingProgressListener.SetLoadingProgress(loadingProgress);
				}
				yield return null;
			}
			UnityEngine.Debug.Log("Загрузка завершилась");
		

            EventSystemServiceStatic.DispatchAll(EVENT_NAME.LOADING_FINISHED);
            this.loadingView.Unload();
			yield break;
		}

		public void Dispose()
		{
			this.loadingView = null;
		}

		[Inject]
		protected LoaderView loadingView;

		private ILoadingProgressListener loadingProgressListener;

		private string nameOfTheLoadedScene;

		protected bool waitingToStartLoading;

		private LoadSceneMode loadSceneMode;
	}
}
