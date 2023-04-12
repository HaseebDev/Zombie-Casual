using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using com.datld.data;
using CustomListView.Weapon;
using DG.Tweening;
using Ez.Pooly;
using QuickEngine.Extensions;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using static AnalyticsConstant;

public class ChangeWeapon : MonoBehaviour
{
    [SerializeField] private Transform _weaponSlotContent;
    [SerializeField] private Transform _arrow;
    [SerializeField] private RectTransform _weaponScrollView;

    [SerializeField] private WeaponGridAdapter _listAdapter;
    // [SerializeField] private Transform _weaponButtonHolder;

    [SerializeField] private GameObject _changeWeaponPanel;
    [SerializeField] private ChangeWeaponButton _changeWeaponButtonPrefab;
    [SerializeField] private GameObject _upgradeIcon;
    [SerializeField] private ReminderUI _reminderUi;

    [SerializeField] private Button _activeWeaponTabButton;
    [SerializeField] private GameObject _lockWeaponTab;
    [SerializeField] private Button _fuseButton;
    [SerializeField] private TextMeshProUGUI _fusionCost;

    private HeroData _currentChangeWpHeroData;

    // private List<ChangeWeaponButton> _changeWeaponBtns;
    private WeaponSlot[] _weaponSlots;
    private Action<HeroData> _onUpgradeWeapon;
    private bool _isUnlocked = false;

    private void CheckAllReminder()
    {
        if (_weaponSlots != null)
        {
            var newEquips = ReminderManager.HasNewEquip();
            foreach (var weaponSlot in _weaponSlots)
            {
                weaponSlot.LoadReminder(newEquips.Item2.Count);
            }
        }
    }


    public void Init()
    {
        _weaponSlots = _weaponSlotContent.GetComponentsInChildren<WeaponSlot>();
        for (int i = 0; i < _weaponSlots.Length; i++)
        {
            _weaponSlots[i].Load(SaveManager.Instance.Data.GameData.TeamSlots[i], i);
        }


        OnNewLevel();
        OnUpdateFormation();

        EventSystemServiceStatic.AddListener(this, EVENT_NAME.LEVEL_UP,
            new Action(OnNewLevel));
    }


    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.LEVEL_UP,
            new Action(OnNewLevel));
    }

    private void OnNewLevel()
    {
        var newEquips = ReminderManager.HasNewEquip();
        _reminderUi.Load(newEquips.Item2.Count);
        CheckAllReminder();

        var isUnlock = DesignHelper.GetUnlockRequirementLevel(EnumHUD.HUD_EQUIPMENT.ToString());
        _isUnlocked = SaveGameHelper.GetMaxCampaignLevel() >=
                      (isUnlock.Item2 - 1);
        _activeWeaponTabButton.interactable = _isUnlocked;
        _lockWeaponTab.SetActive(!_isUnlocked);
        UpdateUpgradeIconState();

        _changeWeaponPanel.SetActive(false);

        CheckFusionAvailable();
    }

    private void CheckFusionAvailable()
    {
        bool isFusionAvailable = GetCanFusionWeapon() != null;
        foreach (var weaponSlot in _weaponSlots)
        {
            weaponSlot.ShowFusionAvailable(isFusionAvailable);
        }
    }

    private void OnUpgrade(HeroData heroData)
    {
        foreach (var wpSlot in _weaponSlots)
        {
            wpSlot.UpdateUpgradeButtonState();
        }

        _onUpgradeWeapon?.Invoke(heroData);
        GamePlayController.instance.gameLevel.OnUpgradeWeapon(heroData);
        UpdateUpgradeIconState();
    }

    public void OnUpdateFormation()
    {
        for (int i = 0; i < _weaponSlots.Length; i++)
        {
            _weaponSlots[i].Load(SaveManager.Instance.Data.GameData.TeamSlots[i], i);
            _weaponSlots[i].SetOnUpgradeCallback(OnUpgrade);
            _weaponSlots[i].SetOnShowChangeWeaponCallback(InitWeaponGrid);
        }

        UpdateUpgradeIconState();
    }

    private void InitWeaponGrid(HeroData heroData, Vector3 position)
    {
        _changeWeaponPanel.SetActive(true);

        if (_listAdapter.IsInitialized)
        {
            OpenChangeWeaponPanel(heroData, position);
        }
        else
        {
            _listAdapter.Initialized += () => OpenChangeWeaponPanel(heroData, position);
        }
    }

    private void OpenChangeWeaponPanel(HeroData heroData, Vector3 position)
    {
        // _listAdapter.Initialized -= () => OpenChangeWeaponPanel(heroData, position);
        //
        // if (_listAdapter.Data.Count != 0)
        //     _listAdapter.Data.RemoveItemsFromStart(_listAdapter.Data.Count);

        _currentChangeWpHeroData = heroData;
        CreateWeaponButtons();

        _arrow.transform.position = position;

        CheckOutSide();

        ReminderManager.SaveCurrentWeaponsState();

        // Hide reminder after open
        _reminderUi.Show(false);
        CheckAllReminder();
        //analytics
        var currentLevel = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CurrentLevel;
        if (currentLevel <= AnalyticsConstant.MAX_TRACKING_LEVEL)
        {
            var eventName = getEventNameByLevel(ANALYTICS_ENUM.TOUCH_INGAME_BTN_CHANGE_WEAPON, currentLevel);
            AnalyticsManager.instance.LogEvent(eventName, new LogEventParam("level", currentLevel));
        }
    }

    private Tuple<string, int, int> GetCanFusionWeapon()
    {
        var wps = SaveManager.Instance.Data.GetAllWeaponDatas(); //.OrderBy(x => -x.FinalPowerData.PercentDmg);
        Dictionary<Tuple<string, int>, int> counter = new Dictionary<Tuple<string, int>, int>();

        foreach (var weaponData in wps)
        {
            if (weaponData.ItemStatus != ITEM_STATUS.Choosing)
            {
                Tuple<string, int> key = new Tuple<string, int>(weaponData.WeaponID, weaponData.Rank);
                if (counter.ContainsKey(key))
                {
                    int count = counter[key];
                    count++;
                    counter[key] = count;
                }
                else
                {
                    counter.Add(key, 1);
                }
            }
        }

        // ID, rank
        List<Tuple<string, int, int>> result = new List<Tuple<string, int, int>>();

        foreach (var VARIABLE in counter)
        {
            if (VARIABLE.Value == 2)
            {
                if (GetUsingCanFusionWP(VARIABLE.Key) != null)
                {
                    result.Add(new Tuple<string, int, int>(VARIABLE.Key.Item1, VARIABLE.Key.Item2, VARIABLE.Value));
                }
            }
            else if (VARIABLE.Value >= 3)
            {
                result.Add(new Tuple<string, int, int>(VARIABLE.Key.Item1, VARIABLE.Key.Item2, VARIABLE.Value));
            }
        }

        if (result.Count == 0)
            return null;

        return result.OrderBy(x => x.Item3).ToList().Last();
    }

    private void CreateWeaponButtons()
    {
        var wps = SaveManager.Instance.Data.GetAllWeaponDatas()
            .OrderBy(x => -(x.Rank * 10000 + x.FinalPowerData.PercentDmg));
        var available = new List<WeaponItemModel>();

        var newEquips = ReminderManager.HasNewEquip();
        int newEquipCount = newEquips.Item2.Count;

        var canFusionItems = GetCanFusionWeapon();
        bool founded = false;

        foreach (var weaponData in wps)
        {
            bool hasInFusion = canFusionItems != null && canFusionItems.Item1 == weaponData.WeaponID &&
                               canFusionItems.Item2 == weaponData.Rank;
            if (weaponData.ItemStatus != ITEM_STATUS.Choosing || hasInFusion)
            {
                if (weaponData.ItemStatus == ITEM_STATUS.Choosing)
                {
                    if (SaveGameHelper.IsEquipByUnlockedHero(weaponData))
                    {
                        if (founded)
                        {
                            continue;
                        }

                        founded = true;
                    }
                    else
                    {
                        continue;
                    } 
                }
             
                bool isNewWeapon = newEquipCount > 0 && newEquips.Item2.Contains(weaponData);
                available.Add(new WeaponItemModel(weaponData, DesignHelper.GetWeaponDesign(weaponData),
                    OnWeaponClick, isNewWeapon, false, weaponData.ItemStatus == ITEM_STATUS.Choosing));

                //   Add(weaponData);
            }
        }

        available = available.OrderBy(x => !x.showReminder).ToList();

        _fuseButton.gameObject.SetActive(canFusionItems != null);

        if (canFusionItems != null)
        {
            // Debug.LogError("COUNTER " + data.Item3);
            var allWp = available.FindAll(x =>
                x._weaponData.WeaponID == canFusionItems.Item1 && x._weaponData.Rank == canFusionItems.Item2);

            var currentFusionElement =
                DesignManager.instance.fusionDesign.FusionElements[allWp[0]._weaponData.Rank - 1];
            _fusionCost.text = $"x{FBUtils.CurrencyConvert(currentFusionElement.GoldCost)}";

            foreach (var x in allWp)
            {
                if (x._weaponData.ItemStatus == ITEM_STATUS.Choosing)
                {
                    allWp.Remove(x);
                    allWp.Add(x);
                    break;
                }
            }

            List<WeaponItemModel> removed = new List<WeaponItemModel>();
            for (int i = 0; i < allWp.Count - 1; i++)
            {
                if (allWp[i]._weaponData.ItemStatus == ITEM_STATUS.Choosing)
                {
                    removed.Add(allWp[i]);
                }
            }

            foreach (var VARIABLE in removed)
            {
                allWp.Remove(VARIABLE);
            }


            if (allWp.Count > 0)
            {
                allWp.Reverse();

                for (int i = 2; i >= 0; i--)
                {
                    var wp = allWp[i];
                    wp.showHighlight = true;

                    available.Remove(wp);
                    available.Insert(0, wp);
                }
            }
        }

        if (_listAdapter.Data.Count != available.Count)
        {
            _listAdapter.Data.RemoveItemsFromStart(_listAdapter.Data.Count);
            _listAdapter.Data.InsertItems(0, available);
        }
        else
        {
            for (int i = 0; i < available.Count; i++)
            {
                _listAdapter.Data[i].Update(available[i]); // = available[i];
                _listAdapter.GetCellViewsHolderIfVisible(i)?.UpdateView(_listAdapter.Data[i]);
            }
        }
    }

    private WeaponData GetUsingCanFusionWP(Tuple<string, int> canFusion)
    {
        WeaponData result = null;
        var wps = SaveManager.Instance.Data.GetAllWeaponDatas(); //.OrderBy(x => -x.FinalPowerData.PercentDmg);
        var usingWp = new List<WeaponData>();

        foreach (var wpData in wps)
        {
            if (wpData.WeaponID == canFusion.Item1 && wpData.Rank == canFusion.Item2 &&
                wpData.ItemStatus == ITEM_STATUS.Choosing)
            {
                foreach (var heroData in SaveManager.Instance.Data.Inventory.ListHeroData)
                {
                    if (heroData.UniqueID != GameConstant.MANUAL_HERO && heroData.UniqueID != "HERO_DEMO" &&
                        (heroData.ItemStatus == ITEM_STATUS.Available || heroData.ItemStatus == ITEM_STATUS.Choosing) &&
                        (heroData.EquippedWeapon == wpData.UniqueID || heroData.EquippedArmour == wpData.UniqueID))
                    {
                        usingWp.Add(wpData);
                        break;
                    }
                }
            }
        }

        if (usingWp.Count > 0)
            result = usingWp.OrderBy(x => x.GetWeaponLevel()).Last();

        return result;
    }


    private void OnWeaponClick(WeaponData weaponData)
    {
        if (weaponData.ItemStatus == ITEM_STATUS.Choosing)
        {
            MasterCanvas.CurrentMasterCanvas.ShowFloatingTextNotify("USING WEAPON");
            return;
        }

        _changeWeaponPanel.SetActive(false);
        _currentChangeWpHeroData.EquipWeapon(weaponData);
        GamePlayController.instance.gameLevel.ChangeWeaponHeroInMap(_currentChangeWpHeroData);
        OnUpdateFormation();

        //analytics
        var currentLevel = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CurrentLevel;
        if (currentLevel <= AnalyticsConstant.MAX_TRACKING_LEVEL)
        {
            List<LogEventParam> results = new List<LogEventParam>();
            results.Add(new LogEventParam("level", currentLevel));
            results.Add(new LogEventParam("weaponID", weaponData.WeaponID));
            AnalyticsManager.instance.LogEvent(
                getEventNameByLevel(ANALYTICS_ENUM.TOUCH_INGAME_BTN_NEW_WEAPON, currentLevel), results);
        }
    }

    public void SetOnUpgradeCallback(Action<HeroData> onUpgradeWeapon)
    {
        _onUpgradeWeapon = onUpgradeWeapon;
    }

    private void CheckOutSide()
    {
        // Arrow
        Vector3[] objectCornersArrow = new Vector3[4];
        var arrowRect = Extensions.rectTransform(_arrow);
        arrowRect.GetWorldCorners(objectCornersArrow);

        if (objectCornersArrow[0].x < 0)
        {
            var position1 = arrowRect.position;
            var delta = arrowRect.rect.width / 2f - position1.x;

            position1 = new Vector3(position1.x + Utils.ConvertToMatchWidthRatio(delta),
                position1.y);

            arrowRect.position = position1;
        }

        if (objectCornersArrow[3].x > Screen.width)
        {
            var position1 = arrowRect.position;
            var delta = arrowRect.rect.width / 2f - (Screen.width - position1.x);

            position1 = new Vector3(position1.x - Utils.ConvertToMatchWidthRatio(delta),
                position1.y);

            arrowRect.position = position1;
        }

        _weaponScrollView.anchoredPosition = new Vector3(0, _weaponScrollView.anchoredPosition.y);
        Vector3[] objectCorners = new Vector3[4];
        _weaponScrollView.GetWorldCorners(objectCorners);
        float width = _weaponScrollView.GetWidth();
        var halfWidth = width / 2f;

        var posTemp = _weaponScrollView.anchoredPosition;
        if (objectCorners[0].x < 0)
        {
            posTemp = new Vector3(posTemp.x + halfWidth - width / 8f, posTemp.y);
        }
        else if (objectCorners[3].x > Screen.width)
        {
            posTemp = new Vector3(posTemp.x - halfWidth + width / 8f, posTemp.y);
        }

        _weaponScrollView.anchoredPosition = posTemp;
    }

    private void UpdateUpgradeIconState()
    {
        _upgradeIcon.SetActive(HasAvailableUpgrade() && _isUnlocked);
    }

    private bool HasAvailableUpgrade()
    {
        foreach (var wpSlot in _weaponSlots)
        {
            if (wpSlot.HasWeapon())
            {
                if (wpSlot.CanUpgrade())
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void ShowLockTabWeaponText()
    {
        var isUnlock = DesignHelper.GetUnlockRequirementLevel(EnumHUD.HUD_EQUIPMENT.ToString());
        _isUnlocked = SaveGameHelper.GetMaxCampaignLevel() >=
                      (isUnlock.Item2 - 1);

        MasterCanvas.CurrentMasterCanvas.ShowFloatingTextNotify(
            LocalizeController.GetText(LOCALIZE_ID_PREF.UNLOCK_IN_LV, isUnlock.Item2 - 1));
    }

    private bool isFusing = false;
    public void OnAutoFuseButtonClick()
    {
        if(isFusing)
            return;
        
        var fuseItems = new List<WeaponData>();
        for (int i = 0; i < 3; i++)
        {
            fuseItems.Add(_listAdapter.Data[i]._weaponData);
        }

        var currentFinal = fuseItems[0];
        var currentFusionElement = DesignManager.instance.fusionDesign.FusionElements[currentFinal.Rank - 1];
        var highestLevel = fuseItems.Max(x => x.GetWeaponLevel());

        // foreach (var VARIABLE in fuseItems)
        // {
        //        Debug.LogError("ID " + VARIABLE.UniqueID + "RANK " + VARIABLE.Rank); 
        // }

        if (CurrencyModels.instance.IsEnough(CurrencyType.GOLD, currentFusionElement.GoldCost))
        {
            CurrencyModels.instance.AddCurrency(CurrencyType.GOLD, -currentFusionElement.GoldCost);
            currentFinal.Rank++;
            currentFinal.SetWeaponLevel(highestLevel);// = highestLevel;
            currentFinal.ResetPowerData();

            MasterCanvas.CurrentMasterCanvas.ShowHUD(EnumHUD.HUD_FUSION_RESULT, false, null, currentFinal,
                fuseItems[1], fuseItems[2]);

            fuseItems[1].Dismantle();
            fuseItems[2].Dismantle();

            CreateWeaponButtons();
            SaveManager.Instance.SetDataDirty();
            AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_UPGRADE);
            CheckFusionAvailable();

            foreach (var VARIABLE in _weaponSlots)
            {
                VARIABLE.UpdateInfo();
            }

            isFusing = true;
            DOVirtual.DelayedCall(1.5f, () =>
            {
                isFusing = false;
            });
        }
        else
        {
            MasterCanvas.CurrentMasterCanvas.ShowNotEnoughHUD(CurrencyType.GOLD, currentFusionElement.GoldCost);
        }
    }
}