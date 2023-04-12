using com.datld.data;
using Ez.Pooly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddOnSpawnGate : BaseAddOnUltimate
{
    public ObstacleProtectGate GatePrefab;
    private bool startDrag = false;

    private ObstacleProtectGate GateSpawnObject;

    private Vector3 offsetMove = Vector3.zero;

    public override void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        base.PreInit(skillID, isUnlocked, args);
    }

    public override void PointerDownSkill(Vector2 pos)
    {
        if (_addOnItem.Status == ITEM_STATUS.Disable || _addOnItem.ItemCount <= 0)
        {
            return;
        }

        float maxHP = GamePlayController.instance.CastleHP.GetHPWithCoeff() * _design.Value / 100f;
        Debug.Log($"GATE HP {maxHP}");
        base.PointerDownSkill(pos);
        GateSpawnObject =
            Pooly.Spawn<ObstacleProtectGate>(GatePrefab.transform, Vector3.zero, Quaternion.identity, null);
        GateSpawnObject.Initialize(maxHP);
        GateSpawnObject.SetStateDeploy(true);
        var worldPos = GamePlayController.instance.touchDetector.GetTouchWorldPos(pos);

        var spawnPos = worldPos;
        spawnPos.x = GamePlayController.instance.gameLevel._skillZoneMarker.endSkillMarker.position.x;

        offsetMove = spawnPos - worldPos;

        GateSpawnObject.transform.position = spawnPos;
        GateSpawnObject.gameObject.SetActiveIfNot(true);
        GateSpawnObject._collider.enabled = false;
        startDrag = true;
    }

    public override void UpdateSkill(float deltaTime)
    {
        base.UpdateSkill(deltaTime);
        if (startDrag)
        {
            var lastPos = GateSpawnObject.transform.position;
            var worldPos = GamePlayController.instance.touchDetector.GetTouchWorldPos(Input.mousePosition);
            var targetPos = worldPos + offsetMove;
            if (targetPos.CheckInSideRect(
                GamePlayController.instance.gameLevel._skillZoneMarker.startSkillMarker.position,
                GamePlayController.instance.gameLevel._skillZoneMarker.endSkillMarker.position))
                GateSpawnObject.transform.position = targetPos;
            else
            {
                GateSpawnObject.transform.position = lastPos;
            }
        }
    }

    public override bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true)
    {
        var basecheck = base.PointerUpSkill(screenPos, checkValidCast);
        if (basecheck && GateSpawnObject != null)
        {
            GateSpawnObject.gameObject.SetActiveIfNot(true);
            GateSpawnObject._collider.enabled = true;
            // GateSpawnObject.SetStateDeploy(false);
            var finalPos = GateSpawnObject.transform.position;
            GateSpawnObject.AnimDeployAtPosition(finalPos);
            GamePlayController.instance.AddObstacle(GateSpawnObject.transform);
        }

        startDrag = false;

        return true;
    }
}