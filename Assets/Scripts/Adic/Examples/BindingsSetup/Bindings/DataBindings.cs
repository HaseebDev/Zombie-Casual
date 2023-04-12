using System;
using Adic.Container;
using Adic.Examples.BindingsSetup.Data;
using Adic.Injection;
using UnityEngine;

namespace Adic.Examples.BindingsSetup.Bindings
{
	[BindingPriority]
	public class DataBindings : IBindingsSetup
	{
		public void SetupBindings(IInjectionContainer container)
		{
			container.Bind<CubeRotationSpeed>().To<CubeRotationSpeed>(new CubeRotationSpeed(0.5f)).When((InjectionContext context) => context.parentInstance is MonoBehaviour && ((MonoBehaviour)context.parentInstance).name.Contains("CubeA")).Bind<CubeRotationSpeed>().To<CubeRotationSpeed>(new CubeRotationSpeed(2f)).When((InjectionContext context) => context.parentInstance is MonoBehaviour && ((MonoBehaviour)context.parentInstance).name.Contains("CubeB")).Bind<CubeRotationSpeed>().To<CubeRotationSpeed>(new CubeRotationSpeed(4.5f)).When((InjectionContext context) => context.parentInstance is MonoBehaviour && ((MonoBehaviour)context.parentInstance).name.Contains("CubeC"));
		}
	}
}
