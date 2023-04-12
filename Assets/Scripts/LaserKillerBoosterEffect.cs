using System;
using Adic;
using Framework.Managers.Ads;
using UnityEngine;

public class LaserKillerBoosterEffect : BoosterEffect
{
    protected override void Start()
    {
        this.Inject();
        base.Init(this.baseAdSystem);
    }

    private void PlayEffectSound()
    {
        AudioSystem.instance.PlaySFX(this.soundEnum);
    }

    public override void ApplyEffect()
    {
        this.PlayEffectSound();
        this.boosterIndicator.SetActive(true);
        UnityEngine.Object.FindObjectOfType<LaserKillerGate>().StartEffect(this.boosterTime);
    }

    public override void ResetEffect()
    {
        this.boosterIndicator.SetActive(false);
    }

    [Inject]
    private BaseAdSystem baseAdSystem;

    [SerializeField]
    private SFX_ENUM soundEnum;


}
