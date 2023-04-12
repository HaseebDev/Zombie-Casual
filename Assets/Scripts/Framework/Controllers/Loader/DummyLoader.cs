using System;
using Framework.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Controllers.Loader
{
	public class DummyLoader : BaseBehavior, ISceneLoader
	{
		public virtual void StartLoad(string _nameOfTheLoadedScene, LoadSceneMode _loadSceneMode)
		{
			UnityEngine.Debug.Log("Загрузка началась");
			SceneManager.LoadScene(_nameOfTheLoadedScene, _loadSceneMode);
			UnityEngine.Debug.Log("Загрузка завершилась");
		}
	}
}
