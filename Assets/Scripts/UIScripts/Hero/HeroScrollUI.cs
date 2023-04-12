using System.Collections;
using System.Collections.Generic;
using QuickType;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class HeroScrollUI : MonoBehaviour
{
    [SerializeField] private Image _heroImg;
    [SerializeField] private LocalizedTMPTextUI _quantity;

    public void Load(RewardData rewardData)
    {
        ResourceManager.instance.GetRewardSprite(rewardData._type, (sp) =>
        {
            _heroImg.sprite = sp;
        },(string)rewardData._extends);

        // ResourceManager.instance.GetHeroAvatar((string) rewardData._extends,_heroImg);
        if (rewardData._value != 0)
            _quantity.text = "x" + rewardData._value;
        else
            _quantity.text = "";
    }
}