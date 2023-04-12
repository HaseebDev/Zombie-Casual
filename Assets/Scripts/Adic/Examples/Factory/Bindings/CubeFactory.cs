using System;
using Adic.Container;
using Adic.Examples.Factory.Behaviours;
using Adic.Injection;
using UnityEngine;

namespace Adic.Examples.Factory.Bindings
{
	public class CubeFactory : IFactory
	{
		public CubeFactory(IInjectionContainer container)
		{
			this.container = container;
			this.container.Bind<Cube>().ToPrefab("07_Factory/Cube");
		}

		public object Create(InjectionContext context)
		{
			Cube cube = this.container.Resolve<Cube>();
			cube.gameObject.AddComponent<Rotator>().speed = UnityEngine.Random.Range(0.05f, 5f);
			cube.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
			Transform component = cube.GetComponent<Transform>();
			float num = 1.5f;
			int num2 = this.currentColumn;
			this.currentColumn = num2 + 1;
			component.position = new Vector3(num * (float)num2, -1.5f * (float)this.currentLine, 0f);
			if (this.currentColumn >= 6)
			{
				this.currentLine++;
				this.currentColumn = 0;
			}
			return cube.gameObject;
		}

		protected const int MAX_COLUMNS = 6;

		protected IInjectionContainer container;

		protected int currentLine;

		protected int currentColumn;
	}
}
