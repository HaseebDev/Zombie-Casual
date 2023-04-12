using com.datld.data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualShootBehaviour : AIBehavior
{
    private HeroData _data;

    public float findDistance { get; private set; }
    public LayerMask maskDetectZombie;

    public Character manualHero { get; private set; }
    private float lastFireTime = 0;
    private float tapRate = 0;

    public override void ApplyBehavior()
    {
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.ON_TOUCH_GROUND,
            new Action<Vector3, bool>(OnTouchOnGround));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.ON_TOUCH_ZOMBIE, new Action<Health>(OnTouchZombie));
    }

    public override void StopBehavior()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.ON_TOUCH_GROUND,
            new Action<Vector3, bool>(OnTouchOnGround));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.ON_TOUCH_ZOMBIE, new Action<Health>(OnTouchZombie));
    }

    // private void Update()
    // {
    //     EventSystemServiceStatic.DispatchAll(EVENT_NAME.ON_TOUCH_GROUND, Vector3.zero, true);
    // }

    public void OnTouchOnGround(Vector3 worldPos, bool forceShoot)
    {
        if (!CanFire())
            return;
        lastFireTime = Time.time;
        
        if (!manualHero.IsEnableHero)
        {
            var manualPos = GamePlayController.instance.gameLevel.GetManualHeroPos();
            manualHero.SetPosition(manualPos);
            manualHero.EnableHero(true);
        }

        manualHero.ResetTimeDisableHero();
        manualHero.FindTargetThenShoot(worldPos, forceShoot);
    }

    private bool CanFire()
    {
        if (tapRate == 0)
        {
            tapRate = Mathf.RoundToInt(DesignHelper.GetSkillDesign(GameConstant.ADD_ON_AUTO_MANUAL_HERO).Number);
        }

        return Time.time - lastFireTime > 1.0f / tapRate * 1.0f;
    }

    public void OnTouchZombie(Health targetPos)
    {
        if (!CanFire())
            return;
        lastFireTime = Time.time;
        
        if (!manualHero.IsEnableHero)
        {
            var manualPos = GamePlayController.instance.gameLevel.GetManualHeroPos();
            manualHero.SetPosition(manualPos);
            manualHero.EnableHero(true);
        }

        manualHero.ResetTimeDisableHero();
        manualHero.ShootAtTarget(targetPos);
    }

    //private IHealth FindNearestTarget(Vector3 worldPos)
    //{
    //    var hits = Physics.OverlapSphere(worldPos, this.findDistance, maskDetectZombie, QueryTriggerInteraction.Collide);

    //    if (hits != null && hits.Length > 0)
    //    {
    //        var targetIndex = -1;
    //        float minDist = float.MaxValue;
    //        IHealth targetZom = null;

    //        for (int i = 0; i < hits.Length; i++)
    //        {
    //            var dist = Vector3.Distance(worldPos, hits[i].transform.position);
    //            IHealth zom = hits[i].GetComponent<IHealth>();
    //            if (dist < this.findDistance && !zom.IsDead() && dist < minDist)
    //            {
    //                targetZom = zom;
    //                targetIndex = i;
    //                minDist = dist;
    //            }
    //        }

    //        if (targetIndex != -1)
    //        {
    //            return targetZom;
    //        }
    //    }

    //    return null;

    //}

    public void Initialize(HeroData data)
    {
        _data = data;
        var Power = _data.GetPowerDataByGameMode();
        findDistance = _data.FinalPowerData.Range;
        manualHero = GamePlayController.instance.gameLevel.ManualHero;
    }
}