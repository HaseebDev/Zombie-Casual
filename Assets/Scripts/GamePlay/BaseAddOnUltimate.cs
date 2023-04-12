using com.datld.data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAddOnUltimate : BaseCharacterUltimate
{
    public override bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true)
    {
        if (_addOnItem != null && (_addOnItem.Status == ITEM_STATUS.Disable || (!_addOnItem.IsUnlimitedItem && _addOnItem.IsConsumableAddOn() && _addOnItem.ItemCount <= 0)))
        {
            if (_addOnItem.ItemCount <= 0)
            {
                NotifyDontHasAddon();
            }
            
            return false;
        }
        else if (_addOnItem != null && !_addOnItem.IsSpecialAddOn())
        {
            if (_addOnItem.ItemCount <= 0)
            {
                NotifyDontHasAddon();
                return false;
            }
            else
                return true;
        }
        return base.PointerUpSkill(screenPos, checkValidCast);
    }
    
}
