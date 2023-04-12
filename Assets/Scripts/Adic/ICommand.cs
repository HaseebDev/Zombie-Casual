using System;

namespace Adic
{
	public interface ICommand
	{
		ICommandDispatcher dispatcher { get; set; }

		string tag { get; set; }

		bool running { get; set; }

		bool keepAlive { get; set; }

		bool singleton { get; }

		int preloadPoolSize { get; }

		int maxPoolSize { get; }

		void Execute(params object[] parameters);

		void Retain();

		void Release();
	}
}
