using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class LaserKillerGate : CountdownEffect
{
    private void Awake()
    {
        this.defPos = base.transform.position;
        this.boxCollider = base.GetComponent<BoxCollider>();
        this.visual.SetActive(false);
        this.boxCollider.enabled = false;
    }

    public override void ApplyEffect()
    {
        base.transform.position = this.defPos;
        this.boxCollider.enabled = true;
        this.visual.SetActive(true);
        UnityEngine.Debug.Log("Apply Effect KILLLL " + this.effectTime.ToString());
        base.transform.DOMoveX(this.finsihPoint.position.x, this.effectTime, false).OnComplete(delegate
        {
            this.boxCollider.enabled = false;
            this.visual.SetActive(false);
        });
    }

    public override void ResetEffect()
    {
        UnityEngine.Debug.Log("RESET Effect KILLLL " + this.effectTime.ToString());
        this.boxCollider.enabled = false;
        this.visual.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        IHealth component = collision.GetComponent<IHealth>();
        if (component != null)
        {
            component.SetDamage(999f, ShotType.NORMAL, null);
        }
    }

    private Vector3 defPos;

    [SerializeField]
    private Transform finsihPoint;

    [SerializeField]
    private GameObject visual;

    private BoxCollider boxCollider;
}
