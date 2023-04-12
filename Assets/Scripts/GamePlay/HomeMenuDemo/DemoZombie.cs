using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Ez.Pooly;

public class DemoZombieData
{
    public float HP;
    public float moveSpeed;
}

public class DemoZombie : MonoBehaviour
{
    public Animator zmAnimator;
    public Health health;

    private float moveSpeed = 0.35f;
    private Motor motor;
    private bool isInited = false;
    private Transform _wallTrf;
    private DemoZombieData Data;

    private bool isReachedWall = false;
    private bool isDie = false;

    private Vector3 targetPosition;

    public Action<DemoZombie> ActionZombieDie;

    private void Awake()
    {
        motor = this.GetComponent<Motor>();
    }

    public void Initialize(DemoZombieData _data, Vector3 targetPos)
    {
        this.Data = _data;
        motor.SetSpeed(Data.moveSpeed);
        _wallTrf = GameObject.FindGameObjectWithTag("wall").transform;
        zmAnimator.transform.localRotation = Quaternion.identity;

        health.Initialize(Data.HP);
        health.Revive();
        health.OnDie = OnZombieDie;
        health._OnTriggerEnter = OnZombieTriggerEnter;
        health.IsTargetable = false;
        isReachedWall = false;
        isDie = false;

        targetPosition = targetPos;
        isInited = true;
    }

    private void OnZombieDie(bool skipAnimDead)
    {
        health.SetDie();
        ActionZombieDie?.Invoke(this);
        DestroyZombie();
    }

    private void Behavior_OnWallReached()
    {
        isReachedWall = true;
        this.zmAnimator.SetBool("attack", true);
    }

    public void UpdateZombie(float _deltaTime)
    {
        if (!isInited || isDie)
            return;
        transform.LookAt(_wallTrf);
        if (!isReachedWall)
            transform.position += transform.forward * moveSpeed * _deltaTime;

        if (!isReachedWall && Vector3.Distance(transform.position, _wallTrf.position) <= 0.05f)
        {
            Behavior_OnWallReached();
        }
        //transform.LookAt(targetPosition);
        //transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * _deltaTime);
    }

    public void DestroyZombie()
    {
        isInited = false;
        isDie = true;
        Pooly.Despawn(transform);
    }

    private void OnZombieTriggerEnter(Collider other)
    {
        if (other.tag.Equals(TagConstant.TAG_ATTACKABLE_TRIGGEER))
        {
            health.IsTargetable = true;
        }
    }
}
