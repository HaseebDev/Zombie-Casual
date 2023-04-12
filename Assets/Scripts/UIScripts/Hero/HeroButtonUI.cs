using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickType.Hero;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class HeroButtonUI : MonoBehaviour
{
    [SerializeField] private Image _heroImage;
    [SerializeField] private LocalizedTMPTextUI _levelText;
    [SerializeField] private GameObject _upgradeIcon;
    [SerializeField] private StarsGroup _starGroup;
    [SerializeField] private ReminderUI _reminderUi;

    [Header("Unlock")] [SerializeField] private Image _unlockedMask;
    [SerializeField] private GameObject _levelPanel;

    private Toggle _toggle;
    private HeroData _heroData;
    public HeroData HeroData => _heroData;
    public Action<HeroData> onSelect;

    private bool _isUnlocked = false;
    
    private void Awake()
    {
       CurrencyModels.instance.AddCallback(OnMoneyChange);
    }

    private void OnMoneyChange(CurrencyType type, long quantity)
    {
        if (type != CurrencyType.GOLD) return;
        
        if (_isUnlocked)
        {
            var heroDesign = DesignHelper.GetHeroDesign(_heroData);
            var cost = _heroData.GetHeroCampaignCost(_heroData.GetHeroLevel() + 1);

            _upgradeIcon.gameObject.SetActive(_heroData.GetHeroLevel() < heroDesign.MaxLevel &&
                                              CurrencyModels.instance.IsEnough(CurrencyType.GOLD, cost));
        }
    }

    private void OnDestroy()
    {
        CurrencyModels.instance.RemoveCallback(OnMoneyChange);
    }

    public void LoadReminderUi(int quantity)
    {
        _reminderUi.Load(quantity);
    }

    public void UpdateToggleGroup(ToggleGroup toggleGroup)
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = false;
        _toggle.group = toggleGroup;
        _toggle.onValueChanged.RemoveAllListeners();
        _toggle.onValueChanged.AddListener(OnToggleValueChange);
    }

    public void Load(HeroData heroData, bool isUnlocked)
    {
        _heroData = heroData;
        var heroDesign = DesignHelper.GetHeroDesign(heroData);
        ResourceManager.instance.GetHeroAvatar(heroDesign.HeroId,_heroImage);
        _levelText.text = $"{heroData.GetHeroLevel()}"; //{heroDesign.MaxLevel}";

        int MaxRank = DesignHelper.GetHeroMaxRank(_heroData.UniqueID);
        _starGroup.Initialize(MaxRank);
        _starGroup.EnableStars(_heroData.Rank);
        
        _isUnlocked = isUnlocked;
        if (isUnlocked)
        {
            _levelPanel.SetActive(true);
            _unlockedMask.gameObject.SetActive(false);

            var cost = _heroData.GetHeroCampaignCost(_heroData.GetHeroLevel() + 1);
            _upgradeIcon.gameObject.SetActive(heroData.GetHeroLevel() < heroDesign.MaxLevel &&
                                              CurrencyModels.instance.IsEnough(CurrencyType.GOLD, cost));
        }
        else
        {
            _levelPanel.SetActive(false);
            _unlockedMask.gameObject.SetActive(true);
            _upgradeIcon.gameObject.SetActive(false);
        }

        if (heroData.HasEnoughShardToUpRank())
        {
            _reminderUi.Load(1);
        }
    }

    public void Select()
    {
        _toggle.isOn = true;
        onSelect?.Invoke(_heroData);
        
    }

    private void OnToggleValueChange(bool isOn)
    {
        if (isOn)
        {
            onSelect?.Invoke(_heroData);
            AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_BUTTON);
        }
           
    }
}