using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using CustomListView.Weapon;
using DG.Tweening;
using Ez.Pooly;
using MEC;
using QuickType.Weapon;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentHolder : MonoBehaviour
{
    public FilterEquipHelper _filterEquipHelper;
    [SerializeField] private WeaponHUDEquipmentGridAdapter _listAdapter;
    public List<WeaponData> AvailableWeaponDatas;
    
    protected Action<WeaponData, WeaponDesign, EquipmentUI> onEquipClick;

    protected virtual void Start()
    {
        _filterEquipHelper.AddOnNewFilterCallback(() => CreateEquipUIFromInventory(null));
    }

    public EquipmentUI GetEquipUI(WeaponData weaponData)
    {
        WeaponHUDEquipmentViewHolder holder =
            _listAdapter.GetCellViewsHolderIfVisible(AvailableWeaponDatas.IndexOf(weaponData));
        return holder?.WeaponButton;
    }

    public void SetEquipClickCallback(Action<WeaponData, WeaponDesign, EquipmentUI> callBack)
    {
        onEquipClick = callBack;
    }

    public virtual void CreateEquipUIFromInventory(Action callBack)
    {
        // _listAdapter.Initialized += () => AfterInitAdapter(callBack);
        _listAdapter.Init();

        DOVirtual.DelayedCall(0.1f, () =>
        {       
            if (_listAdapter.IsInitialized)
            {
                AfterInitAdapter(callBack);
            }
            // else
            // {
            // }  
        });
     
    }

    private void AfterInitAdapter(Action callBack)
    {
        //Timing.KillCoroutines(_createWpCoroutine);
        var wps = _filterEquipHelper.GetSortedWeaponList();
        CreateWeapon(wps, callBack);
    }

    protected virtual void CreateWeapon(List<WeaponData> wps, Action callBack)
    {
        AvailableWeaponDatas = new List<WeaponData>();
        var available = new List<WeaponHUDEquipmentItemModel>();
        var hasNewWeapon = ReminderManager.HasNewEquip();

        foreach (var weaponData in wps)
        {
            if (weaponData.ItemStatus != ITEM_STATUS.Choosing)
            {
                var wpDesign = DesignHelper.GetWeaponDesign(weaponData);
                available.Add(new WeaponHUDEquipmentItemModel(weaponData, wpDesign, onEquipClick,
                    hasNewWeapon.Item1 && hasNewWeapon.Item2.Contains(weaponData)));
                AvailableWeaponDatas.Add(weaponData);
            }
        }

        if (hasNewWeapon.Item1)
            ReminderManager.SaveCurrentWeaponsState();

        available = available.OrderBy(x => !x.showReminder).ToList();

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

    public void OnShowFullHide(WeaponData weaponData)
    {
        if (weaponData != null)
        {
            var value = GetEquipUI(weaponData);
            value?.ShowReminder(false);
            value?.Reload();
        }
    }

    public void CleanUp()
    {
    }

    public void OpenHUDFusion()
    {
        MainMenuCanvas.instance.ShowHUD(EnumHUD.HUD_FUSION);
    }
}