using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using DG.Tweening;
using QuickType;
using QuickType.Chest;
using QuickType.Weapon;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class ChestOpenResult : MonoBehaviour
{
    [SerializeField] private LocalizedTMPTextUI _rankText;
    [SerializeField] private LocalizedTMPTextUI _nameText;
    [SerializeField] private LocalizedTMPTextUI _descriptionText;
    [SerializeField] private GameObject _afterOpenPanel;
    [SerializeField] private EquipmentUI _equipmentUi;
    [SerializeField] private EquipmentUI _fakeEquipmentUi;
    [SerializeField] private GameObject _fakeEquipmentUiParent;
    [SerializeField] private GameObject _shining;

    [Header("Chest")] [SerializeField] private SkeletonGraphic _chestAnim;
    [SerializeField] private Button _chestButton;
    [SerializeField] private Transform _spawnCardPosition;
    [SerializeField] private ParticleSystem _openPS;

    [SerializeField] private GameObject _guideText;

    [Header("Open again buttons")] [SerializeField]
    private GameObject _openAgainDiamond;

    [SerializeField] private Button _openAgainKey;
    [SerializeField] private Button _openAgainAds;
    [SerializeField] private Button _openAgainFree;
    [SerializeField] private GameObject _adsIcon;

    [SerializeField] private ReminderUI _reminderUi;
    [SerializeField] private LocalizedTMPTextUI _keyRemainText;

    [Header("Diamond")] [SerializeField] private LocalizedTMPTextUI _oldCostText;
    [SerializeField] private LocalizedTMPTextUI _newCostText;

    [Header("Key")] [SerializeField] private Image _keyIcon;

    private ChestDesignElement _chestDesignElement;
    private OpenResourceType _openResourceType;

    private bool _showedAdsBefore = false;

    private bool _isOpenAgain = false;
    // private void Awake()
    // {
    //     _chestAnim.AnimationState.End += entry => { };
    // }

    private void OnEnable()
    {
        CurrencyBar.Instance.ShowCurrencys(CurrencyType.DIAMOND);
    }


    public void OnButtonBack()
    {
        gameObject.SetActive(false);
        MainMenuTab.Instance?.Show();
        HUDShop.Instance.ResetLayers();
    }

    public void StartAnimOpen()
    {
        _chestButton.interactable = false;
        // Time.timeScale = 0.1f;
        _chestAnim.AnimationState.SetEmptyAnimation(0, 0);
        _chestAnim.AnimationState.SetAnimation(0, "an_openchest", false);
        AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_CHEST_OPEN);
        DOVirtual.DelayedCall(0.25f, () =>
        {
            _openPS.Play();
            var fake = Instantiate(_fakeEquipmentUiParent, transform);
            fake.gameObject.SetActive(true);
            fake.transform.position = _spawnCardPosition.position;
            fake.transform.DOMove(_equipmentUi.transform.position, 0.2f).OnComplete(() =>
            {
                _afterOpenPanel.gameObject.SetActive(true);
                Destroy(fake.gameObject);
            });
        });

        _guideText.SetActive(false);
    }

    private void PlayDropAnimation()
    {
        _chestButton.interactable = false;
        DOVirtual.DelayedCall(0.1f, () => { _chestAnim.AnimationState.SetAnimation(0, "an_appear", false); });
        DOVirtual.DelayedCall(0.5f, () => { _chestButton.interactable = true; });

        AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_CHEST_APPEAR);
    }


    public void OpenChest(ChestDesignElement chestDesignElement, OpenResourceType openResourceType)
    {
        _guideText.SetActive(true);
        _isOpenAgain = false;
        _showedAdsBefore = false;
        PlayDropAnimation();
        LoadData(chestDesignElement, openResourceType);
    }

    private void OnDisable()
    {
        _chestAnim.AnimationState?.SetEmptyAnimation(0, 0);
    }

    private void LoadData(ChestDesignElement chestDesignElement, OpenResourceType openResourceType)
    {
        MissionManager.Instance.TriggerMission(MissionType.OPEN_CHEST,1);

        MainMenuTab.Instance?.Hide();
        gameObject.SetActive(true);

        _chestDesignElement = chestDesignElement;
        _openResourceType = openResourceType;

        int randomRank = chestDesignElement.GetRandomEquipRank();

        if (chestDesignElement.ChestID == GameConstant.CHEST_LEGENDARY_ID)
        {
            int legendary = RankConstant.LEGENDARY;
            randomRank =
                EquipRankStackManager.CheckAndReset(legendary, randomRank, chestDesignElement.LegendaryEquipStack);
            _chestAnim.Skeleton.SetSkin("gold_chest");
        }
        else
        {
            _chestAnim.Skeleton.SetSkin("blue_chest");
        }

        WeaponData weaponData = SaveGameHelper.RandomWeaponData(randomRank, chestDesignElement.GetIgnoreWeapon());
        WeaponDesign weaponDesign = DesignHelper.GetWeaponDesign(weaponData);
        SaveManager.Instance.Data.Inventory.ListWeaponData.Add(weaponData);
        RankDefine rankDefine = ResourceManager.instance.GetRankDefine(randomRank);

        _rankText.textName = rankDefine.name;
        _nameText.textName = weaponDesign.Name;
        _rankText.targetTMPText.color = rankDefine.color;
        _nameText.targetTMPText.color = rankDefine.color;
        _descriptionText.text = weaponDesign.DescriptionId;
        _shining.SetActive(weaponData.Rank >= RankConstant.LEGENDARY);
        _equipmentUi.Load(weaponData, weaponDesign);
        _fakeEquipmentUi.Load(weaponData, weaponDesign);

        CheckButtonState(chestDesignElement, openResourceType);
        _afterOpenPanel.SetActive(false);
    }

    private void CheckButtonState(ChestDesignElement chestDesignElement, OpenResourceType openResourceType)
    {
        HideAllButtons();
        _reminderUi.Show(false);

        switch (openResourceType)
        {
            case OpenResourceType.FREE:
                _openAgainFree.gameObject.SetActive(true);

                if (chestDesignElement.ChestID == GameConstant.CHEST_LEGENDARY_ID &&
                    SaveManager.Instance.Data.Inventory.TotalAdsLegendaryChest > 0)
                {
                    _reminderUi.Load((int) (SaveManager.Instance.Data.Inventory.TotalAdsLegendaryChest +
                                            SaveManager.Instance.Data.Inventory.TotalLegendaryKey));
                    _openAgainFree.interactable = true;
                }
                else
                {
                    _openAgainFree.interactable = false;
                    CheckButtonState(chestDesignElement, OpenResourceType.DIAMOND);
                }

                break;
            case OpenResourceType.ADS:
                _openAgainAds.gameObject.SetActive(true);

                if (chestDesignElement.ChestID == GameConstant.CHEST_RARE_ID &&
                    SaveManager.Instance.Data.Inventory.TotalAdsRareChest > 0)
                {
                    _reminderUi.Load((int) (SaveManager.Instance.Data.Inventory.TotalAdsRareChest +
                                            SaveManager.Instance.Data.Inventory.TotalRareKey));
                    _openAgainAds.interactable = true;

                    // Force to watch ads
                    // if (!_isOpenAgain)
                    // {
                    //     _adsIcon.SetActive(false);
                    //     _showedAdsBefore = true;
                    // }
                    // else
                    // {
                    //     _adsIcon.SetActive(!_showedAdsBefore);
                    // }
                }
                else
                {
                    _openAgainAds.interactable = false;
                    CheckButtonState(chestDesignElement, OpenResourceType.DIAMOND);
                }

                break;
            case OpenResourceType.KEY_RARE:
                _openAgainKey.gameObject.SetActive(true);
                _keyRemainText.text = $"{SaveManager.Instance.Data.Inventory.TotalRareKey}/{1}";

                if (chestDesignElement.ChestID == GameConstant.CHEST_RARE_ID &&
                    SaveManager.Instance.Data.Inventory.TotalRareKey > 0)
                {
                    _reminderUi.Load((int) (SaveManager.Instance.Data.Inventory.TotalAdsRareChest +
                                            SaveManager.Instance.Data.Inventory.TotalRareKey));

                    ResourceManager.instance.GetRewardSprite(REWARD_TYPE.KEY_CHEST_RARE, s => { _keyIcon.sprite = s; });
                    _openAgainKey.interactable = true;
                }
                else
                {
                    _openAgainKey.interactable = false;
                    CheckButtonState(chestDesignElement, OpenResourceType.ADS);
                }

                break;
            case OpenResourceType.KEY_LEGENDARY:
                _openAgainKey.gameObject.SetActive(true);
                _keyRemainText.text = $"{SaveManager.Instance.Data.Inventory.TotalLegendaryKey}/{1}";

                if (chestDesignElement.ChestID == GameConstant.CHEST_LEGENDARY_ID &&
                    SaveManager.Instance.Data.Inventory.TotalLegendaryKey > 0)
                {
                    _reminderUi.Load((int) (SaveManager.Instance.Data.Inventory.TotalAdsLegendaryChest +
                                            SaveManager.Instance.Data.Inventory.TotalLegendaryKey));
                    ResourceManager.instance.GetRewardSprite(REWARD_TYPE.KEY_CHEST_LEGENDARY,
                        s => { _keyIcon.sprite = s; });
                    _openAgainKey.interactable = true;
                }
                else
                {
                    _openAgainKey.interactable = false;
                    CheckButtonState(chestDesignElement, OpenResourceType.FREE);
                }

                break;
            case OpenResourceType.DIAMOND:
                _oldCostText.text = chestDesignElement.DiamondCost.ToString();
                _newCostText.text = chestDesignElement.DiamondCostContinue.ToString();

                _openAgainDiamond.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(openResourceType), openResourceType, null);
        }
    }

    private void HideAllButtons()
    {
        _openAgainAds.gameObject.SetActive(false);
        _openAgainFree.gameObject.SetActive(false);
        _openAgainKey.gameObject.SetActive(false);
        _openAgainDiamond.gameObject.SetActive(false);
    }

    public void OpenAgainFree()
    {
        if (_chestDesignElement.ChestID == GameConstant.CHEST_LEGENDARY_ID &&
            SaveManager.Instance.Data.Inventory.TotalAdsLegendaryChest > 0)
        {
            SaveManager.Instance.Data.Inventory.TotalAdsLegendaryChest -= 1;
            OpenAgain();
        }
    }

    public void OpenAgainAds()
    {
        if (_chestDesignElement.ChestID == GameConstant.CHEST_RARE_ID &&
            SaveManager.Instance.Data.Inventory.TotalAdsRareChest > 0)
        {
            Action onSuccess = () =>
            {
                _showedAdsBefore = true;
                SaveManager.Instance.Data.Inventory.TotalAdsRareChest -= 1;
                OpenAgain();
                
                MissionManager.Instance.TriggerMission(MissionType.WATCH_VIDEO_OPEN_CHEST);
            };

            // if (_showedAdsBefore)
            // {
            //     onSuccess();
            // }
            // else
            // {
            AdsManager.instance.ShowAdsRewardWithNotify(onSuccess);
            // }
        }
    }

    public void OpenAgainKey()
    {
        if (_chestDesignElement.ChestID == GameConstant.CHEST_RARE_ID &&
            SaveManager.Instance.Data.Inventory.TotalRareKey > 0)
        {
            SaveManager.Instance.Data.Inventory.TotalRareKey -= 1;
            OpenAgain();
        }
        else if (_chestDesignElement.ChestID == GameConstant.CHEST_LEGENDARY_ID &&
                 SaveManager.Instance.Data.Inventory.TotalLegendaryKey > 0)
        {
            SaveManager.Instance.Data.Inventory.TotalLegendaryKey -= 1;
            OpenAgain();
        }
    }

    public void OpenAgainDiamond()
    {
        if (CurrencyModels.instance.IsEnough(CurrencyType.DIAMOND, _chestDesignElement.DiamondCostContinue))
        {
            CurrencyModels.instance.AddCurrency(CurrencyType.DIAMOND, -_chestDesignElement.DiamondCostContinue);
            OpenAgain();
        }
        else
        {
            MainMenuCanvas.instance.ShowNotEnoughHUD(CurrencyType.DIAMOND, _chestDesignElement.DiamondCostContinue);
        }
    }

    private void OpenAgain()
    {
        _isOpenAgain = true;
        LoadData(_chestDesignElement, _openResourceType);
        StartAnimOpen();
    }
}