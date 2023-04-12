using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using QuickType.SkillDesign;

public interface IEffectController
{
    List<BaseEffectProvider> CurrentEffect { get; set; }
    void AddEffect(EffectHit _data);
    void RemoveEffect(EffectType type);
    void ClearAllEffects();
    void UpdateEffectController(float _deltaTime);

}

public interface IEffectObject<T>
{
    T parent { get; set; }
    EffectHit data { get; set; }
    void OnAppliedEffect(T parent, EffectHit _data);
    void OnDestroyEffect();
    void OnEffectTicked();
}

public class EffectController : IEffectController
{
    public static string EVENT_EFFECT_APPLY = "EVENT_EFFECT_APPLY";
    public static string EVENT_EFFECT_DESTROY = "EVENT_EFFECT_DESTROY";
    public static string EVENT_EFFECT_TICK = "EVENT_EFFECT_TICK";

    public EffectController()
    {
        _currentEffects = new List<BaseEffectProvider>();

        EventSystemServiceStatic.AddListener(this, EVENT_EFFECT_APPLY, new Action<EffectType, EffectHit>(EffectApplied));
        EventSystemServiceStatic.AddListener(this, EVENT_EFFECT_DESTROY, new Action<EffectType>(EffectDestroyed));
        EventSystemServiceStatic.AddListener(this, EVENT_EFFECT_TICK, new Action<EffectType>(EffectTicked));
    }

    ~EffectController()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_EFFECT_APPLY, new Action<EffectType, EffectHit>(EffectApplied));
        EventSystemServiceStatic.RemoveListener(this, EVENT_EFFECT_DESTROY, new Action<EffectType>(EffectDestroyed));
        EventSystemServiceStatic.RemoveListener(this, EVENT_EFFECT_TICK, new Action<EffectType>(EffectTicked));
    }

    public List<BaseEffectProvider> CurrentEffect { get => _currentEffects; set => _currentEffects = value; }

    //callbacks
    public Action<EffectType, EffectHit> OnAppliedEffect;
    public Action<EffectType> OnDestroyedEffect;
    public Action<EffectType> OnEffectTicked;

    //private variables
    private List<BaseEffectProvider> _currentEffects = null;

    public bool HasEffect(EffectType type)
    {
        bool result = false;

        result = _currentEffects != null && _currentEffects.FirstOrDefault(x => x._Type == type) != null;
        return result;
    }

    public void AddEffect(EffectHit _data)
    {
        var exists = _currentEffects.FirstOrDefault(x => x._Type == _data.Type);
        if (exists != null)
        {
            //exists.Initialize(this, _data);
            //exists.ApplyEffect();

        }
        else
        {
            BaseEffectProvider effect = new BaseEffectProvider();
            effect.Initialize(this, _data);
            effect.ApplyEffect();
            _currentEffects.Add(effect);
        }

    }

    public void ClearAllEffects()
    {

        _currentEffects = new List<BaseEffectProvider>();
    }

    public void RemoveEffect(EffectType type)
    {
        var exists = _currentEffects.FirstOrDefault(x => x._Type == type);
        if (exists != null)
        {
            _currentEffects.Remove(exists);
        }
    }

    public void UpdateEffectController(float _deltaTime)
    {
        var countEffect = _currentEffects.Count;
        for (int i = countEffect - 1; i >= 0; i--)
        {
            _currentEffects[i].UpdateEffect(_deltaTime);
        }
    }

    public void EffectApplied(EffectType type, EffectHit hit)
    {
        OnAppliedEffect?.Invoke(type, hit);
    }

    public void EffectDestroyed(EffectType type)
    {
        RemoveEffect(type);
        OnDestroyedEffect?.Invoke(type);
    }

    public void EffectTicked(EffectType type)
    {
        OnEffectTicked?.Invoke(type);
    }

}

[Serializable]
public class BaseEffectProvider
{
    public EffectType _Type;
    public float _Duration;
    //public float _Value;
    //public object[] _Extended;
    public SkillDesignElement _Design;

    private bool IsEffective = false;
    private float timerDuration = 0f;
    private float timerTick = 0f;

    private EffectController _parent;
    private EffectHit Data;

    public BaseEffectProvider()
    {

    }

    public virtual void Initialize(EffectController parent, EffectHit _data)
    {
        Data = _data;
        _Type = _data.Type;
        _Duration = _data.Duration;
        //_Value = _data.Value;
        //_Extended = _data.Args;

        if (Data.SkillID != null)
            _Design = DesignHelper.GetSkillDesign(Data.SkillID);
        _parent = parent;

        IsEffective = false;
        timerDuration = 0f;
        timerTick = 0f;
    }

    public virtual void PreInit()
    {

    }

    public void ApplyEffect()
    {
        IsEffective = true;
        EventSystemServiceStatic.Dispatch(_parent, EffectController.EVENT_EFFECT_APPLY, _Type, Data);
        this._parent.OnAppliedEffect(_Type, Data);
    }

    public void DestroyEffect()
    {
        IsEffective = false;
        // EventSystemServiceStatic.Dispatch(_parent, EffectController.EVENT_EFFECT_DESTROY, _Type);
        this._parent.OnDestroyedEffect(_Type);
    }

    public void UpdateEffect(float _deltaTime)
    {
        if (IsEffective)
        {
            timerDuration += _deltaTime;
            timerTick += _deltaTime;

            if (timerTick >= 0.5f)
            {
                timerTick = 0f;

                // EventSystemServiceStatic.Dispatch(_parent, EffectController.EVENT_EFFECT_TICK, _Type);
                //this._parent.OnEffectTicked(_Type);
                this._parent.OnEffectTicked(_Type);

            }

            if (timerDuration >= _Duration)
            {
                timerDuration = 0f;
                //EventSystemServiceStatic.Dispatch(_parent, EffectController.EVENT_EFFECT_TICK, _Type);
                this._parent.RemoveEffect(this._Type);
                DestroyEffect();
            }
        }

    }
}


public class BaseEffectObject<T> : IEffectObject<T>
{
    protected T _parent;
    protected EffectHit _data;
    public EffectHit data { get => _data; set => _data = value; }
    T IEffectObject<T>.parent { get => _parent; set => _parent = value; }

    protected SkillDesignElement _Design { get; set; }

    public virtual void OnAppliedEffect(T parent, EffectHit data)
    {
        this._parent = parent;
        this._data = data;
        this._Design = DesignHelper.GetSkillDesign(data.SkillID);
    }

    public virtual void OnDestroyEffect()
    {

    }

    public virtual void OnEffectTicked()
    {

    }

    public virtual void CleanUp()
    {

    }
}



