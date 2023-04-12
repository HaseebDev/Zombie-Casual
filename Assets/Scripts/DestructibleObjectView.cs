using System;
using Framework.Interfaces;
using Framework.Views;
using UnityEngine;
using UnityEngine.UI;

public class DestructibleObjectView : MonoBehaviour, ITickLate
{
	public void ChangeHp(int _amount)
	{
		this.hp += _amount;
		if (this.hp > 0)
		{
			this.hpTextField.text = this.hp.ToString() + " / " + this.maxHp.ToString();
			this.hpValue.SetNormalizedValue((float)(this.hp / this.maxHp));
			return;
		}
		this.hpTextField.text = "";
		this.hpValue.SetNormalizedValue(0f);
		this.isDestructible = true;
	}

	public void SetHP(int _currentHp, int _maxHp)
	{
		this.maxHp = _maxHp;
		this.hp = _maxHp;
		this.ChangeHp(0);
	}

	public void TickLate()
	{
		this.isDestructible = false;
	}

	[SerializeField]
	private Text hpTextField;

	[SerializeField]
	private BaseFloatValueView hpValue;

	private int maxHp;

	private int hp;

	public bool isDestructible;
}
