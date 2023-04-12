using System;

namespace Adic.Commander.Exceptions
{
	public class CommandException : Exception
	{
		public CommandException(string message) : base(message)
		{
		}

		public const string TYPE_NOT_A_COMMAND = "The type is not a command.";

		public const string MAX_POOL_SIZE = "Reached max pool size for command {0}.";

		public const string NO_COMMAND_FOR_TYPE = "There is no command registered for the type {0}.";
	}
}
