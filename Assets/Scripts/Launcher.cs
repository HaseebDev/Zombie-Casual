using System;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using QuickType.SkillDesign;

public abstract class Launcher : MonoBehaviour, IWeaponLauncher
{
    protected virtual void Start()
    {
        Weapon weapon = this.weapon;
        Initialize(weapon.effectHits);
        //weapon.OnLaunch = (Action<float, float, ShotType>)Delegate.Combine(weapon.OnLaunch, new Action<float, float, ShotType>(this.Launch));
    }

    protected bool muteShootSFX = false;

    public void ResetLauncher(bool muteShoot)
    {
        muteShootSFX = muteShoot;
    }

    public virtual void Launch(float _force, float _damage, ShotType shottype)
    {
        if (weapon.isActiveAndEnabled)
        {
            bool spawnMainBullet = true;

            if (ListEffectHits != null && ListEffectHits.Count > 0)
            {
                foreach (var effect in ListEffectHits)
                {
                    SkillDesignElement Design = null;
                    if (effect.Type != EffectType.NONE)
                    {
                        Design = DesignHelper.GetSkillDesign(effect.SkillID);
                    }
                    else
                        continue;
                    switch (effect.Type)
                    {
                        case EffectType.PASSIVE_DIAGONAL:
                            this.GetNewBullet().Launch(this.FireForce, _damage, shottype);
                            this.GetNewBullet(-0.15f, -10, true).Launch(this.FireForce, _damage * effect.Value / 100f, shottype);
                            this.GetNewBullet(0.15f, 10f, true).Launch(this.FireForce, _damage * effect.Value / 100f, shottype);
                            break;
                        case EffectType.PASSIVE_PIERCE:
                            var bullet = this.GetNewBullet();
                            bullet.SetMultiplyPerHit(effect.Value * 1.0f / 100f);
                            bullet.Launch(this.FireForce, _damage, ShotType.PIERCE);

                            spawnMainBullet = false;
                            break;
                        case EffectType.PASSIVE_AOE:
                            this.GetNewBullet().Launch(_force, _damage, ShotType.AOE);
                            break;
                        case EffectType.PASSIVE_BULLET_TRAIL:
                            this.GetNewBullet(-0.25f, 0f, true).Launch(this.FireForce, _damage * effect.Value / 100f, shottype, null, new Vector3(-40, 0, 0));
                            this.GetNewBullet(0.25f, 0f, true).Launch(this.FireForce, _damage * effect.Value / 100f, shottype, null, new Vector3(40, 0, 0));
                            break;
                        case EffectType.PASSIVE_DEAD_SHOT:
                            this.GetNewBullet().Launch(_force, _damage, ShotType.DEADSHOT);
                            break;
                        default:

                            break;
                    }
                }
            }

            if (spawnMainBullet)
            {
                var mainBullet = this.GetNewBullet();
                mainBullet.Launch(this.FireForce, _damage, shottype);
            }


            if (!this.muteShootSFX)
                AudioSystem.instance.PlaySFX(this._soundFire);

        }
    }

    protected abstract IBullet GetNewBullet(float offsetX = 0f, float localRotY = 0f, params object[] extends);

    private void OnDestroy()
    {
        Weapon weapon = this.weapon;
        //weapon.OnLaunch = (Action<float, float, ShotType>)Delegate.Remove(weapon.OnLaunch, new Action<float, float, ShotType>(this.Launch));
    }

    public void Initialize(List<EffectHit> _effectHits)
    {
        this._listEffectHits = _effectHits;
    }

    [SerializeField]
    protected Weapon weapon;

    [SerializeField]
    public GameObject bulletPrefab;

    [SerializeField]
    public Transform weaponMountPoint;

    [SerializeField]
    public AutoDespawnParticles[] shootParticlesEffects;

    private List<EffectHit> _listEffectHits = new List<EffectHit>();
    public List<EffectHit> ListEffectHits { get => _listEffectHits; set => _listEffectHits = value; }

    public float FireForce;

    private Transform _gunMount;

    public void Initialize(Transform gunMount, Weapon _weapon)
    {
        this.weapon = _weapon;
        if (gunMount)
            weaponMountPoint = gunMount;

    }

    public void ResetMountPoint()
    {
        //weaponMountPoint.transform.position = _gunMount.transform.position;
        //weaponMountPoint.transform.rotation = _gunMount.transform.rotation;
        //weaponMountPoint.transform.position = _gunMount.transform.position;
        //Vector3 rayDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        //weaponMountPoint.LookAt(weaponMountPoint.position + rayDirection * 3f);
        //weaponMountPoint.localRotation = Quaternion.Euler(weaponMountPoint.localEulerAngles.x, 0, weaponMountPoint.localEulerAngles.z);

        //weaponMountPoint.transform.position = _gunMount.position;
        //weaponMountPoint.transform.rotation = _gunMount.rotation;
    }

    [Header("Target Mask")]
    public LayerMask _targetMask;

    [Header("SFX")]
    [Space(10)]
    public SFX_ENUM _soundFire;

}
