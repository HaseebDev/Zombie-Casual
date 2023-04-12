using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecheckUpgradeTalentTutorial : BaseTutorialBehavior
{
    public override void OnEnter()
    {
        int currentTalentLevel = SaveManager.Instance.Data.Inventory.TalentLevel;
        var talentUpgradeElement = DesignHelper.GetTalentUpgradeDesign(currentTalentLevel);
        int cost = talentUpgradeElement.Potion;

        if (!CurrencyModels.instance.IsEnough(CurrencyType.PILL, cost))
        {
            CurrencyModels.instance.AddCurrency(CurrencyType.PILL, cost);
        }
        
        OnExit();
    }

    public override void OnUpdate()
    {
    }
}