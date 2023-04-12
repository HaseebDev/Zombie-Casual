using QuickType.Zombie;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ez.Pooly;

public class ZombieShield02 : Zombie
{
    public ZombieShieldObject _shieldObjectPrefab;

    private ZombieShieldObject _shieldObject;
    public override void Initialize(ZombieElement data, float hpMultiplier, float dmgMultiplier, int wave, int subWave, int index)
    {
        base.Initialize(data, hpMultiplier, dmgMultiplier, wave, subWave, index);
        //if (_shieldObject != null)
        //{
        //    _shieldObject.CleanUp();
        //    _shieldObject = null;
        //}

    }

    public override void Health_OnDie(bool skipAnimDead)
    {
        _shieldObject = Pooly.Spawn<ZombieShieldObject>(_shieldObjectPrefab.transform, transform.position, Quaternion.identity, transform);
        var shieldHP = this.health.GetHPWithCoeff() * _data.Value / 100f;
        _shieldObject.Initialize(shieldHP);
        _shieldObject.transform.localPosition = Vector3.zero;
        _shieldObject.transform.localScale = Vector3.one;
        _shieldObject.transform.localRotation = Quaternion.identity;
        _shieldObject.transform.SetParent(null);
        _shieldObject.PlayAnimDeploy();
        base.Health_OnDie(skipAnimDead);
    }
}
