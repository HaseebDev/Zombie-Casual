using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ez.Pooly;
using System;
using MEC;
using static BaseBomb;

public interface IBaseBomb
{
    float DmgRadius { get; set; }
    float Dmg { get; set; }
    AutoDespawnParticles parExplode { get; set; }
    LayerMask TargetMask { get; }

    void PreInit(float dmg, float radius, string OwnerID, MOVE_TYPE type = MOVE_TYPE.THROW);
    void Launch(Vector3 targetPos, Action OnComplete);
    void ActiveBomb();
    void UpdateBomb(float _deltaTime);
    void CleanUp();
}

public class BaseBomb : MonoBehaviour, IBaseBomb
{
    public enum MOVE_TYPE
    {
        NONE,
        THROW,
        FLY
    }

    //variables
    public AutoDespawnParticles _parExplode;

    protected float _dmgRadius;
    protected float _dmg;
    public LayerMask _targetMask;
    protected MOVE_TYPE _moveType;

    //get/set
    public float Dmg { get => _dmg; set => _dmg = value; }
    public float DmgRadius { get => _dmgRadius; set => _dmgRadius = value; }
    public AutoDespawnParticles parExplode { get => _parExplode; set => _parExplode = value; }
    public LayerMask TargetMask => _targetMask;

    private string OwnerID;
    private Collider[] _boomCollides;

    [Header("SFX")]
    [Space(10)]
    public SFX_ENUM SoundExplode = SFX_ENUM.SFX_BOMB_EXPLODE_01;

    private void Start()
    {
        _boomCollides = new Collider[50];
    }


    public virtual void PreInit(float dmg, float radius, string ownerID, MOVE_TYPE type = MOVE_TYPE.THROW)
    {
        _dmgRadius = radius;
        _dmg = dmg;
        OwnerID = ownerID;
        _moveType = type;
    }

    public virtual void Launch(Vector3 targetPos, Action OnComplete)
    {

    }

    public virtual void ActiveBomb()
    {
        int numCol = Physics.OverlapSphereNonAlloc(transform.position, _dmgRadius * 2.0f, _boomCollides, TargetMask);
        if (numCol > 0)
        {
            Timing.RunCoroutine(ActiveBombCoroutine(numCol));
        }

        GameMaster.instance.PlayEffect(_parExplode, transform.position, Quaternion.identity, null);
        AudioSystem.instance.PlaySFX(SoundExplode);
        CleanUp();

    }

    IEnumerator<float> ActiveBombCoroutine(int numCol)
    {
        if (numCol > 0)
        {
            for (int i = 0; i < numCol; i++)
            {
                var col = _boomCollides[i];
                if (col == null)
                    continue;

                IHealth component = col.transform.GetComponent<IHealth>();
                if (component != null)
                {
                    component.SkipAnimDead(1.0f);
                    component.SetDamage(this.Dmg, ShotType.NORMAL_SKIP_FX, this.OwnerID);
                }
                if (i % 3 == 0)
                    yield return Timing.WaitForOneFrame;
            }
        }
    }

    public virtual void CleanUp()
    {
        Pooly.Despawn(transform);
    }

    public virtual void UpdateBomb(float _deltaTime)
    {

    }
}
