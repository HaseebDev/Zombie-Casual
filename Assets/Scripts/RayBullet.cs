using System;
using UnityEngine;
using System;
using System.Collections.Generic;
using Ez.Pooly;
using DG.Tweening;
using Ez.Pooly;
using UnityEngine.UI;
using System.Linq;

public class RayBullet : MonoBehaviour, IBullet
{
    public Transform _parLocator;
    public LayerMask _targetLayerMask;

    public TrailRenderer _bulletTrail;
    public AutoDespawnParticles _parMuzzle;
    public AutoDespawnParticles _parImpact;
    public static float TIME_PLAY_EFECT = 0.1f;

    private float timerEffect = 0f;
    private bool isBusy = false;
    private AutoDespawnParticles parMuzzleClone;

    private RaycastHit[] raycastHits;
    private int hitCount;

    private Vector3 lastRot;

    protected float _overwriteForce = -1;
    public float OverwriteForce { get => _overwriteForce; set => _overwriteForce = value; }

    private void Start()
    {
        this.currentRayShowTime = this.rayShowTime;
    }

    private void Update()
    {
        timerEffect += Time.deltaTime;
        if (timerEffect >= TIME_PLAY_EFECT)
        {
            isBusy = false;
        }
    }

    public void ResetBullet()
    {
        _bulletTrail.DOKill();
        _bulletTrail.Clear();
        if (parMuzzleClone != null)
        {
            parMuzzleClone.StopEffect();
        }

    }

    public void Initialize(List<EffectHit> listEffects, string _ownerID, LayerMask maskHit, float rangeBullet = float.PositiveInfinity)
    {
        this.effectHits = listEffects;
        isBusy = false;
        timerEffect = 0f;
        this.OwnerID = _ownerID;
        this.raycastHits = new RaycastHit[20];
        this.hitCount = 0;
    }

    public void Launch(float _force, float _damage, ShotType _type, Action OnHit = null, Vector3 _offsetCastDmg = default(Vector3))
    {
        this.bulletDamage = _damage;
        this.shotType = _type;
        this.ActivateRay();
    }

    private void ActivateRay(Vector3 customDirect = default(Vector3))
    {
        timerEffect = 0f;
        this.rayIsOn = true;

        var currentRot = transform.rotation.eulerAngles;
        if (Math.Abs(currentRot.y - lastRot.y) >= 3f)
        {
            //force reset
            isBusy = false;
        }
        lastRot = currentRot;
        parMuzzleClone = GameMaster.instance.PlayEffect(_parMuzzle, _parLocator.position, transform.rotation);
        Ray ray = new Ray(_parLocator.position, base.transform.forward);
        this.currentRayShowTime = this.rayShowTime;
        var finalPos = _parLocator.position + transform.forward * (10f);
        if (!isBusy)
        {
            isBusy = true;
            _bulletTrail.DOKill();
            _bulletTrail.Clear();
            _bulletTrail.transform.position = _parLocator.position + transform.forward;
            _bulletTrail.transform.DOMove(finalPos, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _bulletTrail.transform.position = _parLocator.position + transform.forward;
                _bulletTrail.Clear();
                isBusy = false;
            });
        }

        if (this.shotType == ShotType.PIERCE)
        {
            hitCount = Physics.SphereCastNonAlloc(ray, 0.1f, raycastHits, float.PositiveInfinity, _targetLayerMask);
            if (hitCount > 0)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    var hit = raycastHits[i];
                    CastDmg(hit);
                }
            }
        }
        else
        {
            RaycastHit hit;
            var isHit = Physics.Raycast(ray, out hit, float.PositiveInfinity, _targetLayerMask);
            if (isHit)
            {
                CastDmg(hit);
            }
        }



    }

    private void CastDmg(RaycastHit hit)
    {
        IHealth component = hit.transform.GetComponent<IHealth>();
        if (component != null)
        {
            component.SetDamage(this.bulletDamage, this.shotType, this.OwnerID, this.effectHits);
            if (!component.IsDead() && !GameMaster.IsSpeedUp && GameMaster.instance.OptmizationController.Data.EnableImpactHit)
                GameMaster.instance.PlayEffect(_parImpact, hit.transform.position, hit.transform.rotation);
        }

        if (this.shotType == ShotType.AOE)
        {
            OnDealAoeDmg(hit.transform.position, 1.0f, hit.transform.gameObject.GetInstanceID());
        }
    }

    public void OnDealAoeDmg(Vector3 startPos, float radius, int firstHitZombieID)
    {
        Collider[] cols = Physics.OverlapSphere(startPos, radius, ResourceManager.instance._maskZombieOnly);
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

    public void SetMultiplyPerHit(float _multiplyPerHit)
    {


    }

    public void SetIgnoreEffectHits(List<int> _listChosenEffectHit)
    {
        throw new NotImplementedException();
    }

    private bool rayIsOn;
    private float bulletDamage;
    private ShotType shotType;
    private List<EffectHit> effectHits;
    private string OwnerID;
    private float rayShowTime = 0.1f;
    private float currentRayShowTime;
}
