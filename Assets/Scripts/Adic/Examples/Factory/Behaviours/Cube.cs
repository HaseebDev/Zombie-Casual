using System;
using UnityEngine;

namespace Adic.Examples.Factory.Behaviours
{
	[RequireComponent(typeof(Renderer))]
	public class Cube : MonoBehaviour
	{
		public Color color
		{
			get
			{
				return base.GetComponent<Renderer>().material.color;
			}
			set
			{
				Material material = new Material(Shader.Find("Standard"))
				{
					color = value
				};
				base.GetComponent<Renderer>().material = material;
			}
		}
	}
}
