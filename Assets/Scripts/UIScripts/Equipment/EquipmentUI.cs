using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using DG.Tweening;
using QuickType.Weapon;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] protected Image _bg;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _type;
    [SerializeField] private LocalizedTMPTextUI _levelText;
    [SerializeField] private ReminderUI _reminderUi;
    [SerializeField] private Sprite _clearIcon;

    [SerializeField] private float customLevelTextSize1 = 0;
    [SerializeField] private float customLevelTextSize2 = 0;
    [SerializeField] private bool _shortLevelText = false;

    private WeaponDesign _weaponDesign;
    private WeaponData _weaponData;
    private Action<WeaponData, WeaponDesign, EquipmentUI> onClick;

    public WeaponDesign WeaponDesign => _weaponDesign;
    public WeaponData WeaponData => _weaponData;
    public Graphic TargetGraphic => _icon;
    public Button button;


    private void Start()
    {
        if (button == null)
            GetComponent<Button>()?.onClick.AddListener(OnClick);
        else
        {
            button.onClick.AddListener(OnClick);
        }
    }

    public void ShowReminder(bool isShow)
    {
        _reminderUi?.Show(isShow);
    }

    public virtual void Clear()
    {
        _weaponData = null;
        _weaponDesign = null;
        _icon.gameObject.SetActive(false);
        _levelText.gameObject.SetActive(false);
        _type.gameObject.SetActive(false);

        if (_clearIcon == null)
        {
            ResourceManager.instance.GetRankDefine(1).GetSprite(sprite => { _bg.sprite = sprite; });
        }
        else
        {
            _bg.sprite = _clearIcon;
        }
    }

    public virtual void LoadData(WeaponData weaponData, WeaponDesign weaponDesign)
    {
        _weaponData = weaponData;
        _weaponDesign = weaponDesign;
    }

    public virtual void Load(WeaponDesign weaponDesign)
    {
        _weaponDesign = weaponDesign;
        ResourceManager.instance.GetEquipSprite(weaponDesign.WeaponId, s => { _icon.sprite = s;});
        
        ResourceManager.instance.GetRankDefine(weaponDesign.Rarity).GetSprite(s =>
        {
            _bg.sprite = s;
        });
        
        ResourceManager.instance.GetEquipTypeSprite((int) weaponDesign.EquipType, s =>
        {
            _type.sprite = s;
        });

        _icon.gameObject.SetActive(true);
        _type.gameObject.SetActive(true);
        _levelText.gameObject.SetActive(false);
        ShowReminder(false);
    }

    public virtual void LoadImage(WeaponData weaponData, WeaponDesign weaponDesign)
    {
        ResourceManager.instance.GetEquipSprite(weaponDesign.WeaponId, s => _icon.sprite = s);
        ResourceManager.instance.GetRankDefine(weaponDesign.Rarity).GetSprite(s =>
        {
            _bg.sprite = s;
        });
        
        ResourceManager.instance.GetEquipTypeSprite((int) weaponDesign.EquipType, s =>
        {
            _type.sprite = s;
        });

        _levelText.text = _shortLevelText ? _weaponData.GetWeaponLevel().ToString() : $"Lv.{_weaponData.GetWeaponLevel()}";
        ApplyCustomLevelTextSize();

        _icon.gameObject.SetActive(true);
        _type.gameObject.SetActive(true);
        _levelText.gameObject.SetActive(true);
    }

    public virtual void Load(WeaponData weaponData, WeaponDesign weaponDesign)
    {
        LoadData(weaponData, weaponDesign);
        LoadImage(weaponData, weaponDesign);
        ShowReminder(false);
    }

    public void AddClickListener(Action<WeaponData, WeaponDesign, EquipmentUI> wp)
    {
        onClick = wp;
    }

    protected virtual void OnClick()
    {
        if (_weaponData != null && _weaponDesign != null)
            onClick?.Invoke(_weaponData, _weaponDesign, this);
    }

    public void HideLevelText()
    {
        _levelText.gameObject.SetActive(false);
    }

    public virtual void HideUnNecessaryInfo()
    {
    }

    private void ApplyCustomLevelTextSize()
    {
        if (customLevelTextSize1 == 0 && customLevelTextSize2 == 0)
            return;

        string lvText = "Lv.";
        string numberText = _weaponData.GetWeaponLevel().ToString();
        if (customLevelTextSize1 != 0)
        {
            lvText = $"<size={customLevelTextSize1}>{lvText}</size>";
        }

        if (customLevelTextSize2 != 0)
        {
            numberText = $"<size={customLevelTextSize2}>{numberText}</size>";
        }

        _levelText.text = lvText + numberText;
    }

    public void Reload()
    {
        Load(_weaponData, _weaponDesign);
    }
}