using Ez.Pooly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideLauncher : Launcher
{
    [Header("ExplodeParticle")]
    public AutoDespawnParticles _explodeParticle;

    private Collider[] _hits;

    private void Start()
    {
        _hits = new Collider[5];
    }

    protected override IBullet GetNewBullet(float offsetX = 0, float localRotY = 0, params object[] extends)
    {
        return null;
    }

    public override void Launch(float _force, float _damage, ShotType shottype)
    {
        OnDealAoeDmg(transform.position, 2.0f, _damage);
    }

    public void OnDealAoeDmg(Vector3 startPos, float radius, float dmg)
    {
        int numHit = Physics.OverlapSphereNonAlloc(startPos, radius, _hits, this._targetMask);
        if (numHit > 0)
        {
            for (int i = 0; i < numHit; i++)
            {
                var col = _hits[i];
                IHealth component = col.transform.GetComponent<IHealth>();
                if (component != null)
                {
                    component.SetDamage(dmg, ShotType.NORMAL, null);
                }
            }
            var fxBoom = Pooly.Spawn<AutoDespawnParticles>(_explodeParticle.transform, startPos, Quaternion.identity, null);
            fxBoom.PlayEffect();
        }

    }

}
