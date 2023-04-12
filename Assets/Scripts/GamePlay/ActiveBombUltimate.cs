using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ez.Pooly;
using UnityEngine.EventSystems;
using Framework.Components.Input.TouchDetector;
public class ActiveBombUltimate : BaseCharacterUltimate
{
    public float DRAG_OFFSET = 200f;
    private DragDrop _aimingDrag;

    private bool startDrag = false;

    public override void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        base.PreInit(skillID, isUnlocked, args);

        AddressableManager.instance.SpawnObjectAsync<DragDrop>(POOLY_PREF.AIMING_DRAG, (drag) =>
         {
             _aimingDrag = drag;
             _aimingDrag.transform.SetParent(InGameCanvas.instance.transform);
             _aimingDrag.transform.localPosition = Vector3.zero;
             _aimingDrag.transform.localRotation = Quaternion.identity;
             //_aimingDrag = Pooly.Spawn<DragDrop>(POOLY_PREF.AIMING_DRAG, Vector3.zero, Quaternion.identity, InGameCanvas.instance.transform);
             _aimingDrag.canvas = InGameCanvas.instance.canvas;
             _aimingDrag.gameObject.SetActiveIfNot(false);
         });


        DRAG_OFFSET = 0.15f * Screen.height;

    }

    public override void PointerDownSkill(Vector2 pos)
    {
        if (!CanUse)
            return;

        if (_addOnItem != null && (_addOnItem.Status == com.datld.data.ITEM_STATUS.Disable || _addOnItem.ItemCount <= 0))
        {
            return;
        }

        base.PointerDownSkill(pos);
        _aimingDrag.transform.position = pos;
        _aimingDrag.gameObject.SetActiveIfNot(true);

        startDrag = true;
    }

    public override void UpdateSkill(float deltaTime)
    {
        base.UpdateSkill(deltaTime);
        if (startDrag)
        {
            _aimingDrag.transform.position = Input.mousePosition + Vector3.up * DRAG_OFFSET;
        }
    }

    public override void BeginDragSkill(Vector2 pos)
    {
        base.BeginDragSkill(pos);
        //_aimingDrag.OnBeginDrag(data);
    }

    public override bool PointerUpSkill(Vector2 pos, bool checkValidCast = true)
    {
        if (!CanUse)
            return false;

        var result = true;
        
        if (_addOnItem != null && (_addOnItem.Status == com.datld.data.ITEM_STATUS.Disable || _addOnItem.ItemCount <= 0))
        {
            if (_addOnItem.ItemCount <= 0)
                NotifyDontHasAddon();

            return false;
        }
        
        base.PointerUpSkill(pos, checkValidCast);


        startDrag = false;
        _aimingDrag.gameObject.SetActiveIfNot(false);

        var dist = Vector2.Distance(pos, _pointerDownPos);
        var worldPos = Vector3.zero;
        if (dist <= 20f)
        {
            //worldPos = _centerPos;
            //worldPos = base.DetectHugeZombieZone02(new Vector2(_design.Radius, _design.Radius), _design.Radius);

            worldPos = base.DetectHugeZombieZone02();
        }
        else
        {
            worldPos = GamePlayController.instance.touchDetector.GetTouchWorldPos(_aimingDrag.transform.position);
        }

        ThrowBomb(worldPos);

        return result;
    }

    public virtual void ThrowBomb(Vector3 worldPos)
    {
        var grenade = Pooly.Spawn<BaseBomb>(POOLY_PREF.SINGLE_GRENADE, transform.position, Quaternion.identity, null);
        if (grenade != null)
        {
            grenade.transform.localScale = Vector3.one;
            grenade.PreInit(this.GetUltimateDmg(), _design.Radius, this._OwnerID);
            grenade.Launch(worldPos, () =>
            {
                grenade.ActiveBomb();
            });
        }

        base.PostSkill();
    }

}
