using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddOnBuffDeadShot : BaseAddOnUltimate
{
    private Sprite _spriteIcon;

    public override void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        base.PreInit(skillID, isUnlocked, args);
        ResourceManager.instance.GetUltimateSprite(skillID, s =>
        {
            _spriteIcon = s;
        });
    }

    public override bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true)
    {
        if (base.PointerUpSkill(screenPos, checkValidCast))
        {
            foreach (var hero in GamePlayController.instance.gameLevel._dictCharacter)
            {
                hero.Value.SetBoostCharacter(CHARACTER_BOOST_TYPE.INCREASE_HEADSHOT, _design.Value, _design.Duration, _spriteIcon);
            }

            return true;
        }
        return false;
    }
}
