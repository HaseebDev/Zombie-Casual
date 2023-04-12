using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using CustomListView.WeaponFusion;
using Ez.Pooly;
using MEC;
using QuickType.Weapon;
using UnityEngine;

public class EquipmentFusionHolder : MonoBehaviour
{
    [SerializeField] private FilterEquipFusionHelper _filterFusionEquipHelper;
    [SerializeField] private WeaponHUDFusionGridAdapter _listAdapter;
    public List<WeaponData> AvailableWeaponDatas;
    protected Action<WeaponData, WeaponDesign, EquipmentUI> onEquipClick;
    private List<WeaponHUDFusionItemModel> available;
    
    public EquipmentFusionUI GetEquipUI(WeaponData weaponData)
    {
        WeaponHUDFusionViewHolder holder =
            _listAdapter.GetCellViewsHolderIfVisible(AvailableWeaponDatas.IndexOf(weaponData));
        return holder?.WeaponButton;
    }
    
    public WeaponHUDFusionViewHolder GetViewHolder(WeaponData weaponData)
    {
        WeaponHUDFusionViewHolder holder =
            _listAdapter.GetCellViewsHolderIfVisible(AvailableWeaponDatas.IndexOf(weaponData));
        return holder;
    }

    public WeaponHUDFusionItemModel GetModel(WeaponData weaponData)
    {
        return _listAdapter.Data.First(x => x._weaponData == weaponData);
    }
    
    public void SetEquipClickCallback(Action<WeaponData, WeaponDesign, EquipmentUI> callBack)
    {
        onEquipClick = callBack;
    } 
    
    public void CreateEquipUIFromInventory(Action callBack)
    {
        if (_listAdapter.IsInitialized)
        {
            AfterInitAdapter(callBack);
        }
        else
        {
            _listAdapter.Initialized += () => AfterInitAdapter(callBack);
        }
    }
    
    private void AfterInitAdapter(Action callBack)
    {
        var wps = _filterFusionEquipHelper.GetSortedWeaponList();
        CreateWeapon(wps, callBack);
    }

    protected void CreateWeapon(List<WeaponData> wps, Action callBack)
    {
        AvailableWeaponDatas = new List<WeaponData>();
        available = new List<WeaponHUDFusionItemModel>();

        foreach (var weaponData in wps)
        {
            var wpDesign = DesignHelper.GetWeaponDesign(weaponData);
            
            if (weaponData.ItemStatus == ITEM_STATUS.Choosing)
            {
                foreach (var heroData in SaveManager.Instance.Data.Inventory.ListHeroData)
                {
                    if (heroData.UniqueID != GameConstant.MANUAL_HERO && heroData.UniqueID != "HERO_DEMO" && heroData.ItemStatus != ITEM_STATUS.Disable &&
                        heroData.ItemStatus != ITEM_STATUS.Locked)
                    {
                        if (heroData.EquippedArmour == weaponData.UniqueID ||
                            heroData.EquippedWeapon == weaponData.UniqueID)
                        {
                            available.Add(new WeaponHUDFusionItemModel(weaponData, wpDesign, onEquipClick));
                            AvailableWeaponDatas.Add(weaponData);
                            break;
                        }
                    }
                }
            }
            else
            {
                available.Add(new WeaponHUDFusionItemModel(weaponData, wpDesign, onEquipClick));
                AvailableWeaponDatas.Add(weaponData);
            }
        }

        if (_listAdapter.Data.Count != available.Count)
        {
            _listAdapter.Data.RemoveItemsFromStart(_listAdapter.Data.Count);
            _listAdapter.Data.InsertItems(0, available);
        }
        else
        {
            for (int i = 0; i < available.Count; i++)
            {
                _listAdapter.Data[i].Update(available[i]); // = available[i];
                _listAdapter.GetCellViewsHolderIfVisible(i)?.UpdateView(_listAdapter.Data[i]);
            }
        }

        callBack?.Invoke();
    }

    
    // protected override IEnumerator<float> CreateWeapon(List<WeaponData> wps, Action callBack)
    // {
    //     int count = 0;
    //
    //     foreach (var weaponData in wps)
    //     {
    //         if (weaponData.ItemStatus == ITEM_STATUS.Choosing)
    //         {
    //             foreach (var heroData in SaveManager.Instance.Data.Inventory.ListHeroData)
    //             {
    //                 if (heroData.UniqueID != GameConstant.MANUAL_HERO && heroData.ItemStatus != ITEM_STATUS.Disable &&
    //                     heroData.ItemStatus != ITEM_STATUS.Locked)
    //                 {
    //                     if (heroData.EquippedArmour == weaponData.UniqueID ||
    //                         heroData.EquippedWeapon == weaponData.UniqueID)
    //                     {
    //                         Add(weaponData);
    //                         if (count % 5 == 0)
    //                             yield return Timing.WaitForOneFrame;
    //                         break;
    //                     }
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             Add(weaponData);
    //             if (count % 5 == 0)
    //                 yield return Timing.WaitForOneFrame;
    //         }
    //
    //         count++;
    //     }
    //
    //     callBack?.Invoke();
    // }

    // public override void Add(WeaponData weaponData)
    // {
    //     var wpDesign = DesignHelper.GetWeaponDesign(weaponData);
    //
    //     if (wpDesign != null)
    //     {
    //         var equipmentUi = Pooly.Spawn<EquipmentUI>(POOLY_PREF.EQUIPMENT_UI_HUD_FUSION, Vector3.zero,
    //             Quaternion.identity,
    //             _equipmentHolderTransform);
    //         equipmentUi.transform.localScale = Vector3.one;
    //         LoadToOldUI(equipmentUi, weaponData, wpDesign);
    //         Add(equipmentUi);
    //     }
    // }
    public void ResetState()
    {
        for (int i = 0; i < available.Count; i++)
        {
            available[i].isLocked = false;
            available[i].isSelected = false;
            _listAdapter.Data[i].Update(available[i]); // = available[i];
            _listAdapter.GetCellViewsHolderIfVisible(i)?.UpdateView(_listAdapter.Data[i]);
        } 
    }
}