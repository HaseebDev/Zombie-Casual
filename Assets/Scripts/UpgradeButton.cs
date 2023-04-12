using System;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] protected UIShiny _shiny;
    [SerializeField] protected Image _targetImage;

    private void Awake()
    {
        buttonIsDisabled = true;
    }

    private void OnEnable()
    {
        this.disableButton.onClick.AddListener(new UnityAction(this.OnButtonClicked));
    }

    private void OnDisable()
    {
        this.disableButton.onClick.RemoveListener(new UnityAction(this.OnButtonClicked));
    }

    public virtual void ActionOnClicked()
    {
    }

    private void OnButtonClicked()
    {
        ActionOnClicked();

        Action onClick = this.OnClick;
        if (onClick == null)
        {
            return;
        }

        onClick();
    }

    public void DisableButton()
    {
        _shiny.enabled = false;
        //_targetImage.color = disableButton.colors.disabledColor;

        // if (!this.buttonIsDisabled)
        // {
        // this.disableButtonBgInActive.SetActive(true);
        // this.disableButtonBgActive.SetActive(false);
        // this.disableButton.interactable = false;
        this.buttonIsDisabled = true;
        // }
    }

    public virtual void EnableButton()
    {
        _shiny.enabled = true;
        //_targetImage.color = disableButton.colors.normalColor;

        // if (this.buttonIsDisabled)
        // {
        // this.disableButtonBgInActive.SetActive(false);
        // this.disableButtonBgActive.SetActive(true);
        // this.disableButton.interactable = true;
        this.buttonIsDisabled = false;
        // }
    }

    public virtual void EnableMaximumState()
    {
        if (!this.maximumIsReached)
        {
            this.SetText("MAX");
            this.DisableButton();
            this.maximumIsReached = true;
        }
    }

    public void SetItemLvl(int _level)
    {
        this.itemLevel = _level;
        this.lvlTxtValue.text = _level.ToString();
    }

    public virtual void SetUpgadebleValueTxt(string _upgradableValueText)
    {
        if (this.upgradeValueText == null)
        {
            return;
        }

        this.upgradeValueText.text = _upgradableValueText;
    }

    public void SetText(string _text)
    {
        this.buttonTxtValue.text = _text;
    }

    protected virtual void OnDestroy()
    {
        this.disableButton.onClick.RemoveListener(new UnityAction(this.OnButtonClicked));
    }

    [SerializeField] private Button disableButton;

    [SerializeField] private GameObject disableButtonBgActive;

    [SerializeField] private GameObject disableButtonBgInActive;

    public LocalizedTMPTextUI buttonTxtValue;

    [SerializeField] protected LocalizedTMPTextUI upgradeValueText;

    [SerializeField] protected LocalizedTMPTextUI lvlTxtValue;

    protected int itemLevel;

    private Color defButtonColor;

    private bool buttonIsDisabled;

    protected bool maximumIsReached;

    public Action OnClick;

    public virtual void ResetData()
    {
    }
}