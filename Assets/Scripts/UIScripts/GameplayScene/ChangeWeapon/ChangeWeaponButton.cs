using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using CustomListView.Weapon;
using DG.Tweening;
using Ez.Pooly;
using QuickType.Weapon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AnalyticsConstant;

public class ChangeWeaponButton : MonoBehaviour
{
    public RectTransform Views;
    [SerializeField] private EquipmentUI _equipmentUi;
    [SerializeField] private TextMeshProUGUI _dmgPercentText;
    [SerializeField] private Image _highlight;

    private Action<WeaponData> _onClick;
    private WeaponData _weaponData;
    public WeaponData WeaponData => _weaponData;
    public WeaponItemModel model;
    private Sequence _glowSequence;

    public void Load(WeaponData weaponData, WeaponDesign wpDesign)
    {
        _weaponData = weaponData;
        _equipmentUi.Load(weaponData, wpDesign);
        _dmgPercentText.text = $"{CalcDPS():0.##}%";
    }

    public void AddClickListener(Action<WeaponData> onEquipClick)
    {
        _onClick = onEquipClick;
    }

    public void OnClick()
    {
        _onClick?.Invoke(_weaponData);
    }

    public float CalcDPS()
    {
        return _weaponData.FinalPowerData.PercentDmg;
    }

    public void ShowReminder(bool isShow)
    {
        _equipmentUi.ShowReminder(isShow);
        model.showReminder = isShow;
    }
    
    public void ShowHighlight(bool isShow)
    {
        _highlight.gameObject.SetActive(isShow);

        if (_glowSequence == null)
        {
            _glowSequence = DOTween.Sequence();
            _glowSequence.Append(_highlight.DOFade(0.2f, 0.5f));
            _glowSequence.Append(_highlight.DOFade(1f, 0.5f));
            // _glowSequence.Append(_highlight.DOFade(0.2f, 0.5f));
            _glowSequence.SetLoops(-1);
        }

        if (isShow)
        {
            _glowSequence.Play();
        }
        else
        {
            _glowSequence.Pause();
        }
    }
}