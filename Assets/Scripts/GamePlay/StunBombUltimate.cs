using Ez.Pooly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class StunBombUltimate : ActiveBombUltimate
{
    public ParticleSystem _parStun;

    public override void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        base.PreInit(skillID, isUnlocked, args);
        _parStun.time = 0;
        _parStun.transform.SetParent(null);
        _parStun.gameObject.SetActiveIfNot(false);
    }

    public override void PointerDownSkill(Vector2 screenPos)
    {
        base.PointerDownSkill(screenPos);
    }

    public override void ThrowBomb(Vector3 worldPos)
    {
        var grenade = Pooly.Spawn<BaseBomb>(POOLY_PREF.SINGLE_GRENADE, transform.position, Quaternion.identity, null);
        if (grenade != null)
        {
            grenade.transform.localScale = Vector3.one;
            grenade.PreInit(this.GetUltimateDmg(), _design.Radius, this._OwnerID);
            grenade.Launch(worldPos, () =>
            {
                grenade.ActiveBomb();
                StunAllZombieExceptBoss();

                _parStun.transform.position = worldPos + Vector3.up * 1.5f;
                _parStun.gameObject.SetActiveIfNot(true);
                _parStun.time = 0;
                _parStun.Play(true);
                Timing.CallDelayed(_design.Duration, () =>
                {
                    _parStun.Stop(true);
                });
            });
        }
        base.PostSkill();
    }

    public void StunAllZombieExceptBoss()
    {
        var zombies = GamePlayController.instance._waveController.GetZombiesOnThisWawe();
        foreach (var zom in zombies)
        {
            if (!zom.IsBoss)
            {
                EffectHit fx = new EffectHit();
                fx.Type = EffectType.PASSIVE_HIT_STUN;
                fx.Duration = _design.Duration;
                fx.SkillID = DesignHelper.ConvertEffectTypeToPassiveSkill(fx.Type);
                fx.OwnerID = this._OwnerID;
                zom.EffectController.AddEffect(fx);
            }
        }
    }
}
