using System;
using UnityEngine;
using Ez.Pooly;
using System.Collections;
using System.Collections.Generic;
using MEC;

[RequireComponent(typeof(Motor))]
public class GoToHellBehavior : AIBehavior
{
    private bool isApplied = false;

    private Vector3 hitPoint;

    public void SetHitPoint(Vector3 hit)
    {
        hitPoint = hit;
    }

    public override void ApplyBehavior()
    {
    }

    public void ApplyBehavior(bool skipDeadEffect = false)
    {
        if (!skipDeadEffect)
            Timing.RunCoroutine(AnimDeadCoroutine());
        else
            OnGoToHellComplete?.Invoke();
    }

    IEnumerator<float> AnimDeadCoroutine()
    {
        yield return Timing.WaitForOneFrame;
        GameMaster.PlayEffect(COMMON_FX.FX_ZOMBIE_EXPLODE, hitPoint, Quaternion.identity, null);
        yield return Timing.WaitForOneFrame;
        OnGoToHellComplete?.Invoke();
    }

    private float timerGoHell = 0f;

    public void Update()
    {
        //float _deltaTime = Time.deltaTime;
        //if (isApplied)
        //{
        //    if (this.motor != null)
        //    {
        //        this.motor.MoveYDown(_deltaTime);
        //    }
        //    timerGoHell += _deltaTime;
        //    if (timerGoHell >= 3.0f)
        //    {
        //        OnGoToHellComplete?.Invoke();
        //        timerGoHell = 0f;
        //        isApplied = false;
        //    }
        //}

    }

    public override void StopBehavior()
    {
        base.enabled = false;
        isApplied = false;
    }

    [SerializeField]
    private Motor motor;


    public Action OnGoToHellComplete;
}
