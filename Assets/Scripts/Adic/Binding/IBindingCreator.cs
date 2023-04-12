using System;

namespace Adic.Binding
{
	public interface IBindingCreator
	{
		IBindingFactory Bind<T>();

		IBindingFactory Bind(Type type);
	}
}
