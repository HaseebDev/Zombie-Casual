using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public interface IHealth
{
    bool IsDead();
    bool AllowHeadshot { get; set; }
    bool IsTargetable { get; set; }
    Transform HitMarker { get; }
    float EffectScaleFactor { get; set; }
    void SetDamage(float _dmg, ShotType shotType = ShotType.NORMAL, string CasterID = null,
        List<EffectHit> EffectHits = null, Action<bool, List<EffectArmour>> responseHit = null, float range = 0, Vector3 offsetFloatingText = default(Vector3), List<int> _listIgnoreEffectHitsIndex = null);

    void SkipAnimDead(float inDuration);
}