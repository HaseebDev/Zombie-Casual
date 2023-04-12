using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using DG.Tweening;
using QuickType;
using QuickType.Chest;
using QuickType.Weapon;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class ChestOpenTenResult : MonoBehaviour
{
    [SerializeField] private Transform _equipHolder;

    [Header("Chest")] [SerializeField] private SkeletonGraphic _chestAnim;
    [SerializeField] private Button _chestButton;
    [SerializeField] private Transform _spawnCardPosition;
    [SerializeField] private ParticleSystem _openPS;
    [SerializeField] private GameObject _fakeEquipmentUiParent;
    [SerializeField] private GameObject _guideText;
    [SerializeField] private GameObject _afterOpenPanel;

    // [SerializeField] private LocalizedTMPTextUI _costText;

    private List<EquipmentUI2> _equipmentUis;
    private ChestDesignElement _chestDesignElement;
    private OpenResourceType _openResourceType;

    private void OnEnable()
    {
        CurrencyBar.Instance.ShowCurrencys(CurrencyType.DIAMOND);
    }

    private void Init()
    {
        if (_equipmentUis == null)
        {
            _equipmentUis = new List<EquipmentUI2>();
            foreach (Transform equip in _equipHolder)
            {
                _equipmentUis.Add(equip.GetComponent<EquipmentUI2>());
            }
        }
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
            for (int i = 0; i < 10; i++)
            {
                var fake = Instantiate(_fakeEquipmentUiParent, transform);
                fake.gameObject.SetActive(true);
                fake.GetComponentInChildren<EquipmentUI>()
                    .Load(_equipmentUis[i].WeaponData, _equipmentUis[i].WeaponDesign);

                fake.transform.position = _spawnCardPosition.position;
                int temp = i;
                fake.transform.DOMove(_equipmentUis[i].EquipmentUi.transform.position, 0.2f).OnComplete(() =>
                {
                    if (temp == 9)
                        _afterOpenPanel.gameObject.SetActive(true);

                    Destroy(fake.gameObject);
                });
            }
        
        });

        _guideText.SetActive(false);
    }

    private void OnDisable()
    {
        _chestAnim.AnimationState?.SetEmptyAnimation(0, 0);
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
        MissionManager.Instance.TriggerMission(MissionType.OPEN_CHEST,10);

        PlayDropAnimation();

        Init();
        MainMenuTab.Instance?.Hide();
        gameObject.SetActive(true);

        _chestDesignElement = chestDesignElement;
        _openResourceType = openResourceType;
        // _costText.text = chestDesignElement.DiamondCost.ToString();

        for (int i = 0; i < 10; i++)
        {
            int randomRank = chestDesignElement.GetRandomEquipRank();
            int epicRank = 4;
            randomRank =
                EquipRankStackManager.CheckAndReset(epicRank, randomRank, chestDesignElement.LegendaryEquipStack);

            WeaponData weaponData = SaveGameHelper.RandomWeaponData(randomRank, chestDesignElement.GetIgnoreWeapon());
            WeaponDesign weaponDesign = DesignHelper.GetWeaponDesign(weaponData);
            SaveManager.Instance.Data.Inventory.ListWeaponData.Add(weaponData);

            _equipmentUis[i].Load(weaponData, weaponDesign);
        }

        _guideText.SetActive(true);
        _afterOpenPanel.SetActive(false);
    }
}