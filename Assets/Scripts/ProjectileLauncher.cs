using System;
using UnityEngine;
using Ez.Pooly;
using System.Collections.Generic;

public class ProjectileLauncher : Launcher, IWeaponLauncher
{
    protected override void Start()
    {
        base.Start();
    }

    private void ShowShootParticle()
    {
        if (this.shootParticlesEffects == null)
        {
            return;
        }

        for (int i = 0; i < this.shootParticlesEffects.Length; i++)
        {
            if (this.shootParticlesEffects[i].transform != null)
            {
                var par = Pooly.Spawn<AutoDespawnParticles>(this.shootParticlesEffects[i].transform, weaponMountPoint.position, weaponMountPoint.rotation, null);
                par.PlayEffect();
            }

        }
    }

    protected override IBullet GetNewBullet(float offsetX, float localRotY = 0f, params object[] extends)
    {
        this.ShowShootParticle();
        var targetPos = this.weaponMountPoint.position + this.weaponMountPoint.forward * 1f + this.weaponMountPoint.right * offsetX;
        var targetRot = this.weaponMountPoint.rotation.eulerAngles;
        targetRot.y += localRotY;
        Quaternion rot = Quaternion.Euler(targetRot);

        List<int> _listIgnoreEffects = new List<int>();
        EffectType lastType = EffectType.NONE;
        if (this.ListEffectHits != null && this.ListEffectHits.Count > 0)
        {
            for (int i = 0; i < ListEffectHits.Count; i++)
            {
                var rand = UnityEngine.Random.Range(0, 101);
                if (rand <= ListEffectHits[i].Chance)
                {
                    lastType = ListEffectHits[i].Type;
                }
                else
                    _listIgnoreEffects.Add(i);
            }
        }

        IBullet bullet = null;
        Transform specialBulletPrefab = ResourceManager.instance.GetSpecialBulletPrefab(lastType);
        if (specialBulletPrefab != null)
        {
            bullet = Pooly.Spawn<IBullet>(specialBulletPrefab.transform, targetPos, rot, null);
            bullet.OverwriteForce = 25;

            AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_MANUAL_BULLET);
        }
        else
        {
            bullet = Pooly.Spawn<IBullet>(this.bulletPrefab.transform, targetPos, rot, null);
            bullet.OverwriteForce = -1;
        }

        bullet.SetIgnoreEffectHits(_listIgnoreEffects);
        bullet.Initialize(ListEffectHits, this.weapon.OwnerID, this._targetMask);
        return bullet;
    }
}
