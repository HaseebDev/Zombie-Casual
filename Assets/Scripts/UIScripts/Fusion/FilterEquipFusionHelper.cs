using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using Doozy.Engine.Utils.ColorModels;
using UnityEngine;

public class FilterEquipFusionHelper : MonoBehaviour
{
    [SerializeField] private EquipmentFusionHolder _equipmentHolder;
    private List<WeaponData> tempWeapons;
    
    public void UpdateFilter()
    {
        // List<Tuple<EquipmentUI, long>> orders = new List<Tuple<EquipmentUI, long>>();
        //
        // foreach (var equipmentUi in _equipmentHolder.Equipments)
        // {
        //     Tuple<EquipmentUI, long> newOrder = new Tuple<EquipmentUI, long>(
        //         equipmentUi,
        //         TotalScore(equipmentUi.WeaponData)
        //     );
        //     orders.Add(newOrder);
        // }
        //
        // orders = orders.OrderBy(x => -x.Item2).ToList();
        // foreach (var order in orders)
        // {
        //     order.Item1.transform.SetAsLastSibling();
        // }
    }

    public List<WeaponData> GetSortedWeaponList()
    {
        tempWeapons = SaveManager.Instance.Data.Inventory.ListWeaponData.ToList();
        List<Tuple<WeaponData, long>> orders = new List<Tuple<WeaponData, long>>();

        for (int i = tempWeapons.Count - 1; i >= 0; i--)
        {
            var hero = SaveGameHelper.GetEquipByHeroIncludeAllHero(tempWeapons[i]);
            if (hero != null)
            {
                // Debug.LogError("PRE " + hero.UniqueID);
                if (hero.UniqueID == GameConstant.MANUAL_HERO || hero.UniqueID == "HERO_DEMO" || hero.ItemStatus == ITEM_STATUS.Locked || hero.ItemStatus == ITEM_STATUS.None || hero.ItemStatus == ITEM_STATUS.Disable)
                {
                    // Debug.LogError(hero.UniqueID);
                    tempWeapons.RemoveAt(i);
                }
            }
        }
        
        foreach (var wpData in tempWeapons)
        {
            // Debug.LogError($"INVENTORY {wpData.WeaponID}");
            var equipHero = SaveGameHelper.GetEquipByHero(wpData);
            if (equipHero == null || equipHero.ItemStatus != ITEM_STATUS.Locked)
            {
                // if(equipHero != null)
                    // Debug.LogError($"Weapon {wpData.WeaponID}, HERO {equipHero.UniqueID}, item status {equipHero.ItemStatus}");
                
                Tuple<WeaponData, long> newOrder = new Tuple<WeaponData, long>(
                    wpData,
                    TotalScore(wpData)
                );
            
                orders.Add(newOrder); 
            }
        }

        orders = orders.OrderBy(x => -x.Item2).ToList();
  
        var result = new List<WeaponData>();
        foreach (var VARIABLE in orders)
        {
            result.Add(VARIABLE.Item1);
            // Debug.LogError($"{VARIABLE.Item1.WeaponID}, {VARIABLE.Item2}");
        }

        return result;
    }

    private long Count(WeaponData weaponData)
    {
        var count = tempWeapons.Count(s =>
            s.WeaponID == weaponData.WeaponID &&
            s.Rank == weaponData.Rank);
        
        return count;
    }

    private long TotalScore(WeaponData weaponData)
    {
        var wpDesign = DesignHelper.GetWeaponDesign(weaponData);
        
        long count = Count(weaponData); 
        long quantityScore = count * 100000;
        long weaponTypeScore = (int)wpDesign.EquipType * -10000;
        var nameScore = DesignHelper.GetWeaponNameScore(weaponData.WeaponID) * 1000;
        long rankScore = weaponData.Rank * 100;
        long levelScore = weaponData.GetWeaponLevel() * 1;

        // long total = quantityScore + nameScore + weaponTypeScore + rankScore + levelScore;
        // Debug.LogError($"WEAPON {weaponData.WeaponID}, {quantityScore} {weaponTypeScore} {nameScore} {rankScore} {levelScore}");
        
        return quantityScore + nameScore + weaponTypeScore + rankScore + levelScore;
    }
}