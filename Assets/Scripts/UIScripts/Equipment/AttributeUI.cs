using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using QuickType.Attribute;
using QuickType.SkillDesign;
using QuickType.Weapon;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class AttributeUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private LocalizedTMPTextUI _name;
    [SerializeField] private AttributeTooltip _attributeTooltip;
    [SerializeField] protected LocalizedTMPTextUI _realValue;

    [Header("Bonus")] [SerializeField] private GameObject _bonusZone;
    [SerializeField] private LocalizedTMPTextUI _bonusText;
    [SerializeField] protected LocalizedTMPTextUI _value;

    protected SkillDesignElement _attributeDesign;
    public string AttributeId;
    public double Value;
    public double Bonus;

    public void Load(string attributeID, double value, bool autoHideValue = true)
    {
        Bonus = 0;
        AttributeId = attributeID;
        Value = value;

        // var skillEnum = attributeID.ParseEnum(EffectType.NONE);
        // if(skillEnum != EffectType.NONE)

        _attributeDesign = null;
        DesignManager.instance._dictSkillDesign.TryGetValue(attributeID, out _attributeDesign);
        if (_attributeDesign == null)
        {
            Debug.LogError($"Cant find attribute with ID: {attributeID}");
            return;
        }
        ResourceManager.instance.GetUltimateSprite(attributeID, s =>
        {
            _icon.sprite = s;
        });
        _name.text = _attributeDesign.GetName();
        UpdateValueText(DesignHelper.GetValueTextFromAttribute(value, _attributeDesign));
        _attributeTooltip.UpdateText(string.Format(_attributeDesign.GetDescription(),
            DesignHelper.GetValueTextFromAttribute(value, _attributeDesign)));

        _realValue.gameObject.SetActive(true);
        if (value == 0 && autoHideValue)
        {
            _realValue.gameObject.SetActive(false);
        }

        _bonusZone.SetActive(false);
    }

    public void LoadWithBonusTotal(string attributeID, double value, double bonus = 0, bool autoHideValue = true)
    {
        Load(attributeID, value, autoHideValue);
        Bonus = bonus;
        if (bonus != 0)
        {
            _bonusZone.SetActive(true);

            double total = value + bonus;

            var attributeDesign = DesignManager.instance._dictSkillDesign[attributeID];
            _attributeTooltip.UpdateText(string.Format(attributeDesign.GetDescription(), value));
            _bonusText.gameObject.SetActive(true);
            _bonusText.text = DesignHelper.GetValueTextFromAttribute(total, attributeDesign);

            _realValue.gameObject.SetActive(false);
        }
        else
        {
            _bonusZone.SetActive(false);
        }
    }

    public void LoadWithBonus(string attributeID, double value, double bonus = 0, bool autoHideValue = true)
    {
        Load(attributeID, value, autoHideValue);
        Bonus = bonus;

        if (bonus != 0)
        {
            _bonusZone.SetActive(true);

            // double total = value + bonus;
            // string totalText = $"{total:0}";

            var attributeDesign = DesignManager.instance._dictSkillDesign[attributeID];
            _attributeTooltip.UpdateText(string.Format(attributeDesign.GetDescription(), value));
            _bonusText.gameObject.SetActive(true);
            _bonusText.text = DesignHelper.GetValueTextFromAttribute(bonus, attributeDesign);
            ;

            _realValue.gameObject.SetActive(false);
        }
        else
        {
            _bonusZone.SetActive(false);
        }
    }


    public void ShowTooltip()
    {
        _attributeTooltip.Show();
    }

    public void Reload()
    {
        LoadWithBonus(AttributeId, Value, Bonus);
    }

    protected void UpdateValueText(string s)
    {
        _value.text = s;
        _realValue.text = s;
    }
}