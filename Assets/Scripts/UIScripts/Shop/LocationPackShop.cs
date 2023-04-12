using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using QuickType.LocationPack;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using UnityExtensions.Localization;

public class LocationPackShop : MonoBehaviour
{
    [SerializeField] private LocalizedTMPTextUI _title;
    [SerializeField] private LocalizedTMPTextUI _description;
    [SerializeField] private LocalizedTMPTextUI _oldCost;
    [SerializeField] private LocalizedTMPTextUI _newCost;

    [SerializeField] private CurrencyUI _currencyUiPrefab;

    // [SerializeField] private GameObject _plusPrefab;
    [SerializeField] private Transform _rewardHolder;
    [SerializeField] private GameObject _parent;

    [Header("Tooltip")] [SerializeField] private AttributeTooltip _tooltip;
    [SerializeField] private Transform _arrow;
    [SerializeField] private Transform _box;

    [SerializeField] private GameObject _nextBtn;
    [SerializeField] private GameObject _prevBtn;

    [SerializeField] private LoadIAPButton _loadIapButton;
    
    private List<LocationPackDesignElement> _locationPacks;
    private LocationPackDesignElement CurrentPack => _locationPacks[_currentIndex];
    private int _currentIndex = 0;

    public void Load(LocationPackDesignElement pack)
    {
        _title.text = LocalizeController.GetText(pack.Name);
        _description.text = "";// pack.Description.AsLocalizeString();
        _newCost.text = pack.GetPriceString();
        _oldCost.text = "$" + pack.OldCostValue.ToString();
        _oldCost.gameObject.SetActive(pack.OldCostValue != 0);
        _loadIapButton.StartLoadCost(pack.GetPriceString(),pack.GetIAPProductID(),_newCost.targetTMPText);
        
        UpdateRewards();
    }

    public void LoadIndex(int index)
    {
        _currentIndex = index;
        _nextBtn.gameObject.SetActive(_currentIndex != _locationPacks.Count - 1);
        _prevBtn.gameObject.SetActive(_currentIndex != 0);
        
        Load(CurrentPack);
    }

    public void LoadByLocationID(string locationID)
    {
        var pack = DesignManager.instance.locationPackDesign.LocationPackDesignElements.Find(x =>
            x.UnlockLocationId == locationID);
        int index = _locationPacks.IndexOf(pack);
        LoadIndex(index);
    }

    public void Load()
    {
        //_locationPacks = new List<LocationPackDesignElement>();

        var tempLocationPack = new List<LocationPackDesignElement>();
        
        foreach (var pack in DesignManager.instance.locationPackDesign.LocationPackDesignElements)
        {
            if (!SaveManager.Instance.Data.LocationPackSave.BoughPack.Contains(pack.Id) &&
               SaveGameHelper.GetMaxCampaignLevel() >= pack.UnlockLevel)
            {
                tempLocationPack.Add(pack);
            }
        }

        if (tempLocationPack.Count == 0)
        {
            _locationPacks = tempLocationPack;
            Hide();
            return;
        }
        
        if (_locationPacks != null && _locationPacks.Count == tempLocationPack.Count)
        {
            // skip
        }
        else
        {
            _locationPacks = tempLocationPack;
            if (_locationPacks.Count > 0)
            {
                LoadIndex(0);
                Show();
            }
            else
            {
                Hide();
            }
        }
    }

    public void PreviousButtonClick()
    {
        _currentIndex--;
        if (_currentIndex < 0)
            _currentIndex = _locationPacks.Count - 1;

        LoadIndex(_currentIndex);
    }

    public void NextButtonClick()
    {
        _currentIndex++;
        if (_currentIndex >= _locationPacks.Count)
            _currentIndex = 0;

        LoadIndex(_currentIndex);
    }

    public void OnPurchase()
    {
        IAPManager.instance.PurchaseIAP(CurrentPack.GetIAPProductID(), isSuccess =>
        {
            if (isSuccess)
            {
                MainMenuCanvas.instance.ShowRewardSimpleHUD(CurrentPack.GetRewards(), true);
                SaveAndReload();
            }
            else
            {
                MasterCanvas.CurrentMasterCanvas.ShowPurchaseFail();
            }
        });
    }

    private void SaveAndReload()
    {
        SaveManager.Instance.Data.LocationPackSave.BoughPack.Add(CurrentPack.Id);
        Load();
    }

    // private void SpawnPlus()
    // {
    //     var plus = Instantiate(_plusPrefab, _rewardHolder);
    //     plus.gameObject.SetActive(true);
    // }

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

    public void Show()
    {
        _parent?.SetActive(true);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        _parent?.SetActive(false);
        gameObject.SetActive(false);
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