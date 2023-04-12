using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickTutorialAddHero : BaseTutorialBehavior
{
    public string heroID;

    private bool createClone = false;

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (createClone)
            return;

        var allHeroButtons = FindObjectsOfType<ChangeTeamHeroButton>().ToList();
        foreach (var heroBtn in allHeroButtons)
        {
            if (heroBtn.HeroData.UniqueID == heroID)
            {
                var buttonId = heroBtn.gameObject.AddComponent<TutorialButtonID>();
                buttonId.id = heroID;
                createClone = true;
                return;
            }
        }
    }
}