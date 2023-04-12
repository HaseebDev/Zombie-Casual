using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	public static PoolManager instance
	{
		get
		{
			if (PoolManager._instance == null)
			{
				PoolManager._instance = UnityEngine.Object.FindObjectOfType<PoolManager>();
			}
			return PoolManager._instance;
		}
	}

	public void CreatePool(string poolKey, GameObject prefab, int poolSeedSize)
	{
		if (!this.poolPrefabs.ContainsKey(poolKey))
		{
			this.poolPrefabs.Add(poolKey, prefab);
			if (!prefab.GetComponent<PoolObject>())
			{
				prefab.AddComponent<PoolObject>();
			}
			prefab.GetComponent<PoolObject>().poolKey = poolKey;
			this.poolFreeDictionary.Add(poolKey, new Queue<PoolManager.ObjectInstance>());
			this.poolUsedDictionary.Add(poolKey, new Dictionary<int, PoolManager.ObjectInstance>());
			GameObject gameObject;
			if (this.poolParents.ContainsKey(poolKey))
			{
				gameObject = this.poolParents[poolKey];
			}
			else
			{
				gameObject = new GameObject(poolKey + " pool");
				gameObject.transform.parent = base.transform;
				this.poolParents.Add(poolKey, gameObject);
			}
			for (int i = 0; i < poolSeedSize; i++)
			{
				PoolManager.ObjectInstance objectInstance = new PoolManager.ObjectInstance(UnityEngine.Object.Instantiate<GameObject>(prefab));
				this.poolFreeDictionary[poolKey].Enqueue(objectInstance);
				objectInstance.SetParent(gameObject.transform);
			}
		}
	}

	public GameObject GetObject(string poolKey, Vector3 position, Quaternion rotation)
	{
		if (!this.poolPrefabs.ContainsKey(poolKey))
		{
			return null;
		}
		if (this.poolFreeDictionary[poolKey].Count > 0)
		{
			PoolManager.ObjectInstance objectInstance = this.poolFreeDictionary[poolKey].Dequeue();
			this.poolUsedDictionary[poolKey].Add(objectInstance.gameObject.GetInstanceID(), objectInstance);
			objectInstance.Awake(position, rotation);
			return objectInstance.gameObject;
		}
		PoolManager.ObjectInstance objectInstance2 = new PoolManager.ObjectInstance(UnityEngine.Object.Instantiate<GameObject>(this.poolPrefabs[poolKey]));
		this.poolUsedDictionary[poolKey].Add(objectInstance2.gameObject.GetInstanceID(), objectInstance2);
		objectInstance2.SetParent(this.poolParents[poolKey].transform);
		objectInstance2.Awake(position, rotation);
		return objectInstance2.gameObject;
	}

	public void ReturnObjectToQueue(GameObject gameObject)
	{
		if (gameObject.GetComponent<PoolObject>())
		{
			string poolKey = gameObject.GetComponent<PoolObject>().poolKey;
			gameObject.SetActive(false);
			if (!this.poolUsedDictionary.ContainsKey(poolKey) ||
               !this.poolUsedDictionary[poolKey].ContainsKey(gameObject.GetInstanceID()))
			{
				return;
			}
			PoolManager.ObjectInstance item = this.poolUsedDictionary[poolKey][gameObject.GetInstanceID()];
			this.poolUsedDictionary[poolKey].Remove(gameObject.GetInstanceID());
			this.poolFreeDictionary[poolKey].Enqueue(item);
		}
	}

	private Dictionary<string, GameObject> poolPrefabs = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> poolParents = new Dictionary<string, GameObject>();

	private Dictionary<string, Queue<PoolManager.ObjectInstance>> poolFreeDictionary = new Dictionary<string, Queue<PoolManager.ObjectInstance>>();

	private Dictionary<string, Dictionary<int, PoolManager.ObjectInstance>> poolUsedDictionary = new Dictionary<string, Dictionary<int, PoolManager.ObjectInstance>>();

	private static PoolManager _instance;

	private class ObjectInstance
	{
		public ObjectInstance(GameObject objectInstance)
		{
			this.gameObject = objectInstance;
			this.transform = this.gameObject.transform;
			this.gameObject.SetActive(false);
			this.poolObjectScript = this.gameObject.GetComponent<PoolObject>();
		}

		public void Awake(Vector3 position, Quaternion rotation)
		{
			this.gameObject.SetActive(true);
			this.transform.position = position;
			this.transform.rotation = rotation;
			this.poolObjectScript.OnAwake();
		}

		public void SetParent(Transform parent)
		{
			this.transform.SetParent(parent);
		}

		public GameObject gameObject;

		private Transform transform;

		private PoolObject poolObjectScript;
	}
}
