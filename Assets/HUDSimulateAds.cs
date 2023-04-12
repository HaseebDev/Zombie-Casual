using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ADSTYPE
{
    NONE,
    ADS_INTERSTITIAL,
    ADS_REWARD
}

public class HUDSimulateAds : BaseHUD
{
    public Text txtContent;

    private ADSTYPE CurrentAdsType = ADSTYPE.NONE;

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        CurrentAdsType = (ADSTYPE)args[0];
        switch (CurrentAdsType)
        {
            case ADSTYPE.NONE:
                break;
            case ADSTYPE.ADS_INTERSTITIAL:
                txtContent.text = "INTERSTITIAL ADS";
                break;
            case ADSTYPE.ADS_REWARD:
                txtContent.text = "REWARD ADS";
                break;
            default:
                break;
        }
    }
}
