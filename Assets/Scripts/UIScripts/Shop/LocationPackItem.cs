using System;
using System.Collections;
using System.Collections.Generic;
using QuickType.LocationPack;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using UnityExtensions.Localization;

public class LocationPackItem : MonoBehaviour
{
    [SerializeField] private LocalizedTMPTextUI _title;
    [SerializeField] private LocalizedTMPTextUI _description;
    [SerializeField] private LocalizedTMPTextUI _oldCost;
    [SerializeField] private LocalizedTMPTextUI _newCost;
    [SerializeField] private CurrencyUI _currencyUiPrefab;
    [SerializeField] private Transform _rewardHolder;

    [Header("Tooltip")] [SerializeField] private AttributeTooltip _tooltip;
    [SerializeField] private Transform _arrow;
    [SerializeField] private Transform _box;
    [SerializeField] private LoadIAPButton _loadIapButton;

    private LocationPackDesignElement CurrentPack;
    private Action onClose;

    public void Load(string locationID)
    {
        CurrentPack = DesignManager.instance.locationPackDesign.LocationPackDesignElements.Find(x =>
            x.UnlockLocationId == locationID);
        Load(CurrentPack);
    }

    public void Load(LocationPackDesignElement pack)
    {
        _title.textName = pack.Name;
        _description.text = pack.Description.AsLocalizeString();
        _newCost.text = pack.GetPriceString();
        _oldCost.text = "$" + pack.OldCostValue.ToString();
        _oldCost.gameObject.SetActive(pack.OldCostValue != 0);
        _loadIapButton.StartLoadCost(pack.GetPriceString(),pack.GetIAPProductID(),_newCost.targetTMPText);
        
        UpdateRewards();
    }

    private void UpdateRewards()
    {
        foreach (Transform child in _rewardHolder)
        {
            Destroy(child.gameObject);
        }

        int equip = 0;
        foreach (var rewardData in CurrentPack.GetRewards())
        {
            if (rewardData._type != REWARD_TYPE.RANDOM_EQUIP)
            {
                var rewardUi = Instantiate(_currencyUiPrefab, _rewardHolder);
                rewardUi.gameObject.SetActive(true);
                rewardUi.Load(rewardData);

                var temp = rewardData._type;
                rewardUi.SetOnClickCallback(() => { ShowTooltip(temp, rewardUi.transform.position); });
            }
            else
            {
                equip++;
            }
        }

        if (equip != 0)
        {
            var rewardUi = Instantiate(_currencyUiPrefab, _rewardHolder);
            rewardUi.gameObject.SetActive(true);
            rewardUi.Load(new RewardData(REWARD_TYPE.RANDOM_EQUIP, equip, 4.ToString()));

            rewardUi.SetOnClickCallback(() => { ShowTooltip(REWARD_TYPE.RANDOM_EQUIP, rewardUi.transform.position); });
        }
    }

    public void Hide()
    {
        onClose?.Invoke();
        Destroy(gameObject);
    }

    public void SetOnCloseCallback(Action callback)
    {
        onClose = callback;
    }

    public void OnPurchase()
    {
        IAPManager.instance.PurchaseIAP(CurrentPack.GetIAPProductID(), isSuccess =>
        {
            if (isSuccess)
            {
                MainMenuCanvas.instance.ShowRewardSimpleHUD(CurrentPack.GetRewards(), true);
                SaveManager.Instance.Data.LocationPackSave.BoughPack.Add(CurrentPack.Id);
                Hide();
            }
            else
            {
                MasterCanvas.CurrentMasterCanvas.ShowPurchaseFail();
            }
        });

    }

    private void ShowTooltip(REWARD_TYPE rewardType, Vector3 position)
    {
        _tooltip.UpdateText(LocalizeController.GetText("DESCRIPTION_" + rewardType));
        _tooltip.Show();

        _box.transform.parent = _arrow;

        _arrow.transform.position =
            position -
            new Vector3(0, Utils.ConvertToMatchHeightRatio(_arrow.rectTransform().rect.height / 2), 0);

        _box.transform.parent = _tooltip.transform;
        _box.rectTransform().anchoredPosition = new Vector2(0, _box.rectTransform().anchoredPosition.y);
    }
}