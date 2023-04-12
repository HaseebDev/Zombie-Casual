using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using DG.Tweening;
using QuickEngine.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class HeroPromoManager : MonoBehaviour
{
    [SerializeField] private GameObject _heroPromoButton;
    [SerializeField] private Image _heroAvatar;
    [SerializeField] private StarsGroup _starsGroup;

    private string _lastHeroID;
    private static bool _showInStartGame;
    private static string _lastAutoShowHeroID;

    public void ResetLayer()
    {
        _lastHeroID = "";

        foreach (var VARIABLE in DesignManager.instance.unlockHeroDesign.UnlockHeroDesignElements)
        {
            if (VARIABLE.Available
                && VARIABLE.CostType == "IAP" &&
                SaveGameHelper.GetMaxCampaignLevel() >= VARIABLE.LevelToShow &&
                SaveManager.Instance.Data.GetHeroData(VARIABLE.HeroId).ItemStatus == ITEM_STATUS.Locked)
            {
                _lastHeroID = VARIABLE.HeroId;
            }
        }

        _heroPromoButton.gameObject.SetActive(!_lastHeroID.IsNullOrEmpty());
        if (!_lastHeroID.IsNullOrEmpty())
        {
            var _heroData = SaveManager.Instance.Data.GetHeroData(_lastHeroID);
            ResourceManager.instance.GetHeroAvatar(_lastHeroID,_heroAvatar);
            _starsGroup.EnableStars(_heroData.Rank);
        }
    }

    public void OpenPromo()
    {
        if (!_lastHeroID.IsNullOrEmpty())
        {
            var _heroData = SaveManager.Instance.Data.GetHeroData(_lastHeroID);
            MasterCanvas.CurrentMasterCanvas.ShowHUD(EnumHUD.HUD_PURCHASE_HERO_INGAME, false, null, _heroData, null);
        }
    }


    public void Init()
    {
        _starsGroup.Initialize(5);
        ResetLayer();

        if (_lastAutoShowHeroID != _lastHeroID || !_showInStartGame)
        {
            // if (!_showInStartGame)
            // {
            //     DOVirtual.DelayedCall(3, OpenPromo);
            // }
            // else
            // {
            DOVirtual.DelayedCall(0.5f, OpenPromo);
            // }

            _lastAutoShowHeroID = _lastHeroID;
        }

        _showInStartGame = true;
    }

    private void Awake()
    {
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.PURCHASE_HERO, new Action(ResetLayer));
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.PURCHASE_HERO, new Action(ResetLayer));
    }
}