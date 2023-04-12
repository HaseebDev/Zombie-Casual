using com.datld.data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using MEC;
[Serializable]
public class HeroAnimDefine
{
    public WEAPON_TYPE _type;
    [Header("Idle")]
    public string _animIdle;
    public float _idleInitSpeed = 1;

    [Header("Shoot")]
    public string _animAttack;
    public float _attackInitSpeed = 1;
    public Transform _markerShoot;
}

public class HeroAnimMachine : MonoBehaviour
{
    public Animator _animator;
    public float InitShootSpeed = 0.9f;
    public AnimationEventHandlers _animEventHandler;

    public Action OnLaunchBullet;

    public float delayShoot = 0.1f;

    private string WeaponID;
    public Gun _gunObj { get; private set; }
    private float _shootSpeed = 1.0f;
    private HeroAnimDefine _animationDefine = null;

    private int countPendingShoot = 0;
    private Action<bool> shootCallback;
    private bool isBusyShoot = false;

    private bool isSingleShoot = false;

    public Transform CurrentShootMarker { get; private set; }

    public List<HeroAnimDefine> _listAnimDefine;

    public Stack<Action<bool>> _queueActionShoot = new Stack<Action<bool>>();

    public HeroAnimDefine GetAnimDefine(WEAPON_TYPE type)
    {
        return _listAnimDefine.FirstOrDefault(x => x._type == type);
    }

    public void Initialize(WEAPON_TYPE type, string _weaponID)
    {
        this.WeaponID = _weaponID;
        _animationDefine = GetAnimDefine(type);
        _animator.Play(_animationDefine._animIdle, 0, 0);
        LoadWeapon(type, _weaponID);

        if (_animEventHandler)
        {
            _animEventHandler.OnShootEvent = OnAnimShootEvent;
            _animEventHandler.OnFinishShoot = OnFinishShootAnim;
        }


    }

    public void LoadCurrentShootMarker(Transform parent)
    {
        this.CurrentShootMarker = _animationDefine._markerShoot;
        if (CurrentShootMarker != null)
            this.CurrentShootMarker.SetParent(parent);
    }

    public void CalcShootSpeed(float FireRate)
    {
        // Debug.Log($"CalcShootSpeed!!! with {FireRate}");
        isSingleShoot = false;
        var animShoot = this._animator.GetAnimationInfo(_animationDefine._animAttack);
        if (animShoot != null)
        {
            //Debug.Log($"ANIM SHOOT WITH LENGTH ${animShoot.length}");
            float numInSec = 1.0f / animShoot.length * 1.0f;
            //_shootSpeed = numInSec / FireRate;
            _shootSpeed = FireRate * 1.0f / numInSec * 1.0f;

            if (_shootSpeed <= 0.36f)
            {
                _shootSpeed = 1.0f;
                isSingleShoot = true;          
            }

           // Debug.Log($"CalcShootSpeed {_shootSpeed}");
            _animator.speed = _shootSpeed;
        }
    }

    public void LoadWeapon(WEAPON_TYPE type, string weaponID)
    {
        var gunMarker = _animator.transform.FindChildRecursively("Gun");
        if (gunMarker != null)
        {
            gunMarker.gameObject.DestroyAllChilds();
            //var gunPref = ResourceManager.instance.GetGunPrefab(weaponID);
            //if (gunPref != null)
            //{
            //    _gunObj = Instantiate<Gun>(gunPref, gunMarker);
            //    _gunObj.transform.localScale = Vector3.one;
            //    _gunObj.transform.localPosition = Vector3.zero;
            //}

            ResourceManager.instance.SpawnGun(weaponID, (_gunObj) =>
            {
                _gunObj.transform.SetParent(gunMarker);
                _gunObj.transform.localScale = Vector3.one;
                _gunObj.transform.localPosition = Vector3.zero;
                _gunObj.transform.localRotation = Quaternion.identity;
            });
        }
    }

    public void SetBool(string parameter, bool value)
    {
        _animator.SetBool(parameter, value);
    }

    public void PlayAnimShoot(Action<bool> complete = null)
    {
        shootCallback = complete;
        _animator.speed = _shootSpeed;

        if (!isSingleShoot)
        {
            _animator.SetBool("shoot", true);
        }
        else
        {
            _animator.SetBool("shoot", false);
            _animator.Play(_animationDefine._animAttack, -1, 0);
        }

    }

    public void PlayAnimIdle()
    {
        //_animator.Play(_animationDefine._animIdle, 0, 0);
        //_animator.speed = 1.0f;

        _animator.SetBool("shoot", false);
        _animator.speed = 1.0f;
    }

    #region Animation Event Callbacks
    public void OnAnimShootEvent()
    {
        if (countPendingShoot >= 1)
        {
            shootCallback?.Invoke(true);
            countPendingShoot--;
        }
        OnLaunchBullet?.Invoke();
    }

    public void OnFinishShootAnim()
    {
        //if (countPendingShoot >= 1)
        //{
        //    _animator.Play(_animationDefine._animAttack, 0, 0);
        //    isBusyShoot = true;
        //}
        //else
        //    isBusyShoot = false;
    }
    #endregion

    private void Update()
    {
        //AnimatorStateInfo animationState = _animator.GetCurrentAnimatorStateInfo(0);
        //AnimatorClipInfo[] myAnimatorClip = _animator.GetCurrentAnimatorClipInfo(0);
        //float myTime = myAnimatorClip[0].clip.length * animationState.normalizedTime;
        //Logwin.Log($"anim {myAnimatorClip[0].clip.name}", myTime, "[ANIM DEBUG]");

    }

}
