using System;

namespace Adic.Exceptions
{
	public class BinderException : Exception
	{
		public BinderException(string message) : base(message)
		{
		}

		public const string NULL_BINDING = "There is no binding to be bound.";

		public const string BINDING_KEY_ALREADY_EXISTS = "There's already a binding with the same key.";

		public const string BINDING_TO_INTERFACE = "It's not possible to bind a key to an interface.";
	}
}
