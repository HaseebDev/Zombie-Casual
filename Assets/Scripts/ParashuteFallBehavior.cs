using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

[RequireComponent(typeof(Motor))]
public class ParashuteFallBehavior : AIBehavior
{
    private bool DoingFall = false;
    private Vector3 targetPos;

    private void Awake()
    {
        this.targetY = base.transform.position.y;
    }

    public void PreInit()
    {
        //this.gameObject.SetActiveIfNot(false);
        base.transform.position = new Vector3(base.transform.position.x, 20f, base.transform.position.z);
    }

    public void Initialize(bool instantAnim = false)
    {
        this.InstantAnim = instantAnim;
        DoingFall = false;
    }

    public override void ApplyBehavior()
    {
        if (InstantAnim)
        {
            ForceComplete();
        }
        else
        {
            DoingFall = true;
            if (this.parachuteObject != null)
                this.parachuteObject.SetActive(true);
            base.transform.position = new Vector3(base.transform.position.x, this.targetY + 14f, base.transform.position.z);
            base.enabled = true;
            targetPos = transform.position;
            targetPos.y = targetY;

            //base.transform.DOMoveY(this.targetY, this.fallParachureTime, false).SetEase(Ease.Linear).OnComplete(delegate
            //{
            //    for (int i = 0; i < this.parashureAnimators.Length; i++)
            //    {
            //        this.parashureAnimators[i].SetBool("ParachuteFallOver", true);
            //    }
            //    this.StopBehavior();
            //    HideParachure();
            //    //base.Invoke("HideParachure", this.hideParachuteTime);

            //    CompleteBehaviour(true);
            //});
        }

    }

    private void Update()
    {
        float _deltaTime = Time.deltaTime;
        if (DoingFall)
        {
            this.gameObject.SetActiveIfNot(true);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _deltaTime * 20f);
            if (transform.position == targetPos)
            {
                DoingFall = false;
                for (int i = 0; i < this.parashureAnimators.Length; i++)
                {
                    this.parashureAnimators[i].SetBool("ParachuteFallOver", true);
                }
                this.StopBehavior();
                HideParachure();
                //base.Invoke("HideParachure", this.hideParachuteTime);
                AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_PARACHUTE_LANDING);
                CompleteBehaviour(true);
            }
        }
    }

    public void CompleteBehaviour(bool playAnimComplete = true)
    {
        if (playAnimComplete)
            GameMaster.PlayEffect(COMMON_FX.FX_PARASHUTE_SLAM, transform.position, Quaternion.identity, null);
        OnBehaviourComplete?.Invoke();
    }

    public void ForceComplete()
    {
        this.gameObject.SetActiveIfNot(true);
        HideParachure();
        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        CompleteBehaviour(false);
    }

    public void SetTargetY(float targetY)
    {
        this.targetY = targetY;
    }

    public void HideParachure()
    {
        if (this.parachuteObject != null)
            this.parachuteObject.SetActive(false);
    }

    public override void StopBehavior()
    {
        base.enabled = false;
    }

    public void JumpUp(Action complete)
    {
        var targetPosY = targetY + 14f;
        base.transform.DOMoveY(targetPosY, this.fallParachureTime, false).OnComplete(() =>
        {
            complete?.Invoke();
        });
    }

    [SerializeField]
    private Animator[] parashureAnimators;

    [SerializeField]
    private GameObject parachuteObject;

    private float fallParachureTime = 0.7f;

    [SerializeField]
    private float hideParachuteTime;

    private float targetY;

    public bool InstantAnim { get; private set; }

    public Action OnBehaviourComplete;
}
