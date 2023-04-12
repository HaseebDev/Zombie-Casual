using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickType.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;
using static AnalyticsConstant;

public class ChangeTeamHeroButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _dpsText;
    [SerializeField] private Image _iconImage;

    [SerializeField] private GameObject _lockPanel;
    [SerializeField] private GameObject _levelPanel;
    [SerializeField] private GameObject _buyBtn;
    [SerializeField] private GameObject _cantUnlockPanel;
    [SerializeField] private LocalizedTMPTextUI _cantUnlockText;
    [SerializeField] private RectTransform _mask;

    private Action<HeroData> _onClick;
    private Action _onBack;

    private HeroData _heroData;
    public HeroData HeroData => _heroData;
    private bool _isSelected = false;

    public void Load(HeroData heroData)
    {
        var heroDesign = DesignHelper.GetHeroDesign(heroData);
        _levelText.text = heroData.GetHeroLevel().ToString();
 

        _dpsText.text = $"{heroData.FinalPowerData.CalcDPS():0}";
        _heroData = heroData;
        
        LoadHeroSprite(_heroData.UniqueID);

        bool isUnlocked = heroData.IsUnlocked();
        bool isAvailable = heroData.ItemStatus == ITEM_STATUS.Available;

        _lockPanel.SetActive(!isUnlocked);
        _levelPanel.SetActive(isUnlocked);

        if (!isUnlocked)
        {
            _mask.sizeDelta = Vector2.zero;
            var unlockHeroElement = DesignHelper.GetUnlockHeroDesignElement(heroData.UniqueID);
            var costType = unlockHeroElement.CostType.ToEnum<CostType>();

            // var unlockRequire = DesignHelper.IsRequirementAvailable(heroData.UniqueID);
            if (costType == CostType.FREE)
            {
                _cantUnlockPanel.SetActive(true);
                _cantUnlockText.text = $"Level {unlockHeroElement.UnlockLevel}";
            }
            else
            {
                var unlockCurrency = unlockHeroElement.GetCurrencyType();
                _buyBtn.gameObject.SetActive(true);
                _cantUnlockPanel.SetActive(false);
            }
        }

        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (!_isSelected)
            {
                if (isUnlocked && isAvailable)
                    _onClick?.Invoke(_heroData);
                else
                {
                    MasterCanvas.CurrentMasterCanvas.ShowHUD(EnumHUD.HUD_PURCHASE_HERO_INGAME, false, null, _heroData,
                        _onBack);
                }

                if (_heroData != null)
                {
                    //analytics
                    var currentLevel = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CurrentLevel;
                    if (currentLevel <= AnalyticsConstant.MAX_TRACKING_LEVEL)
                    {
                        List<LogEventParam> results = new List<LogEventParam>();
                        results.Add(new LogEventParam("level", currentLevel));
                        results.Add(new LogEventParam("weaponID", _heroData.UniqueID));
                        AnalyticsManager.instance.LogEvent(getEventNameByLevel(ANALYTICS_ENUM.TOUCH_INGAME_HERO_AVATAR, currentLevel), results);
                    }
                }

            }
        });
    }

    public void SetOnClickCallback(Action<HeroData> onClick)
    {
        _onClick = onClick;
    }

    public void SetOnBackCallback(Action onBack)
    {
        _onBack = onBack;
    }

    public void LoadHeroSprite(string heroID)
    {
        ResourceManager.instance.GetHeroAvatar(heroID, s =>
        {
            _iconImage.sprite = s;
        });
    }
}