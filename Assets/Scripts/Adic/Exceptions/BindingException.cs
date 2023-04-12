using System;

namespace Adic.Exceptions
{
	public class BindingException : Exception
	{
		public BindingException(string message) : base(message)
		{
		}

		public const string TYPE_NOT_ASSIGNABLE = "The related type is not assignable from the source type.";

		public const string TYPE_NOT_FACTORY = "The type doesn't implement Adic.IFactory.";

		public const string INSTANCE_NOT_ASSIGNABLE = "The instance is not of the given type.";
	}
}
