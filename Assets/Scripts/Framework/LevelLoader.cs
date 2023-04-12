using System;
using Adic;
using UnityEngine;

namespace Framework
{
    public class LevelLoader : MonoBehaviour
    {
        private void Awake()
        {
            this.levelFolder = GameObject.Find(this.levelFolderName).transform;
        }

        private void ClearLevelFolder()
        {
            foreach (object obj in this.levelFolder)
            {
                UnityEngine.Object.Destroy(((Transform)obj).gameObject);
            }
        }

        public void LoadLevel(int levelNumb)
        {
            //this.ClearLevelFolder();
            //GameLevel gameLevel = UnityEngine.Object.Instantiate<GameLevel>(Resources.Load<GameLevel>(string.Format("Levels/{0}", levelNumb)), this.levelFolder);
            //gameLevel.Initialize();

            //this.commandDispatcher.Dispatch<LevelLoadedCommand>(Array.Empty<object>());
        }

        private string levelFolderName = "[LEVELS]";

        private Transform levelFolder;

        [Inject]
        private ICommandDispatcher commandDispatcher;
    }
}
