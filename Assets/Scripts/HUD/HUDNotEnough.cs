using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Doozy.Engine.UI.Input;
using QuickType.Shop;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class HUDNotEnough : BaseHUD
{
    [SerializeField] private LocalizedTMPTextUI title;
    [SerializeField] private LocalizedTMPTextUI quantityText;
    [SerializeField] private Image icon;
    [SerializeField] private ShopItemUI _shopItemUi;

    [SerializeField] private GameObject _itemHolder;
    [SerializeField] private GameObject _buttonHolder;
    [SerializeField] private Sprite _goldBG;
    [SerializeField] private Sprite _wpCoinBG;
    [SerializeField] private Sprite _potionBG;

    private Action goToShop;
    private MyPopup _myPopup;

    public override void Awake()
    {
        base.Awake();
        refreshLastLayer = false;
        _myPopup = GetComponent<MyPopup>();
    }

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);

        GamePlayController.instance?.SetPauseGameplay(true);

        _myPopup.OnShow();
        CurrencyType currencyType = (CurrencyType)args[0];
        long quantity = (long)args[1];

        ResourceManager.instance.GetCurrencySprite(currencyType, icon);
        long remain = quantity - CurrencyModels.instance.GetValueFromEnum(currencyType);
        quantityText.text = remain.ToString();

        Action onOpenShop = null;

        switch (currencyType)
        {
            case CurrencyType.DIAMOND:
                _buttonHolder.SetActive(true);
                _itemHolder.SetActive(false);
                title.text =
                    $"{LOCALIZE_ID_PREF.NOT_ENOUGH_RESOURCES.AsLocalizeString()}. {LOCALIZE_ID_PREF.GET_MORE_NOW.AsLocalizeString()}"; // "Not enough resource. Get more now";}
                _shopItemUi.gameObject.SetActive(false);
                var availablePack = GetNextAvailablePack(ShopType.DIAMOND, remain);

                if (GamePlayController.instance != null)
                {
                    goToShop = () =>
                    {
                        InGameCanvas.instance.ShowShop(INGAME_SHOP.GEM);
                        InGameCanvas.instance.HighlightShopPack(availablePack);
                    };
                }
                else
                {
                    onOpenShop = () =>
                    {
                        HUDShop.Instance.SnapToShopType(ShopType.DIAMOND);
                        DOVirtual.DelayedCall(0.2f, () => { HUDShop.Instance.HighLightPackage(availablePack); });
                    };

                    goToShop = () => { MainMenuCanvas.instance.ShowHUD(EnumHUD.HUD_SHOP, true, null, onOpenShop); };
                }


                break;
            case CurrencyType.GOLD:
            // onOpenShop = () => HUDShop.Instance.SnapToShopType(ShopType.GOLD);
            case CurrencyType.WEAPON_SCROLL:
            // onOpenShop = () => HUDShop.Instance.SnapToShopType(ShopType.WEAPON_COIN);
            case CurrencyType.PILL:
                // onOpenShop = () => HUDShop.Instance.SnapToShopType(ShopType.POTION);
                _buttonHolder.SetActive(false);
                _itemHolder.SetActive(true);
                LoadInstancePurchase(currencyType, remain);
                goToShop = () => { OnButtonBack(); };
                title.textName = LOCALIZE_ID_PREF.NOT_ENOUGH_RESOURCES;
                break;
            
            default:
                MasterCanvas.CurrentMasterCanvas.ShowFloatingTextNotify(LOCALIZE_ID_PREF.NOT_ENOUGH_RESOURCES.AsLocalizeString());
                break;
        }
    }

    private void LoadInstancePurchase(CurrencyType type, long remain)
    {
        _shopItemUi.gameObject.SetActive(true);
        ShopType shopType = ShopType.GOLD;

        Sprite bg = null;
        switch (type)
        {
            case CurrencyType.GOLD:
                shopType = ShopType.GOLD;
                bg = _goldBG;
                break;
            case CurrencyType.PILL:
                shopType = ShopType.POTION;
                bg = _potionBG;
                break;
            case CurrencyType.WEAPON_SCROLL:
                shopType = ShopType.WEAPON_COIN;
                bg = _wpCoinBG;
                break;
        }

        var availablePack = GetNextAvailablePack(shopType, remain);
        if (availablePack != null)
        {
            switch (shopType)
            {
                case ShopType.GOLD:
                case ShopType.WEAPON_COIN:
                    if (SaveGameHelper.IsFreePack(shopType, availablePack))
                    {
                        availablePack = SaveGameHelper.ConvertToFreePack(shopType, availablePack);
                    }
                    break;
            }
            
            _shopItemUi.Load(availablePack);
            _shopItemUi.SetOnPurchaseCallback(OnPurchase);
            _shopItemUi.SetBg(bg);
        }
    }

    private ShopDesignElement GetNextAvailablePack(ShopType shopType, long remain)
    {
        ShopDesignElement result = null;

        foreach (var shopDesignElement in DesignManager.instance.shopDesign.ShopDesignElement)
        {
            if (shopDesignElement.Id.Contains(shopType.ToString()))
            {
                shopDesignElement.ResetData();
                result = shopDesignElement;

                var reward = shopDesignElement.GetRewardWithBonus();
                if (reward[0]._value >= remain)
                {
                    return shopDesignElement;
                }
            }
        }

        return result;
    }

    public void GoToShop()
    {
        OnButtonBack();
        // if (!MasterCanvas.IsInGameScene)
        goToShop?.Invoke();
    }

    public override void Hide(Action<bool> hideComplete = null)
    {
        base.Hide(hideComplete);
        GamePlayController.instance?.SetPauseGameplay(false);
    }

    public virtual void OnPurchase(CostData costData, List<RewardData> rewardDatas, ShopItemUI shopItemUi,
        Action<bool> callback = null)
    {
        //NetworkDetector.instance.checkInternetConnection((reached) =>
        //{
        //    if (reached)
        //    {
        //        AddReward(rewardDatas);
        //        MasterCanvas.CurrentMasterCanvas.SpawnCollectAnim(rewardDatas, shopItemUi.transform.position, 0, 5,
        //            100);
        //    }

        //    OnButtonBack();
        //    callback?.Invoke(reached);
        //});

        AddReward(rewardDatas);
        MasterCanvas.CurrentMasterCanvas.SpawnCollectAnim(rewardDatas, shopItemUi.transform.position, 0, 5,
            100);

        OnButtonBack();
        callback?.Invoke(true);
        
        if (SaveGameHelper.IsFreePack(shopItemUi.ShopDesignElement))
        {
            SaveGameHelper.SaveBoughFreePack(shopItemUi.ShopDesignElement);
        }
    }

    public void AddReward(List<RewardData> rewardDatas)
    {
        foreach (var rewardData in rewardDatas)
        {
            SaveManager.Instance.Data.AddReward(rewardData);
        }
    }
}