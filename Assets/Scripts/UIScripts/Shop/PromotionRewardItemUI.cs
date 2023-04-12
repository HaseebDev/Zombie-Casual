using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromotionRewardItemUI : MonoBehaviour
{
    [SerializeField] private Image _icon;

    [SerializeField] private Text _value;

    public void Load(RewardData rewardData)
    {
        ResourceManager.instance.GetRewardSprite(rewardData._type, s => { _icon.sprite = s; },
            (string) rewardData._extends);
        _value.text = "+" + rewardData._value.ToString();
    }
}