using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    [SerializeField] private float scaleToValue;
    private Vector3 _initScale;

    private bool isBusy = false;

    private void Awake()
    {
        _initScale = transform.localScale;
    }

    public void Pulse()
    {
        if (isBusy)
            return;

        isBusy = true;
        transform.DOKill();
        transform.localScale = _initScale;
        transform.DOScale(_initScale * scaleToValue, 0.1f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => { isBusy = false; });
    }
}