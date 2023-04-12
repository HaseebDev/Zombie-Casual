using Ez.Pooly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlyingAirDrop : AirDrop
{
    public float SpeedMove = 5f;

    private float TimeTravel = 0f;
    private bool IsTraveling = false;
    private float timerTravel = 0f;

    Vector3 targetPos;
    private short currentSide = -1; // -1 left, 1 right
    private float currentX = 0;

    private Vector3 _startPos;
    private Vector3 _endPos;
    private bool pendingDestroy = false;

    public override void Initialize(float _timeTravel = 0)
    {
        base.Initialize(TimeTravel);
        TimeTravel = _timeTravel;
        IsTraveling = false;
        pendingDestroy = false;
    }

    public void TravelAroundMap(Vector3 startPos, Vector3 endPos)
    {
        _startPos = startPos;
        _startPos.z -= 5f;

        _endPos = endPos;
        _endPos.z += 5f;

        currentX = _startPos.x;
        currentX += Random.Range(2f, 4f);

        var initPos = _startPos + Vector3.up * 3f;
        transform.position = initPos;

        timerTravel = 0f;
        currentSide = -1;
        targetPos = getRandTargetPos();

        IsTraveling = true;
    }

    private Vector3 getRandTargetPos()
    {
        Vector3 pos = transform.position;
        if (currentSide == -1)
        {
            pos.z = _endPos.z;

            currentX += Random.Range(2f, 4f);
            pos.x = currentX;
        }
        else if (currentSide == 1)
        {
            pos.z = _startPos.z;
            currentX += Random.Range(2f, 4f);
            pos.x = currentX;
        }

        transform.LookAt(pos);
        return pos;

    }

    private void Update()
    {
        if (IsTraveling)
        {
            float deltaTime = Time.deltaTime;
            timerTravel += deltaTime;
            if (timerTravel >= TimeTravel)
            {
                pendingDestroy = true;
            }

            float step = SpeedMove * deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            if (Mathf.Abs(transform.position.z - targetPos.z) <= 0.2f)
            {
                currentSide *= -1;
                targetPos = getRandTargetPos();

                if (pendingDestroy)
                {
                    DestroyAirDrop();
                }
            }
        }
    }

    public void DestroyAirDrop()
    {
        IsTraveling = false;
        pendingDestroy = false;
        currentSide = -1;
        //Pooly.Despawn(transform);
        GameObject.Destroy(this.gameObject);
    }

    public override void OnPointerUp()
    {
        //handle show dialog airdrop here!!
        AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_EQUIP_ARMOUR);

        var progress = SaveManager.Instance.Data.GetPlayProgress(GameMaster.instance.currentMode);
        int maxLevel = progress.MaxLevel;

        var _airDropRewards = DesignHelper.GetAirDropRewardsByPrefix(GamePlayController.instance.gameMode, maxLevel, GameConstant.FLYING_AIR_DROP);
        if (_airDropRewards != null)
        {
            //HUDAirDropData data = new HUDAirDropData();
            //data._listRewardNormal = _airDropRewards.Item1;
            //data._listRewardExtra = _airDropRewards.Item2;
            //InGameCanvas.instance.ShowHUD(EnumHUD.HUD_AIR_DROP, false, null, data, true);
            if(SaveManager.Instance.Data.AddRewards(_airDropRewards))
            {
                TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_REWARD_SIMPLE, false, null, _airDropRewards, false, false);
            }
        }
        //Pooly.Despawn(transform);
        GameObject.Destroy(this.gameObject);

    }
}
