using System;
using System.Collections.Generic;
using UnityEngine;
using Ez.Pooly;
using MEC;

public class BoomBullet : PoolObject, IBullet
{
    public LayerMask TargetLayerMask;

    public ParticleSystem parTrail;

    [Header("SFX")]
    [Space(10)]
    public SFX_ENUM SoundExplode = SFX_ENUM.SFX_BOMB_EXPLODE_01;

    private bool isHit = false;
    private Transform targetTrf = null;

    private Collider[] _hitColliders;

    private float _timerDestroy = 0f;
    private bool _isLaunch = false;

    private List<int> _ignoreEffectHitsIndex;

    protected float rangeBullet;
    public override void OnAwake()
    {
        this.boomIsOn = false;
        base.Done(this.lifeTime);
    }

    private void Start()
    {
        this.visual = base.transform.GetChild(0);
        this.boomCollider = base.GetComponent<SphereCollider>();
        _hitColliders = new Collider[10];
    }

    public void ResetBullet()
    {

    }

    public virtual void Initialize(List<EffectHit> listEffects, string _OwnerID, LayerMask maskHit, float rangeBullet = float.PositiveInfinity)
    {
        this.effectHits = listEffects;
        this.OwnerID = _OwnerID;
        isHit = false;
        targetTrf = null;
        this.TargetLayerMask = maskHit;
        this.rangeBullet = rangeBullet;
        this.boomIsOn = false;
        _isLaunch = false;
    }

    public virtual void Launch(float _force, float _damage, ShotType _type, Action OnHit = null, Vector3 _offsetCastDmg = default(Vector3))
    {
        this.MultiplyPerHit = 1.0f;
        this.bulletSpeed = _force;
        this.bulletDamage = _damage;
        this.shotType = _type;
        _isLaunch = true;

        if (this.parTrail != null)
        {
            parTrail.time = 0;
            parTrail.Play(true);
        }

        if (this.visual != null)
        {
            this.visual.gameObject.SetActive(true);
        }

        this.boomIsOn = false;
        _timerDestroy = 0f;
    }

    private Vector3 FindMainTargetPosition()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(new Ray(base.transform.position, base.transform.forward), out raycastHit, 1000f, this.TargetLayerMask))
        {
            return raycastHit.transform.position;
        }
        return Vector3.zero;
    }

    public virtual void Update()
    {
        if (!this.boomIsOn && _isLaunch)
        {
            base.transform.position += base.transform.forward * Time.deltaTime * this.bulletSpeed;

            _timerDestroy += Time.deltaTime;
            if (_timerDestroy >= 2.0f)
            {
                DestroyBullet();
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!_isLaunch || !GameMaster.IsZombieTag(collision.tag) || isHit)
        {
            return;
        }

        isHit = true;
        IHealth component = collision.transform.GetComponent<IHealth>();
        targetTrf = collision.transform;
        if (component != null)
        {
            this.boomIsOn = true;
            this.boomCollider.radius = this.attackRadius;

            var hitPoint = collision.transform.position;

            component.SetDamage(this.bulletDamage, this.shotType, this.OwnerID, this.effectHits, null, 0, Vector3.zero, _ignoreEffectHitsIndex);
            OnDealAoeDmg(hitPoint, 1.0f, collision.gameObject.GetInstanceID());
            this.visual.gameObject.SetActive(false);

            DestroyBullet();
        }

        if (this.shotType != ShotType.PIERCE)
        {
            if (this.boomIsOn)
            {
                DestroyBullet();
            }
        }

    }

    public void OnDealAoeDmg(Vector3 startPos, float radius, int firstHitZombieID)
    {
        int numCollide = Physics.OverlapSphereNonAlloc(startPos, radius, _hitColliders, ResourceManager.instance._maskZombieOnly);
        if (numCollide > 0)
        {
            int num = numCollide >= _hitColliders.Length ? _hitColliders.Length : numCollide;
            for (int i = num - 1; i >= 0; i--)
            {
                var col = _hitColliders[i];
                IHealth component = col.transform.GetComponent<IHealth>();
                if (component != null && col.gameObject.GetInstanceID() != firstHitZombieID)
                {
                    component.SetDamage(this.bulletDamage, ShotType.FIRE, this.OwnerID);
                }
            }

        }

    }

    public void DestroyBullet()
    {
        this.boomIsOn = false;
        _isLaunch = false;
        AutoDespawnParticles autoDestroy = boomEffect.gameObject.AddComponentIfNot<AutoDespawnParticles>();

        var posY = targetTrf != null ? targetTrf.position.y : transform.position.y;
        var targetPos = transform.position;
        targetPos.y = posY;
        GameMaster.instance.PlayEffect(autoDestroy, targetPos, Quaternion.identity, null);
        AudioSystem.instance.PlaySFX(this.SoundExplode);
        //Timing.CallDelayed(1.0f, () =>
        //{

        //});
        Pooly.Despawn(transform);
    }

    public void SpawnBoomFX(Vector3 position, Transform parent)
    {
        var boomFX = Pooly.Spawn(boomEffect.transform, position, Quaternion.identity, null);
        AutoDespawnParticles autoDestroy = boomFX.gameObject.AddComponentIfNot<AutoDespawnParticles>();
        autoDestroy.transform.SetParent(parent);
        autoDestroy.transform.localScale = Vector3.one;
        autoDestroy.type = DESPAWN_TYPE.WHEN_FINISH;
        autoDestroy.PlayEffect();

    }

    public void SetMultiplyPerHit(float _multiplyPerHit)
    {
        MultiplyPerHit = _multiplyPerHit;
    }

    public void SetIgnoreEffectHits(List<int> _listIgnoreEffects)
    {
        this._ignoreEffectHitsIndex = _listIgnoreEffects;
    }

    protected float bulletSpeed;

    private float bulletDamage;
    private ShotType shotType;
    private List<EffectHit> effectHits;
    private string OwnerID;

    private float MultiplyPerHit = 1.0f;

    //private int collisionLayerMask = 512;

    [SerializeField]
    private float attackRadius;

    private float lifeTime = 8f;

    private SphereCollider boomCollider;

    private bool boomIsOn;

    [SerializeField]
    private ParticleSystem boomEffect;

    private Vector3 mainTargetPos;

    private Transform visual;

    protected float _overwriteForce = -1;
    public float OverwriteForce { get => _overwriteForce; set => _overwriteForce = value; }
}
