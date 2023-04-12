using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class BaseMapObstacle : MonoBehaviour
{
    protected Health _health;
    public BoxCollider _collider;
    public MeshRenderer _meshRender;

    [Header("Colour When Hit")]
    protected Color _colorNormal;
    public Color _colorHit;

    protected virtual void Awake()
    {
        _health = this.GetComponent<Health>();
        _collider = this.GetComponent<BoxCollider>();
        _colorNormal = _meshRender.material.color;
    }

    public virtual void Initialize(float MaxHP)
    {
        _health.Initialize(MaxHP, false);
        _health.OnHit += OnHitDmg;
        _health.OnDie += OnDie;

        _meshRender.material.color = _colorNormal;
    }

    public virtual void OnHitDmg(float dmg)
    {
        FlashColourWhenHit();
    }

    public void FlashColourWhenHit()
    {
        StartCoroutine(FlashColourWhenHitCoroutine());
    }

    IEnumerator FlashColourWhenHitCoroutine()
    {
        _meshRender.material.DOColor(_colorHit, 0.135f);
        yield return new WaitForSeconds(0.3f);
        _meshRender.material.DOColor(_colorNormal, 0.135f);
    }

    public virtual void OnDie(bool skipAnimDead)
    {
        _health.OnHit -= OnHitDmg;
        _health.OnDie -= OnDie;

    }

    public void PushbackAndBlock(Collider other)
    {
        if (GameMaster.IsZombieTag(other.tag))
        {
            var zom = other.gameObject.GetComponentInParent<Zombie>();
            if (zom != null)
            {
                var zomCollider = other.GetComponent<BoxCollider>();
                if (this._collider.IsInside(zom.transform.position))
                {
                    var newPos = zom.transform.position;

                    newPos.x -= (_collider.size.x + zomCollider.size.x / 2.0f);
                    zom.transform.position = newPos;
                }

                zom.goToWallBeh.OnSwitchTarget(this._health);
                zom.goToWallBeh.OnWallReached?.Invoke(true);

            }
        }
    }
}
