using DG.Tweening;
using Ez.Pooly;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObstacleProtectGate : BaseMapObstacle
{
    public List<MAP_NAME> mapNames = new List<MAP_NAME>();
    private Material _mat;

    [Header("Materials")] public Material _matNormal;
    public Material _matTransparent;

    [Header("Effects")] public List<ParticleSystem> _effectsDeploy;

    public List<ObstacleColorByMap> obstacleColors;
    
    private bool isDeploying;
    
    protected override void Awake()
    {
        base.Awake();
        _health.IsShowFloatingDmg = false;
    }

    public void SetStateDeploy(bool isDeploy)
    {
        isDeploying = isDeploy;
        _meshRender.material = isDeploy ? _matTransparent : _matNormal;
    }

    public void AnimDeployAtPosition(Vector3 pos, bool changeMaterial = true)
    {
        if (changeMaterial)
            SetStateDeploy(false);

        var startPos = pos + Vector3.up * 5f;
        transform.position = startPos;
        transform.DOMove(pos, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            for (int i = _effectsDeploy.Count - 1; i >= 0; i--)
            {
                _effectsDeploy[i].gameObject.SetActiveIfNot(true);
                _effectsDeploy[i].time = 0;
                _effectsDeploy[i].Play(true);
            }
        });
    }

    public void HideAllParticle()
    {
        for (int i = _effectsDeploy.Count - 1; i >= 0; i--)
        {
            _effectsDeploy[i].gameObject.SetActiveIfNot(false);
        }
    }

    public override void Initialize(float MaxHP)
    {
        if (obstacleColors != null && obstacleColors.Count > 0)
        {
            foreach (var VARIABLE in obstacleColors)
            {
                Color finalColor = Color.white;
                
                var colorData = VARIABLE.colors.Find(x => x.mapName == DesignHelper.GetCurrentMapByLevel());
                if (colorData == null)
                {
                    colorData =  VARIABLE.colors.Find(x => x.mapName == MAP_NAME.NONE);
                }

                if (colorData != null)
                {
                    finalColor = colorData.color;
                }

                VARIABLE.meshRenderer.material.color = finalColor;
                _colorNormal = finalColor;
            }
        }
        
        base.Initialize(MaxHP);

       
        
        // for (int i = _effectsDeploy.Count - 1; i >= 0; i--)
        // {
        //     _effectsDeploy[i].gameObject.SetActiveIfNot(false);
        // }
    }
    
    public override void OnDie(bool skipAnimDead)
    {
        if (!skipAnimDead)
            GameMaster.PlayEffect(COMMON_FX.FX_EXPLODE_METAL, transform.position, Quaternion.identity);
        
        Pooly.Despawn(this.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        PushbackAndBlock(other);
    }

    private void OnTriggerStay(Collider other)
    {
        PushbackAndBlock(other);
    }

    [Button("FindColor")]
    public void FindObstacleColor()
    {
        obstacleColors = new List<ObstacleColorByMap>();
        
        var allChild = transform.GetComponentsInChildren<Transform>();
        foreach (var VARIABLE in allChild)
        {
            var comp = VARIABLE.GetComponent<ObstacleColorByMap>();
            if (comp != null)
            {
                comp.meshRenderer = comp.GetComponent<MeshRenderer>();
                obstacleColors.Add(comp);
            }
        }
    }
}