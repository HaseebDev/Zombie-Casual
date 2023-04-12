using System;
using UnityEngine;

public class Motor : MonoBehaviour
{
    public static float PUSH_BACK_DURATION = 0.2f;
    private void Awake()
    {
        this.speed = UnityEngine.Random.Range(0.3f, 0.4f);
    }

    private float StartSpeed = 0f;
    private float StartTurningSpeed = 0f;

    private float turningSpeedMultiplier = 0f;

    private float reduceSpeedMultiplier = 0f;

    public void Initialize(float defSpeed, float turnningSpeed)
    {
        this.StartSpeed = defSpeed;
        this.StartTurningSpeed = turnningSpeed;
        this.speed = StartSpeed;
        this.turningSpeed = StartTurningSpeed;
        this.reduceSpeedMultiplier = 1.0f;

        this.turningSpeedMultiplier = turningSpeed / defSpeed;

        SetReduceSpeedMultiplier(1.0f);
    }

    public void SetSpeed(float _newSpeed)
    {
        this.speed = _newSpeed;
        this.turningSpeed = turningSpeedMultiplier * _newSpeed;
    }

    public void SetSpeedMultiply(float _multiply)
    {
        this.speed *= _multiply;
        this.turningSpeed *= _multiply;
    }

    public void SetReduceSpeedMultiplier(float _reduce)
    {
        this.reduceSpeedMultiplier = _reduce;
    }

    public void ResetSpeed()
    {
        this.speed = StartSpeed;
    }

    public float GetDefSpeed()
    {
        return this.speed;
    }

    public float GetFinalSpeed()
    {
        return (this.speed + this.AdditionalSpeed) * reduceSpeedMultiplier * 1.0f;
    }

    public float GetFinalTurningSpeed()
    {
        return (this.turningSpeed + this.AdditionalSpeed) * reduceSpeedMultiplier * 1.0f;
    }

    public void MoveUp(float _deltaTime)
    {
        base.transform.position += Vector3.left * this.pushBackSpeed * _deltaTime;
        TimerPushBack -= _deltaTime;
        if (TimerPushBack <= 0f)
        {
            IsPushingBack = false;
            TimerPushBack = 0f;
        }

    }

    public void MoveDown(float _deltaTime)
    {
        if (this.isMove)
        {
            base.transform.position += Vector3.right * GetFinalSpeed() * _deltaTime;
        }
    }

    public void MoveYDown(float _deltaTime)
    {
        if (this.isMove)
        {
            base.transform.position += Vector3.down * GetFinalSpeed() * _deltaTime;
        }
    }

    public void MoveLeft(float _deltaTime)
    {
        if (this.isMove)
        {
            base.transform.position -= Vector3.forward * GetFinalTurningSpeed() * _deltaTime;
        }
    }

    public void MoveRight(float _deltaTime)
    {
        if (this.isMove)
        {
            base.transform.position += Vector3.forward * GetFinalTurningSpeed() * _deltaTime;
        }
    }

    private float speed = 0.35f;
    private float turningSpeed = 0.2f;
    private float pushBackSpeed = 0.35f;

    public bool isMove = true;

    private float TimerPushBack = 0f;
    public bool IsPushingBack = false;

    public void PushBack(float _pushBackSpeed = 0.35f, float pushBackTime = 0.2f)
    {
        TimerPushBack = pushBackTime;
        IsPushingBack = true;
        pushBackSpeed = _pushBackSpeed;
    }

    #region AddSpeed
    public float AdditionalSpeed { get; private set; }
    public void SetAdditionalSpeed(float _addSpeed)
    {
        AdditionalSpeed = _addSpeed;
    }

    #endregion
}
