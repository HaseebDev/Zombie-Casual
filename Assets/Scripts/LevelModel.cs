using System;
using Adic;
using com.datld.data;
using Framework.Interfaces;
using Framework.Managers.Event;
using UnityEngine;

public class LevelModel : MonoBehaviour, IDisposable, ITickLate, ISave, ILoad
{
    public static LevelModel instance;

    private PlayProgress _data;
    private bool _useForceLevel = false;

    public float TotalZomHP { get; private set; }

    private void Awake()
    {
        instance = this;
        Load();
    }

    public void Init(PlayProgress data, int level = 0)
    {
        _data = data;
        if (level != 0)
        {
            this.currentLevel = level;
            _useForceLevel = true;
        }

        this.currentWave = _data.CurrentWave;
    }

    public float Exp {
        get { return this.exp; }
        private set {
            this.exp = value;
            this.OnExpChanged = true;
        }
    }

    public float MaxExp {
        get { return this.maxExp; }
        private set { this.maxExp = value; }
    }

    public int CurrentLevel {
        get { return this.currentLevel; }
        set { this.currentLevel = value; }
    }

    public int CurrentWave {
        get { return this.currentWave; }
        set { this.currentWave = value; }
    }

    public void Save()
    {
        //PlayerPrefs.SetInt("currentLevel", this.CurrentLevel);
        //PlayerPrefs.SetFloat("exp", this.Exp);

        if (this.CurrentLevel > _data.CurrentLevel)
            _data.SetCurrentLevel(CurrentLevel);
    }

    public void Load()
    {
        if (!_useForceLevel)
            this.currentLevel = _data != null ? _data.CurrentLevel : 0;
    }

    public float getLevelProgressRatio()
    {
        return this.Exp / this.MaxExp;
    }

    public void AddExp(long _exp)
    {
        if (this.Exp + (float)_exp >= this.MaxExp)
        {
            //this.LevelReadyToUp();
        }

        if (this.Exp < this.MaxExp)
        {
            this.Exp += (float)_exp;

            EventSystemServiceStatic.DispatchAll(EVENT_NAME.EXP_GROW);
        }
    }

    public void DecExp(long _exp)
    {
        if (this.Exp > 0f)
        {
            this.Exp -= (float)_exp;
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.EXP_FALL);
            return;
        }

        this.Exp = 0f;
    }

    public LevelProgressData GetProgressData()
    {
        return new LevelProgressData(this.Exp, this.MaxExp, this.CurrentLevel);
    }

    public void LevelReadyToUp()
    {
        this.LevelisReadyToUp = false;
        this.Exp = 0f;
        int num = this.CurrentLevel;
        this.CurrentLevel = num + 1;
        this.MaxExp = (float)this.GetMaxExpValue();

        int currentMax = _data.MaxLevel;

        if (this.CurrentLevel > _data.CurrentLevel)
            _data.SetCurrentLevel(CurrentLevel);

        _data.MaxLevel = currentLevel > currentMax ? currentLevel : currentMax;
        SaveManager.Instance.SetDataDirty();

        if (!this.LevelisReadyToUp)
        {
            this.LevelisReadyToUp = true;
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.LEVEL_READY_UP);
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.LEVEL_COMPLETE);

            GamePlayController.instance.gameLevel.ManualHero.ResetHeroData();

        }

    }

    public void LevelUp()
    {
        GamePlayController.instance.CurrentLevel = CurrentLevel;
        EventSystemServiceStatic.DispatchAll(EVENT_NAME.LEVEL_UP);
        this.Save();
    }


    private int GetMaxExpValue()
    {
        return Mathf.CeilToInt(20f * Mathf.Pow(1.1f, (float)this.currentLevel));
    }

    public void Dispose()
    {
    }

    public void TickLate()
    {
        this.OnExpChanged = false;
    }

    public bool OnExpChanged;

    private bool LevelisReadyToUp;

    public float exp;

    public float maxExp;

    private int currentLevel;
    private int currentWave;
}