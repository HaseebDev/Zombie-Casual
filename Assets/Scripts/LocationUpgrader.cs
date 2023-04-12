using System;
using System.Collections.Generic;
using UnityEngine;

public class LocationUpgrader : MonoBehaviour
{
	public void Init(UpgradeableObject _upgradeableObject)
	{
		this.upgradeableObject = _upgradeableObject;
	}

	public void UpdateVisualUprages()
	{
		for (int i = 0; i < this.charsShowLevels.Count; i++)
		{
			if (this.upgradeableObject.Level >= this.charsShowLevels[i].level && this.charsShowLevels[i].animIsOn)
			{
				this.charsShowLevels[i].locationObject.EnableAnim();
				this.lastShowedIndex = i + 1;
			}
		}
	}

	[SerializeField]
	private List<LocationUpgradesLevel> charsShowLevels;

	private UpgradeableObject upgradeableObject;

	private int lastShowedIndex;
}
