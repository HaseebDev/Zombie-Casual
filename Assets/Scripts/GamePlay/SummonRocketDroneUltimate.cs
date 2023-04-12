using Ez.Pooly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SummonRocketDroneUltimate : BaseCharacterUltimate
{
    public static float MAX_SIDE_ANGLE = 90;
    public static float UPSIDE_ANGLE = 60;
    public static float SET_DELAY = 0.2f;
    public static float LAUNCH_FORCE = 10f;

    [Header("Config")]
    public float _AdditionalSpeed = 0.3f;

    public Transform _HeliPrefab;
    public HomingBoomBullet _rocketPrefab;

    private List<HomingBoomBullet> _listRockets;
    private GameObject _heliGO;
    private Character hero;

    private float _bulletDmg;
    private int SideNumber;
    private float UnitAngle;
    private int TotalSet;
    private float timerRocketSet = 0f;

    private int setCount;

    private bool TriggerLaunched = false;

    public void PreloadRockets()
    {
        _listRockets = new List<HomingBoomBullet>();
        for (int i = 0; i < DesignSkill.Number; i++)
        {
            var bullet = Pooly.Spawn<HomingBoomBullet>(_rocketPrefab.transform, Vector3.zero, Quaternion.identity, transform);
            if (bullet != null)
            {
                Pooly.Despawn(bullet.transform);
            }
        }
    }

    public override void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        base.PreInit(skillID, isUnlocked, args);
        PreloadRockets();

        if (_heliGO == null)
        {
            _heliGO = Pooly.Spawn(_HeliPrefab, Vector3.zero, Quaternion.identity, null).gameObject;

            _heliGO.transform.rotation = transform.rotation;
            _heliGO.SetActiveIfNot(false);
        }



        SideNumber = (int)(DesignSkill.Number / 2);
        UnitAngle = MAX_SIDE_ANGLE / SideNumber;
        timerRocketSet = 0f;
        TotalSet = (int)(DesignSkill.Duration / SET_DELAY);
        TriggerLaunched = false;

        _bulletDmg = GetUltimateDmg() / DesignSkill.Number;
    }

    public override void PointerDownSkill(Vector2 screenPos)
    {
        if (hero == null)
            hero = GamePlayController.instance.gameLevel._dictCharacter[this._OwnerID];

        base.PointerDownSkill(screenPos);
        SideNumber = (int)(DesignSkill.Number / 2);
        UnitAngle = MAX_SIDE_ANGLE / SideNumber;
        timerRocketSet = 0f;
        setCount = 0;
        TriggerLaunched = false;

        _heliGO.transform.position = hero.transform.position + Vector3.up * 2f - _heliGO.transform.forward * 5f;
    }

    public override bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true)
    {
        float _timeFx = 0.5f;
        var result = base.PointerUpSkill(screenPos, checkValidCast);

        _heliGO.transform.position = hero.transform.position + Vector3.up * 0.5f;
        _heliGO.transform.localScale = Vector3.one * 0.3f;
        _heliGO.SetActiveIfNot(true);

        _heliGO.transform.DOScale(1.0f, _timeFx).SetEase(Ease.Linear);
        _heliGO.transform.DOMove(hero.transform.position + Vector3.up * 2.5f, _timeFx).SetEase(Ease.Linear).OnComplete(() =>
        {

            TriggerLaunched = true;
        });


        return result;
    }

    public override void UpdateSkill(float deltaTime)
    {
        base.UpdateSkill(deltaTime);
        if (TriggerLaunched)
        {
            if (setCount < TotalSet && timerRocketSet <= 0f)
            {
                FireRocketSet();
                setCount++;
                timerRocketSet = SET_DELAY;
            }
            else if (setCount >= TotalSet)
            {
                TriggerLaunched = false;
                _heliGO.transform.DOMove(_heliGO.transform.position + _heliGO.transform.up * 20f, 1f).SetEase(Ease.Linear);
            }

            timerRocketSet -= deltaTime;
        }
    }

    public void FireRocketSet()
    {
        var target = FindZombieBoss();
        if (target == null)
        {
            var listZomHealth = base.GetRandomZombies();
            if (listZomHealth != null && listZomHealth.Count > 0)
                target = listZomHealth[UnityEngine.Random.Range(0, listZomHealth.Count)];
            else
                target = null;
        }

        for (int i = 0; i < DesignSkill.Number; i++)
        {
            var bullet = Pooly.Spawn<HomingBoomBullet>(_rocketPrefab.transform, Vector3.zero, Quaternion.identity, transform);
            bullet.Initialize(null, null, ResourceManager.instance._maskZombieOnly);
            bullet.transform.localScale = Vector3.one;
            bullet.transform.position = _heliGO.transform.position - Vector3.up * 0.5f;

            //float randY =  UnitAngle *(-SideNumber + i) + UnityEngine.Random.Range(-90f, 90f);

            float randY = UnityEngine.Random.Range(-150, 150f);
            float randX = UnityEngine.Random.Range(-UPSIDE_ANGLE, UPSIDE_ANGLE);
            bullet.transform.localRotation = Quaternion.Euler(randX, randY, 0);
            bullet.transform.SetParent(null);
            bullet.SetTarget(target);
            bullet.SetAdditionSpeed(_AdditionalSpeed);
            bullet.Launch(LAUNCH_FORCE, _bulletDmg, ShotType.NORMAL);
            _listRockets.Add(bullet);
        }

    }

    public override void ResetSkill(bool hardReset = false)
    {
        base.ResetSkill(hardReset);

        ResetRockets();
    }

    public void ResetRockets()
    {
        foreach (var item in _listRockets)
        {
            Pooly.Despawn(item.transform);
        }

        _listRockets.Clear();
        TriggerLaunched = false;
        setCount = 0;
        _heliGO.transform.DOKill();
        _heliGO.gameObject.SetActiveIfNot(false);
    }

    public override void CleanUp()
    {
        base.CleanUp();
        ResetRockets();
    }
}
