using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using UnityEngine;

public class ClickTutorialEquip : BaseTutorialBehavior
{
    public string weaponID;
    private ChangeWeaponButton _equipmentUi;
    private ChangeWeaponButton[] _equipmentHolder;

    private void FindWeaponData()
    {
        if (_equipmentUi == null)
        {
            _equipmentHolder = FindObjectsOfType<ChangeWeaponButton>();
            if (_equipmentHolder != null)
            {
                _equipmentUi = _equipmentHolder.ToList().Find(x => x.WeaponData != null && x.WeaponData.WeaponID == weaponID);
                // foreach (var VARIABLE in _equipmentHolder)
                // {
                //     // if (VARIABLE.WeaponData != null)
                //         // Debug.LogError(VARIABLE.WeaponData.WeaponID);
                //
                //     if (VARIABLE.WeaponData != null && VARIABLE.WeaponData.WeaponID == weaponID)
                //     {
                //         _equipmentUi = VARIABLE;
                //         break;
                //     }
                // }

                if (_equipmentUi != null)
                {
                    // Debug.LogError("FOUND " + _equipmentUi.WeaponData.WeaponID);
                    var buttonID = _equipmentUi.Views.gameObject.AddComponent<TutorialButtonID>();
                    buttonID.id = weaponID;

                    ClickTutorialType clickType = (ClickTutorialType) behaviorType;
                    clickType.buttonID = weaponID;
                }
            }
        }
    }

    public override void OnUpdate()
    {
        if (_equipmentUi == null)
        {
            FindWeaponData();
        }
        else
        {
            if (!showed)
            {
                behaviorType.OnEnter();
                showed = true;
            }
        }

        if (showed)
            behaviorType.OnUpdate();
    }
}