using System;
using System.Collections.Generic;

namespace Adic.Binding
{
	public interface IBinder : IBindingCreator
	{
		event BindingAddedHandler beforeAddBinding;

		event BindingAddedHandler afterAddBinding;

		event BindingRemovedHandler beforeRemoveBinding;

		event BindingRemovedHandler afterRemoveBinding;

		void AddBinding(BindingInfo binding);

		IList<BindingInfo> GetBindings();

		IList<BindingInfo> GetBindingsFor<T>();

		IList<BindingInfo> GetBindingsFor(Type type);

		IList<BindingInfo> GetBindingsFor(object identifier);

		IList<BindingInfo> GetBindingsTo<T>();

		IList<BindingInfo> GetBindingsTo(Type type);

		bool ContainsBindingFor<T>();

		bool ContainsBindingFor(Type type);

		bool ContainsBindingFor(object identifier);

		void Unbind<T>();

		void Unbind(Type type);

		void Unbind(object identifier);

		void UnbindInstance(object instance);

		void UnbindByTag(string tag);
	}
}
