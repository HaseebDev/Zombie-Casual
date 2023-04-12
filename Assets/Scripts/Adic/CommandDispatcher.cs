using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adic.Commander.Exceptions;
using Adic.Container;
using UnityEngine;

namespace Adic
{
	public class CommandDispatcher : IDisposable, ICommandDispatcher, ICommandPool
	{
		public CommandDispatcher(IInjectionContainer container)
		{
			this.commands = new Dictionary<Type, object>();
			this.container = container;
			this.commandsToRegister = new List<Type>();
			this.eventCallerExtension = this.container.GetExtension<EventCallerContainerExtension>();
			if (this.eventCallerExtension == null)
			{
				this.container.RegisterExtension<EventCallerContainerExtension>();
				this.eventCallerExtension = this.container.GetExtension<EventCallerContainerExtension>();
			}
		}

		public void Init()
		{
			foreach (Type commandType in this.commandsToRegister)
			{
				this.RegisterCommand(commandType);
			}
		}

		public DispatcherOptions Dispatch<T>(params object[] parameters) where T : ICommand
		{
			return this.Dispatch(typeof(T), parameters);
		}

		public DispatcherOptions Dispatch(Type type, params object[] parameters)
		{
			DispatcherOptions dispatcherOptions = new DispatcherOptions(this);
			this.Dispatch(type, dispatcherOptions, parameters);
			return dispatcherOptions;
		}

		private void Dispatch(Type type, DispatcherOptions options, params object[] parameters)
		{
			if (!this.ContainsRegistration(type))
			{
				throw new CommandException(string.Format("There is no command registered for the type {0}.", type));
			}
			object obj = this.commands[type];
			ICommand command;
			if (obj is ICommand)
			{
				command = (ICommand)obj;
			}
			else
			{
				command = this.GetCommandFromPool(type, (List<ICommand>)obj);
				this.container.Inject<ICommand>(command);
			}
			command.dispatcher = this;
			command.running = true;
			command.Execute(parameters);
			options.command = command;
			if (!command.keepAlive)
			{
				this.Release(command);
				return;
			}
			if (command is IUpdatable && !this.eventCallerExtension.updateable.Contains((IUpdatable)command))
			{
				this.eventCallerExtension.updateable.Add((IUpdatable)command);
				return;
			}
		}

		public DispatcherOptions InvokeDispatch<T>(float time, params object[] parameters) where T : ICommand
		{
			return this.InvokeDispatch(typeof(T), time, parameters);
		}

		public DispatcherOptions InvokeDispatch(Type type, float time, params object[] parameters)
		{
			DispatcherOptions dispatcherOptions = new DispatcherOptions(this);
			this.StartCoroutine(this.DispatchByTimer(type, dispatcherOptions, time, parameters));
			return dispatcherOptions;
		}

		private IEnumerator DispatchByTimer(Type type, DispatcherOptions options, float time, params object[] parameters)
		{
			yield return new WaitForSeconds(time);
			this.Dispatch(type, options, parameters);
			yield break;
		}

		public ICommandDispatcher Release(ICommand command)
		{
			if (!command.running)
			{
				return this;
			}
			if (command is IUpdatable && this.eventCallerExtension.updateable.Contains((IUpdatable)command))
			{
				this.eventCallerExtension.updateable.Remove((IUpdatable)command);
			}
			if (command is IDisposable)
			{
				((IDisposable)command).Dispose();
			}
			command.running = false;
			command.keepAlive = false;
			return this;
		}

		public ICommandDispatcher ReleaseAll()
		{
			foreach (KeyValuePair<Type, object> keyValuePair in this.commands)
			{
				if (keyValuePair.Value is ICommand)
				{
					this.Release((ICommand)keyValuePair.Value);
				}
				else
				{
					List<ICommand> list = (List<ICommand>)keyValuePair.Value;
					for (int i = 0; i < list.Count; i++)
					{
						this.Release(list[i]);
					}
				}
			}
			return this;
		}

		public ICommandDispatcher ReleaseAll<T>() where T : ICommand
		{
			return this.ReleaseAll(typeof(T));
		}

		public ICommandDispatcher ReleaseAll(Type type)
		{
			foreach (KeyValuePair<Type, object> keyValuePair in this.commands)
			{
				if (keyValuePair.Value is ICommand && keyValuePair.Value.GetType().Equals(type))
				{
					this.Release((ICommand)keyValuePair.Value);
				}
				else if (keyValuePair.Value is List<ICommand>)
				{
					List<ICommand> list = (List<ICommand>)keyValuePair.Value;
					for (int i = 0; i < list.Count; i++)
					{
						ICommand command = list[i];
						if (command.GetType().Equals(type))
						{
							this.Release(command);
						}
					}
				}
			}
			return this;
		}

		public ICommandDispatcher ReleaseAll(string tag)
		{
			foreach (KeyValuePair<Type, object> keyValuePair in this.commands)
			{
				if (keyValuePair.Value is ICommand && tag.Equals(((ICommand)keyValuePair.Value).tag))
				{
					this.Release((ICommand)keyValuePair.Value);
				}
				else if (keyValuePair.Value is List<ICommand>)
				{
					List<ICommand> list = (List<ICommand>)keyValuePair.Value;
					for (int i = 0; i < list.Count; i++)
					{
						ICommand command = list[i];
						if (tag.Equals(command.tag))
						{
							this.Release(command);
						}
					}
				}
			}
			return this;
		}

		public bool ContainsRegistration<T>() where T : ICommand
		{
			return this.commands.ContainsKey(typeof(T));
		}

		public bool ContainsRegistration(Type type)
		{
			return this.commands.ContainsKey(type);
		}

		public Type[] GetAllRegistrations()
		{
			return this.commands.Keys.ToArray<Type>();
		}

		public Coroutine StartCoroutine(IEnumerator routine)
		{
			return this.eventCallerExtension.behaviour.StartCoroutine(routine);
		}

		public void StopCoroutine(Coroutine coroutine)
		{
			this.eventCallerExtension.behaviour.StopCoroutine(coroutine);
		}

		public void AddCommand(Type type)
		{
			this.commandsToRegister.Add(type);
		}

		public void PoolCommand(Type commandType)
		{
			ICommand command = (ICommand)this.container.Resolve(commandType);
			if (this.commands.ContainsKey(commandType))
			{
				return;
			}
			if (command.singleton)
			{
				this.commands.Add(commandType, command);
				return;
			}
			List<ICommand> list = new List<ICommand>(command.preloadPoolSize);
			list.Add(command);
			if (command.preloadPoolSize > 1)
			{
				for (int i = 1; i < command.preloadPoolSize; i++)
				{
					list.Add((ICommand)this.container.Resolve(commandType));
				}
			}
			this.commands.Add(commandType, list);
		}

		public ICommand GetCommandFromPool(Type commandType)
		{
			ICommand result = null;
			if (this.commands.ContainsKey(commandType))
			{
				object obj = this.commands[commandType];
				result = this.GetCommandFromPool(commandType, (List<ICommand>)obj);
			}
			return result;
		}

		public ICommand GetCommandFromPool(Type commandType, List<ICommand> pool)
		{
			ICommand command = null;
			for (int i = 0; i < pool.Count; i++)
			{
				ICommand command2 = pool[i];
				if (!command2.running)
				{
					command = command2;
					break;
				}
			}
			if (command == null)
			{
				if (pool.Count > 0 && pool.Count >= pool[0].maxPoolSize)
				{
					throw new CommandException(string.Format("Reached max pool size for command {0}.", pool[0].ToString()));
				}
				command = (ICommand)this.container.Resolve(commandType);
				pool.Add(command);
			}
			return command;
		}

		public void Dispose()
		{
			this.ReleaseAll();
			this.commands.Clear();
		}

		private void RegisterCommand(Type commandType)
		{
			if (!commandType.IsClass && commandType.IsAssignableFrom(typeof(ICommand)))
			{
				throw new CommandException("The type is not a command.");
			}
			if (!commandType.IsAbstract)
			{
				this.container.Bind<ICommand>().To(commandType);
				this.PoolCommand(commandType);
			}
		}

		protected Dictionary<Type, object> commands;

		protected IInjectionContainer container;

		protected EventCallerContainerExtension eventCallerExtension;

		protected IList<Type> commandsToRegister;
	}
}
