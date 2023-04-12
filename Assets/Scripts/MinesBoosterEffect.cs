using System;
using Adic;
using Framework.Managers.Ads;
using UnityEngine;

public class MinesBoosterEffect : BoosterEffect
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
        UnityEngine.Object.FindObjectOfType<MinefieldController>().ApplyEffect();
    }

    public override void ResetEffect()
    {
        this.boosterIndicator.SetActive(false);
    }

    [Inject]
    private BaseAdSystem baseAdSystem;

    private MinefieldController minesBoosterEffect;

    [SerializeField]
    private SFX_ENUM soundEnum;

}
