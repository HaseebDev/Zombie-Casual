using System;
using Framework.Utility;
using UnityEngine;

public abstract class UpgradeableObject : MonoBehaviour, ISave, ILoad
{
    public int Level {
        get {
            return this.upgradeableObjectData.level;
        }
        private set {
            this.upgradeableObjectData.level = Math.Min(this.upgradeableObjectData.maxLevel, value);
        }
    }

    public int MaxLevel {
        get {
            return this.upgradeableObjectData.maxLevel;
        }
    }

    public int Price {
        get {
            return this.upgradeableObjectData.price;
        }
        protected set {
            this.upgradeableObjectData.price = value;
        }
    }

    public int Value {
        get {
            return this.upgradeableObjectData.value;
        }
        protected set {
            this.upgradeableObjectData.value = value;
        }
    }

    private void Awake()
    {
    }

    protected virtual void Start()
    {
        UpgradebleObjectsController upgradebleObjectsController = this.upgradebleObjectsController;
        upgradebleObjectsController.OnUpgradebleObjectsLoaded = (Action)Delegate.Combine(upgradebleObjectsController.OnUpgradebleObjectsLoaded, new Action(this.UpgradebleObjectsController_OnUpgradebleObjectsLoaded));
        this.upgradeButton.DisableButton();
        UpgradeButton upgradeButton = this.upgradeButton;
        upgradeButton.OnClick = (Action)Delegate.Combine(upgradeButton.OnClick, new Action(this.UpgradeButton_OnClick));
        this.LoadUpgradeableObject();
    }

    protected virtual void LoadUpgradeableObject()
    {
        this.upgardebleValueGameObject = this.upgradebleObjectsController.GetUpgradebleObject(this.upgradeableObjectId);
        this.CalculatePrice();
        this.Load();
        this.UpdateButtonTexts();
        this.objectIsLoaded = true;
    }

    private void UpgradebleObjectsController_OnUpgradebleObjectsLoaded()
    {
        this.LoadUpgradeableObject();
    }

    private void UpdateButtonTexts()
    {
        this.upgradeButton.SetText("$" + Converter.CurrencyConvert((long)this.Price));
        this.upgradeButton.SetItemLvl(this.Level);
        this.UpdateUpgradebleValueText();
    }

    public virtual void UpdateUpgradebleValueText()
    {
        this.upgradeButton.SetUpgadebleValueTxt(this.Value.ToString());


    }

    private void UpgradeButton_OnClick()
    {
        this.Upgrade();
    }

    public void Save()
    {

    }

    public void Load()
    {

    }

    private void Update()
    {
        if (!this.objectIsLoaded)
        {
            return;
        }
        if (this.MaxLevelIsReached())
        {
            this.upgradeButton.EnableMaximumState();
            return;
        }
        if (this.IsAvableForBuying())
        {
            this.upgradeButton.EnableButton();
            return;
        }
        this.upgradeButton.DisableButton();
    }

    public void Upgrade()
    {
        if (this.IsAvableForBuying() && !this.MaxLevelIsReached())
        {
            CurrencyModels.instance.Golds -= (long)this.Price;
            int level = this.Level;
            this.Level = level + 1;
            this.CalculatePrice();
            this.CalculateValue();
            this.UpdateButtonTexts();
            this.SuccesedUprade();
            this.Save();
        }
    }

    public bool IsAvableForBuying()
    {
        return CurrencyModels.instance.Golds >= (long)this.Price;
    }

    public bool MaxLevelIsReached()
    {
        return this.Level == this.MaxLevel;
    }

    public abstract void CalculatePrice();

    public abstract void CalculateValue();

    private void OnDestroy()
    {
        UpgradeButton upgradeButton = this.upgradeButton;
        upgradeButton.OnClick = (Action)Delegate.Remove(upgradeButton.OnClick, new Action(this.UpgradeButton_OnClick));
    }

    [SerializeField]
    private UpgradeButton upgradeButton;

    protected GameObject upgardebleValueGameObject;

    private LocationModel locationModel;

    public Action SuccesedUprade;

    [SerializeField]
    private UpgradeableObjectData upgradeableObjectData;

    private bool objectIsLoaded;

    public int growValueK;

    [SerializeField]
    protected string upgradeableObjectId;

    //protected int locationId;

    [SerializeField]
    private UpgradebleObjectsController upgradebleObjectsController;
}
