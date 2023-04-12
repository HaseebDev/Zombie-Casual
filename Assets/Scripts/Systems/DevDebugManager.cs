using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using SRF;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class DevDebugManager : MonoBehaviour
{
    // [SerializeField] private HUDHomeMenu _hudHomeMenu;
    // [SerializeField] private Toggle _promotionToggle;
    // [SerializeField] private Toggle _locationPackToggle;
    //
    // private void Awake()
    // {
    //     _promotionToggle.isOn = PlayerPrefs.GetInt("debug_on_promotion", 0) == 1;
    //     _locationPackToggle.isOn = PlayerPrefs.GetInt("debug_on_location", 0) == 1;
    //
    //     _promotionToggle.onValueChanged.AddListener(v => { PlayerPrefs.SetInt("debug_on_promotion", v ? 1 : 0); });
    //     _locationPackToggle.onValueChanged.AddListener(v => { PlayerPrefs.SetInt("debug_on_location", v ? 1 : 0); });
    // }
    //
    // public void AddRandomAddOn()
    // {
    //     for (int i = 0; i < 5; i++)
    //     {
    //         var randomId =
    //             DesignManager.instance.skillDesign.Skills
    //                 .Random(); //[Random.Range(0,DesignManager.instance.skillDesign.Skills.Count)];
    //         
    //         while (randomId.SkillType != SkillType.ADD_ON)
    //         {
    //             randomId =
    //                 DesignManager.instance.skillDesign.Skills
    //                     .Random();
    //         }
    //
    //         // if(SaveManager.Instance.Data.Inventory.ListAddOnItems.ToList().Find(x=>x.ItemID == randomId.SkillId) == null)
    //         // SaveManager.Instance.Data.AddAddONItem(randomId.SkillId, 2);
    //         // else
    //         SaveManager.Instance.Data.AddAddONItem(randomId.SkillId, 2);
    //         // Debug.LogError("ADD id " + randomId.SkillId);
    //     }
    //
    //     _hudHomeMenu.ResetLayers();
    //     // foreach (var item in SaveManager.Instance.Data.Inventory.ListAddOnItems)
    //     // {
    //     //     bool add = UnityEngine.Random.Range(0, 2) == 1;
    //     //     if (add)
    //     //     {
    //     //         item.ItemCount += 100;
    //     //         if (item.Status == com.datld.data.ITEM_STATUS.Disable)
    //     //             item.Status = com.datld.data.ITEM_STATUS.Available;
    //     //     }
    //     // }
    // }
    //
    // public void AddRandomHero()
    // {
    //     var allHeroes = SaveManager.Instance.Data.Inventory.ListHeroData.ToList();
    //     foreach (var id in allHeroes)
    //     {
    //         if (id.ItemStatus == ITEM_STATUS.Locked)
    //         {
    //             id.UnlockHero();
    //             Debug.LogError("ADD HERO " + id.UniqueID);
    //             break;
    //         }
    //     }
    //
    //     _hudHomeMenu.ResetLayers();
    // }
    //
    // public bool IsHidePromotion()
    // {
    //     return !_promotionToggle.isOn;
    // }
    //
    // public bool IsHideLocationPack()
    // {
    //     return !_locationPackToggle.isOn;
    // }
    //
    // public bool IsDebug()
    // {
    //     return true;
    // }
    //
    // public void IncreaseLevel(int level)
    // {
    //     SaveManager.Instance.Data.GameData.CampaignProgress.CurrentLevel += level;
    //     _hudHomeMenu.ResetLayers();
    // }
    //
    // public void CheatLevel(int level)
    // {
    //     SaveManager.Instance.Data.GameData.CampaignProgress.CurrentLevel = level;
    //     _hudHomeMenu.ResetLayers();
    // }
    //
    // public void ShowFlyingText()
    // {
    //     MainMenuCanvas.instance.ShowFloatingTextNotify("HELLO");
    // }
}