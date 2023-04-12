using System;
using Framework.Utility;
using UnityEngine;

namespace Framework.Components
{
	[RequireComponent(typeof(CircleCollider2D))]
	public class Explosion : MonoBehaviour
	{
		public void Awake()
		{
			this.expRadius = base.GetComponent<CircleCollider2D>();
		}

		private void FixedUpdate()
		{
			if (this.exploded)
			{
				if (this.currentRadius < this.explosion_max_sixe)
				{
					this.currentRadius += this.explosion_rate;
				}
				else
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				this.expRadius.radius = this.currentRadius;
			}
		}

		public void Boom()
		{
			this.exploded = true;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (this.exploded)
			{
				Rigidbody2D component = collision.gameObject.GetComponent<Rigidbody2D>();
				if (component != null)
				{
					component.AddExplosionForce2D(this.explosion_force, base.transform.position, this.currentRadius);
				}
			}
		}

		[SerializeField]
		private float explosion_force;

		[SerializeField]
		private float explosion_rate;

		[SerializeField]
		private float explosion_max_sixe;

		[SerializeField]
		private float currentRadius;

		private bool exploded;

		private Rigidbody2D rb;

		private CircleCollider2D expRadius;
	}
}
