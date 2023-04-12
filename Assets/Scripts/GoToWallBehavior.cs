using System;
using UnityEngine;

[RequireComponent(typeof(Motor))]
public class GoToWallBehavior : AIBehavior
{
    public float ROTATION_ANGLE = 30;

    public bool enableMoveLeftRight = true;
    public float DurationMoveLR_Min = 3f;
    public float DurationMoveLR_Max = 5f;

    private float DurationMoveLR = 0f;
    public Transform _rotTransform;

    public float LimitTopX = 0f;

    private bool IsPause = false;

    private Health _targetPlayer = null;
    private float AttackRange = 0f;

    public void Initialize(float minTurning, float maxTurning, bool enableTurning, float attackRange, float limitTopX, float rotateAngle)
    {
        this.motor = base.GetComponent<Motor>();
        DurationMoveLR_Max = minTurning;
        DurationMoveLR_Min = minTurning;
        this.LimitTopX = limitTopX;
        enableMoveLeftRight = enableTurning;
        AttackRange = attackRange;
        IsPause = false;
        IsApplied = false;
        reachedWall = false;

        ROTATION_ANGLE = rotateAngle;
    }

    private float _timerMoveLR = 0f;
    private float _currentLRDirect = 0f;

    private bool IsApplied = false;
    private bool IsReachAttackRange = false;
    private bool reachedWall = false;

    public override void ApplyBehavior()
    {
        IsReachAttackRange = false;
        base.Invoke("FindTargetPlayer", 1f);

        IsApplied = false;

        if (enableMoveLeftRight)
        {
            _currentLRDirect = (UnityEngine.Random.Range(0, 100) % 2 == 0) ? 1 : -1;
            DurationMoveLR = UnityEngine.Random.Range(DurationMoveLR_Min, DurationMoveLR_Max);
        }
        else
        {
            _rotTransform.rotation = Quaternion.Euler(0, 90, 0);
        }

        IsApplied = true;


    }

    public void FindWall()
    {
        var walls = GameObject.FindGameObjectsWithTag("wall");
        foreach (var wall in walls)
        {
            if (wall.name == "wall")
                wallTransform = wall.transform;
            else if (wall.name == "wallL")
            {
                wallLTranform = wall.transform;
            }
            else if (wall.name == "wallR")
            {
                wallRTranform = wall.transform;
            }
        }
    }

    public void FindTargetPlayer()
    {
        FindWall();
        if (GamePlayController.instance != null)
            _targetPlayer = GamePlayController.instance.gameLevel.FindRandPlayer();
        else
        {
            if (wallTransform != null)
                _targetPlayer = wallTransform.GetComponent<Health>();
        }


        if (_targetPlayer)
        {
            this.motor.isMove = true;
            this.motor.enabled = true;
            this.reachedWall = false;

            OnFoundTarget?.Invoke(_targetPlayer);
        }

    }

    private bool IsReachedWall()
    {
        if (_targetPlayer == null || this == null)
            return false;

        return (base.transform.position.x > this.wallTransform.position.x ||
            transform.position.x >= _targetPlayer.transform.position.x);
    }

    public void UpdateBehaviour(float _deltaTime)
    {
        if (this.motor.IsPushingBack)
        {
            if (this.transform.position.x > LimitTopX)
                this.motor.MoveUp(_deltaTime);
            else
                this.motor.IsPushingBack = false;


            //if wall reached then reset to walk again!!!!
            if (reachedWall)
            {
                reachedWall = false;
                this.motor.isMove = true;
                this.OnWallReached?.Invoke(false);
            }
            return;
        }

        if (IsPause || !IsApplied || reachedWall)
            return;

        if (this.motor != null)
        {
            if (!this.motor.IsPushingBack)
            {
                this.motor.MoveDown(_deltaTime);
                OnMoveLR(_deltaTime);
            }
            //else
            //{
            //    if (this.transform.position.x > LimitTopX)
            //        this.motor.MoveUp(_deltaTime);
            //    else
            //        this.motor.IsPushingBack = false;
            //}

        }
        if ((CheckAttackRange() || this.wallTransform != null && IsReachedWall()) && !reachedWall)
        {
            reachedWall = true;
            this.motor.isMove = false;
            this.OnWallReached?.Invoke(true);
            this.wallTransform = null;
        }

    }

    public bool CheckAttackRange()
    {
        if (this == null)
            return false;
        var reachedRange = false;
        reachedRange = this._targetPlayer != null && /*Mathf.Abs*/(_targetPlayer.transform.position.x - base.transform.position.x) <= this.AttackRange;
        return reachedRange;
    }

    public override void StopBehavior()
    {
        this.wallTransform = null;
        this.IsApplied = false;
    }

    public void SetPause(bool _value)
    {
        this.IsPause = _value;
    }

    private Motor motor;

    public Transform wallTransform;
    private Transform wallLTranform;
    private Transform wallRTranform;

    private MeshRenderer meshRenderer;

    public Action<bool> OnWallReached;

    public Action OnAttackRangeRached;

    public Action<Health> OnFoundTarget;

    private void ChangeDirectMove()
    {
        _currentLRDirect *= -1;
        DurationMoveLR = UnityEngine.Random.Range(DurationMoveLR_Min, DurationMoveLR_Max);
        _timerMoveLR = 0;
    }

    public void OnMoveLR(float _deltaTime)
    {
        if (enableMoveLeftRight && this.motor != null)
        {
            _timerMoveLR += _deltaTime;

            if (_timerMoveLR >= DurationMoveLR)
            {
                ChangeDirectMove();
            }

            if (_currentLRDirect == 1)
            {
                this.motor.MoveRight(_deltaTime);
                if (_rotTransform)
                {
                    _rotTransform.rotation = Quaternion.Lerp(_rotTransform.rotation, Quaternion.Euler(0, ROTATION_ANGLE, 0), _deltaTime * 5);
                    //_rotTransform.rotation = Quaternion.Euler(0, ROTATION_ANGLE, 0);
                }

                if (wallRTranform && transform.position.z >= wallRTranform.position.z)
                {
                    ChangeDirectMove();
                }
            }
            else if (_currentLRDirect == -1)
            {
                this.motor.MoveLeft(_deltaTime);
                if (_rotTransform)
                {
                    _rotTransform.rotation = Quaternion.Lerp(_rotTransform.rotation, Quaternion.Euler(0, 180 - ROTATION_ANGLE, 0), _deltaTime * 5);

                    //_rotTransform.rotation = Quaternion.Euler(0, 180 - ROTATION_ANGLE, 0);
                }

                if (wallLTranform && transform.position.z <= wallLTranform.position.z)
                {
                    ChangeDirectMove();
                }
            }
        }
    }

    public virtual void SetPauseBehaviour(bool _value)
    {
        IsPause = _value;
    }

    public virtual void OnSwitchTarget(Health newTarget)
    {
        _targetPlayer = newTarget;
        OnFoundTarget?.Invoke(_targetPlayer);
    }

}
