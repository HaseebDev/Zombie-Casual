using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[Serializable]
	public class SaveGame
	{
		public SaveGame()
		{
		}

		public SaveGame(string s, List<SceneObject> list)
		{
			this.savegameName = s;
			this.sceneObjects = list;
		}

		public string savegameName = "New SaveGame";

		public List<SceneObject> sceneObjects = new List<SceneObject>();
	}
}
