using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI valueText;

    private Action _onClick;

    public Graphic TargetGraphic => iconImg;

    public void Load(RewardData rewardData)
    {
         ResourceManager.instance.GetRewardSprite(rewardData._type, s =>
         {
             iconImg.sprite = s;
         }, (string) rewardData._extends);
        if (valueText == null)
        {
            valueText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (rewardData._value != 0)
            valueText.text = $"x{FBUtils.CurrencyConvert(rewardData._value)}";
        else
            valueText.text = "";
    }

    public void SetOnClickCallback(Action callback)
    {
        _onClick = callback;
    }

    public void OnClick()
    {
        _onClick?.Invoke();
    }
}