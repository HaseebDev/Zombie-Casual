using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentHeroBottom : MonoBehaviour
{
    public DotSwitcher _dotSwitcher;
    public ReminderUI _newHeroNextButtonReminder;
    public ReminderUI _newHeroReminder;
    public GameObject _lockManageHero;
    public Action NextHeroCallback { get; set; }
    public Action PrevHeroCallback { get; set; }

    public void OpenHUDHero()
    {
        var unlockRequire = DesignHelper.GetUnlockRequirementLevel(EnumHUD.HUD_HERO.ToString());
        if (unlockRequire.Item1)
        {
            MainMenuCanvas.instance.ShowHUD(EnumHUD.HUD_HERO, false);
        }
        else
        {
            MainMenuCanvas.instance.ShowFloatingTextNotify(LocalizeController.GetText(LOCALIZE_ID_PREF.UNLOCK_IN_LV,
                unlockRequire.Item2));
        }
    }

    public void NextHero()
    {
        NextHeroCallback?.Invoke();
    }

    public void PrevHero()
    {
        PrevHeroCallback?.Invoke();
    }
    
    
}