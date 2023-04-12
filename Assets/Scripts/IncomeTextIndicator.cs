using System;
using TMPro;
using UnityEngine;

public class IncomeTextIndicator : PoolObject
{
	private void OnEnable()
	{
		this.textField.text = "$" + base.gameObject.name;
	}

	public override void OnAwake()
	{
		base.Done(this.lifeTime);
	}

	private void Update()
	{
		base.transform.position += new Vector3(0f, this.verticalSpeed, 0f) * Time.deltaTime;
	}

	[SerializeField]
	private TextMeshPro textField;

	private string textValue;

	[SerializeField]
	private float lifeTime = 2f;

	[SerializeField]
	private float verticalSpeed;
}
