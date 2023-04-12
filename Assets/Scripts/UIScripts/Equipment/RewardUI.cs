using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using QuickType.Weapon;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;
using TMPro;

public class RewardUI : MonoBehaviour
{
    [SerializeField] private EquipmentUI _equipmentUi;
    [SerializeField] private CurrencyUI _currencyUi;
    [SerializeField] private HeroScrollUI _heroScrollUi;
    [SerializeField] private Transform _transform;
    [SerializeField] private LocalizedTMPTextUI _name;

    [SerializeField] private Image _claimed;
    [SerializeField] private Image _locked;
    [SerializeField] private GameObject _shining;

    [SerializeField] private TextMeshProUGUI _txtTopTitle;

    private Action<RewardData> onClick;
    private RewardData _rewardData;
    public RewardData Data => _rewardData;
    private Button _button;

    private void OnClick()
    {
        onClick?.Invoke(_rewardData);
    }

    public void ShowShining(bool isShow)
    {
        _shining.SetActive(isShow);
    }

    private void InitButton()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }
    }

    public void Load(RewardData rewardData, bool clickable = true, string topTitle = null)
    {
        InitButton();
        _transform.DestroyAllChildImmediate();
        gameObject.SetActive(true);

        if (rewardData._type == REWARD_TYPE.EQUIP || rewardData._type == REWARD_TYPE.RANDOM_EQUIP)
        {
            var equipUI = Instantiate(_equipmentUi, _transform);
            WeaponDesign wpDesign = null;

            if (rewardData._extends is string)
            {
                var formatEquip = DesignHelper.FormatEquipRewardStr((string)rewardData._extends);
                wpDesign = DesignHelper.GetWeaponDesign(formatEquip.Key, formatEquip.Value, 1); ;
            }
            else
            {
                var equipData = (WeaponData)rewardData._extends;
                wpDesign = DesignHelper.GetWeaponDesign(equipData);
            }

            _name.text = wpDesign.Name.AsLocalizeString();
            _button.targetGraphic = equipUI.TargetGraphic;

            equipUI.Load(wpDesign);
            equipUI.gameObject.SetActive(true);

            Destroy(equipUI.GetComponent<Button>());
        }
        else if (rewardData._type == REWARD_TYPE.ADD_ON)
        {
            var currencyUi = Instantiate(_currencyUi, _transform);
            _name.text = ""; // rewardData._extends.ToString();
            _button.targetGraphic = currencyUi.TargetGraphic;

            currencyUi.Load(rewardData);
            currencyUi.gameObject.SetActive(true);
            Destroy(currencyUi.GetComponent<Button>());
        }
        else if (rewardData._type == REWARD_TYPE.SCROLL_HERO)
        {
            var heroShard = Instantiate(_heroScrollUi, _transform);
            _name.text = ""; // rewardData._extends.ToString();
            heroShard.Load(rewardData);
            heroShard.gameObject.SetActive(true);
            Destroy(heroShard.GetComponent<Button>());
        }
        else
        {
            var currencyUi = Instantiate(_currencyUi, _transform);
            _name.text = ""; //rewardData._type.ToString();
            _button.targetGraphic = currencyUi.TargetGraphic;

            currencyUi.Load(rewardData);
            currencyUi.gameObject.SetActive(true);
            Destroy(currencyUi.GetComponent<Button>());
        }

        if (!clickable)
            GetComponent<Button>().interactable = false;
        _rewardData = rewardData;

        if (_txtTopTitle != null)
            _txtTopTitle.gameObject.SetActiveIfNot(false);

        if (_txtTopTitle != null && !string.IsNullOrEmpty(topTitle))
        {
            _txtTopTitle.gameObject.SetActiveIfNot(true);
            _txtTopTitle.text = topTitle;
        }
    }

    public void Claimed()
    {
        _claimed.gameObject.SetActive(true);
        _button.interactable = false;
    }

    public void SetLock(bool isLock)
    {
        _locked.gameObject.SetActive(isLock);
        _button.interactable = !isLock;
    }

    public void SetOnClick(Action<RewardData> callBack)
    {
        onClick = callBack;
    }
}