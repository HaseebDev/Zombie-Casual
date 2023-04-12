using System;
using Adic.Commander;
using Adic.Util;

namespace Adic
{
	[Serializable]
	public class CommandReference
	{
		public void DispatchCommand(params object[] parameters)
		{
			CommanderUtils.DispatchCommand(TypeUtils.GetType(this.commandNamespace, this.commandName), parameters);
		}

		public string commandNamespace;

		public string commandName;
	}
}
