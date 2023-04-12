using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentHeroEquipHolder : MonoBehaviour
{
    public EquipmentUI3 _equippedArmour;
    public EquipmentUI3 _equippedWeapon; 
    public GameObject _goLockedArmour;
    
    public void OnButtonArmour()
    {
        if (!GameMaster.ENABLE_ARMOUR_FEATURE)
            MainMenuCanvas.instance.ShowFloatingTextNotify(LOCALIZE_ID_PREF.COMING_SOON.AsLocalizeString());
    }
}
