using System;

namespace Adic.Injection
{
	public class InjectionContext
	{
		public InjectionMember member;

		public Type memberType;

		public string memberName;

		public object identifier;

		public Type parentType;

		public object parentInstance;

		public Type injectType;
	}
}
