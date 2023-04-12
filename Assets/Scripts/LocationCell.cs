using System;
using Framework.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LocationCell : ItemCellBase
{
    public override void Start()
    {
        base.Start();
        this.changeLocBtn.onClick.AddListener(new UnityAction(this.ChangeLoc));
    }

    public void Init(LocationModel _locationModel, LevelModel _levelModel, CurrencyModels _coinsModel)
    {
        this.locationModel = _locationModel;
        this.coinsModel = _coinsModel;
        this.changeLocBtn.enabled = false;
        this.Load();
        if (this.locationModel.maxUnlockedLocation >= this.locationId)
        {
            this.changeLocBtn.enabled = true;
            this.isBuyed = true;
        }
        this.UpdateLocationChooseButton();
        this.UpdateValues();
    }

    public void UpdateLocationChooseButton()
    {
        if (this.LocationIsCurrent())
        {
            this.isBuyed = true;
            this.choosenVisual.SetActive(true);
            return;
        }
        this.changeLocBtn.enabled = true;
        this.choosenVisual.SetActive(false);
    }

    public bool LocationIsCurrent()
    {
        return this.locationModel.currentLocation == this.locationId;
    }

    private string getSaveName()
    {
        return string.Format("lockLikesInSec{0}", this.locationId);
    }

    private void Save()
    {
        PlayerPrefs.SetString(this.getSaveName(), this.lockLikesInSec.ToString());
    }

    private void Load()
    {
        this.lockLikesInSec = long.Parse(PlayerPrefs.GetString(this.getSaveName(), "0"));
    }

    public void SetLocationInSecValue(long _locInSec)
    {
        this.lockLikesInSec = _locInSec;
        this.Save();
        this.UpdateValues();
    }

    private void ChangeLoc()
    {
        if (this.LocationIsCurrent())
        {
            return;
        }
        this.locationModel.ChangeLocation(this.locationId);
    }

    protected override void SetupLevelGate()
    {
        if (this.unlockLevel > LevelModel.instance.CurrentLevel)
        {
            this.changeLocBtn.enabled = false;
            this.lvlGate.UpdateBuyGate((long)this.unlockLevel, true);
            return;
        }
        this.lvlGate.UpdateBuyGate((long)this.unlockLevel, false);
    }

    public override void BuyCell()
    {
        if (this.unlockCost <= this.coinsModel.Golds)
        {
            this.coinsModel.Golds -= this.unlockCost;
            this.isBuyed = true;
            this.locationModel.UnlockLocation(this.locationId);
            this.changeLocBtn.enabled = true;
            this.UpdatePayGate();
        }
    }

    protected override void UpdatePayGate()
    {
        if (!this.isBuyed)
        {
            this.changeLocBtn.enabled = false;
        }
        this.payGate.UpdateBuyGate(this.unlockCost, !this.isBuyed);
    }

    public override void UpdateValues()
    {
        this.itemNameTxt.text = this.locationName;
        this.UpdateGates();
        this.itemDescTxt.text = string.Format("${0} / sec", Converter.CurrencyConvert(this.lockLikesInSec));
        this.SetupPayGateButton();
    }

    protected override void SetupPayGateButton()
    {
        if (!this.isBuyed)
        {
            this.payGate.UpdateUnlockCellButton(this.unlockCost, this.coinsModel.Golds);
        }
    }

    protected override void UpdateGates()
    {
        this.SetupLevelGate();
        if (this.unlockLevel <= LevelModel.instance.CurrentLevel && this.unlockCost > 0L)
        {
            this.UpdatePayGate();
        }
    }

    private void OnDestroy()
    {
        this.changeLocBtn.onClick.RemoveListener(new UnityAction(this.ChangeLoc));
    }

    private bool isInitiated;

    [SerializeField]
    private int locationId;

    [SerializeField]
    private string locationName;

    private long lockLikesInSec;

    [SerializeField]
    private Button changeLocBtn;

    private LocationModel locationModel;

    [SerializeField]
    private GameObject choosenVisual;
}
