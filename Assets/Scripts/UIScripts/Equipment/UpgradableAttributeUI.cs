using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UpgradableAttributeUI : AttributeUI
{
    private Animator _animator;

    private Tween _numberTween;

    // private Sequence _sequence;
    private double _oldBonus = -1;

    public void PlayUpgradeAnim()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        _animator.speed = 1;
        _animator.Play("UpgradeAttibute", -1, 0);
        UpdateTextAnim();

        AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_UPGRADE);
    }

    private void OnEnable()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        _animator.Play("UpgradeAttibute", -1, 0f);
        _animator.speed = 0;
    }

    public void UpdateTextAnim()
    {
        // Value = Bonus;
        // string valueText = $"{Value:00}"; //.ToString();
        // UpdateValueText(_attributeDesign.IsPercent() ? valueText + "%" : valueText);

        _numberTween?.Kill();
        _numberTween = DOTween.To(() => Value, (x) =>
        {
            Value = x;
            UpdateValueText(DesignHelper.GetValueTextFromAttribute(Value, _attributeDesign));
        }, Bonus, 0.5f).SetEase(Ease.Linear); //.OnComplete(() => { lastCurrencyValue = currentValue; });

        // _sequence?.Kill();
        // _sequence = DOTween.Sequence();
        // _sequence.Append(_value.transform.DOScale(1.2f, 0.25f).OnComplete(() =>
        // {
        //     Value = Bonus;
        //     string valueText = $"{Value:00}"; //.ToString();
        //     UpdateValueText(_attributeDesign.IsPercent() ? valueText + "%" : valueText);
        // }));
        //
        // _sequence.Append(_value.transform.DOScale(1, 0.25f));
    }
}