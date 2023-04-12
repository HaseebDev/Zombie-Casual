using System;
using Framework.Interfaces;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class ManualDamage : MonoBehaviour, IRayCastListiner
{
    private void Start()
    {
        this.tapDamageUData = UnityEngine.Object.FindObjectOfType<TapDamageUData>();
        this.health = base.GetComponent<Health>();
    }

    public float ManualDmg {
        get {
            return this.tapDamageUData.GetUpgradableDmg;
        }
    }

    private void OnMouseDown()
    {
        this.GiveRewardForTap();
    }

    private void GiveRewardForTap()
    {
        //this.health.SetDamage(this.ManualDmg);
    }

    public void RayCastClick()
    {
        if (Vector3.Distance(this.currentStepPos, UnityEngine.Input.mousePosition) > this.tapDetectDistance)
        {
            this.currentStepPos = UnityEngine.Input.mousePosition;
            this.GiveRewardForTap();
        }
    }

    private Vector3 currentStepPos;

    [SerializeField]
    private float tapDetectDistance;

    private IHealth health;

    private TapDamageUData tapDamageUData;
}
