using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ez.Pooly;

public class RewardItemView : MonoBehaviour
{
    public TextMeshProUGUI txtValue;
    public Image img;

    [SerializeField] private HeroScrollUI _heroScrollUi;

    private HeroScrollUI _heroScrollView;

    public RewardData Data { get; private set; }

    public long CurrentValue {
        get { return Data != null ? Data._value : 0; }
    }

    public void Initialize(RewardData _data, bool autoHideText = false)
    {
        Data = _data;
        txtValue.text = _data._value.ToString(); //_data._value == 0 ? "" : _data._value.ToString();
        if (autoHideText && _data._value == 0)
            txtValue.text = "";

        if (_data._type == REWARD_TYPE.SCROLL_HERO)
        {
            _heroScrollView?.gameObject.SetActiveIfNot(true);
            txtValue.gameObject.SetActiveIfNot(false);
            img.gameObject.SetActiveIfNot(false);

            if (_heroScrollView == null)
            {
                _heroScrollView = Pooly.Spawn<HeroScrollUI>(_heroScrollUi.transform, Vector3.zero, Quaternion.identity, transform);
                _heroScrollView.transform.localScale = Vector3.one;
            }


            _heroScrollView.Load(_data);
        }
        else
        {
            _heroScrollView?.gameObject.SetActiveIfNot(false);
            txtValue.gameObject.SetActiveIfNot(true);
            img.gameObject.SetActiveIfNot(true);
            ResourceManager.instance.GetRewardSprite(_data._type, s =>
            {
                img.sprite = s;
            }, (string)_data._extends);
        }


    }

    public void SetValue(long newValue)
    {
        Data._value = newValue;
        txtValue.text = Data._value.ToString();
    }
}