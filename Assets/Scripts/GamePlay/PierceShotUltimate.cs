using Ez.Pooly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PierceShotUltimate : BaseCharacterUltimate
{
    public Bullet bulletPrefab;

    private Transform _aimingShot;
    private bool startDrag = false;
    protected Bullet _bullet;

    protected float _dragAngle = 0f;
    protected float _offsetX = 0f;

    public override void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        base.PreInit(skillID, isUnlocked, args);

        AddressableManager.instance.SpawnObjectAsync<Transform>(POOLY_PREF.AIMING_PRICE_SHOT_3D, (aming) =>
         {
             //_aimingShot = Pooly.Spawn(POOLY_PREF.AIMING_PRICE_SHOT_3D, Vector3.zero, Quaternion.identity, null);
             _aimingShot = aming;
             _aimingShot.transform.localPosition = Vector3.zero;
             _aimingShot.gameObject.SetActiveIfNot(false);
             _aimingShot.localScale = Vector3.one;
         });



        _bullet = Pooly.Spawn<Bullet>(bulletPrefab.transform, Vector3.zero, Quaternion.identity, null);
        _bullet.gameObject.SetActiveIfNot(false);
        _bullet.transform.position = transform.position;

        startDrag = false;
    }

    public override void PointerDownSkill(Vector2 screenPos)
    {
        var heroPos = transform.position + Vector3.up;
        _aimingShot.position = heroPos;
        _aimingShot.transform.rotation = Quaternion.Euler(Vector3.zero);
        _aimingShot.gameObject.SetActiveIfNot(true);
        base.PointerDownSkill(screenPos);

        var screenPointTarget = GamePlayController.instance.MainCamera.WorldToScreenPoint(_aimingShot.transform.position);
        _offsetX = screenPointTarget.x - screenPos.x;

        startDrag = true;

    }

    public override bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true)
    {
        var result = true;
        base.PointerUpSkill(screenPos, checkValidCast);
        _aimingShot.gameObject.SetActiveIfNot(false);
        startDrag = false;

        ShotBullet(transform.position, -_dragAngle);

        base.PostSkill();
        return result;
    }

    public override void UpdateSkill(float deltaTime)
    {
        base.UpdateSkill(deltaTime);
        if (startDrag)
        {
            var mousePos = Input.mousePosition + Vector3.up * 1200f;

            mousePos.x += _offsetX;
            var dir = mousePos - GamePlayController.instance.MainCamera.WorldToScreenPoint(_aimingShot.transform.position); ;
            _dragAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _dragAngle = Mathf.Clamp(_dragAngle, -60f + 90f, 60f + 90f);
            _aimingShot.rotation = Quaternion.AngleAxis(_dragAngle, Vector3.down);
        }
    }

    public virtual void ShotBullet(Vector3 pos, float angle)
    {
        _bullet.transform.rotation = Quaternion.AngleAxis(_dragAngle, Vector3.down);
        _bullet.transform.position = pos + Vector3.up;
        _bullet.gameObject.SetActiveIfNot(true);
        _bullet.Launch(_design.Value, this.GetUltimateDmg(), ShotType.NORMAL);
    }

    public override void CleanUp()
    {
        base.CleanUp();
        if (this._aimingShot != null)
        {
            GameObject.Destroy(this._aimingShot.gameObject);
        }
    }
}
