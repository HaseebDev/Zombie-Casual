using System;
using UnityEngine.SceneManagement;

namespace Framework.Controllers.Loader
{
	public interface ISceneLoader
	{
		void StartLoad(string _nameOfTheLoadedScene, LoadSceneMode loadSceneMode);
	}
}
