using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Interfaces;
using UnityEngine;

namespace Framework.Utility
{
	public static class Extensions
	{
		public static void SetAlpha(this SpriteRenderer r, float alpha)
		{
			Color color = r.color;
			r.color = new Color(color.r, color.g, color.b, alpha);
		}

		public static Color SetAlpha(this SpriteRenderer r, Color c, float alpha)
		{
			return new Color(c.r, c.g, c.b, alpha);
		}

		public static void AddExplosionForce2D(this Rigidbody2D body, float expForce, Vector3 expPosition, float expRadius)
		{
			Vector3 vector = body.transform.position - expPosition;
			float num = 1f - vector.magnitude / expRadius;
			if (num <= 0f)
			{
				num = 0f;
			}
			body.AddForce(vector.normalized * expForce * num);
		}

		public static bool Every(this float step, float time)
		{
			return step % time == 0f;
		}

		public static bool Every(this int step, float time)
		{
			return (float)step % time == 0f;
		}

		public static float Plus(this float arg0, float val, float clamp = 1f)
		{
			return Math.Min(clamp, arg0 + val);
		}

		public static float Minus(this float arg0, float val, float clamp = 0f)
		{
			return Math.Max(clamp, arg0 - val);
		}

		public static int Plus(this int arg0, int val, int clamp = 1)
		{
			return Math.Min(clamp, arg0 + val);
		}

		public static int Minus(this int arg0, int val, int clamp = 0)
		{
			return Math.Max(clamp, arg0 - val);
		}

		public static float Between(this Vector2 v)
		{
			if (UnityEngine.Random.value <= 0.5f)
			{
				return v.y;
			}
			return v.x;
		}

		public static int Between(this object o, int a, int b, float chance = 0.5f)
		{
			if (UnityEngine.Random.value <= chance)
			{
				return b;
			}
			return a;
		}

		public static float Between(this object o, float a, float b, float chance = 0.5f)
		{
			if (UnityEngine.Random.value <= chance)
			{
				return b;
			}
			return a;
		}

		public static T RandomExcept<T>(this T[] list, T exceptVal)
		{
			T result = list[UnityEngine.Random.Range(0, list.Length)];
			while (result.Equals(exceptVal))
			{
				result = list[UnityEngine.Random.Range(0, list.Length)];
			}
			return result;
		}

		public static T RandomExcept<T>(this List<T> list, T exceptVal)
		{
			T result = list[UnityEngine.Random.Range(0, list.Count)];
			while (result.Equals(exceptVal))
			{
				result = list[UnityEngine.Random.Range(0, list.Count)];
			}
			return result;
		}

		public static T Random<T>(this List<T> list, T[] itemsToExclude)
		{
			T t = list[UnityEngine.Random.Range(0, list.Count)];
			while (itemsToExclude.Contains(t))
			{
				t = list[UnityEngine.Random.Range(0, list.Count)];
			}
			return t;
		}

		public static T Random<T>(this List<T> list)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public static T Select<T>(this List<T> vals) where T : IRandom
		{
			float num = 0f;
			float[] array = new float[vals.Count];
			for (int i = 0; i < array.Length; i++)
			{
				float[] array2 = array;
				int num2 = i;
				T t = vals[i];
				array2[num2] = t.returnChance;
				num += array[i];
			}
			float num3 = (float)Extensions._r.NextDouble() * num;
			for (int j = 0; j < array.Length; j++)
			{
				if (num3 < array[j])
				{
					return vals[j];
				}
				num3 -= array[j];
			}
			return vals[0];
		}

		public static T Select<T>(this T[] vals) where T : IRandom
		{
			float num = 0f;
			float[] array = new float[vals.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = vals[i].returnChance;
				num += array[i];
			}
			float num2 = (float)Extensions._r.NextDouble() * num;
			for (int j = 0; j < array.Length; j++)
			{
				if (num2 < array[j])
				{
					return vals[j];
				}
				num2 -= array[j];
			}
			return vals[0];
		}

		public static T Select<T>(this T[] vals, out int index) where T : IRandom
		{
			index = -1;
			if (vals == null || vals.Length == 0)
			{
				return default(T);
			}
			float num = 0f;
			float[] array = new float[vals.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = vals[i].returnChance;
				num += array[i];
			}
			float num2 = (float)Extensions._r.NextDouble() * num;
			for (int j = 0; j < array.Length; j++)
			{
				if (num2 < array[j])
				{
					index = j;
					return vals[j];
				}
				num2 -= array[j];
			}
			return vals[0];
		}

		public static T Random<T>(this T[] vals)
		{
			return vals[UnityEngine.Random.Range(0, vals.Length)];
		}

		public static T RandomDequeue<T>(this List<T> vals)
		{
			int index = UnityEngine.Random.Range(0, vals.Count);
			T result = vals[index];
			vals.RemoveAt(index);
			return result;
		}

		public static T AddGet<T>(this GameObject co) where T : Component
		{
			T t = co.GetComponent<T>();
			if (t == null)
			{
				t = co.AddComponent<T>();
			}
			return t;
		}

		public static T AddGet<T>(this Transform co) where T : Component
		{
			T t = co.GetComponent<T>();
			if (t == null)
			{
				t = co.gameObject.AddComponent<T>();
			}
			return t;
		}

		public static T Find<T>(this GameObject go, string path)
		{
			return go.transform.Find(path).GetComponent<T>();
		}

		public static Transform FindDeep(this Transform obj, string id)
		{
			if (obj.name == id)
			{
				return obj;
			}
			int childCount = obj.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform transform = obj.GetChild(i).FindDeep(id);
				if (transform != null)
				{
					return transform;
				}
			}
			return null;
		}

		public static List<T> GetAll<T>(this Transform obj)
		{
			List<T> list = new List<T>();
			obj.GetComponentsInChildren<T>(list);
			return list;
		}

		private static System.Random _r = new System.Random();
	}
}
