using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Ez.Pooly;

public class ZoneBombUltimate : BaseCharacterUltimate
{
    [Header("Boomb Prefab")]
    public BaseBomb BoomPrefab;

    public float DelayThrow = 0.4f;

    protected int countBomb = 0;
    protected bool triggerStarted = false;
    protected float timerThrow = 0f;

    public override void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        base.PreInit(skillID, isUnlocked, args);

        countBomb = 0;
        triggerStarted = false;
        timerThrow = 0f;
    }

    public override void PointerDownSkill(Vector2 screenPos)
    {
        base.PointerDownSkill(screenPos);
        ResetSkill();
    }

    public override bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true)
    {
        var result = false;

        if (this._addOnItem != null && (this._addOnItem.Status == com.datld.data.ITEM_STATUS.Disable || this._addOnItem.ItemCount <= 0))
        {
            if (_addOnItem.ItemCount <= 0)
            {
                NotifyDontHasAddon();
            }

            return false;
        }

        if (base.PointerUpSkill(screenPos, checkValidCast))
        {
            triggerStarted = true;
            result = true;
        }

        return result;
    }

    public override void ResetSkill(bool hardReset = false)
    {
        countBomb = 0;
        triggerStarted = false;
        timerThrow = 0f;
    }

    public override void UpdateSkill(float deltaTime)
    {
        base.UpdateSkill(deltaTime);
        if (triggerStarted)
        {
            StartSkillUpdate(deltaTime);
        }
    }

    public virtual void StartSkillUpdate(float _deltaTime)
    {
        timerThrow += _deltaTime;
        if (countBomb < _design.Number && timerThrow >= DelayThrow)
        {
            timerThrow = 0f;
            RandThrowGrenade();
            countBomb++;
        }
    }

    public void RandThrowGrenade()
    {
        var randX = UnityEngine.Random.Range(_startSpawnPos.x, _endSpanwPos.x);
        var randZ = UnityEngine.Random.Range(_startSpawnPos.z, _endSpanwPos.z);
        var randPos = new Vector3(randX, _startSpawnPos.y, randZ);
        ThrowBomb(randPos);
    }

    public virtual void ThrowBomb(Vector3 worldPos)
    {
        var startPos = worldPos + Vector3.up * 15f;
        startPos.x += UnityEngine.Random.Range(-3f, 3f);
        startPos.z += UnityEngine.Random.Range(-3f, 3f);

        var grenade = Pooly.Spawn<BaseBomb>(BoomPrefab.transform, startPos, Quaternion.identity, null);
        if (grenade != null)
        {
            grenade.transform.localScale = Vector3.one;
            grenade.PreInit(this.GetUltimateDmg(), _design.Radius, this._OwnerID, BaseBomb.MOVE_TYPE.FLY);
            grenade.Launch(worldPos, () =>
            {
                grenade.ActiveBomb();
            });
        }
    }
}
