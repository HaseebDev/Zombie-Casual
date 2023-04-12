using System;
using System.Collections;
using UnityEngine;

namespace Adic
{
	public class DispatcherOptions : ICommandDispatcher
	{
		public ICommand command
		{
			get
			{
				return this.internalCommand;
			}
			set
			{
				this.internalCommand = value;
				if (this.internalCommand != null)
				{
					this.ApplyTag(this.internalCommand);
				}
			}
		}

		public DispatcherOptions(ICommandDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}

		public ICommandDispatcher Tag(string tag)
		{
			this.tag = tag;
			if (this.command != null)
			{
				this.ApplyTag(this.command);
			}
			return this.dispatcher;
		}

		private void ApplyTag(ICommand commandToApply)
		{
			if (!string.IsNullOrEmpty(this.tag))
			{
				this.command.tag = this.tag;
			}
		}

		public void Init()
		{
			this.dispatcher.Init();
		}

		public DispatcherOptions Dispatch<T>(params object[] parameters) where T : ICommand
		{
			return this.dispatcher.Dispatch<T>(parameters);
		}

		public DispatcherOptions Dispatch(Type type, params object[] parameters)
		{
			return this.dispatcher.Dispatch(type, parameters);
		}

		public DispatcherOptions InvokeDispatch<T>(float time, params object[] parameters) where T : ICommand
		{
			return this.dispatcher.InvokeDispatch<T>(time, parameters);
		}

		public DispatcherOptions InvokeDispatch(Type type, float time, params object[] parameters)
		{
			return this.dispatcher.InvokeDispatch(type, time, parameters);
		}

		public ICommandDispatcher Release(ICommand command)
		{
			return this.dispatcher.Release(command);
		}

		public ICommandDispatcher ReleaseAll()
		{
			return this.dispatcher.ReleaseAll();
		}

		public ICommandDispatcher ReleaseAll<T>() where T : ICommand
		{
			return this.dispatcher.ReleaseAll<T>();
		}

		public ICommandDispatcher ReleaseAll(Type type)
		{
			return this.dispatcher.ReleaseAll(type);
		}

		public ICommandDispatcher ReleaseAll(string tag)
		{
			return this.dispatcher.ReleaseAll(tag);
		}

		public bool ContainsRegistration<T>() where T : ICommand
		{
			return this.dispatcher.ContainsRegistration<T>();
		}

		public bool ContainsRegistration(Type type)
		{
			return this.dispatcher.ContainsRegistration(type);
		}

		public Type[] GetAllRegistrations()
		{
			return this.dispatcher.GetAllRegistrations();
		}

		public Coroutine StartCoroutine(IEnumerator routine)
		{
			return this.dispatcher.StartCoroutine(routine);
		}

		public void StopCoroutine(Coroutine coroutine)
		{
			this.dispatcher.StopCoroutine(coroutine);
		}

		private ICommandDispatcher dispatcher;

		private string tag;

		private ICommand internalCommand;
	}
}
