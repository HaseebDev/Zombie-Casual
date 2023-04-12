using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BlinkText : MonoBehaviour
{
    [SerializeField] private float _speed = 0.5f;
    [SerializeField] private float _endValue = 1f;
    [SerializeField] private float _startValue = 0f;

    private void Awake()
    {
        var text = GetComponent<TextMeshProUGUI>();
        // text.DOFade(_startValue, 0);

        // Sequence sequence = DOTween.Sequence();
        // sequence.Append(text.DOFade(_endValue, _speed).SetEase(Ease.Linear));
        // sequence.Append(text.DOFade(_startValue, _speed).SetEase(Ease.Linear));
        // sequence.SetLoops(-1).SetEase(Ease.Linear);
        text.DOFade(_startValue, _speed)
            .SetLoops(-1, LoopType.Yoyo);
    }
}