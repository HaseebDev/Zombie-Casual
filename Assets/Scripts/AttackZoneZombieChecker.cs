using System;
using Adic;
using Framework.Managers.Event;
using UnityEngine;

public class AttackZoneZombieChecker : MonoBehaviour
{
    private void Start()
    {
        this.Inject();
    }

    public static AttackZoneZombieChecker instance;

    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameMaster.IsZombieTag(other.tag))
        {
            this.zombieCounter++;
            this.StartAttackStatus();
        }
    }

    private void Update()
    {
        if (this.attackStatus)
        {
            this.sec -= Time.deltaTime;
            if (this.sec <= 0f)
            {
                this.zombieCounter = 0;
                this.zoneCollider.enabled = false;
                this.zoneCollider.enabled = true;
            }
        }
        if (this.zombieCounter == 0 && this.attackStatus)
        {
            this.StopAttackStatus();
        }
    }

    private void StartAttackStatus()
    {
        if (!this.attackStatus)
        {
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.ZOMBIE_IN_ATTACK_ZONE);
        }
        this.sec = 1f;
        this.attackStatus = true;
    }

    private void StopAttackStatus()
    {
        if (this.attackStatus)
        {
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.ZOMBIE_LEAVE_ATTACK_ZONE);
        }
        this.sec = 1f;
        this.attackStatus = false;
    }

    public bool AttackBasement(float dmg)
    {
        return GamePlayController.instance.campaignData.AttackBasement(dmg);
    }

    [SerializeField]
    private int zombieCounter;

    [SerializeField]
    private BoxCollider zoneCollider;

    private float sec;

    [SerializeField]
    private bool attackStatus;
}
