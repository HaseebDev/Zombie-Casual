using System;
using UnityEngine;

namespace Adic
{
	public class PrefabBinding
	{
		public UnityEngine.Object prefab { get; private set; }

		public Type type { get; set; }

		public PrefabBinding(UnityEngine.Object prefab, Type type)
		{
			this.prefab = prefab;
			this.type = type;
		}
	}
}
