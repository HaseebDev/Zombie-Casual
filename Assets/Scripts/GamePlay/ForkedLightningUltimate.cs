using DigitalRuby.LightningBolt;
using Ez.Pooly;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkedLightningUltimate : BaseCharacterUltimate
{
    private Character hero;

    private LightningBoltScript lightningLinePrefab;

    public override void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        base.PreInit(skillID, isUnlocked, args);
        AddressableManager.instance.LoadObjectAsync<LightningBoltScript>(POOLY_PREF.LIGHTING_LINE, (light) =>
         {
             lightningLinePrefab = light;
         });
    }

    public override void PointerDownSkill(Vector2 screenPos)
    {
        base.PointerDownSkill(screenPos);
        if (hero == null)
            hero = GamePlayController.instance.gameLevel._dictCharacter[this._OwnerID];
    }

    public override bool PointerUpSkill(Vector2 screenPos, bool checkValidCast = true)
    {
        var result = base.PointerUpSkill(screenPos, checkValidCast);

        var zombie = this.DetectNearestZombie();
        if (zombie != null)
        {
            ShootLightningBullet(zombie);
        }

        base.PostSkill();

        return result;
    }

    public virtual void ShootLightningBullet(Zombie target)
    {
        if (lightningLinePrefab == null)
            return;

        hero._animationController.PlayAnimShoot();
        var startPos = hero.weapon._launcher.weaponMountPoint.position;
        LightningBoltScript light = Pooly.Spawn<LightningBoltScript>(lightningLinePrefab.transform,
                  transform.position + Vector3.up * 0.3f, Quaternion.identity, null);
        light.Initialize();
        light.PlayAnim(startPos, target.health.HitMarker.transform.position, 1.0f,
            () => { Pooly.Despawn(light.transform); });

        EffectHit fx = new EffectHit();
        fx.Type = EffectType.PASSIVE_HIT_LIGHTING;
        fx.Duration = _design.Duration;
        fx.BaseDmg = GetUltimateDmg();
        fx.Value = _design.Value;
        fx.SkillID = DesignHelper.ConvertEffectTypeToPassiveSkill(fx.Type);
        fx.OwnerID = this._OwnerID;
        fx.Number = (int)_design.Number;
        fx.Chance = 100;

        target.EffectController.AddEffect(fx);
        Timing.CallDelayed(0.2f, () =>
        {
            hero._animationController.PlayAnimIdle();
        });

    }

}
