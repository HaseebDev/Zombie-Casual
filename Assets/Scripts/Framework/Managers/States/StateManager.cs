using System;
using Adic;
using Framework.Common;
using Framework.Controllers.Loader;
using Framework.Interfaces;
using UnityEngine.SceneManagement;

namespace Framework.Managers.States
{
	public class StateManager : BaseBehavior
	{
		public void LoadState(IState state)
		{
			state.Load();
		}

		public bool AdditionalScenesIsLoaded()
		{
			return SceneManager.sceneCount > 1;
		}

		public void UnloadCurrentScene()
		{
			SceneManager.UnloadSceneAsync(this.currentSceneName);
		}

		public void ReloadCurrentScene()
		{
			this.AddScene(this.currentSceneName, true);
		}

		public void AddScene(string sceneName, bool unloadCurrentScene = true)
		{
			if (unloadCurrentScene && this.currentSceneName != null)
			{
				this.UnloadCurrentScene();
			}
			this.currentSceneName = sceneName;
			this.loader.StartLoad(sceneName, LoadSceneMode.Additive);
		}

		public void ChangeScene(string sceneName)
		{
			this.currentSceneName = sceneName;
			this.loader.StartLoad(sceneName, LoadSceneMode.Single);
		}

		[Inject]
		private ISceneLoader loader;

		private string currentSceneName;
	}
}
