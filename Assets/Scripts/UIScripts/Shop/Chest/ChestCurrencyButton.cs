using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestCurrencyButton : MonoBehaviour
{
    [SerializeField] private Text _value;
    [SerializeField] private Text _remindValue;
    [SerializeField] private GameObject _remindPanel;
    [SerializeField] private Image _icon;

    public void Load(REWARD_TYPE type, int value, bool loadWithReminder = true)
    {
         ResourceManager.instance.GetRewardSprite(type, s =>
         {
             _icon.sprite = s;
         });
        _value.text = value.ToString();
        _remindValue.text = _value.text;
        _remindPanel.SetActive(loadWithReminder);
    }

}
