using System;
using System.Collections.Generic;
using UnityEngine;

public interface IBullet
{
    float OverwriteForce { get; set; }
    void ResetBullet();
    void SetIgnoreEffectHits(List<int> _listChosenEffectHit);
    void Initialize(List<EffectHit> listEffects, string OwnerID, LayerMask maskHit, float rangeBullet = float.PositiveInfinity);
    void Launch(float _force, float _damage, ShotType _type, Action OnHit = null, Vector3 offsetCastDmg = default(Vector3));
    void SetMultiplyPerHit(float _multiplyPerHit);

}
