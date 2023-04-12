using System;

namespace Adic.Injection
{
	public interface IInjector
	{
		event TypeResolutionHandler beforeResolve;

		event TypeResolutionHandler afterResolve;

		event BindingEvaluationHandler bindingEvaluation;

		event BindingResolutionHandler bindingResolution;

		event InstanceInjectionHandler beforeInject;

		event InstanceInjectionHandler afterInject;

		ResolutionMode resolutionMode { get; set; }

		T Resolve<T>();

		T Resolve<T>(object identifier);

		object Resolve(Type type);

		object Resolve(object identifier);

		object Resolve(Type type, object identifier);

		T[] ResolveAll<T>();

		T[] ResolveAll<T>(object identifier);

		object[] ResolveAll(Type type);

		object[] ResolveAll(object identifier);

		object[] ResolveAll(Type type, object identifier);

		T Inject<T>(T instance) where T : class;

		object Inject(object instance);
	}
}
