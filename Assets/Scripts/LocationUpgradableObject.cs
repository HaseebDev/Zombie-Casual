using System;
using UnityEngine;

public class LocationUpgradableObject : MonoBehaviour
{
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animator.enabled = false;
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void EnableAnim()
	{
		this.animator.enabled = true;
	}

	private void Update()
	{
	}

	private Animator animator;
}
