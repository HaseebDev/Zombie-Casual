using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using UnityEngine;

public class PreCheckWeaponTutorial : BaseTutorialBehavior
{
    public string weaponID;
    
    public override void OnEnter()
    {
        ResetState();
        OnCompleteShowEvent();
    }

    public override void OnUpdate()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.Data != null && HasEnoughRequire())
        {
            var weaponData = SaveManager.Instance.Data.Inventory.ListWeaponData.ToList()
                .Find(x => x.WeaponID == weaponID && x.ItemStatus == ITEM_STATUS.Available || x.ItemStatus == ITEM_STATUS.None);
            
            if (weaponData == null)
            {
                SaveManager.Instance.Data.AddWeapon(weaponID);
            }
            
            OnExit();
        }
    }
}