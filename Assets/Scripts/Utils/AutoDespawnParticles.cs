using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ez.Pooly;

public enum DESPAWN_TYPE
{
    WHEN_FINISH = 0,
    TIMER
}

[RequireComponent(typeof(ParticleSystem))]
public class AutoDespawnParticles : MonoBehaviour
{
    public DESPAWN_TYPE type = DESPAWN_TYPE.WHEN_FINISH;
    public float DelayDespawn = 3f;
    public float ThresHoldDuration = 0f;

    private float timerDespawn = 0f;

    public ParticleSystem ps { get; private set; }


    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        timerDespawn = 0f;
    }

    private void OnEnable()
    {
        timerDespawn = 0f;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        if (ps)
        {
            if (type == DESPAWN_TYPE.TIMER && DelayDespawn > 0)
            {
                timerDespawn += deltaTime;
                if (timerDespawn >= DelayDespawn)
                {
                    timerDespawn = 0f;
                    Pooly.Despawn(transform);
                }
            }
            else if (type == DESPAWN_TYPE.WHEN_FINISH)
            {
                if (!ps.IsAlive())
                {
                    Pooly.Despawn(transform);
                }
            }

        }
    }

    public bool IsPlayingEffect()
    {
        return ps.isPlaying;
    }

    public void PlayEffect()
    {
        timerDespawn = 0f;
        ps.gameObject.SetActiveIfNot(true);
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play(true);
    }

    public void StopEffect()
    {
        ps.time = 0;
        ps.Stop(true);
        timerDespawn = 0;
    }

    public void ResetPar()
    {
        timerDespawn = 0f;
    }

    public void CleanUp()
    {
        ps.time = 0;
        ps.Stop(true);
        Pooly.Despawn(transform);
    }

}
