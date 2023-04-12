using System;
using UnityEngine;

namespace Adic.Commander.Behaviours
{
	[AddComponentMenu("")]
	public abstract class NamespaceCommandBehaviour : MonoBehaviour
	{
		public string commandNamespace;

		public string commandName;
	}
}
