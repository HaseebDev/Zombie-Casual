using System;
using Adic.Container;
using UnityEngine;

namespace Adic.Examples.Factory.Bindings
{
	public class PrefabsBindings : IBindingsSetup
	{
		public void SetupBindings(IInjectionContainer container)
		{
			container.Bind<GameObject>().ToFactory<CubeFactory>();
		}
	}
}
