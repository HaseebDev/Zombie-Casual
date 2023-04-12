using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QuickType;
using QuickType.Promotion;
using QuickType.Shop;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class PromotionShop : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private LocalizedTMPTextUI _nameText;
    [SerializeField] private Text _endText;
    [SerializeField] private Text _costText;
    [SerializeField] private Text _oldCostText;
    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private Toggle _togglePrefab;
    [SerializeField] private Image _currencyIcon;
    [SerializeField] private Transform _mainContent;

    [SerializeField] private Text _descriptionText;
    [SerializeField] private GameObject _descriptionBG;

    [SerializeField] private PromotionRewardItemUI _promotionRewardItemPrefab;
    [SerializeField] private Transform _rewardHolder;

    private static bool _showedInSession = false;
    private Action<bool> _hasPromotion;
    private int _currentIndex = 0;
    private List<PromotionDesignElement> _availablePromotions;
    private List<Toggle> _toggles;
    private PromotionDesignElement CurrentPromo => _availablePromotions[_currentIndex];

    public bool HasPromo()
    {
        return _availablePromotions.Count != 0;
    }

    public void LoadIndex(int index)
    {
        _currentIndex = index;
        Load(_availablePromotions[_currentIndex]);
    }

    public void PreviousButtonClick()
    {
        _currentIndex--;
        if (_currentIndex < 0)
            _currentIndex = _availablePromotions.Count - 1;

        _toggles[_currentIndex].isOn = true;
    }

    public void NextButtonClick()
    {
        _currentIndex++;
        if (_currentIndex >= _availablePromotions.Count)
            _currentIndex = 0;

        _toggles[_currentIndex].isOn = true;
    }

    public void Load(PromotionDesignElement promotion)
    {
        ResourceManager.instance.GetShopItemSprite(promotion.Icon, s =>
        {
            _icon.sprite = s;
        });
        _nameText.textName = promotion.Name;

        UnlockRequirementDesignElement unlockRequirementDesign = DesignHelper.GetUnlockRequirement(promotion.Id);
        if (unlockRequirementDesign.HasTime())
        {
            _endText.text = unlockRequirementDesign.GetEndTime().ToString();
        }
        else _endText.text = "";

        var costData = promotion.GetCost();
        var oldCostData = promotion.GetOldCost();

        _costText.text = costData.PriceStr;
        if (oldCostData.Value != 0 && oldCostData.Value != costData.Value)
        {
            _oldCostText.text = oldCostData.PriceStr;
            _oldCostText.gameObject.SetActive(true);
        }
        else
        {
            _oldCostText.gameObject.SetActive(false);
        }

        _descriptionText.text = promotion.Description;
        _descriptionBG.SetActive(promotion.Description.Length > 1);

        var costType = promotion.GetCost().Type;
        ResourceManager.instance.GetCostTypeSprite(costType, s =>
        {
            _currencyIcon.sprite = s;
        });

        UpdateReward();
        _mainContent.localScale = Vector3.zero;
        _mainContent.DOScale(1, 0.3f);
    }

    public void OnPurchase()
    {
        var shopDesignElement = CurrentPromo;
        var costData = shopDesignElement.GetCost();
        List<RewardData> rewardDatas = shopDesignElement.GetReward();
        Action getReward = () => { MainMenuCanvas.instance.ShowRewardSimpleHUD(rewardDatas, true); };

        if (costData.Type == CostType.IAP)
        {
            IAPManager.instance.PurchaseIAP(shopDesignElement.getIAPProductID(), isSuccess =>
            {
                // Request IAP 
                if (isSuccess)
                {
                    getReward?.Invoke();
                    SaveManager.Instance.Data.ShopData.BoughtPromotionPack.Add(shopDesignElement.Id);
                    Load();
                }
                else
                {
                    MasterCanvas.CurrentMasterCanvas.ShowPurchaseFail();
                }
            });
        }
        else
        {
            if (CurrencyModels.instance.IsEnough(CurrencyType.DIAMOND, (long) costData.Value))
            {
                CurrencyModels.instance.AddCurrency(CurrencyType.DIAMOND, (long) -costData.Value);

                getReward?.Invoke();
            }
            else
            {
                MainMenuCanvas.instance.ShowNotEnoughHUD(CurrencyType.DIAMOND, (long) costData.Value);
            }
        }
    }

    private void UpdateReward()
    {
        var rewards = _availablePromotions[_currentIndex].GetReward();
        for (int i = 1; i < _rewardHolder.childCount; i++)
        {
            Destroy(_rewardHolder.GetChild(i).gameObject);
        }

        foreach (var reward in rewards)
        {
            var rewardRow = Instantiate(_promotionRewardItemPrefab, _rewardHolder);
            rewardRow.Load(reward);
        }
    }

    public void Load()
    {
        _availablePromotions = new List<PromotionDesignElement>();
        _toggles = new List<Toggle>();
        _toggleGroup.transform.DestroyAllChild();

        int index = 0;

        foreach (var promotion in DesignManager.instance.promotionDesign.PromotionDesignElement)
        {
            bool canShow = DesignHelper.IsRequirementAvailable(promotion.Id);
            if (!SaveManager.Instance.Data.ShopData.BoughtPromotionPack.Contains(promotion.Id) && canShow)
            {
                _availablePromotions.Add(promotion);

                var newToggle = Instantiate(_togglePrefab, _toggleGroup.transform);
                int indexTemp = index;

                newToggle.onValueChanged.AddListener((v) => { LoadIndex(indexTemp); });
                newToggle.gameObject.SetActive(true);
                if (index == 0)
                {
                    newToggle.isOn = true;
                }

                _toggles.Add(newToggle);
                index++;
            }
            else
            {
                // Debug.LogError(
                // $"Not start or ended promotion, now: {now}, start{promotion.GetStartTime()}, end{promotion.EndTime}");
            }
        }

        if (_availablePromotions.Count > 0)
        {
            LoadIndex(0);
            gameObject.SetActive(true);
            _hasPromotion?.Invoke(true);
        }
        else
        {
            gameObject.SetActive(false);
            _hasPromotion?.Invoke(false);
        }

        if (_showedInSession)
        {
            gameObject.SetActive(false);
        }

        _showedInSession = true;
    }

    public void SetOnPurchaseCallback(Action<bool> hasPromo)
    {
        _hasPromotion = hasPromo;
    }
}