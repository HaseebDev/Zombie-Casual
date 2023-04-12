using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions.Localization;

public class TalentTooltip : AttributeTooltip
{
    [SerializeField] private LocalizedTMPTextUI _name;

    public void UpdateText(string name, string content)
    {
        _name.text = name;
        base.UpdateText(content);
    }
}