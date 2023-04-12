using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class BaseTutorialBehavior : BaseState
{
    public int level = 0;
    public string ShowEventName = "";
    public EnumHUD waitHUD;
    public bool isInGame = false;

    protected bool showed = false;
    protected bool completeShowEvent = false;
    public BaseTutorialType behaviorType;

    public virtual void OnEnter()
    {
        ResetState();

        behaviorType.onExit = OnExit;
        behaviorType.OnEnter();

        if (ShowEventName != "")
        {
            EventSystemServiceStatic.AddListener(this, ShowEventName, new Action(OnCompleteShowEvent));
        }
        else
            OnCompleteShowEvent();
    }

    public void ResetState()
    {
        base.OnEnter();
        showed = false;
        completeShowEvent = false;
    }
    
    protected void OnCompleteShowEvent()
    {
        completeShowEvent = true;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (HasEnoughRequire())
        {
            behaviorType.OnEnter();
            showed = true;
        }

        if (showed)
        {
            behaviorType.OnUpdate();
        }
    }

    protected bool HasEnoughRequire()
    {
        bool passLevelBool = level == 0;

        if (!passLevelBool && SaveManager.Instance.Data != null)
        {
            if (isInGame)
            {
                passLevelBool = GamePlayController.instance != null &&
                                GamePlayController.instance.CurrentLevel >= level;
            }
            else
                passLevelBool = SaveGameHelper.GetCurrentCampaignLevel() >= level;
        }

        return !showed && passLevelBool && completeShowEvent && PreHUDRequirement();
    }

    public virtual bool PreHUDRequirement()
    {
        if (waitHUD == EnumHUD.NONE)
            return true;

        if (MasterCanvas.CurrentMasterCanvas == null)
            return false;

        try
        {
            BaseHUD hud = null;
            hud = MasterCanvas.CurrentMasterCanvas.GetHUD(waitHUD);

            return hud != null && hud.gameObject.activeInHierarchy;
        }
        catch
        {
            return false;
        }
    }
}