using System.Collections;
using System.Collections.Generic;
using com.datld.talent;
using UnityEngine;

public class AddOnMedic : BaseAddOnUltimate
{
    public override bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true)
    {
        if (base.PointerUpSkill(screenPos, checkValidCast))
        {
            if (_addOnItem.ItemCount <= 0)
            {
                NotifyDontHasAddon();
            }
            else if (!GamePlayController.instance.IsValidRefillHP())
            {
                InGameCanvas.instance.ShowFloatingTextNotify(LOCALIZE_ID_PREF.FULL_HP.AsLocalizeString());
                return false;
            }
            else
            {
                GamePlayController.instance.RefillHPPercentGamePlay(_design.Value); // refill from design (percentage)
                GamePlayController.instance.RefillHPGamePlay(ModelTalent.recoverHPGameplay); // refill from talent
                return true;
            }
        }

        return false;
    }
}