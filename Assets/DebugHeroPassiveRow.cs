using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugHeroPassiveRow : MonoBehaviour
{
    public Text passiveText;
    public Toggle _toggleChecked;

    private bool isChecked = false;
    private Character Character;
    private EffectType SKILLID;

    public void Initialize(Character _char, EffectType passiveSkill, bool isEnabled)
    {
        SKILLID = passiveSkill;
        Character = _char;
        isChecked = isEnabled;
        passiveText.text = passiveSkill.ToString();
        _toggleChecked.isOn = isChecked;

        _toggleChecked.onValueChanged.RemoveAllListeners();
        _toggleChecked.onValueChanged.AddListener(OnClickedRow);
    }

    public void OnClickedRow(bool Value)
    {
        isChecked = Value;
        _toggleChecked.isOn = Value;

        if (Character != null)
        {
            if (isChecked)
                Character.AddEffectHit(Character.Data.defaultEffectHit(SKILLID.ToString()));
            else
                Character.RemoveEffectHit(SKILLID);
        }
    }
}
