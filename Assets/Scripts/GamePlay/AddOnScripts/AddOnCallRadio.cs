using Ez.Pooly;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddOnCallRadio : ZoneBombUltimate
{
    public float StartOffsetX = 20f;
    public float StartOffsetY = 5f;


    public static int NUM_BOOM_ROW = 3;
    public static float OFFSET_Z = 2.0f;
    public static float OFFSET_X = 2.0f;

    protected Vector3 _localStartSpawnPos;
    protected Vector3 _localEndSpanwPos;
    protected Vector3 _localCenterPos;

    private float TravelZ = 0f;

    [Header("Cheat Damge for level! must replace later")]
    public float CheatDmgLevelMultiply = 10f;

    public override void ResetSkill(bool hardReset = false)
    {
        countBomb = 0;
        triggerStarted = false;
        timerThrow = 0f;
        TravelZ = _localEndSpanwPos.z - OFFSET_Z;

        if (SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CurrentLevel <= 3)
        {
            CheatDmgLevelMultiply = 2000f;

        }
        else
        {
            CheatDmgLevelMultiply = 1.0f;

        }
    }

    public override void PointerDownSkill(Vector2 screenPos)
    {
        if (!CanUse)
            return;

        base.PointerDownSkill(screenPos);
        ResetSkill();
    }

    public override bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true)
    {
        if (CanUse)
            return base.PointerUpSkill(screenPos, checkValidCast);
        return false;
    }

    public override void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        base.PreInit(skillID, isUnlocked, args);
        _addOnItem = SaveManager.Instance.Data.GetAddOnItem(skillID);

        _localStartSpawnPos = GamePlayController.instance.gameLevel._skillZoneMarker.startSkillMarker.localPosition;
        _localEndSpanwPos = GamePlayController.instance.gameLevel._skillZoneMarker.endSkillMarker.localPosition;
        _localCenterPos = GamePlayController.instance.gameLevel._skillZoneMarker.centerSkillMarker.localPosition;
        TravelZ = _localEndSpanwPos.z - OFFSET_Z;


        if (SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CurrentLevel <= 3)
        {
            CheatDmgLevelMultiply = 2000f;

        }
        else
        {
            CheatDmgLevelMultiply = 1.0f;

        }
        this.CanUse = true;
    }

    public override void StartSkillUpdate(float _deltaTime)
    {
        timerThrow += _deltaTime;
        if (countBomb < _design.Number && timerThrow >= DelayThrow)
        {
            TravelZ += OFFSET_Z;
            ThrowRowBomb(TravelZ);
            timerThrow = 0f;
        }
    }

    //throw 3 bombs
    private void ThrowRowBomb(float travelZ)
    {
        Timing.RunCoroutine(ThrowRowBombCoroutine(travelZ));
    }

    IEnumerator<float> ThrowRowBombCoroutine(float travelZ)
    {
        int numOneRow = 0;
        if (countBomb + NUM_BOOM_ROW <= _design.Number)
            numOneRow = NUM_BOOM_ROW;
        else
            numOneRow = (int)_design.Number - countBomb;

        countBomb += numOneRow;
        float travelX = _localStartSpawnPos.x;
        for (int i = 0; i < numOneRow; i++)
        {
            var targetLocal = new Vector3(travelX, 0, travelZ);
            var wPos = GamePlayController.instance.gameLevel._skillZoneMarker.transform.TransformPoint(targetLocal);
            ThrowBomb(wPos);
            travelX += OFFSET_X;
            yield return Timing.WaitForOneFrame;
        }

        //CHEAT FOR TUTORIAL
        if (SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CurrentLevel <= 3)
        {
            yield return Timing.WaitForSeconds(1.5f);
            GamePlayController.instance._waveController.ClearAllZombieOnWave();
        }
    }

    public override void ThrowBomb(Vector3 worldPos)
    {
        var startPos = worldPos;
        startPos.y += StartOffsetY;
        startPos.x += StartOffsetX;

        var grenade = Pooly.Spawn<BaseBomb>(BoomPrefab.transform, startPos, Quaternion.identity, null);
        if (grenade != null)
        {
            grenade.transform.localScale = Vector3.one;
            var dmg = this.GetUltimateDmg() * CheatDmgLevelMultiply;
            grenade.PreInit(dmg, _design.Radius, this._OwnerID, BaseBomb.MOVE_TYPE.FLY);
            grenade.Launch(worldPos, () => { grenade.ActiveBomb(); });
        }
    }
}