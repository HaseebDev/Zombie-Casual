using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyEffectZone : MonoBehaviour
{
    public EffectType effectType = EffectType.NONE;
    private SphereCollider spCollider;
    private bool ZoneStartTriggered = false;
    private float CastDuration = 0f;
    private float timerDuration = 0f;

    private void Awake()
    {
        spCollider = GetComponent<SphereCollider>();
    }

    private EffectHit _effectHit;

    public void Initialize(EffectHit effectHit, float Radius)
    {
        this._effectHit = effectHit;
        transform.localScale = Vector3.one;
        this.spCollider.radius = Radius;
        this.spCollider.enabled = false;
        this.CastDuration = _effectHit.Duration;
        this.effectType = effectHit.Type;
    }

    public void SetTriggerZone(bool value)
    {
        ZoneStartTriggered = value;
        this.spCollider.enabled = true;
        timerDuration = 0f;
        _effectHit.Duration = CastDuration;
    }

    public void UpdateEffectZone(float deltaTime)
    {
        if (ZoneStartTriggered)
        {
            timerDuration += deltaTime;
            if (timerDuration >= CastDuration)
            {
                ZoneStartTriggered = false;
                timerDuration = 0f;
            }

            if (_effectHit != null)
            {
                _effectHit.Duration -= deltaTime;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ZoneStartTriggered)
            return;

        if (GameMaster.IsZombieTag(other.tag))
        {
            var zom = other.GetComponentInParent<Zombie>();
            if (zom)
            {
                if (zom.EffectController != null)
                    zom.EffectController.AddEffect(_effectHit);
            }
        }
    }
}
