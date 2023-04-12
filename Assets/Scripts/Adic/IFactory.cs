using System;
using Adic.Injection;

namespace Adic
{
	public interface IFactory
	{
		object Create(InjectionContext context);
	}
}
