using System;

namespace Adic.Exceptions
{
	public class InjectorException : Exception
	{
		public InjectorException(string message) : base(message)
		{
		}

		public InjectorException(string message, Exception cause) : base(message, cause)
		{
		}

		public const string NO_CONSTRUCTORS = "There are no constructors on the type \"{0}\".";

		public const string CANNOT_INSTANTIATE_INTERFACE = "Interface \"{0}\" cannot be instantiated.";
	}
}
