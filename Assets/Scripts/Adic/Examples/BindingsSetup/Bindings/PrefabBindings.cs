using System;
using Adic.Container;
using UnityEngine;

namespace Adic.Examples.BindingsSetup.Bindings
{
	public class PrefabBindings : IBindingsSetup
	{
		public void SetupBindings(IInjectionContainer container)
		{
			container.Bind<Transform>().ToPrefabSingleton("06_BindingsSetup/CubeA").Bind<Transform>().ToPrefabSingleton("06_BindingsSetup/CubeB").Bind<Transform>().ToPrefabSingleton("06_BindingsSetup/CubeC");
		}
	}
}
