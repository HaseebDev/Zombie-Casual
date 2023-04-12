using System;
using Adic.Binding;

namespace Adic.Cache
{
	public interface IReflectionCache
	{
		ReflectedClass this[Type type]
		{
			get;
		}

		IReflectionFactory reflectionFactory { get; set; }

		void Add(Type type);

		void Remove(Type type);

		ReflectedClass GetClass(Type type);

		bool Contains(Type type);

		void CacheFromBinder(IBinder binder);
	}
}
