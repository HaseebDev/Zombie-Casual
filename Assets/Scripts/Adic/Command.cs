using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adic
{
	public abstract class Command : ICommand, IDisposable
	{
		public ICommandDispatcher dispatcher { get; set; }

		public string tag { get; set; }

		public bool running { get; set; }

		public bool keepAlive { get; set; }

		public virtual bool singleton
		{
			get
			{
				return true;
			}
		}

		public virtual int preloadPoolSize
		{
			get
			{
				return 1;
			}
		}

		public virtual int maxPoolSize
		{
			get
			{
				return 10;
			}
		}

		public abstract void Execute(params object[] parameters);

		public virtual void Retain()
		{
			this.keepAlive = true;
		}

		public virtual void Release()
		{
			this.keepAlive = false;
			this.dispatcher.Release(this);
		}

		public virtual void Dispose()
		{
			while (this.coroutines.Count > 0)
			{
				this.StopCoroutine(this.coroutines[0]);
			}
		}

		protected void Invoke(Action method, float time)
		{
			IEnumerator routine = this.MethodInvoke(method, time);
			this.StartCoroutine(routine);
		}

		protected Coroutine StartCoroutine(IEnumerator routine)
		{
			Coroutine coroutine = this.dispatcher.StartCoroutine(routine);
			this.coroutines.Add(coroutine);
			this.Retain();
			return coroutine;
		}

		protected void StopCoroutine(Coroutine coroutine)
		{
			this.dispatcher.StopCoroutine(coroutine);
			this.coroutines.Remove(coroutine);
		}

		private IEnumerator MethodInvoke(Action method, float time)
		{
			yield return new WaitForSeconds(time);
			method();
			yield break;
		}

		private List<Coroutine> coroutines = new List<Coroutine>(1);
	}
}
