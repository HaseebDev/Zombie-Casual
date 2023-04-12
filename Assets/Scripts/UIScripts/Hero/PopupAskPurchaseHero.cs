using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickType.Shop;
using QuickType.UnlockHero;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupAskPurchaseHero : MonoBehaviour
{
    [SerializeField] private HeroButtonUI _heroButtonUi;
    [SerializeField] private Image _currencyIcon;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private LoadIAPButton _loadIapButton;

    private MyPopup _myPopup;
    private Action _onPurchase;

    private MyPopup MyPopup
    {
        get
        {
            if (_myPopup == null)
                _myPopup = GetComponent<MyPopup>();
            return _myPopup;
        }
    }

    public void Purchase()
    {
        _onPurchase?.Invoke();
        MyPopup.Hide();
    }

    public void Show(HeroData heroData, CostData costData, UnlockHeroDesignElement unlockHeroDesignElement,
        Action onPurchase)
    {
        _onPurchase = onPurchase;
        _heroButtonUi.Load(heroData, true);
        ResourceManager.instance.GetCostTypeSprite(costData.Type, s => { _currencyIcon.sprite = s; });
        _loadIapButton.CompleteLoad();

        if (costData.IsIAP())
        {
            _valueText.text = costData.PriceStr;
            _currencyIcon.gameObject.SetActive(false);
            _loadIapButton.StartLoadCost(costData.PriceStr,unlockHeroDesignElement.getIAPProductID(), _valueText);
        }
        else
        {
            _currencyIcon.gameObject.SetActive(true);
            _valueText.text = $"x{FBUtils.CurrencyConvert((long) costData.Value)}";
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        MyPopup.Hide();
    }
}