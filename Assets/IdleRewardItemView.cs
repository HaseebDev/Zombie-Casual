using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IdleRewardItemView : MonoBehaviour
{
    public Text txtValue;
    public Image rewardImg;
    private RewardData _data;

    public void Initialize(RewardData rewardData)
    {
        _data = rewardData;
        txtValue.text = $"x{ _data._value.ToString()}";

         ResourceManager.instance.GetRewardSprite(_data._type, s =>
         {
             rewardImg.sprite = s;
         }, (string)_data._extends);
    }
}
