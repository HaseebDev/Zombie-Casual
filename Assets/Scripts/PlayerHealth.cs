using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.talent;
using UnityEngine;

//Health of our team!!!
public class PlayerHealth : Health
{
    public override void SetDamage(float _dmg, ShotType _type, string casterId,
        List<EffectHit> effectHits, Action<bool, List<EffectArmour>> responseHit, float range, Vector3 offsetScreen = default(Vector3),
        List<int> _listIgnoreEffectHits = null)
    {
        float reduceDamage = range > 5 ? ModelTalent.rangeBlock : ModelTalent.meleeBlock;
        _dmg -= reduceDamage;
        if (_dmg < 0)
            _dmg = 0;

        Debug.LogError($"Range{range}, reduce damage{reduceDamage}, damage {_dmg}");

        base.SetDamage(_dmg, _type, casterId, effectHits, responseHit, range, offsetScreen);
        Character character = (Character)this.Parent;
        if (character != null)
        {
            responseHit?.Invoke(true, character.effectArmours);
        }
    }
}