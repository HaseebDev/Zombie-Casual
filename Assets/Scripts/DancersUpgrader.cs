using System;
using System.Collections.Generic;
using UnityEngine;

public class DancersUpgrader : MonoBehaviour
{
	public void Init(UpgradeableObject _upgradeableObject)
	{
		this.upgradeableObject = _upgradeableObject;
	}

	public void LoadAllUpgrades()
	{
		for (int i = 0; i < this.charsShowLevels.Count; i++)
		{
			if (this.upgradeableObject.Level >= this.charsShowLevels[i].level)
			{
				this.lastShowedIndex = i + 1;
			}
		}
		if (this.lastShowedIndex > this.charsShowLevels.Count)
		{
			return;
		}
		this.UpdateVisual();
	}

	private float СalucateProgress()
	{
		float num = (float)(this.upgradeableObject.Level % 10);
		if (Mathf.CeilToInt(num) == 0)
		{
			num = 10f;
		}
		return num;
	}

	public void UpdateVisual()
	{
	}

	public bool progressIsFilled()
	{
		return this.СalucateProgress() / 10f >= 1f;
	}

	[SerializeField]
	private List<CharacterShowLevel> charsShowLevels;

	private int lastShowedIndex;

	private UpgradeableObject upgradeableObject;
}
