using Ez.Pooly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicWaveUltimate : PierceShotUltimate
{

    public float PushBackSpeed = 1.0f;

    public override void ShotBullet(Vector3 pos, float angle)
    {
        _bullet = Pooly.Spawn<Bullet>(bulletPrefab.transform, Vector3.zero, Quaternion.identity, null);
        _bullet.transform.rotation = Quaternion.AngleAxis(_dragAngle, Vector3.down);
        _bullet.transform.position = pos + Vector3.up - transform.forward * 0.5f;
        _bullet.gameObject.SetActiveIfNot(true);
        _bullet.GetComponent<SphereCollider>().radius = _design.Radius;

        PushBackBullet _pushBullet = (PushBackBullet)_bullet;
        _pushBullet.ResetValue(_design.Value, PushBackSpeed);
        _bullet.Launch(20, this.GetUltimateDmg(), ShotType.NORMAL);
    }
}
