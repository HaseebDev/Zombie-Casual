using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CharacterGrenade : BaseBomb
{
    public void SetMoveType(MOVE_TYPE _type)
    {
        _moveType = _type;
    }
    private Vector3 _targetPos;

    private bool isFlying = false;
    private float HeightExtends = 0f;

    public float MoveSpeed = 3f;
    private Action ActionComplete;

    public override void Launch(Vector3 targetPos, Action OnComplete)
    {
        base.Launch(targetPos, OnComplete);
        _targetPos = targetPos;
        isFlying = true;
        ActionComplete = OnComplete;
        switch (this._moveType)
        {
            case MOVE_TYPE.NONE:
                break;
            case MOVE_TYPE.THROW:
                transform.LookAt(_targetPos);
                transform.DOKill();
                transform.DOJump(targetPos, 3, 1, 0.8f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    isFlying = false;
                    OnComplete?.Invoke();
                });

                break;
            case MOVE_TYPE.FLY:

                transform.DOMove(targetPos, 1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    isFlying = false;
                    OnComplete?.Invoke();
                });
                break;
            default:
                break;
        }

    }


    private void Update()
    {
        if (isFlying)
        {
            transform.LookAt(_targetPos);

        }
    }

    //private void Update()
    //{

    //}
}
