using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Framework.Interfaces;
using UnityEngine;

public enum CurrencyType
{
    NONE,
    GOLD,
    DIAMOND,
    TOKEN,
    PILL,
    WEAPON_SCROLL,
    ARMOUR_SCROLL
}

public class CurrencyModels : MonoBehaviour, ITickLate, ISave, ILoad
{
    private ObscuredLong golds;
    private ObscuredLong diamonds;
    private ObscuredLong tokens;
    private ObscuredLong pills;
    private ObscuredLong weaponScrolls;
    private ObscuredLong armourSrolls;

    public static CurrencyModels instance;
    private Action<CurrencyType, long> onAddCurrency;

    private void Awake()
    {
        instance = this;
    }

    public void AddCallback(Action<CurrencyType, long> callBack)
    {
        onAddCurrency += callBack;
    }

    public void RemoveCallback(Action<CurrencyType, long> callBack)
    {
        if (onAddCurrency != null) onAddCurrency -= callBack;

    }

    #region getter / setters

    public ObscuredLong Golds {
        get { return this.golds; }
        set {
            this.golds = value;
            SaveManager.Instance.Data.Inventory.TotalGold = this.golds;
            SaveManager.Instance.SetDataDirty();
            this.OnStarsChanged = true;
            onAddCurrency?.Invoke(CurrencyType.GOLD, value);
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_CURRENCY, CurrencyType.GOLD);
        }
    }

    public ObscuredLong Diamonds {
        get { return this.diamonds; }
        set {
            this.diamonds = value;
            Debug.LogError($"Update diamond {value}");

            // SaveManager.Instance.Data.Inventory.TotalDiamond = this.diamonds;
            SaveManager.Instance.SetDataDirty();
            this.OnStarsChanged = true;
            onAddCurrency?.Invoke(CurrencyType.DIAMOND, value);
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_CURRENCY, CurrencyType.DIAMOND);

        }
    }

    public ObscuredLong Tokens {
        get { return this.tokens; }
        set {
            this.tokens = value;
            SaveManager.Instance.Data.Inventory.TotalToken = this.tokens;
            SaveManager.Instance.SetDataDirty();
            this.OnStarsChanged = true;
            onAddCurrency?.Invoke(CurrencyType.TOKEN, value);
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_CURRENCY, CurrencyType.TOKEN);
        }
    }

    public ObscuredLong Pills {
        get { return this.pills; }
        set {
            this.pills = value;
            SaveManager.Instance.Data.Inventory.TotalPill = this.pills;
            SaveManager.Instance.SetDataDirty();
            this.OnStarsChanged = true;
            onAddCurrency?.Invoke(CurrencyType.PILL, value);
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_CURRENCY, CurrencyType.PILL);
        }
    }

    public ObscuredLong WeaponScrolls {
        get { return this.weaponScrolls; }
        set {
            this.weaponScrolls = value;
            SaveManager.Instance.Data.Inventory.TotalWeaponScroll = this.weaponScrolls;
            SaveManager.Instance.SetDataDirty();
            this.OnStarsChanged = true;
            onAddCurrency?.Invoke(CurrencyType.WEAPON_SCROLL, value);
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_CURRENCY, CurrencyType.WEAPON_SCROLL);

        }
    }

    public ObscuredLong ArmourScrolls {
        get { return this.armourSrolls; }
        set {
            this.armourSrolls = value;
            SaveManager.Instance.Data.Inventory.TotalArmourScroll = this.armourSrolls;
            SaveManager.Instance.SetDataDirty();
            this.OnStarsChanged = true;
            onAddCurrency?.Invoke(CurrencyType.ARMOUR_SCROLL, value);
            EventSystemServiceStatic.DispatchAll(EVENT_NAME.UPDATE_CURRENCY, CurrencyType.ARMOUR_SCROLL);
        }
    }

    public ObscuredLong CurrentGameCurrency {
        get { return GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE ? Golds : Tokens; }
        set {
            if (GamePlayController.instance.gameMode == GameMode.CAMPAIGN_MODE)
                Golds = value;
            else
                Tokens = value;
        }
    }

    #endregion

    private bool loaded = false;

    public void Load()
    {
        if (loaded)
            return;
        loaded = true;
        this.golds = SaveManager.Instance.Data.Inventory.TotalGold;
        this.diamonds = SaveManager.Instance.Data.Inventory.TotalDiamond;
        this.tokens = SaveManager.Instance.Data.Inventory.TotalToken;
        this.pills = SaveManager.Instance.Data.Inventory.TotalPill;
        this.weaponScrolls = SaveManager.Instance.Data.Inventory.TotalWeaponScroll;
        this.armourSrolls = SaveManager.Instance.Data.Inventory.TotalArmourScroll;
    }

    public void AddCurrency(CurrencyType type, long value)
    {
        switch (type)
        {
            case CurrencyType.GOLD:
                Golds += value;
                break;
            case CurrencyType.DIAMOND:
                Diamonds += value;
                break;
            case CurrencyType.TOKEN:
                Tokens += value;
                break;
            case CurrencyType.PILL:
                Pills += value;
                break;
            case CurrencyType.WEAPON_SCROLL:
                WeaponScrolls += value;
                break;
            case CurrencyType.ARMOUR_SCROLL:
                ArmourScrolls += value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_GAME_HUD);
    }

    public bool IsEnough(CurrencyType type, float value)
    {
        switch (type)
        {
            case CurrencyType.GOLD:
                return Golds >= value;
            case CurrencyType.DIAMOND:
                return Diamonds >= value;
            case CurrencyType.TOKEN:
                return Tokens >= value;
            case CurrencyType.PILL:
                return Pills >= value;
            case CurrencyType.WEAPON_SCROLL:
                return weaponScrolls >= value;
            case CurrencyType.ARMOUR_SCROLL:
                return armourSrolls >= value;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public long GetValueFromEnum(CurrencyType type)
    {
        switch (type)
        {
            case CurrencyType.NONE:
                return 0;
            case CurrencyType.GOLD:
                return Golds;
            case CurrencyType.DIAMOND:
                return Diamonds;
            case CurrencyType.TOKEN:
                return Tokens;
            case CurrencyType.PILL:
                return Pills;
            case CurrencyType.WEAPON_SCROLL:
                return WeaponScrolls;
            case CurrencyType.ARMOUR_SCROLL:
                return ArmourScrolls;
        }

        return 0;
    }

    public void Save()
    {
        SaveManager.Instance.SetDataDirty();
    }

    public void TickLate()
    {
        this.OnStarsChanged = false;
    }

    public bool OnStarsChanged;

    public void RemoveAllCallback()
    {
        onAddCurrency = null;
    }
}