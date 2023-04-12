using System;
using UnityEngine;

[RequireComponent(typeof(Motor))]
public class RunBehavior : AIBehavior
{
    public override void ApplyBehavior()
    {
        this.runIsTrue = false;
    }

    public void StartRun(Animator _animator, float _runtime)
    {
        if (this.runIsTrue)
        {
            return;
        }
        this.runIsTrue = true;
        this.runTime = _runtime;
        this.animator = _animator;
        this.animator.SetBool("running", true);
        this.defSpeed = this.motor.GetDefSpeed();
        this.motor.SetSpeed(this.defSpeed * 4f);
    }

    public void UpdateBehaviour(float _deltaTime)
    {
        if (this.runIsTrue)
        {
            this.runTime -= _deltaTime;
            if (this.runTime <= 0f)
            {
                this.StopBehavior();
            }
        }
    }

    public override void StopBehavior()
    {
        this.runIsTrue = false;
        this.OnRunEnd();
        if (this.motor == null || this.animator == null)
        {
            return;
        }
        this.motor.SetSpeed(this.defSpeed);
        this.animator.SetBool("running", false);
    }

    [SerializeField]
    private Motor motor;

    [SerializeField]
    private float runTime;

    private float defSpeed;

    [SerializeField]
    private bool runIsTrue;

    private Animator animator;

    public Action OnRunEnd = delegate ()
    {
    };
}
