using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using TMPro;
using UnityEngine;
using UnityExtensions.Localization;

public class HUDPurhcaseHeroInGame : BaseHUD
{
    [SerializeField] private HeroInfoUIHelper _heroInfoUiHelper;
    [SerializeField] private EquipmentUI _equipmentUi;
    [SerializeField] private LocalizedTMPTextUI _weaponName;

    [Header("Char Preview Prefab")] public ShopHeroPreview _heroPreviewPrefab;
    private ShopHeroPreview _heroPreview;

    protected Action onClose;

    public override void Init()
    {
        if (!isInit)
        {
            _heroPreview = FindObjectOfType<ShopHeroPreview>();
            
            if (_heroPreview == null)
            {
                _heroPreview = GameObject.Instantiate(_heroPreviewPrefab, null);
                _heroPreview.transform.position = Vector3.one * 999f;
                _heroPreview.gameObject.SetActiveIfNot(true);
            }
        }

        base.Init();
    }

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        Init();

        HeroData heroData = (HeroData) args[0];
        onClose = (Action) args[1];

        _heroInfoUiHelper.onUnlock = OnUnlock;
        _heroInfoUiHelper.Load(heroData);
        _heroInfoUiHelper.HideBonus();

        var equippedWeapon = SaveManager.Instance.Data.GetWeaponData(heroData.EquippedWeapon);
        var wpDesign = DesignHelper.GetWeaponDesign(equippedWeapon);
        _equipmentUi.Load(equippedWeapon, wpDesign);
        _weaponName.textName = wpDesign.Name;
        _weaponName.text = wpDesign.GetLocalizeName();

        GamePlayController.instance?.SetPauseGameplay(true);
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_HERO_PREVIEW, heroData);
        base.PreInit(type, _parent, args);
    }

    private void OnUnlock(HeroData heroData)
    {
        OnButtonBack();
    }

    public override void OnButtonBack()
    {
        base.OnButtonBack();
        onClose?.Invoke();
        GamePlayController.instance?.SetPauseGameplay(false);
    }
}