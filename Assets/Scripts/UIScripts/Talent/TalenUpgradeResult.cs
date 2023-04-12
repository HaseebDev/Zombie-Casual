using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityExtensions.Localization;

public class TalenUpgradeResult : MonoBehaviour
{
    [SerializeField] private TalentUI _talentUi;

    [SerializeField] private LocalizedTMPTextUI _name;
    [SerializeField] private LocalizedTMPTextUI _description;
    [SerializeField] private LocalizedTMPTextUI _oldStat;
    [SerializeField] private LocalizedTMPTextUI _newStat;

    private void OnEnable()
    {
        MainMenuCanvas.instance?.HideTabAndCurrencyBar();
    }

    private void OnDisable()
    {
        MainMenuCanvas.instance?.ShowTabAndCurrencyBar();
    }

    public void Load(TalentData talentData)
    {
        gameObject.SetActive(true);

        var talentDesign = DesignHelper.GetTalentDesign(talentData);

        if (talentDesign.IsPercent)
        {
            string specialChar = "%";
            _newStat.text = talentData.TalentValue + specialChar;
            _oldStat.text = talentDesign.GetValue(talentData.TalentLevel - 1) + specialChar;
        }
        else
        {
            _newStat.text = talentData.TalentValue.ToString();
            _oldStat.text = talentDesign.GetValue(talentData.TalentLevel - 1).ToString();
        }

        _name.text = LocalizeController.GetText(talentDesign.Name);
        _description.text = LocalizeController.GetText(talentDesign.Description);

        _talentUi.Load(talentDesign);
    }
}