using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class FilterEquipHelper : MonoBehaviour
{
    private FilterType _lastUpdateFiler = FilterType.NONE;

    public class OrderEquip
    {
        public EquipmentUI EquipmentUi;
        public long Score;
        public WeaponData WeaponData;

        public OrderEquip(WeaponData weaponData)
        {
            this.WeaponData = weaponData;
        }

        public OrderEquip(EquipmentUI equipmentUi)
        {
            EquipmentUi = equipmentUi;
        }

        public void CalCulateScore(FilterType type)
        {
            if (type == FilterType.BY_RARITY)
            {
                Score = EquipmentUi.WeaponDesign.Rarity * 100000 + -(int) EquipmentUi.WeaponDesign.EquipType * 10000 +
                        EquipmentUi.WeaponData.GetWeaponLevel() * 1;
            }
            else if (type == FilterType.BY_EQUIPMENT_TYPE)
            {
                Score = -(int) EquipmentUi.WeaponDesign.EquipType * 100000 +
                        (int) EquipmentUi.WeaponDesign.Rarity * 10000 +
                        EquipmentUi.WeaponData.GetWeaponLevel() * 1;
            }
            else if (type == FilterType.BY_DPS)
            {
                Score = (int) (EquipmentUi.WeaponData.FinalPowerData.PercentDmg *
                               EquipmentUi.WeaponData.FinalPowerData.Firerate) * 1000000 +
                        (int) EquipmentUi.WeaponDesign.Rarity * 10000 +
                        EquipmentUi.WeaponData.GetWeaponLevel() * 1;
            }
        }

        public void CalculateScoreByWpData(FilterType type)
        {
            var wpDesign = DesignHelper.GetWeaponDesign(WeaponData);

            if (type == FilterType.BY_RARITY)
            {
                Score = wpDesign.Rarity * 100000 + -(int) wpDesign.EquipType * 10000 +
                        WeaponData.GetWeaponLevel() * 1;
            }
            else if (type == FilterType.BY_EQUIPMENT_TYPE)
            {
                Score = -(int) wpDesign.EquipType * 100000 +
                        (int) wpDesign.Rarity * 10000 +
                        WeaponData.GetWeaponLevel() * 1;
            }
            else if (type == FilterType.BY_DPS)
            {
                Score = (int) (WeaponData.FinalPowerData.PercentDmg *
                               WeaponData.FinalPowerData.Firerate) * 1000000 +
                        (int) wpDesign.Rarity * 10000 +
                        WeaponData.GetWeaponLevel() * 1;
            }
        }
    }

    public enum FilterType
    {
        BY_RARITY,
        BY_EQUIPMENT_TYPE,
        BY_DPS,
        NONE
    }

    [SerializeField] private EquipmentHolder _equipmentHolder;
    [SerializeField] private LocalizedTMPTextUI _filterText;
    private FilterType _currentFilterType = FilterType.BY_DPS;
    private Action OnNewFilter;

    public void AddOnNewFilterCallback(Action callback)
    {
        OnNewFilter = callback;
    }
    
    public List<WeaponData> GetSortedWeaponList()
    {
        List<OrderEquip> orders = new List<OrderEquip>();

        var allWps = SaveManager.Instance.Data.Inventory.ListWeaponData;

        foreach (var wpData in allWps)
        {
            OrderEquip newOrder = new OrderEquip(wpData);
            newOrder.CalculateScoreByWpData(_currentFilterType);
            orders.Add(newOrder);
        }

        orders = orders.OrderBy(x => -x.Score).ToList();
        var result = new List<WeaponData>();
        foreach (var VARIABLE in orders)
        {
            result.Add(VARIABLE.WeaponData);
        }

        _lastUpdateFiler = _currentFilterType;
        return result;
    }

    public void UpdateFilter(bool force = false)
    {
        if (_lastUpdateFiler != _currentFilterType || force)
        {
            OnNewFilter?.Invoke();
            //List<OrderEquip> orders = new List<OrderEquip>();
            
            // Debug.LogError($"UPDATE FILTER {_currentFilterType}, LEN";
            // foreach (var weaponData in _equipmentHolder.AvailableWeaponDatas)
            // {
            //     OrderEquip newOrder = new OrderEquip(weaponData);
            //     newOrder.CalCulateScore(_currentFilterType);
            //     orders.Add(newOrder);
            // }
            
            //orders = orders.OrderBy(x => -x.Score).ToList();
            
            // foreach (var order in orders)
            // {
            //     order.EquipmentUi.transform.SetAsLastSibling();
            // }
        }
        
        string textName = "";
        switch (_currentFilterType)
        {
            case FilterType.BY_RARITY:
                textName = LOCALIZE_ID_PREF.BY_RARITY;
                break;
            case FilterType.BY_EQUIPMENT_TYPE:
                textName = LOCALIZE_ID_PREF.BY_EQUIPMENT;
                break;
            case FilterType.BY_DPS:
                textName = LOCALIZE_ID_PREF.BY_DPS;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _lastUpdateFiler = _currentFilterType;
        _filterText.textName = textName;
    }

    public void SwitchFilter()
    {
        int currentIndex = (int) _currentFilterType;
        currentIndex++;
        if (currentIndex > 2)
            currentIndex = 0;

        _currentFilterType = (FilterType) currentIndex;
        UpdateFilter();
    }

    public void OnResetLayers()
    {
        UpdateFilter();
    }
}