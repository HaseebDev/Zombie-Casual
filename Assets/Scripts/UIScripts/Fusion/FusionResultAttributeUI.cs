using System.Collections;
using System.Collections.Generic;
using QuickType.SkillDesign;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class FusionResultAttributeUI : MonoBehaviour
{
    [SerializeField] private LocalizedTMPTextUI _name;
    [SerializeField] private TextMeshProUGUI _oldValueText;
    [SerializeField] private TextMeshProUGUI _newValueText;

    public void Load(string name, float oldValue, float newValue, bool isPercent = false)
    {
        _name.text = name;
        _oldValueText.text = oldValue.ToString();
        _newValueText.text = newValue.ToString();
        if (isPercent)
        {
            _oldValueText.text += "%";
            _newValueText.text += "%";
        }
    }

    public void Load(SkillDesignElement skillDesignElement, float oldValue, float newValue)
    {
        _name.text = skillDesignElement.GetName();
        _oldValueText.text = DesignHelper.GetValueTextFromAttribute(oldValue, skillDesignElement);
        _newValueText.text = DesignHelper.GetValueTextFromAttribute(newValue, skillDesignElement);
    }
}