using System;
using Adic.Util;
using UnityEngine;

namespace Adic.Commander.Behaviours
{
	[AddComponentMenu("Adic/Commander/Command dispatch")]
	public class CommandDispatch : NamespaceCommandBehaviour
	{
		protected void Awake()
		{
			this.commandType = TypeUtils.GetType(this.commandNamespace, this.commandName);
		}

		public void DispatchCommand()
		{
			CommanderUtils.DispatchCommand(this.commandType, Array.Empty<object>());
		}

		public void DispatchCommand(params object[] parameters)
		{
			CommanderUtils.DispatchCommand(this.commandType, parameters);
		}

		protected Type commandType;
	}
}
