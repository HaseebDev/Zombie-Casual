using System;
using Adic.Binding;
using Adic.Cache;
using Adic.Injection;

namespace Adic.Container
{
	public interface IInjectionContainer : IBinder, IBindingCreator, IInjector, IDisposable
	{
		object identifier { get; }

		IReflectionCache cache { get; }

		IInjectionContainer Init();

		IInjectionContainer RegisterExtension<T>() where T : IContainerExtension;

		IInjectionContainer UnregisterExtension<T>() where T : IContainerExtension;

		T GetExtension<T>() where T : IContainerExtension;

		IContainerExtension GetExtension(Type type);

		bool HasExtension<T>();

		bool HasExtension(Type type);

		void Clear();
	}
}
