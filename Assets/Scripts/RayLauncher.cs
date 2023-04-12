using Ez.Pooly;
using System;
using UnityEngine;

public class RayLauncher : Launcher
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
            this.shootParticlesEffects[i].PlayEffect();
        }
    }

    protected override IBullet GetNewBullet(float offsetX = 0f, float localRotY = 0f, params object[] extends)
    {
        bool forceCreateBullet = false;
        if (extends != null && extends.Length > 0)
        {
            forceCreateBullet = (bool)extends[0];
        }

        IBullet result = null;
        this.ShowShootParticle();
        var targetPos = this.weaponMountPoint.position + this.weaponMountPoint.transform.right * offsetX;
        var targetRot = this.weaponMountPoint.rotation.eulerAngles;
        targetRot.y += localRotY;
        Quaternion rot = Quaternion.Euler(targetRot);

        if (forceCreateBullet)
        {
            var bulletTrf = Pooly.Spawn(this.bulletPrefab.transform, targetPos, rot, null);
            bulletTrf.position = targetPos;
            bulletTrf.rotation = rot;
            result = bulletTrf.GetComponent<IBullet>();
        }
        else
        {
            if (rayStart == null)
            {
                lastCreatedBullet = Pooly.Spawn(this.bulletPrefab.transform, targetPos, rot, null);
                rayStart = lastCreatedBullet.GetComponent<IBullet>();
            }
            lastCreatedBullet.position = targetPos;
            lastCreatedBullet.rotation = rot;
            result = rayStart;
        }

        result.ResetBullet();
        result.Initialize(this.ListEffectHits, this.weapon.OwnerID, ResourceManager.instance._maskZombieOnly);

        return result;
    }

    protected Transform lastCreatedBullet;
    private IBullet rayStart;
}
