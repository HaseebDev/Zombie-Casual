using Ez.Pooly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddOnActiveFlameBox : ActiveBombUltimate
{
    public ParticleSystem FxFireZone;
    private ApplyEffectZone FireZone;

    private bool startedFireZone = false;
    private float timerZone = 0f;
    private float EffectDuration { get { return _design.Duration; } }

    public static float ZONE_RADIUS = 3f;

    public override void PreInit(string skillID, bool isUnlocked, params object[] args)
    {
        base.PreInit(skillID, isUnlocked, args);
        _addOnItem = SaveManager.Instance.Data.GetAddOnItem(skillID);

        EffectHit fxHit = new EffectHit()
        {
            BaseDmg = GetUltimateDmg(),
            Duration = EffectDuration,
            OwnerID = null,
            Type = EffectType.PASSIVE_HIT_FIRE,
            SkillID = EffectType.PASSIVE_HIT_FIRE.ToString(),
            Value = _design.Value,
            Chance = _design.Chance
        };

        FxFireZone.transform.SetParent(null);
        FxFireZone.gameObject.SetActiveIfNot(false);
        var shape = FxFireZone.shape;
        shape.scale = Vector3.one * _design.Radius;
        //FxFireZone.transform.localScale = Vector3.one * _design.Radius;

        //  FireZone = Pooly.Spawn<ApplyEffectZone>(POOLY_PREF.APPLY_EFFECT_ZONE, Vector3.zero, Quaternion.identity, null);

        AddressableManager.instance.SpawnObjectAsync<ApplyEffectZone>(POOLY_PREF.APPLY_EFFECT_ZONE, (zone) =>
         {
             FireZone = zone;
             FireZone.Initialize(fxHit, _design.Radius);
             ResetSkill();
         });


    }

    public override void PointerDownSkill(Vector2 pos)
    {
        base.PointerDownSkill(pos);
        ResetSkill();
    }

    public override void ResetSkill(bool hardReset = false)
    {
        FireZone.gameObject.SetActiveIfNot(false);
        startedFireZone = false;
        timerZone = 0f;
        FxFireZone.time = 0;
        FxFireZone.Stop(true);

        if (hardReset)
        {
            FxFireZone.gameObject.SetActiveIfNot(false);
        }
    }

    public override void ThrowBomb(Vector3 worldPos)
    {
        //var grenade = Pooly.Spawn<BaseBomb>(POOLY_PREF.SINGLE_GRENADE, transform.position, Quaternion.identity, null);
        //if (grenade != null)
        //{
        //    Vector3 startPos = worldPos;
        //    startPos += Vector3.up * 10f;
        //    startPos += Vector3.forward * 3f;
        //    startPos += Vector3.right * 10f;


        //    grenade.transform.position = startPos;
        //    grenade.transform.localScale = Vector3.one;
        //    grenade.PreInit(this.GetUltimateDmg(), _design.Radius, this._OwnerID);
        //    grenade.Launch(worldPos, () =>
        //    {
        //        grenade.ActiveBomb();
        //        SpawnFireZone(worldPos + Vector3.up, _design.Radius);
        //    });
        //}

        SpawnFireZone(worldPos + Vector3.up, _design.Radius);

        base.PostSkill();
    }

    public void SpawnFireZone(Vector3 position, float radius)
    {
        FxFireZone.gameObject.SetActiveIfNot(true);
        FxFireZone.transform.position = position;
        FxFireZone.Play();

        //zone
        FireZone.gameObject.SetActiveIfNot(true);
        FireZone.transform.position = position;
        // FireZone.transform.localScale = Vector3.one * radius;
        FireZone.SetTriggerZone(true);

        startedFireZone = true;
    }

    public override void UpdateSkill(float deltaTime)
    {
        base.UpdateSkill(deltaTime);

        if (startedFireZone)
        {
            FireZone?.UpdateEffectZone(deltaTime);
            timerZone += deltaTime;
            if (timerZone >= EffectDuration)
            {
                ResetSkill();
            }
        }
    }

    public override void CleanUp()
    {
        startedFireZone = false;
        if (FxFireZone.gameObject != null)
        {
            GameObject.Destroy(FxFireZone.gameObject);
        }

        if (FireZone.gameObject != null)
        {
            //GameObject.Destroy(FireZone.gameObject);
            GameObject.Destroy(FireZone.gameObject);
        }
    }
}
