using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBoss2Head : Zombie
{
    public AutoDespawnParticles _parHitSlamPrefab;

    public Transform _markerHitSlam;

    public override void AnimationCallbackShoot()
    {
        base.AnimationCallbackShoot();
        GameMaster.instance.PlayEffect(_parHitSlamPrefab, _markerHitSlam.transform.position, Quaternion.identity, null);

        GamePlayController.instance.DoCameraShake();
    }
}
