using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIEffects;
using QuickType;
using QuickType.Shop;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;
using static AnalyticsConstant;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] protected Image _background;
    [SerializeField] protected Image _icon;
    [SerializeField] protected Image _iconCurrency;
    [SerializeField] protected LocalizedTMPTextUI _nameText;
    [SerializeField] protected LocalizedTMPTextUI _valueText;
    [SerializeField] protected LocalizedTMPTextUI _newText;
    [SerializeField] protected LocalizedTMPTextUI _oldText;
    [SerializeField] protected Button _buyButton;

    [SerializeField] protected GameObject _bonusPanel;
    [SerializeField] protected LocalizedTMPTextUI _bonusText;
    [SerializeField] protected GameObject _goodChoiceImg;
    [SerializeField] protected ReminderUI _reminderUi;
    [SerializeField] protected UIShiny _uiShiny;
    [SerializeField] protected LoadIAPButton _loadIapButton;

    protected ShopDesignElement _shopDesignElement;
    public ShopDesignElement ShopDesignElement => _shopDesignElement;
    private Action<CostData, List<RewardData>, ShopItemUI, Action<bool>> _onPurchase;
    protected int _value;


    public void SetOnPurchaseCallback(Action<CostData, List<RewardData>, ShopItemUI, Action<bool>> callback)
    {
        _onPurchase = callback;
    }

    public void ShowReminder(bool isShow)
    {
        _reminderUi?.Show(isShow);
    }

    private void Awake()
    {
        _buyButton.onClick.AddListener(OnPurchase);
    }

    private void OnPurchase()
    {
        //NetworkDetector.instance.checkInternetConnection((reached) =>
        //{
        //    if (!reached)
        //        return;
        //    ContinuePurchase();
        //});

        ContinuePurchase();

        AnalyticsManager.instance.LogEvent(ANALYTICS_ENUM.PURCHASE_SHOP_ITEM,
            new LogEventParam("item-id", _shopDesignElement.Id));
    }

    public void SetScale(float scale)
    {
        _icon.transform.localScale = Vector3.one * scale;
    }


    private bool checkDuplicate = false;

    private void ContinuePurchase()
    {
        checkDuplicate = true;
        var costData = _shopDesignElement.GetCost();
        List<RewardData> rewardDatas = _shopDesignElement.GetRewardWithBonus();

        switch (costData.Type)
        {
            case CostType.NONE:
                break;
            case CostType.IAP:
                IAPManager.instance.PurchaseIAP(_shopDesignElement.getIAPProductID(), isSuccess =>
                {
                    // Request IAP 
                    if (isSuccess)
                    {
                        _onPurchase?.Invoke(costData, rewardDatas, this, null);
                    }
                    else
                    {
                        MasterCanvas.CurrentMasterCanvas.ShowPurchaseFail();
                    }
                });

                break;
            case CostType.DIAMOND:
            case CostType.GOLD:
                CurrencyType type = costData.Type.ConvertToCurrencyType();
                if (CurrencyModels.instance.IsEnough(type, (long)costData.Value))
                {
                    CurrencyModels.instance.AddCurrency(type, (long)-costData.Value);
                    _onPurchase?.Invoke(costData, rewardDatas, this, null);
                }
                else
                {
                    MasterCanvas.CurrentMasterCanvas.ShowNotEnoughHUD(type, (long)costData.Value);
                }

                break;
            case CostType.FREE:
                _onPurchase?.Invoke(costData, rewardDatas, this, null);
                break;
            case CostType.ADS:
                AdsManager.instance.ShowAdsRewardWithNotify(() =>
                {
                    // TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING, null, true);
                    if (checkDuplicate)
                    {
                        TopLayerCanvas.instance.ShowHUDLoading(false);
                        _onPurchase?.Invoke(costData, rewardDatas, this, null);
                        if (_shopDesignElement.isFreePack)
                        {
                            MissionManager.Instance.TriggerMission(MissionType.GET_FREE_RESOURCE,rewardDatas[0]._type);
                        }
                    }
                    checkDuplicate = false;

                });
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected virtual void OnEnable()
    {
        // Skip reload diamond pack
        if (_shopDesignElement != null)
        {
            _shopDesignElement.ResetData();
            var rewards = _shopDesignElement.GetReward();

            if (_value != (int)rewards[0]._value)
            {
                Load(_shopDesignElement);
            }
        }
    }

    public virtual void Load(ShopDesignElement shopDesignElement)
    {
        _shopDesignElement = shopDesignElement;

        ResourceManager.instance.GetShopItemSprite(shopDesignElement.Icon, s => { _icon.sprite = s; });
        _nameText.textName = shopDesignElement.Name;
        var rewards = shopDesignElement.GetReward();
        _valueText.text = FBUtils.CurrencyAddComma(rewards[0]._value); //.ToString();
        _value = (int)rewards[0]._value;

        CostData costData = shopDesignElement.GetCost();
        _newText.text = costData.PriceStr;

        if (_oldText != null)
        {
            _oldText.text = FBUtils.CurrencyAddComma(_shopDesignElement.OldCostValue); //.ToString();
            _oldText.gameObject.SetActive(_shopDesignElement.OldCostValue != 0);
        }

        ResourceManager.instance.GetCostTypeSprite(costData.Type, s => { _iconCurrency.sprite = s; });

        _iconCurrency.gameObject.SetActive(true);
        switch (costData.Type)
        {
            case CostType.FREE:
                _iconCurrency.gameObject.SetActive(false);
                _newText.text = LOCALIZE_ID_PREF.FREE.AsLocalizeString();//.AsLocalizeString();
                break;
            case CostType.ADS:
                _newText.text = LOCALIZE_ID_PREF.FREE.AsLocalizeString();//.AsLocalizeString();
                break;
            case CostType.IAP:
                _newText.text = costData.PriceStr;
                _iconCurrency.gameObject.SetActive(false);
                _loadIapButton?.StartLoadCost(costData.PriceStr, _shopDesignElement.getIAPProductID(), _newText.targetTMPText);
                break;
        }

        if (_bonusPanel != null && _bonusText != null)
        {
            _bonusPanel.gameObject.SetActive(_shopDesignElement.Bonus != 0);
            _bonusText.text = "+" + _shopDesignElement.Bonus;
        }

        if (_goodChoiceImg != null)
            _goodChoiceImg.gameObject.SetActive(shopDesignElement.GoodChoice);
    }

    public void SetBg(Sprite bgSprite)
    {
        _background.sprite = bgSprite;
    }

    public void ActiveShiny(bool isActive)
    {
        if (_uiShiny != null && _uiShiny.enabled != isActive)
        {
            _uiShiny.enabled = isActive;
        }
    }
}