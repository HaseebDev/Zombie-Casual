using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;

public enum MARKER_STATE
{
    CURRENT,
    LOCKED,
    UNLOCKED
}

public class MarkerItemUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public List<Image> stars;
    
    public Image bg;
    public Transform flagHolder;
    
    private int _level;

    public void Load(int level, int currentLevel, int star)
    {
        this._level = level;
        text.text = level.ToString();
        bg.transform.localScale = Vector3.one;
        flagHolder.localScale = Vector3.one;
        
        var flag = HUDSelectCampaignLevel.Instance.Flag;
        if (flag.transform.parent == flagHolder)
        {
            flag.transform.SetParent(HUDSelectCampaignLevel.Instance.transform);
            flag.SetActive(false);
        }
        
        if (currentLevel == level)
        {
            ResourceManager.instance.GetMarkerBG(MARKER_STATE.CURRENT,bg);
            bg.transform.localScale = Vector3.one * 1.3f;
            flagHolder.localScale = Vector3.one * 1.3f;

            flag.transform.SetParent(flagHolder);
            flag.rectTransform().anchoredPosition = Vector2.zero;
            flag.transform.localScale = Vector3.one;
            flag.SetActive(true);
        }
        
        else if (currentLevel < level)
        {
            ResourceManager.instance.GetMarkerBG(MARKER_STATE.LOCKED,bg);
        }
        else
        {
            ResourceManager.instance.GetMarkerBG(MARKER_STATE.UNLOCKED,bg);
        }

        for (int i = 1; i <= 3; i++)
        {
            var tempSprite = stars[i - 1];
            ResourceManager.instance.GetStarSprite(i <= star, tempSprite);
        }
    }

    public void OnClick()
    {
        int stars = SaveGameHelper.GetStarAtLevel(_level);
        //always get max!
        var tuppleRewards = SaveManager.Instance.Data.GetCampaignRewards(_level, 3);
        MainMenuCanvas.instance.ShowHUD(EnumHUD.HUD_PREVIEW_CAMPAIGN_LEVEL, false, null, tuppleRewards.Item1,
            tuppleRewards.Item2, _level, stars);
    }
}