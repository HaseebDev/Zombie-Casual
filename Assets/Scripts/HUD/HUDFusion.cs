using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using DG.Tweening;
using Doozy.Engine.Utils.ColorModels;
using Ez.Pooly;
using Google.Protobuf.WellKnownTypes;
using QuickType.Fusion;
using QuickType.Weapon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDFusion : BaseHUD
{
    [SerializeField] private EquipmentFusionHolder _equipmentFusionHolder;
    [SerializeField] private FilterEquipFusionHelper _filterEquipFusionHelper;
    [SerializeField] private AttributeUI _unlockAttributeUi;
    [SerializeField] private GameObject _preFusionInfo;
    [SerializeField] private Transform _resourcesHolder;
    [SerializeField] private EquipmentUI _baseResourceUi;
    [SerializeField] private GameObject _helpText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private Button _fusionButton;
    [SerializeField] private Text _currentRankText;
    [SerializeField] private Text _newRankText;

    private WeaponData _baseResourceWeaponData;
    private List<EquipmentUI> _resourceSlots;
    private FusionElement _currentFusionElement;

    public override void Awake()
    {
        base.Awake();
        _equipmentFusionHolder.SetEquipClickCallback(OnEquipClick);
        _baseResourceUi.AddClickListener(OnRemoveBaseFusionEquip);

        _resourceSlots = new List<EquipmentUI>();
        for (int i = 0; i < _resourcesHolder.childCount; i++)
        {
            var equipmentUI = _resourcesHolder.GetChild(i).GetComponent<EquipmentUI>();
            equipmentUI.AddClickListener(OnRemoveResourceFusionEquip);
            _resourceSlots.Add(equipmentUI);
        }
    }

    public override void CleanUp()
    {
        base.CleanUp();
        foreach (var VARIABLE in _resourceSlots)
        {
            Pooly.Despawn(VARIABLE.transform);
        }
    }

    public override void ResetLayers()
    {
        base.ResetLayers();
        ResetResource();

        InitEquipment(() =>
        {
            ResetEquipmentHolder();
            // _filterEquipFusionHelper.UpdateFilter();
        });
    }


    private void InitEquipment(Action callBack)
    {
        // _equipmentFusionHolder.DestroyAllEquipmentUI();
        _equipmentFusionHolder.CreateEquipUIFromInventory(callBack);
    }

    public override void OnButtonBack()
    {
        CurrencyBar.Instance.ResetDefaultUI();
        // MainMenuTab.Instance.Show();
        MainMenuTab.Instance.OnButtonHero();
        // base.OnButtonBack();
    }

    private void OnEquipClick(WeaponData weaponData, WeaponDesign weaponDesign, EquipmentUI equipmentUi)
    {
        // Choose base resource
        if (_baseResourceWeaponData == null)
        {
            // Check is unique rank
            if (weaponData.Rank < GameConstant.MAXIMUM_RANK)
            {
                _baseResourceWeaponData = weaponData;
                _baseResourceUi.LoadData(_baseResourceWeaponData, weaponDesign);

                CreateFakeAnimation(equipmentUi, _baseResourceUi.transform,
                    () => _baseResourceUi.LoadImage(_baseResourceWeaponData, weaponDesign));
                OnBaseResourceSelected();
            }
            else // Show notify max rank
            {
                MainMenuCanvas.instance.ShowFloatingTextNotify(LOCALIZE_ID_PREF.EQUIP_MAX_RANK.AsLocalizeString());
            }
        }
        else // Choose resource
        {
            // Equip available
            if (weaponData.ItemStatus != ITEM_STATUS.Choosing)
            {
                if (weaponData.WeaponID == _baseResourceWeaponData.WeaponID &&
                    weaponData.Rank == _baseResourceWeaponData.Rank)
                {
                    // Loop to find empty slot 
                    for (int i = 0; i < _currentFusionElement.Quantity; i++)
                    {
                        var resourceSlot = _resourceSlots[i];

                        // Add to empty slot
                        if (resourceSlot.WeaponData == null)
                        {
                            resourceSlot.LoadData(weaponData, weaponDesign);
                            CreateFakeAnimation(equipmentUi, resourceSlot.transform,
                                () => resourceSlot.LoadImage(weaponData, weaponDesign));

                            SetStateEquipInInventory(weaponData, true);
                            CheckEnoughResource();

                            break;
                        }
                    }
                }
                else // Select wrong
                {
                    MainMenuCanvas.instance.ShowNotifyHUD("Please choose an equip has same rank nad type");
                }
            }
            else // Equip is equipping by a hero
            {
                MainMenuCanvas.instance.ShowNotifyHUD("Equip is used by a hero");
            }
        }
    }

    private void OnBaseResourceSelected()
    {
        // _preFusionInfo.SetActive(true);
        _resourcesHolder.gameObject.SetActive(true);
        _helpText.SetActive(false);

        var weaponData = _baseResourceWeaponData;
        var weaponDesign = _baseResourceUi.WeaponDesign;

        _currentFusionElement = DesignManager.instance.fusionDesign.FusionElements[weaponData.Rank - 1];
        _costText.text = $"x{_currentFusionElement.GoldCost}";
        //
        // var lockedAttribute = weaponDesign.GetNextLockedAttribute();
        // if (lockedAttribute != null)
        // {
        //     _unlockAttributeUi.gameObject.SetActive(true);
        //     _unlockAttributeUi.Load(lockedAttribute.Item1, lockedAttribute.Item2);
        // }
        //
        // var currentRankDefine = ResourceManager.instance.GetRankDefine(weaponData.Rank);
        // var newRankDefine = ResourceManager.instance.GetRankDefine(weaponData.Rank + 1);
        // _currentRankText.text = currentRankDefine.name;
        // _newRankText.text = newRankDefine.name;
        // _currentRankText.color = currentRankDefine.color;
        // _newRankText.color = newRankDefine.color;


        // Open resource slot
        for (int i = 0; i < _currentFusionElement.Quantity; i++)
        {
            _resourceSlots[i].gameObject.SetActive(true);
        }

        // Update state in inventory to selected
        SetStateEquipInInventory(weaponData, true);

        // Disable useless equip
        foreach (var weaponDataTemp in _equipmentFusionHolder.AvailableWeaponDatas)
        {
            var model = _equipmentFusionHolder.GetModel(weaponDataTemp);
            if (model != null)
            {
                model.isLocked = !weaponData.CanFusionWith(weaponDataTemp);
                var holder = _equipmentFusionHolder.GetViewHolder(weaponDataTemp);
                holder?.UpdateView(model);
            }
        }
    }

    private void OnRemoveBaseFusionEquip(WeaponData weaponData, WeaponDesign weaponDesign, EquipmentUI equipmentUi)
    {
        ResetAll();
    }

    private void OnRemoveResourceFusionEquip(WeaponData weaponData, WeaponDesign weaponDesign, EquipmentUI equipmentUi)
    {
        foreach (var resourceSlot in _resourceSlots)
        {
            if (resourceSlot.WeaponData == weaponData)
            {
                SetStateEquipInInventory(weaponData, false);
                resourceSlot.Clear();
                _fusionButton.interactable = false;
                break;
            }
        }
    }

    private void SetStateEquipInInventory(WeaponData weaponData, bool isSelected)
    {
        var availableWp = _equipmentFusionHolder.AvailableWeaponDatas;

        if (availableWp != null &&
            availableWp.Count > 0)
        {
            foreach (var weaponDataTemp in availableWp)
            {
                if (weaponDataTemp == weaponData)
                {
                    var model = _equipmentFusionHolder.GetModel(weaponDataTemp);
                    if (model != null)
                    {
                        model.isSelected = isSelected;
                        var holder = _equipmentFusionHolder.GetViewHolder(weaponDataTemp);
                        holder?.UpdateView(model);
                    }
                }
            }
        }


        // foreach (var equipmentFusionUi in _equipmentFusionHolder.Equipments)
        // {
        //     if (equipmentFusionUi.WeaponData == weaponData)
        //     {
        //         var temp = (EquipmentFusionUI) equipmentFusionUi;
        //         temp.SetIsSelected(isSelected);
        //     }
        // }
    }

    private void ClearAllResources()
    {
        foreach (var resourceSlot in _resourceSlots)
        {
            SetStateEquipInInventory(resourceSlot.WeaponData, false);
            resourceSlot.Clear();
            resourceSlot.gameObject.SetActive(false);
        }
    }

    private void ClearBaseResource()
    {
        _currentFusionElement = null;
        _baseResourceWeaponData = null;
        _baseResourceUi.Clear();
    }

    private void ResetAll()
    {
        ResetResource();
        ResetEquipmentHolder();
    }

    private void ResetResource()
    {
        ClearBaseResource();
        ClearAllResources();
        _filterEquipFusionHelper.UpdateFilter();
        // _preFusionInfo.SetActive(false);
        _fusionButton.interactable = false;
        _resourcesHolder.gameObject.SetActive(false);
        _helpText.SetActive(true);
    }

    private void ResetEquipmentHolder()
    {
        _equipmentFusionHolder.ResetState();
    }

    private void CheckEnoughResource()
    {
        bool isEnough = true;

        for (int i = 0; i < _currentFusionElement.Quantity; i++)
        {
            if (_resourceSlots[i].WeaponData == null)
            {
                isEnough = false;
                break;
            }
        }

        if (isEnough)
        {
            _fusionButton.interactable = true;
        }
    }

    public void Fusion()
    {
        //NetworkDetector.instance.checkInternetConnection((reach) =>
        //{
        //    if (reach)
        //        StartFusion();
        //});

        StartFusion();
    }

    private void StartFusion()
    {
        if (CurrencyModels.instance.IsEnough(CurrencyType.GOLD, _currentFusionElement.GoldCost))
        {
            CurrencyModels.instance.AddCurrency(CurrencyType.GOLD, -_currentFusionElement.GoldCost);
            int highestLevel = _baseResourceWeaponData.GetWeaponLevel();

            foreach (var equipmentUi in _resourceSlots)
            {
                if (equipmentUi.WeaponData != null)
                {
                    if (highestLevel < equipmentUi.WeaponData.GetWeaponLevel())
                    {
                        highestLevel = equipmentUi.WeaponData.GetWeaponLevel();
                    }
                }

                equipmentUi.WeaponData?.Dismantle();
            }

            _baseResourceWeaponData.Rank++;
            _baseResourceWeaponData.SetWeaponLevel(highestLevel);
            _baseResourceWeaponData.ResetPowerData();
            MasterCanvas.CurrentMasterCanvas.ShowHUD(EnumHUD.HUD_FUSION_RESULT,false,null,_baseResourceWeaponData, _resourceSlots[0].WeaponData, _resourceSlots[1].WeaponData );

            SaveManager.Instance.SaveData();
            
            ResetResource();

            InitEquipment(() =>
            {
                ResetEquipmentHolder();
                // _filterEquipFusionHelper.UpdateFilter();
            });

            AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_UPGRADE);
        }
        else
        {
            MainMenuCanvas.instance.ShowNotEnoughHUD(CurrencyType.GOLD, _currentFusionElement.GoldCost);
        }
    }

    private void CreateFakeAnimation(EquipmentUI target, Transform destination, Action callback = null)
    {
        var clone = Instantiate(target.gameObject, transform);
        var rect = clone.GetComponent<RectTransform>();
        rect.sizeDelta = target.GetComponent<RectTransform>().sizeDelta;
        clone.GetComponent<EquipmentUI>().HideUnNecessaryInfo();
        clone.transform.position = target.transform.position;

        rect.DOSizeDelta(destination.GetComponent<RectTransform>().sizeDelta, 0.18f);
        clone.transform.DOMove(destination.position, 0.2f).OnComplete(() =>
        {
            Destroy(clone);
            callback?.Invoke();
        });
    }
    
    
}