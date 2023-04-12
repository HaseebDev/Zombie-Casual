using DG.Tweening;
using Ez.Pooly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseZombieObject : MonoBehaviour
{
    public Health _health;
    public MeshRenderer _meshRender;
    protected Material _mat;

    [Header("Effects")]
    public List<ParticleSystem> _effectsDeploy;

    protected bool IsDeactive;

    public void Initialize(float MaxHP)
    {
        _health.Initialize(MaxHP, false);
        _health.OnHit += OnHitDmg;
        _health.OnDie += OnDie;

        _mat = gameObject.GetComponentInChildren<MeshRenderer>().material;
        _mat.color = Color.white;

        for (int i = _effectsDeploy.Count - 1; i >= 0; i--)
        {
            _effectsDeploy[i].gameObject.SetActiveIfNot(false);
        }

        IsDeactive = false;
    }

    private void OnHitDmg(float dmg)
    {
        FlashColourWhenHit();
    }

    public void FlashColourWhenHit()
    {
        StartCoroutine(FlashColourWhenHitCoroutine());
    }

    IEnumerator FlashColourWhenHitCoroutine()
    {
        _meshRender.material.DOColor(Color.red, 0.135f);
        yield return new WaitForSeconds(0.3f);
        _meshRender.material.DOColor(Color.white, 0.135f);
    }

    public virtual void OnDie(bool skipAnimDead)
    {
        _health.OnHit -= OnHitDmg;
        _health.OnDie -= OnDie;
        Pooly.Despawn(this.transform);
    }

    public void CleanUp()
    {
        for (int i = _effectsDeploy.Count - 1; i >= 0; i--)
        {
            _effectsDeploy[i].gameObject.SetActiveIfNot(false);
        }
        _health.OnHit -= OnHitDmg;
        _health.OnDie -= OnDie;
        Pooly.Despawn(this.transform);

        IsDeactive = true;
    }

}
