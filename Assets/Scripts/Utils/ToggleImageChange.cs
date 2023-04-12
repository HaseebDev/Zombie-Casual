using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ToggleImageChange : MonoBehaviour
{
    [SerializeField] private Image _targetGraphic;
    [SerializeField] private Sprite _onSprite;
    [SerializeField] private Sprite _offSprite;

    private void Awake()
    {
        GetComponent<Toggle>().onValueChanged.AddListener((isOn) =>
        {
            _targetGraphic.sprite = isOn ? _onSprite : _offSprite;
            AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_BUTTON);
        });
    }
}