using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickType.Chest;
using UnityEngine;
using static AnalyticsConstant;

public class LegendaryChestHelper : RareChestHelper
{
    public override void Load()
    {
        Init();
        CheckLastReceiveTime();

        if (SaveManager.Instance.Data.Inventory.TotalLegendaryKey > 0)
        {
            ShowType(OpenResourceType.KEY_LEGENDARY);
        }
        else if (_userInventory.TotalAdsLegendaryChest > 0)
        {
            ShowType(OpenResourceType.FREE);
        }
        else
        {
            ShowType(OpenResourceType.DIAMOND);
        }
    }

    protected override void ShowType(OpenResourceType type)
    {
        _costIcon.gameObject.SetActive(true);

        switch (type)
        {
            case OpenResourceType.FREE:
                _costIcon.gameObject.SetActive(false);
                _costText.text = LOCALIZE_ID_PREF.GET_FREE.AsLocalizeString();
                break;
            case OpenResourceType.KEY_LEGENDARY:
                ResourceManager.instance.GetRewardSprite(REWARD_TYPE.KEY_CHEST_LEGENDARY,
                    s => { _costIcon.sprite = s; });
                _costText.text = $"{_userInventory.TotalLegendaryKey}/1";
                break;
            case OpenResourceType.DIAMOND:

                ResourceManager.instance.GetRewardSprite(REWARD_TYPE.DIAMOND, s => { _costIcon.sprite = s; });
                _costText.text = _chestDesignElement.DiamondCost.ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        UpdateStackText();
        _currentOpenType = type;
    }

    public override void Purchase()
    {
        switch (_currentOpenType)
        {
            case OpenResourceType.NONE:
                break;
            case OpenResourceType.FREE:
                OpenByAd();
                break;
            case OpenResourceType.ADS:
                break;
            case OpenResourceType.KEY_RARE:
                break;
            case OpenResourceType.KEY_LEGENDARY:
                OpenByKey();
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

    protected override void OpenByAd()
    {
        // Show ads
        //NetworkDetector.instance.checkInternetConnection((reached) =>
        //{

        //});

        if (_userInventory.TotalAdsLegendaryChest > 0)
        {
            _userInventory.TotalAdsLegendaryChest -= 1;
            UpdateStackText();
            GetReward(OpenResourceType.FREE);
        }
    }

    protected override void OpenByKey()
    {
        //NetworkDetector.instance.checkInternetConnection((reached) =>
        //{
        //    if (reached)
        //    {

        //    }
        //});

        if (_userInventory.TotalLegendaryKey > 0)
        {
            _userInventory.TotalLegendaryKey -= 1;
            UpdateStackText();
            GetReward(OpenResourceType.KEY_LEGENDARY);
        }
    }

    public new static int GetStack()
    {
        int result = 0;
        int quantity = 0;
        long now = TimeService.instance.GetCurrentTimeStamp();
        long lastReceiveTimeStamp = SaveManager.Instance.Data.ShopData.LastReceiveFreeAdsLegendaryChestTime;
        var chestDesign =
            DesignManager.instance.chestDesign.ChestDesignElements.Find(x =>
                x.ChestID == GameConstant.CHEST_LEGENDARY_ID);

        if (lastReceiveTimeStamp != 0)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(now - lastReceiveTimeStamp);

            SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime +=
                (float) timeSpan.TotalHours / chestDesign.ReceiveHours;

            quantity = (int) SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime;
            SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime -= quantity;

            if (SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime < 0)
                SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime = 0;
        }

        var userInventory = SaveManager.Instance.Data.Inventory;

        userInventory.TotalAdsLegendaryChest += quantity;

        if (userInventory.TotalAdsLegendaryChest > chestDesign.MaxStack)
            userInventory.TotalAdsLegendaryChest = chestDesign.MaxStack;

        SaveManager.Instance.Data.ShopData.LastReceiveFreeAdsLegendaryChestTime =
            TimeService.instance.GetCurrentTimeStamp();
        result = (int) userInventory.TotalAdsLegendaryChest + (int) userInventory.TotalLegendaryKey;
        return result;
    }

    protected virtual int CalculateQuantity()
    {
        int quantity = 0;
        long now = TimeService.instance.GetCurrentTimeStamp();
        long lastReceiveTimeStamp = SaveManager.Instance.Data.ShopData.LastReceiveFreeAdsLegendaryChestTime;
        if (lastReceiveTimeStamp != 0)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(now - lastReceiveTimeStamp);

            SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime +=
                (float) timeSpan.TotalHours / _chestDesignElement.ReceiveHours;
            // Debug.Log(
            // $"Legendary, Stack Time {SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime} hours");

            quantity = (int) SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime;
            SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime -= quantity;

            if (SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime < 0)
                SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime = 0;

            NotificationManager.Instance.ScheduleChest(ChestType.LEGENDARY,
                _chestDesignElement.ReceiveHours *
                (1 - SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime));

            // Debug.Log(
            // $"Legendary, After receive {quantity}, Stack Time {SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime} hours");
        }
        else
        {
            quantity = 0;
            NotificationManager.Instance.ScheduleChest(ChestType.LEGENDARY, _chestDesignElement.ReceiveHours);
            // Debug.Log($"Legendary, First time, get {quantity} free time");
        }

        return quantity;
    }

    protected override void CheckLastReceiveTime()
    {
        int quantity = CalculateQuantity();
        _userInventory.TotalAdsLegendaryChest += quantity;

        if (_userInventory.TotalAdsLegendaryChest > _chestDesignElement.MaxStack)
            _userInventory.TotalAdsLegendaryChest = _chestDesignElement.MaxStack;

        SaveManager.Instance.Data.ShopData.LastReceiveFreeAdsLegendaryChestTime =
            TimeService.instance.GetCurrentTimeStamp();
        SaveManager.Instance.SetDataDirty();
        UpdateTimeText();
    }

    protected override string GetChestID()
    {
        return GameConstant.CHEST_LEGENDARY_ID;
    }

    protected override void UpdateStackText()
    {
        int total = (int) _userInventory.TotalAdsLegendaryChest + (int) _userInventory.TotalLegendaryKey;
        _reminderUi.Load(total);
    }

    protected override void UpdateTimeText()
    {
        // TimeSpan temp = TimeSpan.FromHours(24);
        // Debug.LogError($"{temp.TotalHours:D2} {temp.Minutes}");

        TimeSpan utilNextFree =
            TimeSpan.FromHours((1 - SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime) *
                               _chestDesignElement.ReceiveHours);

        int hour = (int) utilNextFree.TotalHours;
        int minute = (int) utilNextFree.Minutes;

        _hourText.text = $"{hour}";
        _minuteText.text = $"{minute}";

        // Debug.LogError(SaveManager.Instance.Data.ShopData.StackFreeAdsLegendaryChestTime + " " +
        // _chestDesignElement.ReceiveHours + " " + utilNextFree.ToString(@"hh\:mm"));
    }
}