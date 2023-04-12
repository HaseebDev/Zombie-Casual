using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using QuickType.StarReward;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class StarRewardUI : MonoBehaviour
{
    [SerializeField] private RewardUI _defaultReward;
    [SerializeField] private RewardUI _battlePassReward;
    [SerializeField] private TextMeshProUGUI _starUnlockText;
    [SerializeField] private TextMeshProUGUI _mileStoneText;

    private StarRewardDesignElement _starRewardDesignElement;
    public StarRewardDesignElement StarRewardDesignElement => _starRewardDesignElement;

    private Action<StarRewardDesignElement, RewardData> _onClickDefaultReward;
    private Action<StarRewardDesignElement, RewardData> _onClickBPReward;

    [SerializeField] private GameObject _enoughImage;
    [SerializeField] private GameObject _lockDefault;
    [SerializeField] private GameObject _lockBP;
    [SerializeField] private Button _claimDefaultBtn;
    [SerializeField] private Button _claimBPBtn;
    [SerializeField] private UIShiny _claimDefaultBtnShiny;
    [SerializeField] private UIShiny _claimBPBtnShiny;

    [SerializeField] private GameObject _claimedDefault;
    [SerializeField] private GameObject _claimedBP;

    public void LoadMileStoneIndex(int index)
    {
        _mileStoneText.text = $"{index + 1}";
    }

    public void Load(StarRewardDesignElement starRewardDesignElement)
    {
        _starRewardDesignElement = starRewardDesignElement;
        _starUnlockText.text = starRewardDesignElement.Star.ToString();

        _defaultReward.Load(CreateRewardData(_starRewardDesignElement, true));
        _battlePassReward.Load(CreateRewardData(_starRewardDesignElement, false));
    }

    public void OnClickDefaultClaim()
    {
        _onClickDefaultReward?.Invoke(_starRewardDesignElement, CreateRewardData(_starRewardDesignElement, true));
    }

    public void OnClickBpClaim()
    {
        _onClickBPReward?.Invoke(_starRewardDesignElement, CreateRewardData(_starRewardDesignElement, false));
    }

    private RewardData CreateRewardData(StarRewardDesignElement starRewardDesignElement, bool isDefault)
    {
        float value = isDefault
            ? starRewardDesignElement.FreeRewardValue
            : starRewardDesignElement.BattlePassRewardValue;
        string type = isDefault
            ? starRewardDesignElement.FreeRewardType
            : starRewardDesignElement.BattlePassRewardType;

        RewardData rewardData = new RewardData(type,(long) value);
        if (rewardData._type == REWARD_TYPE.EQUIP)
        {
            string[] wpArray = type.Split('/');
            int rank = 1;
            
            if (wpArray.Length == 2)
                rank = int.Parse(wpArray[1]);
            
            rewardData._extends =
                SaveGameHelper.defaultWeaponData(wpArray[0], rank);
        }

        return rewardData;
    }

    public void SetOnClickCallback(Action<StarRewardDesignElement, RewardData> defaultReward,
        Action<StarRewardDesignElement, RewardData> bpReward)
    {
        _onClickDefaultReward = defaultReward;
        _onClickBPReward = bpReward;
    }

    public void SetLock(bool isLock)
    {
        _claimDefaultBtn.gameObject.SetActive(true);
        _claimBPBtn.gameObject.SetActive(true);
        
        _enoughImage.SetActive(!isLock);
        _claimDefaultBtn.interactable = !isLock;
        _claimBPBtn.interactable = !isLock;

        _claimBPBtnShiny.enabled = false;
        _claimDefaultBtnShiny.enabled = !isLock;

        _lockDefault.SetActive(isLock);
        _lockBP.SetActive(isLock);

        if (!isLock && !BattlePassManager.HasBattlePass)
        {
            _claimBPBtn.interactable = false;
            _lockBP.SetActive(true);
        }
    }

    public void SetClaim(bool isDefault)
    {
        if (isDefault)
        {
            _claimDefaultBtn.gameObject.SetActive(false);
            _claimedDefault.gameObject.SetActive(true);
        }
        else
        {
            _claimBPBtn.gameObject.SetActive(false);
            _claimedBP.gameObject.SetActive(true);
        }
    }

    public void ResetClaim()
    {
        _claimedDefault.gameObject.SetActive(false);
        _claimedBP.gameObject.SetActive(false);
    }
}