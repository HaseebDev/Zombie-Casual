using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameContext : BaseSystem<MainGameContext>
{
    public static MainGameContext instance;

    public GamePlayController gameplayController;

    private void Awake()
    {
        instance = this;
    }

    //private void Start()
    //{
    //    Initialize();
    //}

    public override void Initialize(params object[] pars)
    {
        base.Initialize(pars);
        gameplayController.Initialize(GameMaster.instance.currentMode, pars[0]);
    }

    private void Update()
    {
        float _deltaTime = Time.deltaTime;
        gameplayController?.UpdateSystem(_deltaTime);
    }

}
