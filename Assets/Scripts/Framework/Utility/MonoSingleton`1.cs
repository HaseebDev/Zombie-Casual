using System;
using UnityEngine;

namespace Framework.Utility
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		public static T instance
		{
			get
			{
				if (MonoSingleton<T>.m_Instance == null)
				{
					MonoSingleton<T>.m_Instance = (UnityEngine.Object.FindObjectOfType(typeof(T)) as T);
					if (MonoSingleton<T>.m_Instance == null)
					{
						UnityEngine.Debug.LogWarning("No instance of " + typeof(T).ToString() + ", a temporary one is created.");
						MonoSingleton<T>.isTemporaryInstance = true;
						MonoSingleton<T>.m_Instance = new GameObject("Temp Instance of " + typeof(T).ToString(), new Type[]
						{
							typeof(T)
						}).GetComponent<T>();
						if (MonoSingleton<T>.m_Instance == null)
						{
							UnityEngine.Debug.LogError("Problem during the creation of " + typeof(T).ToString());
						}
					}
					if (!MonoSingleton<T>._isInitialized)
					{
						MonoSingleton<T>._isInitialized = true;
						MonoSingleton<T>.m_Instance.Init();
					}
				}
				return MonoSingleton<T>.m_Instance;
			}
		}

		public static bool isTemporaryInstance { get; private set; }

		private void Awake()
		{
			if (MonoSingleton<T>.m_Instance == null)
			{
				MonoSingleton<T>.m_Instance = (this as T);
			}
			else if (MonoSingleton<T>.m_Instance != this)
			{
				UnityEngine.Object.DestroyImmediate(this);
				return;
			}
			if (!MonoSingleton<T>._isInitialized)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				MonoSingleton<T>._isInitialized = true;
				MonoSingleton<T>.m_Instance.Init();
			}
		}

		public virtual void Init()
		{
		}

		private void OnApplicationQuit()
		{
			MonoSingleton<T>.m_Instance = default(T);
		}

		private static T m_Instance;

		private static bool _isInitialized;
	}
}
