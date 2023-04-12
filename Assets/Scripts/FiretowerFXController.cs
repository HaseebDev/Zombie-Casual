using System;
using UnityEngine;

public class FiretowerFXController : MonoBehaviour
{
    private void Start()
    {
        TowerBehavior towerBehavior = this.towerBehavior;
        towerBehavior.OnTargetFound = (Action<Transform>)Delegate.Combine(towerBehavior.OnTargetFound, new Action<Transform>(this.TowerBehavior_OnTargetFound));
        TowerBehavior towerBehavior2 = this.towerBehavior;
        towerBehavior2.OnTargetSearch = (Action)Delegate.Combine(towerBehavior2.OnTargetSearch, new Action(this.TowerBehavior_OnTargetSearch));
        this.StopShootParticle();
    }

    private void TowerBehavior_OnTargetSearch()
    {
        this.StopShootParticle();
    }

    private void TowerBehavior_OnTargetFound(Transform target)
    {
        this.PlayShootParticle();
    }

    private void PlayShootParticle()
    {
        for (int i = 0; i < this.particleSystems.Length; i++)
        {
            this.particleSystems[i].Play();
        }
    }

    private void StopShootParticle()
    {
        for (int i = 0; i < this.particleSystems.Length; i++)
        {
            this.particleSystems[i].Stop();
        }
    }

    private void OnDestroy()
    {
        TowerBehavior towerBehavior = this.towerBehavior;
        towerBehavior.OnTargetFound = (Action<Transform>)Delegate.Remove(towerBehavior.OnTargetFound, new Action<Transform>(this.TowerBehavior_OnTargetFound));
        TowerBehavior towerBehavior2 = this.towerBehavior;
        towerBehavior2.OnTargetSearch = (Action)Delegate.Remove(towerBehavior2.OnTargetSearch, new Action(this.TowerBehavior_OnTargetSearch));
    }

    [SerializeField]
    private ParticleSystem[] particleSystems;

    [SerializeField]
    private TowerBehavior towerBehavior;
}
