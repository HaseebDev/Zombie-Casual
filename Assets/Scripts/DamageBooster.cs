using System;
using UnityEngine;

public class DamageBooster : CountdownEffect
{
    private void Awake()
    {
        //this.boostDamageEffect = base.GetComponent<IBoostDamageEffect>();
        //this.damageBoosterEffect.SetActive(false);
    }

    public override void ApplyEffect()
    {
        //this.boostDamageEffect.BoostDamageEffect(true);
        //this.damageBoosterEffect.SetActive(true);
    }

    public override void ResetEffect()
    {
        //this.boostDamageEffect.BoostDamageEffect(false);
        //this.damageBoosterEffect.SetActive(false);
    }

    private IBoostDamageEffect boostDamageEffect;

    [SerializeField]
    private GameObject damageBoosterEffect;
}
