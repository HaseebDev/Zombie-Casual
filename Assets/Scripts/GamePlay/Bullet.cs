using System;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using Ez.Pooly;
using System.Linq;

public class Bullet : PoolObject, IBullet
{
    //[Header("Layer Mask")]
    //public LayerMask _maskHit;
    public static readonly float TIME_DESTROY_BULLET = 5f;

    protected LayerMask _maskHit;
    public AutoDespawnParticles _parImpact;
    public ParticleSystem _parProjectile;

    protected Action OnHit;
    protected TrailRenderer _bulletTrail;
    protected RaycastHit[] raycastHits;
    protected int hitCount;

    protected bool IsLaunch;
    protected RaycastHit singleHit;
    protected Vector3 targetPos;
    protected float rangeBullet;

    protected float timerTravel = 0f;

    [Header("Effects")]
    [SerializeField] private bool HideFXBlood = false;

    protected Ray _ray;
    private RaycastHit _rayCastHit;

    private List<int> _ignoreEffectHitsIndex;

    protected float _overwriteForce = -1;
    public float OverwriteForce { get => _overwriteForce; set => _overwriteForce = value; }

    public override void OnAwake()
    {
        base.Done(this.lifeTime);
    }

    public void ResetBullet()
    {

    }

    private void Awake()
    {
        this._bulletTrail = GetComponent<TrailRenderer>();
    }

    public virtual void Update()
    {
        if (IsLaunch)
        {
            float deltaTime = Time.deltaTime;
            if (singleHit.transform != null && this.shotType != ShotType.PIERCE)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, deltaTime * bulletSpeed);

                if (Vector3.Distance(transform.position, targetPos) <= 0.1f)
                {
                    DestroyBullet();
                }
            }
            else
            {
                transform.position += transform.forward * deltaTime * bulletSpeed;
            }

            timerTravel += deltaTime;
            if (timerTravel >= TIME_DESTROY_BULLET)
            {
                timerTravel = 0f;
                DestroyBullet();
            }

        }
    }

    public virtual void Initialize(List<EffectHit> listEffects, string _OwnerID, LayerMask targetMask, float _rangeBullet = float.PositiveInfinity)
    {
        _maskHit = targetMask;
        this.effectHits = listEffects;
        this.OwnerID = _OwnerID;
        raycastHits = new RaycastHit[20];
        this.rangeBullet = _rangeBullet;
        timerTravel = 0f;
        this.MultiplyPerHit = 1.0f;
        _ray = new Ray();
    }

    public virtual void Launch(float _force, float _damage, ShotType _type, Action onHit = null, Vector3 offsetCastDmg = default(Vector3))
    {

        if (this._overwriteForce > 0)
            this.bulletSpeed = _overwriteForce;
        else
            this.bulletSpeed = _force;

        this.bulletDamage = _damage;
        this.shotType = _type;
        this.OnHit = onHit;
        this.IsLaunch = true;
        //rb.velocity = Vector3.zero;
        //rb.isKinematic = true;
        if (_bulletTrail)
        {
            _bulletTrail.Clear();
            _bulletTrail.enabled = true;
            _bulletTrail.emitting = true;
        }

        if (_parProjectile != null)
        {
            _parProjectile.Clear(true);
            _parProjectile.time = 0;
            _parProjectile.Play(true);
        }

        if (shotType == ShotType.PIERCE)
        {
            Timing.CallDelayed(2.0f, () =>
            {
                DestroyBullet();
            });
        }

        _ray.origin = transform.position;
        _ray.direction = transform.forward;
        if (this.shotType == ShotType.PIERCE)
        {
            raycastHits = Physics.RaycastAll(_ray, float.PositiveInfinity, _maskHit);
            RaycastHit[] array = (from h in raycastHits orderby h.distance select h).ToArray<RaycastHit>();
            hitCount = array.Length;
            if (hitCount > 0)
            {
                var tempDmg = this.bulletDamage;
                var dmgDecreasePerHit = this.bulletDamage * MultiplyPerHit;
                for (int i = 0; i < array.Length; i++)
                {
                    var hit = array[i];
                    CastDmg(hit, tempDmg, ShotType.NORMAL, offsetCastDmg);
                    tempDmg -= dmgDecreasePerHit;
                    if (tempDmg < 1)
                        break;
                }
            }
        }
        else
        {
            var isHit = Physics.Raycast(_ray, out singleHit, float.PositiveInfinity, _maskHit);
            if (isHit)
            {
                CastDmg(singleHit, this.bulletDamage, _type, offsetCastDmg);
            }
        }

        if (singleHit.transform != null)
        {
            targetPos = singleHit.transform.position;
            targetPos.y = transform.position.y;
        }

    }

    public void CastDmg(RaycastHit hit, float dmg, ShotType _shotType = ShotType.NORMAL, Vector3 offsetFloatingText = default(Vector3))
    {
        IHealth component = hit.transform.GetComponent<IHealth>();
        if (component != null)
        {
            var spawnPoint = component.HitMarker != null ? component.HitMarker.position : hit.point;

            spawnPoint -= transform.forward * 0.3f;

            component.SetDamage(dmg, _shotType, this.OwnerID, this.effectHits, null, 0, offsetFloatingText, _ignoreEffectHitsIndex);

            if (!component.IsDead() && !GameMaster.IsSpeedUp && GameMaster.instance.OptmizationController.Data.EnableImpactHit)
                GameMaster.instance.PlayEffect(_parImpact, spawnPoint, hit.transform.rotation, hit.transform.parent);

            if (!HideFXBlood && GameMaster.instance.OptmizationController.Data.EnableBlood)
            {
                if (hit.transform.CompareTag(TagConstant.TAG_BOSS))
                {
                    GameMaster.instance.PlayEffect(ResourceManager.instance._effectResources.GetEffect(COMMON_FX.FX_BLOOD), spawnPoint, hit.transform.rotation, hit.transform.parent, component.EffectScaleFactor);
                }
            }


        }

        if (this.shotType == ShotType.AOE)
        {
            OnDealAoeDmg(hit.transform.position, 1.0f, hit.transform.gameObject.GetInstanceID());
        }
    }

    public void CastDmg(Transform hit, float _dmg, ShotType _shotType = ShotType.NORMAL, Vector3 offsetFloatingText = default(Vector3))
    {
        IHealth component = hit.GetComponent<IHealth>();
        if (component != null)
        {
            var spawnPoint = component.HitMarker != null ? component.HitMarker.position : hit.transform.position;

            spawnPoint -= transform.forward * 0.3f;

            component.SetDamage(_dmg, _shotType, this.OwnerID, this.effectHits, null, 0, offsetFloatingText, _ignoreEffectHitsIndex);

            if (!component.IsDead() && !GameMaster.IsSpeedUp && GameMaster.instance.OptmizationController.Data.EnableImpactHit)
                GameMaster.instance.PlayEffect(_parImpact, spawnPoint, hit.transform.rotation, hit.transform.parent);

            if (!HideFXBlood && GameMaster.instance.OptmizationController.Data.EnableBlood)
            {
                if (hit.transform.CompareTag(TagConstant.TAG_BOSS))
                {
                    GameMaster.instance.PlayEffect(ResourceManager.instance._effectResources.GetEffect(COMMON_FX.FX_BLOOD), spawnPoint, hit.transform.rotation, hit.transform.parent, component.EffectScaleFactor);
                }
            }


        }

        if (this.shotType == ShotType.AOE)
        {
            OnDealAoeDmg(hit.transform.position, 1.0f, hit.transform.gameObject.GetInstanceID());
        }
    }


    public virtual void DestroyBullet()
    {
        base.Done();
        IsLaunch = false;
        if (_bulletTrail)
        {
            _bulletTrail.enabled = false;
            _bulletTrail.emitting = false;
        }

        OnHit?.Invoke();
        OnHit = null;
    }

    protected float bulletSpeed;
    protected float bulletDamage;
    protected ShotType shotType;
    protected float MultiplyPerHit = 1.0f;

    protected float DmgDecreasePerHit = 0f;

    public void SetMultiplyPerHit(float newValue)
    {
        MultiplyPerHit = newValue;
    }

    protected List<EffectHit> effectHits;
    protected string OwnerID;

    protected float lifeTime = 4f;

    public virtual void OnDealAoeDmg(Vector3 startPos, float radius, int firstHitZombieID)
    {
        Collider[] cols = Physics.OverlapSphere(startPos, radius, _maskHit);
        if (cols != null && cols.Length > 0)
        {
            for (int i = cols.Length - 1; i >= 0; i--)
            {
                var col = cols[i];
                IHealth component = col.transform.GetComponent<IHealth>();
                if (component != null && col.gameObject.GetInstanceID() != firstHitZombieID)
                {
                    component.SetDamage(this.bulletDamage, ShotType.FIRE, this.OwnerID);
                }

            }

            var fxBoom = Pooly.Spawn<AutoDespawnParticles>(POOLY_PREF.FX_EXPLODE, startPos, Quaternion.identity, null);
            fxBoom.PlayEffect();
        }

    }

    public void SetIgnoreEffectHits(List<int> _listChosenEffectHit)
    {
        _ignoreEffectHitsIndex = _listChosenEffectHit;
    }
}
