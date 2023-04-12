using System;

namespace Adic
{
	public interface ICommandPool
	{
		void AddCommand(Type type);

		void PoolCommand(Type commandType);

		ICommand GetCommandFromPool(Type commandType);
	}
}
