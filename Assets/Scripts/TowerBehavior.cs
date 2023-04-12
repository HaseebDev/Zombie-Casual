using System;
using System.Collections;
using UnityEngine;

public class TowerBehavior : AIBehavior
{
    public LayerMask maskDetectZombie;
    public bool AutoFindTarget = true;
    public Transform _gunMount { get; private set; }

    private float DurationSwitchTarget;
    private float _timerSwitchTarget = 0f;

    private Collider[] _collidersAtPos;
    private RaycastHit[] _raycastHit;
    private Ray _rayAtPos;

    public void SetGunMount(Transform trf)
    {
        _gunMount = trf;
    }

    public void ResetData(float findRange, float durationSwitch = -1)
    {
        this.findDistance = findRange;
        this.DurationSwitchTarget = durationSwitch;
        _timerSwitchTarget = 0f;
        _rayAtPos = new Ray();
    }

    public void CleanUp()
    {
        this.target = null;
      
        SwitchState(COMBAT_AI_STATES.FIND_TARGET);
    }


    private void Start()
    {
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.ZOMBIE_WAVE_STARTED, new Action(WaweController_OnWaweStarted));
        _raycastHit = new RaycastHit[1];
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.ZOMBIE_WAVE_STARTED, new Action(WaweController_OnWaweStarted));
    }

    private void WaweController_OnWaweStarted()
    {
        if (GamePlayController.instance)
            this.zombies = GamePlayController.instance._waveController.GetZombiesOnThisWawe().ToArray();
    }

    public void UpdateBehaviour(float _deltaTime)
    {
        //auto switch target
        if (AutoFindTarget)
        {
            if (this.currentState == COMBAT_AI_STATES.CAPTURE_TARGET)
            {
                this.LookAtTarget();

                if (DurationSwitchTarget > 0)
                {
                    _timerSwitchTarget += _deltaTime;
                    if (_timerSwitchTarget >= DurationSwitchTarget)
                    {
                        this.FindNearestTarget();
                        _timerSwitchTarget = 0f;
                    }
                }

                if (!target.IsTargetable)
                {
                    SwitchState(COMBAT_AI_STATES.FIND_TARGET);
                }
            }
            else if (this.currentState == COMBAT_AI_STATES.FIND_TARGET)
            {
                this.FindNearestTarget();
            }
        }
    }

    public bool ManualFindTarget(Vector3 worldPos)
    {
        bool targetFound = false;
        targetFound = this.FindNearestTargetAtPos(worldPos);
        if (targetFound)
        {
            SwitchState(COMBAT_AI_STATES.CAPTURE_TARGET);
            this.LookAtTarget();
        }
        else
            SwitchState(COMBAT_AI_STATES.FIND_TARGET);
        return targetFound;
    }

    public bool ShootAtTarget(Health newTarget)
    {
        bool targetFound = false;

        if (!newTarget.IsDead())
        {
            this.target = newTarget;
            this.SwitchState(COMBAT_AI_STATES.CAPTURE_TARGET);
            this.LookAtTarget();
        }


        return targetFound;

    }

    public bool ManualLockTarget(Health newTarget)
    {
        bool targetFound = false;

        if (!newTarget.IsDead())
        {
            this.target = newTarget;
            this.LookAtTarget();
            SwitchState(COMBAT_AI_STATES.CAPTURE_TARGET);
            targetFound = true;
        }
        else
        {
            SwitchState(COMBAT_AI_STATES.FIND_TARGET);
            this.target = null;
        }
        return targetFound;
    }

    private void findTarget()
    {
        //for (int i = 0; i < this.zombies.Length; i++)
        //{
        //    if (Vector3.Distance(base.transform.position, this.zombies[i].transform.position) < this.findDistance && !this.zombies[i].IsDead)
        //    {
        //        int num = UnityEngine.Random.Range(i, this.zombies.Length - 1);
        //        this.target = this.zombies[num];

        //        this.SwitchState(COMBAT_AI_STATES.CAPTURE_TARGET);
        //        return;
        //    }
        //}
    }

    private bool FindNearestTarget()
    {
        bool foundTarget = false;
        var hits = Physics.OverlapSphereNonAlloc(transform.position, this.findDistance, _collidersAtPos, maskDetectZombie, QueryTriggerInteraction.Collide);
        if (hits > 0)
        {
            var targetIndex = -1;
            float minDist = float.MaxValue;
            Health targetZom = null;
            for (int i = 0; i < hits; i++)
            {
                var dist = Vector3.Distance(transform.position, _collidersAtPos[i].transform.position);
                Health zom = _collidersAtPos[i].GetComponent<Health>();
                if (zom != null && GameMaster.IsZombieTag(zom.tag) && zom.IsTargetable && dist < this.findDistance && !zom.IsDead() && dist < minDist)
                {
                    targetZom = zom;
                    targetIndex = i;
                    minDist = dist;
                }
            }

            if (targetIndex != -1)
            {
                this.target = targetZom;
                this.LookAtTarget();
                this.SwitchState(COMBAT_AI_STATES.CAPTURE_TARGET);
                foundTarget = true;
            }
            else
            {
                this.SwitchState(COMBAT_AI_STATES.FIND_TARGET);
            }
        }

        return foundTarget;

    }

    //private bool FindNearestTargetAtPos(Vector3 worldPos)
    //{
    //    bool foundTarget = false;
    //    var numHit = Physics.OverlapSphereNonAlloc(worldPos, 0.05f, _collidersAtPos, ResourceManager.instance._maskZombieOnly);
    //    if (numHit > 0)
    //    {
    //        var targetIndex = -1;
    //        float minDist = float.MaxValue;
    //        Health targetZom = null;
    //        for (int i = 0; i < numHit; i++)
    //        {
    //            var dist = Vector3.Distance(worldPos, _collidersAtPos[i].transform.position);
    //            Health zom = _collidersAtPos[i].GetComponent<Health>();
    //            if (!zom.IsDead() && dist < minDist)
    //            {
    //                targetZom = zom;
    //                targetIndex = i;
    //                minDist = dist;
    //            }
    //        }

    //        if (targetIndex != -1)
    //        {
    //            this.target = targetZom;
    //            this.LookAtTarget();
    //            this.SwitchState(COMBAT_AI_STATES.CAPTURE_TARGET);
    //            foundTarget = true;
    //            Logwin.Log("[Manual Shoot]", "locked target!!!");
    //        }
    //        else
    //        {
    //            this.SwitchState(COMBAT_AI_STATES.FIND_TARGET);
    //        }

    //    }
    //    else
    //    {
    //        foundTarget = FindNearestTarget();
    //        Logwin.Log("[Manual Shoot]", "not found target!!!");
    //    }

    //    return foundTarget;
    //}

    private bool FindNearestTargetAtPos(Vector3 worldPos)
    {
        bool foundTarget = false;
        Vector3 origin = transform.position + Vector3.up;
        Vector3 targetPos = worldPos;
        targetPos.y = origin.y;
        _rayAtPos.origin = origin;
        _rayAtPos.direction = (targetPos - origin);

        var numHit = Physics.RaycastNonAlloc(_rayAtPos, _raycastHit, int.MaxValue, ResourceManager.instance._maskZombieOnly);
        if (numHit > 0)
        {
            Health targetZom = _raycastHit[0].transform.GetComponent<Health>();
            if (targetZom != null)
            {
                this.target = targetZom;
                this.LookAtTarget();
                this.SwitchState(COMBAT_AI_STATES.CAPTURE_TARGET);
                foundTarget = true;
            }
            else
            {
                this.SwitchState(COMBAT_AI_STATES.FIND_TARGET);
            }

        }
        else
        {
            foundTarget = FindNearestTarget();
        }

        return foundTarget;
    }

    private void LookAtTarget()
    {
        //base.transform.LookAt(targetPostition);
        if (this.transform == null || target == null)
        {
            SwitchState(COMBAT_AI_STATES.FIND_TARGET);
            return;
        }

        Quaternion targetRot = transform.rotation;
        if (_gunMount != null)
        {
            Vector3 targetPostition = new Vector3(target.transform.position.x,
                                  transform.position.y,
                                   target.transform.position.z);
            var norMount = new Vector3(_gunMount.position.x, transform.position.y, _gunMount.position.z);
            Vector3 offsetLookAt = norMount - transform.position;
            float distOffset = Vector3.Distance(norMount, transform.position);
            float angleOffset = Vector3.Angle(_gunMount.transform.right, transform.right);

            transform.LookAt(targetPostition - (offsetLookAt * distOffset));
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + angleOffset, transform.localEulerAngles.z);
        }
        else
        {
            Vector3 targetPostition = new Vector3(target.transform.position.x,
                                  transform.position.y,
                                  target.transform.position.z);
            base.transform.LookAt(targetPostition);
        }

        if (this.target.IsDead())
        {
            FindNearestTarget();
        }
    }

    private void SwitchState(COMBAT_AI_STATES newState)
    {
        if (newState == this.currentState)
        {
            return;
        }
        this.currentState = newState;
        if (this.currentState == COMBAT_AI_STATES.FIND_TARGET)
        {
            this.OnTargetSearch();
            return;
        }
        else if (this.currentState == COMBAT_AI_STATES.CAPTURE_TARGET)
        {
            if (target != null)
                this.OnTargetFound?.Invoke(target.transform);
        }

    }

    private Coroutine applyBehaviorCoroutine;
    private IEnumerator ApplyBehaviorCoroutine()
    {
        base.enabled = true;
        if (GamePlayController.instance)
        {
            var zombieWave = GamePlayController.instance._waveController.GetZombiesOnThisWawe();
            while (zombieWave == null)
            {
                zombieWave = GamePlayController.instance._waveController.GetZombiesOnThisWawe();
                yield return null;
            }

            this.zombies = zombieWave.ToArray();
            this.OnTargetSearch();
            _collidersAtPos = new Collider[100];
            this.SwitchState(COMBAT_AI_STATES.FIND_TARGET);
        }
        else
        {
            this.OnTargetSearch();
            _collidersAtPos = new Collider[100];
            this.SwitchState(COMBAT_AI_STATES.FIND_TARGET);
        }
   
    }
    public override void ApplyBehavior()
    {
        if(applyBehaviorCoroutine != null)
            StopCoroutine(applyBehaviorCoroutine);
        applyBehaviorCoroutine = StartCoroutine(ApplyBehaviorCoroutine());
    }

    public override void StopBehavior()
    {
        base.enabled = false;
    }

    [SerializeField]
    private Health target;

    [SerializeField]
    private float findDistance = 15f;

    [SerializeField]
    private Zombie[] zombies;

    [SerializeField]
    private COMBAT_AI_STATES currentState;

    public Action OnTargetSearch = delegate ()
    {
    };

    public Action<Transform> OnTargetFound;

}
