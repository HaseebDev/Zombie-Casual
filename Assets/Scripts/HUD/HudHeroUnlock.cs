using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using UnityEngine;
using UnityEngine.UI;

public class HudHeroUnlock : BaseHUD
{
    [SerializeField] private Image _heroAvatar;

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        HeroData heroData = (HeroData) args[0];
        ResourceManager.instance.GetHeroAvatar(heroData.UniqueID,_heroAvatar);
    }
}