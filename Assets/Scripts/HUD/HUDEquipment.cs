using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.datld.data;
using DG.Tweening;
using Ez.Pooly;
using MEC;
using QuickType.Weapon;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class HUDEquipment : CanExitHUD
{
    public AssetReference _equipmentFullDetailPrefab;
    public AssetReference _bgPrefab;
    public AssetReference _heroEquipInfoUiPrefab;
    public AssetReference _equipmentHolderPrefab;
    public AssetReference _heroPreviewPrefab;
    public Transform content;
    public GameObject loadingIcon;

    private EquipmentFullDetail _equipmentFullDetail;
    private MyPopup1 _equipFullDetailPopup;
    private HeroEquipInfoUI _heroEquipInfoUi;
    private FilterEquipHelper _filterEquipHelper;
    private EquipmentHolder _equipmentHolder;

    private ShopHeroPreview _heroPreview;

    public void Init(Action callback)
    {
        if (!isInit)
        {
            Timing.RunCoroutine(InitAsync(callback));
        }
        else
        {
            callback?.Invoke();
        }

        base.Init();
    }

    IEnumerator<float> InitAsync(Action callback)
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        _heroPreview = FindObjectOfType<ShopHeroPreview>();
        AsyncOperationHandle<GameObject> op;

        if (_heroPreview == null)
        {
            op = _heroPreviewPrefab.InstantiateAsync(null); //GameObject.Instantiate(_heroPreviewPrefab, null);
            while (!op.IsDone)
                yield return Timing.WaitForOneFrame;

            _heroPreview = op.Result.GetComponent<ShopHeroPreview>();
            _heroPreview.transform.position = Vector3.one * 999f;
            _heroPreview.gameObject.SetActive(true);
        }
        
        op = _heroEquipInfoUiPrefab.InstantiateAsync(content);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        _heroEquipInfoUi = op.Result.GetComponent<HeroEquipInfoUI>();
        _heroEquipInfoUi.transform.SetSiblingIndex(0);
        yield return Timing.WaitUntilDone(_heroEquipInfoUi.LoadPrefab());
        _heroEquipInfoUi.Init();
        _heroEquipInfoUi.heroPreview.gameObject.SetActive(true);

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(Pooly.Instance.CreateClones(POOLY_PREF.EQUIPMENT_UI_VARIANT,20)));
        
        op = _equipmentHolderPrefab.InstantiateAsync(content);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;
        
        _equipmentHolder = op.Result.GetComponent<EquipmentHolder>();
        _filterEquipHelper = _equipmentHolder._filterEquipHelper;
        _equipmentHolder.transform.SetSiblingIndex(1);

        // yield return Timing.WaitForSeconds(0.1f);
        
        op = _equipmentFullDetailPrefab.InstantiateAsync(transform);
        while (!op.IsDone)
            yield return Timing.WaitForOneFrame;

        _equipmentFullDetail = op.Result.GetComponent<EquipmentFullDetail>();
        _equipFullDetailPopup = _equipmentFullDetail.GetComponent<MyPopup1>();
        
        _equipmentFullDetail.Init();

        _equipmentFullDetail.onEquip = OnEquipItem;
        _equipmentFullDetail.onUnEquip = OnUnEquipItem;
        _equipmentFullDetail.onUpgrade = OnUpgrade;
        _equipmentFullDetail.HideCallback = HideEquipFullDetail;
        _heroEquipInfoUi.onShowFullDetail = ShowEquipFullDetail;
        _equipmentHolder.SetEquipClickCallback(ShowEquipFullDetail);
        _equipmentFullDetail.SetOnDismantleCallback(DismantleEquip);
        
        callback?.Invoke();
        Application.backgroundLoadingPriority = ThreadPriority.High;
        loadingIcon.SetActive(false); 
    }

    protected override List<string> GetPreInitAsset()
    {
        return new List<string>()
        {
            POOLY_PREF.EQUIPMENT_UI_HUD_EQUIPMENT
        };
    }

    public override void CleanUp()
    {
        base.CleanUp();
        _equipmentHolder.CleanUp();
    }

    public override void ResetLayers()
    {
        Init(() =>
        {
            base.ResetLayers();
            // InitEquipment();
            HideEquipFullDetail();
            ForceHideFullEquip();

            _heroEquipInfoUi.RefreshLayer();
            _filterEquipHelper.OnResetLayers();

            UpdateReminderHero();
            MainMenuCanvas.instance.ShowTab(true);
        });
    }

    private void UpdateReminderHero()
    {
        var newHero = ReminderManager.HasNewHero();
        int count = newHero.Item2.Count;
        _heroEquipInfoUi.UpdateNewHeroReminder(count + SaveGameHelper.GetCanUpRankHeroCount());
    }

    private void OnEquipItem(WeaponData data)
    {
        RunEquipAnimation(data, () => { });

        _heroEquipInfoUi.Equip(data);
        InitEquipment();
    }

    private void RunEquipAnimation(WeaponData weaponData, Action callBack)
    {
        var ui = _equipmentHolder.GetEquipUI(weaponData);
        if (ui == null)
        {
            callBack?.Invoke();
            return;
        }

        var design = DesignHelper.GetWeaponDesign(weaponData);
        if (weaponData != null && ui != null)
        {
            EquipmentUI target = design.EquipType == GameConstant.EQUIP_TYPE_WEAPON
                ? _heroEquipInfoUi.EquippedWeapon
                : _heroEquipInfoUi.EquippedArmour;

            CreateFakeAnimation(ui, target.transform, () => { callBack?.Invoke(); });
            if (target.WeaponData != null)
                CreateFakeAnimation(target, ui.transform, null);

            // ui.gameObject.SetActive(false);
            target.Clear();
        }
    }

    private void CreateFakeAnimation(EquipmentUI target, Transform destination, Action callback = null)
    {
        var clone = Instantiate(target.gameObject, transform);
        var rect = clone.GetComponent<RectTransform>();
        rect.sizeDelta = destination.GetComponent<RectTransform>().sizeDelta;
        rect.localScale = Vector3.zero;
        clone.transform.position = target.transform.position;

        rect.DOScale(1, 0.2f);
        // rect.DOSizeDelta(destination.GetComponent<RectTransform>().sizeDelta, 0.2f);
        clone.transform.DOMove(destination.position, 0.2f).OnComplete(() =>
        {
            Destroy(clone);
            callback?.Invoke();
        });
    }

    private void OnUnEquipItem(WeaponData data)
    {
        _heroEquipInfoUi.UnEquip(data);
        InitEquipment();
    }

    private void OnUpgrade()
    {
        // _heroEquipInfoUi.RefreshLayer();
    }

    private void AddFakeEquip()
    {
        SaveManager.Instance.Data.Inventory.TotalLegendaryKey = 2;
        SaveManager.Instance.Data.Inventory.TotalRareKey = 2;
        // if (!CurrencyModels.instance.IsEnough(CurrencyType.GOLD, 999999))
        //     CurrencyModels.instance.AddCurrency(CurrencyType.GOLD, 999999);

        // if (SaveManager.Instance.Data.Inventory.ListWeaponData.Count < 20)
        // {
        //     for (int i = 0; i < 80; i++)
        //     {
        //         var wp = SaveGameHelper.RandomWeaponData();
        //         SaveManager.Instance.Data.Inventory.ListWeaponData.Add(wp);
        //     }
        // }
    }

    private void InitEquipment()
    {
        // _equipmentHolder.DestroyAllEquipmentUI();
        _equipmentHolder.CreateEquipUIFromInventory(() =>
        {
            var newHero = ReminderManager.HasNewHero();
            int newHeroAndUprank = newHero.Item2.Count + SaveGameHelper.GetCanUpRankHeroCount();
            MainMenuTab.Instance.ShowReminder(EnumHUD.HUD_EQUIPMENT, newHeroAndUprank != 0, newHeroAndUprank);
        });
    }

    private void ShowEquipFullDetail(WeaponData weaponData, WeaponDesign weaponDesign, EquipmentUI equipmentUi)
    {
        ShowEquipFullDetail(weaponData, weaponDesign, EquipState.NORMAL);
    }

    private void ShowEquipFullDetail(WeaponData weaponData, WeaponDesign weaponDesign,
        EquipState state = EquipState.NORMAL)
    {
        _equipmentFullDetail.State = state;
        _equipmentFullDetail.Load(weaponData, weaponDesign);
    }

    public void HideEquipFullDetail()
    {
        _heroEquipInfoUi.RefreshLayer();

        _equipmentFullDetail.OnHide();
        _equipmentHolder.OnShowFullHide(_equipmentFullDetail.WeaponData);

        if (_equipFullDetailPopup.gameObject.activeInHierarchy)
            _equipFullDetailPopup.Hide();

        _filterEquipHelper.UpdateFilter(true);
    }

    public void ForceHideFullEquip()
    {
        _equipFullDetailPopup.gameObject.SetActive(false);
    }

    public void DismantleEquip(WeaponData weaponData, WeaponDesign weaponDesign)
    {
        weaponData.Dismantle();
        ResetLayers();
    }
}