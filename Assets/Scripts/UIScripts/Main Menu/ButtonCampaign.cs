using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class ButtonCampaign : MonoBehaviour
{
    [SerializeField] private LocalizedTMPTextUI _location;
    [SerializeField] private LocalizedTMPTextParam _level;
    [SerializeField] private Image _progressImg;

    public void Load()
    {
        var levelDesign = DesignManager.instance.LevelCampaignDesign;
        string lastLocationID = SaveGameHelper.GetLastLocationId();
        var lastLevel = levelDesign.FindLast(x => x.LocationId == lastLocationID);

        _location.textName = lastLocationID;
        string levelProgress = $"{SaveGameHelper.GetCurrentCampaignLevel()}/{lastLevel.Level}";
        _level.UpdateParams(levelProgress);

        _progressImg.fillAmount = (float) SaveGameHelper.GetCurrentCampaignLevel() / lastLevel.Level;
    }
}