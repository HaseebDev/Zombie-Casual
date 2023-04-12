using System;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    private void Start()
    {
        this.sphereCollider = base.GetComponent<SphereCollider>();
        this.sphereCollider.enabled = false;
    }

    public void Activate()
    {
        this.mineIsActive = true;
        this.sphereCollider.enabled = true;
        float time = UnityEngine.Random.Range(0.5f, 2f);
        base.Invoke("Boom", time);
    }

    private void Boom()
    {
        this.explodeParticle.Simulate(1f);
        this.explodeParticle.Play();
        for (int i = 0; i < this.targetList.Count; i++)
        {
            this.targetList[i].SetDamage(this.boomDmg, ShotType.NORMAL, null);
        }
        this.targetList.Clear();
        this.mineIsActive = false;
        this.sphereCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!this.mineIsActive)
        {
            return;
        }
        IHealth component = collision.GetComponent<IHealth>();
        if (component != null)
        {
            this.targetList.Add(component);
        }
    }

    private float boomDmg = 100f;

    private bool mineIsActive;

    [SerializeField]
    private ParticleSystem explodeParticle;

    private List<IHealth> targetList = new List<IHealth>();

    private SphereCollider sphereCollider;
}
