using System;

namespace Adic
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	[Obsolete("As of version 2.20, please use Inject attribute instead.")]
	public class PostConstruct : Attribute
	{
	}
}
