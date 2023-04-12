using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.UI.Extensions
{
	public class SaveLoadMenu : MonoBehaviour
	{
		private void Start()
		{
			if (this.usePersistentDataPath)
			{
				this.savePath = Application.persistentDataPath + "/Saved Games/";
			}
			this.prefabDictionary = new Dictionary<string, GameObject>();
			foreach (GameObject gameObject in Resources.LoadAll<GameObject>(""))
			{
				if (gameObject.GetComponent<ObjectIdentifier>())
				{
					this.prefabDictionary.Add(gameObject.name, gameObject);
					UnityEngine.Debug.Log("Added GameObject to prefabDictionary: " + gameObject.name);
				}
			}
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				this.showMenu = !this.showMenu;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.F5))
			{
				this.SaveGame();
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.F9))
			{
				this.LoadGame();
			}
		}

		private void OnGUI()
		{
			if (this.showMenu)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Exit to Windows", Array.Empty<GUILayoutOption>()))
				{
					Application.Quit();
					return;
				}
				if (GUILayout.Button("Save Game", Array.Empty<GUILayoutOption>()))
				{
					this.SaveGame();
					return;
				}
				if (GUILayout.Button("Load Game", Array.Empty<GUILayoutOption>()))
				{
					this.LoadGame();
					return;
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
		}

		private IEnumerator wait(float time)
		{
			yield return new WaitForSeconds(time);
			yield break;
		}

		public void SaveGame()
		{
			this.SaveGame("QuickSave");
		}

		public void SaveGame(string saveGameName)
		{
			if (string.IsNullOrEmpty(saveGameName))
			{
				UnityEngine.Debug.Log("SaveGameName is null or empty!");
				return;
			}
			SaveLoad.saveGamePath = this.savePath;
			SaveGame saveGame = new SaveGame();
			saveGame.savegameName = saveGameName;
			List<GameObject> list = new List<GameObject>();
			foreach (ObjectIdentifier objectIdentifier in UnityEngine.Object.FindObjectsOfType(typeof(ObjectIdentifier)) as ObjectIdentifier[])
			{
				if (objectIdentifier.dontSave)
				{
					UnityEngine.Debug.Log("GameObject " + objectIdentifier.gameObject.name + " is set to dontSave = true, continuing loop.");
				}
				else
				{
					if (string.IsNullOrEmpty(objectIdentifier.id))
					{
						objectIdentifier.SetID();
					}
					list.Add(objectIdentifier.gameObject);
				}
			}
			foreach (GameObject gameObject in list)
			{
				gameObject.SendMessage("OnSerialize", SendMessageOptions.DontRequireReceiver);
			}
			foreach (GameObject go in list)
			{
				saveGame.sceneObjects.Add(this.PackGameObject(go));
			}
			SaveLoad.Save(saveGame);
		}

		public void LoadGame()
		{
			this.LoadGame("QuickSave");
		}

		public void LoadGame(string saveGameName)
		{
			this.ClearScene();
			SaveGame saveGame = SaveLoad.Load(saveGameName);
			if (saveGame == null)
			{
				UnityEngine.Debug.Log("saveGameName " + saveGameName + "couldn't be found!");
				return;
			}
			List<GameObject> list = new List<GameObject>();
			foreach (SceneObject sceneObject in saveGame.sceneObjects)
			{
				GameObject gameObject = this.UnpackGameObject(sceneObject);
				if (gameObject != null)
				{
					list.Add(gameObject);
				}
			}
			foreach (GameObject gameObject2 in list)
			{
				string idParent = gameObject2.GetComponent<ObjectIdentifier>().idParent;
				if (!string.IsNullOrEmpty(idParent))
				{
					foreach (GameObject gameObject3 in list)
					{
						if (gameObject3.GetComponent<ObjectIdentifier>().id == idParent)
						{
							gameObject2.transform.parent = gameObject3.transform;
						}
					}
				}
			}
			foreach (GameObject gameObject4 in list)
			{
				gameObject4.SendMessage("OnDeserialize", SendMessageOptions.DontRequireReceiver);
			}
		}

		public void ClearScene()
		{
			object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
			foreach (GameObject gameObject in array)
			{
				if (gameObject.CompareTag("DontDestroy"))
				{
					UnityEngine.Debug.Log("Keeping GameObject in the scene: " + gameObject.name);
				}
				else
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
		}

		public SceneObject PackGameObject(GameObject go)
		{
			ObjectIdentifier component = go.GetComponent<ObjectIdentifier>();
			SceneObject sceneObject = new SceneObject();
			sceneObject.name = go.name;
			sceneObject.prefabName = component.prefabName;
			sceneObject.id = component.id;
			if (go.transform.parent != null && go.transform.parent.GetComponent<ObjectIdentifier>())
			{
				sceneObject.idParent = go.transform.parent.GetComponent<ObjectIdentifier>().id;
			}
			else
			{
				sceneObject.idParent = null;
			}
			List<string> list = new List<string>
			{
				"UnityEngine.MonoBehaviour"
			};
			List<object> list2 = new List<object>();
			object[] array = go.GetComponents<Component>();
			foreach (object obj in array)
			{
				if (list.Contains(obj.GetType().BaseType.FullName))
				{
					list2.Add(obj);
				}
			}
			foreach (object component2 in list2)
			{
				sceneObject.objectComponents.Add(this.PackComponent(component2));
			}
			sceneObject.position = go.transform.position;
			sceneObject.localScale = go.transform.localScale;
			sceneObject.rotation = go.transform.rotation;
			sceneObject.active = go.activeSelf;
			return sceneObject;
		}

		public ObjectComponent PackComponent(object component)
		{
			ObjectComponent objectComponent = new ObjectComponent();
			objectComponent.fields = new Dictionary<string, object>();
			Type type = component.GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			objectComponent.componentName = type.ToString();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo != null && fieldInfo.FieldType.IsSerializable && ((!TypeSystem.IsEnumerableType(fieldInfo.FieldType) && !TypeSystem.IsCollectionType(fieldInfo.FieldType)) || TypeSystem.GetElementType(fieldInfo.FieldType).IsSerializable))
				{
					object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(DontSaveField), true);
					bool flag = false;
					object[] array2 = customAttributes;
					for (int j = 0; j < array2.Length; j++)
					{
						if (((Attribute)array2[j]).GetType() == typeof(DontSaveField))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						objectComponent.fields.Add(fieldInfo.Name, fieldInfo.GetValue(component));
					}
				}
			}
			return objectComponent;
		}

		public GameObject UnpackGameObject(SceneObject sceneObject)
		{
			if (!this.prefabDictionary.ContainsKey(sceneObject.prefabName))
			{
				UnityEngine.Debug.Log("Can't find key " + sceneObject.prefabName + " in SaveLoadMenu.prefabDictionary!");
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefabDictionary[sceneObject.prefabName], sceneObject.position, sceneObject.rotation);
			gameObject.name = sceneObject.name;
			gameObject.transform.localScale = sceneObject.localScale;
			gameObject.SetActive(sceneObject.active);
			if (!gameObject.GetComponent<ObjectIdentifier>())
			{
				gameObject.AddComponent<ObjectIdentifier>();
			}
			ObjectIdentifier component = gameObject.GetComponent<ObjectIdentifier>();
			component.id = sceneObject.id;
			component.idParent = sceneObject.idParent;
			this.UnpackComponents(ref gameObject, sceneObject);
			foreach (ObjectIdentifier objectIdentifier in gameObject.GetComponentsInChildren<ObjectIdentifier>())
			{
				if (objectIdentifier.transform != gameObject.transform && string.IsNullOrEmpty(objectIdentifier.id))
				{
					UnityEngine.Object.Destroy(objectIdentifier.gameObject);
				}
			}
			return gameObject;
		}

		public void UnpackComponents(ref GameObject go, SceneObject sceneObject)
		{
			foreach (ObjectComponent objectComponent in sceneObject.objectComponents)
			{
				if (!go.GetComponent(objectComponent.componentName))
				{
					Type type = Type.GetType(objectComponent.componentName);
					go.AddComponent(type);
				}
				object component = go.GetComponent(objectComponent.componentName);
				Type type2 = component.GetType();
				foreach (KeyValuePair<string, object> keyValuePair in objectComponent.fields)
				{
					FieldInfo field = type2.GetField(keyValuePair.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField);
					if (field != null)
					{
						object value = keyValuePair.Value;
						field.SetValue(component, value);
					}
				}
			}
		}

		public bool showMenu;

		public bool usePersistentDataPath = true;

		public string savePath;

		public Dictionary<string, GameObject> prefabDictionary;
	}
}
