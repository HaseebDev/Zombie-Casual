using System;
using System.Collections;
using UnityEngine;

namespace Adic
{
	public interface ICommandDispatcher
	{
		void Init();

		DispatcherOptions Dispatch<T>(params object[] parameters) where T : ICommand;

		DispatcherOptions Dispatch(Type type, params object[] parameters);

		DispatcherOptions InvokeDispatch<T>(float time, params object[] parameters) where T : ICommand;

		DispatcherOptions InvokeDispatch(Type type, float time, params object[] parameters);

		ICommandDispatcher Release(ICommand command);

		ICommandDispatcher ReleaseAll();

		ICommandDispatcher ReleaseAll<T>() where T : ICommand;

		ICommandDispatcher ReleaseAll(Type type);

		ICommandDispatcher ReleaseAll(string tag);

		bool ContainsRegistration<T>() where T : ICommand;

		bool ContainsRegistration(Type type);

		Type[] GetAllRegistrations();

		Coroutine StartCoroutine(IEnumerator routine);

		void StopCoroutine(Coroutine coroutine);
	}
}
