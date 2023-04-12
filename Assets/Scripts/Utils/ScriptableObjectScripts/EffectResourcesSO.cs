using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class EffectDefine
{
    public COMMON_FX type;
    public AutoDespawnParticles particles;
}

[CreateAssetMenu(fileName = "EffectResources", menuName = "ScriptableObjects/EffectResourcesSO", order = 1)]
public class EffectResourcesSO : ScriptableObject
{
    public List<EffectDefine> listEffects;

    public AutoDespawnParticles GetEffect(COMMON_FX _type)
    {
        var def = listEffects.FirstOrDefault(x => x.type == _type);
        if (def != null)
            return def.particles;
        else
            Debug.LogError($"GetEffect failed {_type}");

        return null;
    }

}
