using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using UnityEngine;

public class PreCheckHeroTutorial : BaseTutorialBehavior
{
    public string heroID;

    public override void OnEnter()
    {
        OnCompleteShowEvent();
    }

    public override void OnUpdate()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.Data != null && HasEnoughRequire())
        {
            var heroData = SaveManager.Instance.Data.Inventory.ListHeroData.ToList()
                .Find(x => x.UniqueID == heroID && x.ItemStatus != ITEM_STATUS.Locked);

            if (heroData == null)
            {
                var defaultHero = SaveManager.Instance.Data.GetHeroData(heroID);
                if (defaultHero == null)
                    SaveManager.Instance.Data.AddHero(heroID);

                defaultHero = SaveManager.Instance.Data.GetHeroData(heroID);
                defaultHero.UnlockHero();
                ReminderManager.SaveCurrentHeroState();
            }

            Debug.LogError("ADD hero and exit");
            OnExit();
        }
    }
}