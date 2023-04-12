using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickType;
using QuickType.Chest;
using QuickType.Shop;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;
using static AnalyticsConstant;

public class RareChestHelper : MonoBehaviour
{
    // [SerializeField] protected Button _openByAdsBtn;
    // [SerializeField] protected Button _openByKeyBtn;
    // [SerializeField] protected Button _openByDiamondBtn;
    [SerializeField] protected LocalizedTMPTextUI _hourText;
    [SerializeField] protected LocalizedTMPTextUI _minuteText;
    [SerializeField] protected LocalizedTMPTextUI _costText;
    [SerializeField] protected ReminderUI _reminderUi;

    [SerializeField] protected Image _costIcon;

    // [SerializeField] protected Text _keyRemainText;
    // [SerializeField] protected Text _diamondCostText;
    // [SerializeField] protected ReminderUI _stackAdsvalueText;
    [SerializeField] protected ChestType _chestType;

    protected OpenResourceType _currentOpenType = OpenResourceType.FREE;
    protected UserInventory _userInventory;
    protected ChestDesignElement _chestDesignElement;
    public Action<ChestDesignElement, OpenResourceType> OnOpenChest { get; set; }

    protected virtual void Init()
    {
        if (_chestDesignElement == null)
        {
            _userInventory = SaveManager.Instance.Data.Inventory;
            _chestDesignElement =
                DesignManager.instance.chestDesign.ChestDesignElements.Find(x => x.ChestID == GetChestID());
        }
    }


    public virtual void Load()
    {
        Init();
        CheckLastReceiveTime();

        if (SaveManager.Instance.Data.Inventory.TotalRareKey > 0)
        {
            ShowType(OpenResourceType.KEY_RARE);
        }
        else if (_userInventory.TotalAdsRareChest > 0)
        {
            ShowType(OpenResourceType.ADS);
        }
        else
        {
            ShowType(OpenResourceType.DIAMOND);
        }
    }

    // protected virtual void HideAllBuyButtons()
    // {
    //     _openByKeyBtn.gameObject.SetActive(false);
    //     _openByAdsBtn.gameObject.SetActive(false);
    //     _openByDiamondBtn.gameObject.SetActive(false);
    // }

    protected virtual void ShowType(OpenResourceType type)
    {
        // HideAllBuyButtons();
        switch (type)
        {
            case OpenResourceType.ADS:
                _costText.text = LOCALIZE_ID_PREF.FREE_DRAW.AsLocalizeString();
                ResourceManager.instance.GetCostTypeSprite(CostType.ADS, s => { _costIcon.sprite = s; });

                // _openByAdsBtn.gameObject.SetActive(true);
                break;
            case OpenResourceType.KEY_RARE:
                _costText.text = $"{_userInventory.TotalRareKey}/1";
                ResourceManager.instance.GetRewardSprite(REWARD_TYPE.KEY_CHEST_RARE, s => { _costIcon.sprite = s; });

                // _openByKeyBtn.gameObject.SetActive(true);
                // _keyRemainText.text = $"{_userInventory.TotalRareKey}/1";
                break;
            case OpenResourceType.DIAMOND:
                ResourceManager.instance.GetRewardSprite(REWARD_TYPE.DIAMOND, s => { _costIcon.sprite = s; });
                _costText.text = _chestDesignElement.DiamondCost.ToString();

                // _openByDiamondBtn.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        UpdateStackText();
        _currentOpenType = type;
    }

    public virtual void Purchase()
    {
        switch (_currentOpenType)
        {
            case OpenResourceType.NONE:
                break;
            case OpenResourceType.FREE:
                break;
            case OpenResourceType.ADS:
                OpenByAd();
                break;
            case OpenResourceType.KEY_RARE:
                OpenByKey();
                break;
            case OpenResourceType.KEY_LEGENDARY:
                break;
            case OpenResourceType.DIAMOND:
                OpenByDiamond();
                break;
            case OpenResourceType.GOLD:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        AnalyticsManager.instance.LogEvent(ANALYTICS_ENUM.PURCHASE_CHEST, new LogEventParam("chest-id", GetChestID()));
    }

    protected virtual void OpenByAd()
    {
        //NetworkDetector.instance.checkInternetConnection((reached) =>
        //{
        //    if (reached)
        //    {
        //        // Show ads
        //        if (_userInventory.TotalAdsRareChest > 0)
        //        {
        //            AdsManager.instance.ShowAdsRewardWithNotify(() =>
        //            {
        //                _userInventory.TotalAdsRareChest -= 1;
        //                UpdateStackText();
        //                GetReward(OpenResourceType.ADS);
        //            });
        //        }
        //    }
        //});

        // Show ads
        if (_userInventory.TotalAdsRareChest > 0)
        {
            AdsManager.instance.ShowAdsRewardWithNotify(() =>
            {
                _userInventory.TotalAdsRareChest -= 1;
                UpdateStackText();
                GetReward(OpenResourceType.ADS);
                
                MissionManager.Instance.TriggerMission(MissionType.WATCH_VIDEO_OPEN_CHEST);
            });
        }
    }

    protected void OpenByDiamond()
    {
        //NetworkDetector.instance.checkInternetConnection((reached) =>
        //{
        //    if (reached)
        //    {
             
        //    }
        //});

        if (CurrencyModels.instance.IsEnough(CurrencyType.DIAMOND, _chestDesignElement.DiamondCost))
        {
            CurrencyModels.instance.AddCurrency(CurrencyType.DIAMOND, -_chestDesignElement.DiamondCost);
            GetReward(OpenResourceType.DIAMOND);
        }
        else
        {
            MainMenuCanvas.instance.ShowNotEnoughHUD(CurrencyType.DIAMOND, _chestDesignElement.DiamondCost);
        }
    }

    protected virtual void OpenByKey()
    {
        void MinusKey()
        {
            if (_userInventory.TotalRareKey > 0)
            {
                _userInventory.TotalRareKey -= 1;
                UpdateStackText();
                GetReward(OpenResourceType.KEY_RARE);
            }
        }

        // Skip check internet if tutorial
        if (TutorialManager.instance.CurrentTutorialPhaseIndex == 4)
        {
            MinusKey();
        }
        else
        {
            //NetworkDetector.instance.checkInternetConnection((reached) =>
            //{
            //    if (reached)
            //    {
                    
            //    }
            //});

            MinusKey();
        }
    }

    protected void GetReward(OpenResourceType type)
    {
        OnOpenChest?.Invoke(_chestDesignElement, type);
        Load();
    }

    public static int GetStack()
    {
        int result = 0;
        int quantity = 1;

        long now = TimeService.instance.GetCurrentTimeStamp();
        long lastReceiveTimeStamp = SaveManager.Instance.Data.ShopData.LastReceiveFreeAdsRareChestTime;
        var chestDesign =
            DesignManager.instance.chestDesign.ChestDesignElements.Find(x => x.ChestID == GameConstant.CHEST_RARE_ID);
        if (lastReceiveTimeStamp != 0)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(now - lastReceiveTimeStamp);

            SaveManager.Instance.Data.ShopData.StackFreeAdsRareChestTime +=
                (float) timeSpan.TotalHours / chestDesign.ReceiveHours;

            quantity = (int) SaveManager.Instance.Data.ShopData.StackFreeAdsRareChestTime;
            SaveManager.Instance.Data.ShopData.StackFreeAdsRareChestTime -= quantity;

            if (SaveManager.Instance.Data.ShopData.StackFreeAdsRareChestTime < 0)
                SaveManager.Instance.Data.ShopData.StackFreeAdsRareChestTime = 0;
        }

        var userInventory = SaveManager.Instance.Data.Inventory;

        userInventory.TotalAdsRareChest += quantity;

        if (userInventory.TotalAdsRareChest > chestDesign.MaxStack)
            userInventory.TotalAdsRareChest = chestDesign.MaxStack;

        SaveManager.Instance.Data.ShopData.LastReceiveFreeAdsRareChestTime = TimeService.instance.GetCurrentTimeStamp();
        result = (int) userInventory.TotalAdsRareChest + (int) userInventory.TotalRareKey;
        return result;
    }

    protected virtual int CalculateQuantity()
    {
        int quantity = 0;
        long now = TimeService.instance.GetCurrentTimeStamp();
        long lastReceiveTimeStamp = SaveManager.Instance.Data.ShopData.LastReceiveFreeAdsRareChestTime;

        if (lastReceiveTimeStamp != 0)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(now - lastReceiveTimeStamp);

            SaveManager.Instance.Data.ShopData.StackFreeAdsRareChestTime +=
                (float) timeSpan.TotalHours / _chestDesignElement.ReceiveHours;
            // Debug.Log($"Rare, Stack Time {SaveManager.Instance.Data.ShopData.StackFreeAdsRareChestTime} hours");

            quantity = (int) SaveManager.Instance.Data.ShopData.StackFreeAdsRareChestTime;
            SaveManager.Instance.Data.ShopData.StackFreeAdsRareChestTime -= quantity;

            if (SaveManager.Instance.Data.ShopData.StackFreeAdsRareChestTime < 0)
                SaveManager.Instance.Data.ShopData.StackFreeAdsRareChestTime = 0;

            NotificationManager.Instance.ScheduleChest(ChestType.RARE,
                _chestDesignElement.ReceiveHours * (1 - SaveManager.Instance.Data.ShopData.StackFreeAdsRareChestTime));

            // Debug.Log(
            // $"Rare, After receive {quantity}, Stack Time {SaveManager.Instance.Data.ShopData.StackFreeAdsRareChestTime} hours");
        }
        else
        {
            quantity = 1;
            NotificationManager.Instance.ScheduleChest(ChestType.RARE, _chestDesignElement.ReceiveHours);
            // Debug.Log($"Rare, First time, get {quantity} free time");
        }

        return quantity;
    }

    protected virtual void CheckLastReceiveTime()
    {
        int quantity = CalculateQuantity();
        _userInventory.TotalAdsRareChest += quantity;

        if (_userInventory.TotalAdsRareChest > _chestDesignElement.MaxStack)
            _userInventory.TotalAdsRareChest = _chestDesignElement.MaxStack;

        SaveManager.Instance.Data.ShopData.LastReceiveFreeAdsRareChestTime = TimeService.instance.GetCurrentTimeStamp();
        SaveManager.Instance.SetDataDirty();
        UpdateTimeText();
    }

    protected virtual string GetChestID()
    {
        return GameConstant.CHEST_RARE_ID;
    }

    protected virtual void UpdateStackText()
    {
        int total = (int) _userInventory.TotalAdsRareChest + (int) _userInventory.TotalRareKey;
        _reminderUi.Load(total);
    }

    protected virtual void UpdateTimeText()
    {
        TimeSpan utilNextFree =
            TimeSpan.FromHours((1 - SaveManager.Instance.Data.ShopData.StackFreeAdsRareChestTime) *
                               _chestDesignElement.ReceiveHours);

        int hour = (int) utilNextFree.TotalHours;
        int minute = (int) utilNextFree.Minutes;

        _hourText.text = $"{hour}";
        _minuteText.text = $"{minute}";

        // Debug.LogError(SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime + " " +
        // _chestDesignElement.ReceiveHours + " " + utilNextFree.ToString(@"hh\:mm"));
    }
}