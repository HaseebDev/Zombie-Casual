using System;
using UnityEngine;

namespace Framework.Common
{
	public class BaseBehavior : MonoBehaviour
	{
		public Transform CachedTransform
		{
			get
			{
				if (!this._transformCached)
				{
					this._transformCached = true;
					this._transform = base.GetComponent<Transform>();
				}
				return this._transform;
			}
		}

		public GameObject CachedGameObject
		{
			get
			{
				if (!this._goCached)
				{
					this._goCached = true;
					this._gameobject = base.GetComponent<GameObject>();
				}
				return this._gameobject;
			}
		}

		public Rigidbody2D CachedRigidBody2D
		{
			get
			{
				if (!this._rb2dCached)
				{
					this._rb2dCached = true;
					this._rigidbody2D = base.GetComponent<Rigidbody2D>();
				}
				return this._rigidbody2D;
			}
		}

		public Rigidbody CachedRigidBody
		{
			get
			{
				if (!this._rbCached)
				{
					this._rbCached = true;
					this._rigidbody = base.GetComponent<Rigidbody>();
				}
				return this._rigidbody;
			}
		}

		public Collider CachedCollider
		{
			get
			{
				if (!this._colliderCached)
				{
					this._colliderCached = true;
					this._collider = base.GetComponent<Collider>();
				}
				return this._collider;
			}
		}

		public RectTransform CachedRectTransform
		{
			get
			{
				if (!this._recttransformCached)
				{
					this._recttransformCached = true;
					this._recttransform = base.GetComponent<RectTransform>();
				}
				return this._recttransform;
			}
		}

		public AudioSource CachedAudioSource
		{
			get
			{
				if (!this._audioSourceCached)
				{
					this._audioSourceCached = true;
					this._audioSource = base.GetComponent<AudioSource>();
				}
				return this._audioSource;
			}
		}

		protected virtual void Start()
		{
			this.Inject();
		}

		private Transform _transform;

		private bool _transformCached;

		private GameObject _gameobject;

		private bool _goCached;

		private Rigidbody2D _rigidbody2D;

		private bool _rb2dCached;

		private Rigidbody _rigidbody;

		private bool _rbCached;

		private Collider _collider;

		private bool _colliderCached;

		private RectTransform _recttransform;

		private bool _recttransformCached;

		private AudioSource _audioSource;

		private bool _audioSourceCached;
	}
}
