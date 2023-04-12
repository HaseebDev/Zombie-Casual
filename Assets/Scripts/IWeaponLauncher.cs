using System;
using System.Collections.Generic;

public interface IWeaponLauncher
{
    List<EffectHit> ListEffectHits { get; set; }
    void Initialize(List<EffectHit> _effectHits);
    void Launch(float _force, float _damage, ShotType _type);
}
