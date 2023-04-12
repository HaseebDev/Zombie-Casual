using Ez.Pooly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonBombUltimate : ActiveBombUltimate
{
    public static float ZONE_RADIUS = 3f;

    public ParticleSystem FxPoisonZone;
    private ApplyEffectZone poisonZone;

    private bool startedPoisonzone = false;
    private float timerZone = 0f;

    private void Awake()
    {

    }

    private float EffectDuration { get { return _design.Duration; } }

    public override void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        base.PreInit(skillID, isUnlocked, args);
        EffectHit fxHit = new EffectHit()
        {
            BaseDmg = GetUltimateDmg() * _design.Value,
            Duration = EffectDuration,
            OwnerID = null,
            Type = EffectType.PASSIVE_HIT_POISON,
            SkillID = EffectType.PASSIVE_HIT_POISON.ToString(),
            Value = _design.Value,
        };

        FxPoisonZone.transform.SetParent(null);


        AddressableManager.instance.SpawnObjectAsync<ApplyEffectZone>(POOLY_PREF.APPLY_EFFECT_ZONE, (zone) =>
        {
            poisonZone = zone;
            poisonZone.Initialize(fxHit, ZONE_RADIUS);
        });
        
        ResetSkill();
    }

    public override void PointerDownSkill(Vector2 pos)
    {
        base.PointerDownSkill(pos);
        ResetSkill();
    }

    public override void ResetSkill(bool hardReset = false)
    {
        poisonZone.gameObject.SetActiveIfNot(false);
        startedPoisonzone = false;
        timerZone = 0f;
        FxPoisonZone.time = 0;
        FxPoisonZone.Stop(true);

        if(hardReset)
        {
            FxPoisonZone.gameObject.SetActiveIfNot(false);
        }
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
                SpawnPoisonZone(worldPos, _design.Radius);

            });

        }


        base.PostSkill();

    }

    public void SpawnPoisonZone(Vector3 position, float radius)
    {
        FxPoisonZone.gameObject.SetActiveIfNot(true);
        FxPoisonZone.transform.position = position;
        FxPoisonZone.time = 0;
        FxPoisonZone.Play(true);

        //zone
        poisonZone.gameObject.SetActiveIfNot(true);
        poisonZone.transform.position = position;
        poisonZone.SetTriggerZone(true);

        startedPoisonzone = true;

    }

    public override void UpdateSkill(float deltaTime)
    {
        base.UpdateSkill(deltaTime);
        if (startedPoisonzone)
        {
            poisonZone?.UpdateEffectZone(deltaTime);
            timerZone += deltaTime;
            if (timerZone >= EffectDuration)
            {
                CleanUp();
            }
        }

    }

    public void CleanUp()
    {
        startedPoisonzone = false;
        poisonZone?.gameObject.SetActiveIfNot(false);
        FxPoisonZone.Stop(true);
    }
}
