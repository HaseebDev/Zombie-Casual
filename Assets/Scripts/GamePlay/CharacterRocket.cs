using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRocket : BaseBomb
{
    private Vector3 _targetPos;
    public override void Launch(Vector3 targetPos, Action OnComplete)
    {
        base.Launch(targetPos, OnComplete);
        _targetPos = targetPos;
        transform.LookAt(_targetPos);
        transform.DOMove(targetPos, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
    }
}
