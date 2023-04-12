using System;
using UnityEngine;

public class BloodFloorSplinter : MonoBehaviour
{
    private void Awake()
    {
        //if (this.health == null)
        //    this.health = GetComponentInParent<Health>();
    }

    private void Start()
    {
        //Health health = this.health;
        //health.OnHit = (Action<float>)Delegate.Combine(health.OnHit, new Action<float>(this.Health_OnHit));
        //Health health2 = this.health;
        //health2.OnDie = (Action<bool>)Delegate.Combine(health2.OnDie, new Action<bool>(this.Health_OnDie));
    }

    private void Health_OnDie(bool skipAnimDead)
    {
        //this.SpawnBloodWithChanceOnRandomRadius(0.8f);
    }

    private void SpawnBloodWithChanceOnRandomRadius(float chance)
    {
        //if (UnityEngine.Random.value <= chance)
        //{
        //	int num = UnityEngine.Random.Range(1, 3);
        //	for (int i = 0; i < num; i++)
        //	{
        //		float radius = UnityEngine.Random.RandomRange(-0.5f, 0.5f);
        //		this.bloodFloorDispancerManager.CreateBloodEffectOnFloor(base.transform.position.x, base.transform.position.z, radius);
        //	}
        //}
    }

    private void Health_OnHit(float dmg)
    {
        //this.SpawnBloodWithChanceOnRandomRadius(0.5f);
    }

    private void OnDestroy()
    {
        //Health health = this.health;
        //health.OnHit = (Action<float>)Delegate.Remove(health.OnHit, new Action<float>(this.Health_OnHit));
        //Health health2 = this.health;
        //health2.OnDie = (Action<bool>)Delegate.Remove(health2.OnDie, new Action<bool>(this.Health_OnDie));
    }

    [SerializeField]
    private Health health;

    [SerializeField]
    private BloodFloorDispancerManager bloodFloorDispancerManager;
}
