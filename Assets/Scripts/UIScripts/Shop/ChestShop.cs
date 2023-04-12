using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using QuickType.Chest;
using UnityEngine;

public enum ChestType
{
    RARE,
    LEGENDARY
}

public enum OpenResourceType
{
    NONE,
    FREE,
    ADS,
    KEY_RARE,
    KEY_LEGENDARY,
    DIAMOND,
    GOLD
}

public class ChestShop : MonoBehaviour
{
    [SerializeField] private RareChestHelper _rareChestHelper;
    [SerializeField] private LegendaryChestHelper _legendaryChestHelper;
    [SerializeField] private TenLegendaryChestHelper _tenLegendaryChestHelper;
    public ChestOpenResult _chestOpenResultUi;
    public ChestOpenTenResult _chestOpenTenResult;

    public void Init()
    {
        _rareChestHelper.OnOpenChest += OnOpenChest;
        _legendaryChestHelper.OnOpenChest += OnOpenChest;
        _tenLegendaryChestHelper.OnOpenChest += OnOpenChestTen;
    }

    public void Load(bool isFirstTime = false)
    {
        _rareChestHelper.Load();
        _legendaryChestHelper.Load();
        _tenLegendaryChestHelper.Load();
        // if (!isFirstTime)
        // {
        //     _chestOpenResultUi.gameObject.SetActive(false);
        //     _chestOpenTenResult.gameObject.SetActive(false);
        // }
    }

    private void OnOpenChest(ChestDesignElement chestDesignElement, OpenResourceType openResourceType)
    {
        if (_chestOpenResultUi == null)
        {
            Timing.RunCoroutine(WaitCoroutine(() =>
            {
                _chestOpenResultUi.OpenChest(chestDesignElement, openResourceType);
            }));
        }
        else
        {
            _chestOpenResultUi.OpenChest(chestDesignElement, openResourceType);
        }
    }

    private void OnOpenChestTen(ChestDesignElement chestDesignElement, OpenResourceType openResourceType)
    {
        if (_chestOpenTenResult == null)
        {
            Timing.RunCoroutine(WaitCoroutine(() =>
            {
                _chestOpenTenResult.OpenChest(chestDesignElement, openResourceType);
            }));
        }
        else
        {
            _chestOpenTenResult.OpenChest(chestDesignElement, openResourceType);
        }
    }

    private IEnumerator<float> WaitCoroutine(Action callback)
    {
        TopLayerCanvas.instance.ShowHUDLoadingView(true);
        while (_chestOpenResultUi == null || _chestOpenTenResult == null)
        {
            yield return Timing.WaitForOneFrame;
        }

        TopLayerCanvas.instance.ShowHUDLoadingView(false);
        callback?.Invoke();
    }

    public void HideAllResultPanel()
    {
        if (_chestOpenResultUi != null)
            _chestOpenResultUi.gameObject.SetActive(false);
        if (_chestOpenTenResult != null)
            _chestOpenTenResult.gameObject.SetActive(false);
    }
}