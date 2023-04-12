using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Coffee.UIEffects;
using DG.Tweening;

[SerializeField]
public struct AddOnInfoData
{
    public string AddOnID;
    public string title;
    public string content;
    public int number;
}

public class HUDSpecialAddOnInfo : BaseHUD
{
    public Image imgAddOn;
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtContent;
    public TextMeshProUGUI txtNumber;

    public TextMeshProUGUI txtReachedMaxAirDrop;
    public Image iconAds;
    public Button _btnEarn;

    public LoadIAPButton loadIapButton;

    public RectTransform _rectNum;
    private AddOnInfoData Data;
    private Action OnEarnAds;
    private Action OnUseItem;

    private bool ShowIdleBundle;

    [Header("PURCHASE IAP")] public RectTransform _rectIAP;
    public Image imgAutoTapper;
    public Image imgSpeedup;
    public UIShiny _uiShiny;
    public TextMeshProUGUI _txtIdleBundlePrice;

    public VerticalLayoutGroup _layout;


    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        Data = (AddOnInfoData) args[0];
        OnEarnAds = (Action) args[1];
        OnUseItem = (Action) args[2];
        ShowIdleBundle = (bool) args[3];

        ResourceManager.instance.GetUltimateSprite(Data.AddOnID, s => { imgAddOn.sprite = s; });
        txtTitle.text = Data.title.ToUpper();
        txtContent.text = Data.content;
        txtNumber.text = Data.number.ToString();

        GamePlayController.instance.SetPauseGameplay(true);
        var addonItem = SaveManager.Instance.Data.GetAddOnItem(Data.AddOnID);
        _rectNum.gameObject.SetActiveIfNot(false);
        _btnEarn.interactable = true;
        txtReachedMaxAirDrop.gameObject.SetActiveIfNot(false);
        //special airdrop here!!!
        if (Data.AddOnID == GameConstant.ADD_ON_AIR_DROP)
        {
            // iconAds.gameObject.SetActiveIfNot(false);

            var airdropDesign = DesignHelper.GetSkillDesign(Data.AddOnID);
            //check max earn today
            if (SaveManager.Instance.Data.DayTrackingData.TodayEarnAddonAirDrop >= airdropDesign.Number)
            {
                _btnEarn.interactable = false;
                txtReachedMaxAirDrop.gameObject.SetActiveIfNot(true);
            }
        }
        else
            iconAds.gameObject.SetActiveIfNot(true);

        //iap
        _rectIAP.gameObject.SetActiveIfNot(ShowIdleBundle);
        ResourceManager.instance.GetUltimateSprite(GameConstant.ADD_ON_AUTO_MANUAL_HERO,
            s => { imgAutoTapper.sprite = s; });
        
        ResourceManager.instance.GetUltimateSprite(GameConstant.ADD_ON_SPEED_UP, s =>
        {
            imgSpeedup.sprite = s;
        });
        
        _txtIdleBundlePrice.text = IAPManager.instance.GetProductPrice(IAPConstant.idle_bundle);
        loadIapButton.StartLoadCost(_txtIdleBundlePrice.text,IAPConstant.idle_bundle,_txtIdleBundlePrice);
        
        _uiShiny.Play();

        // gameObject.SetActive(!gameObject.activeSelf);
        // gameObject.SetActive(!gameObject.activeSelf);

        _layout.spacing = 5;
        DOVirtual.DelayedCall(0.1f, () => { _layout.spacing = 0; });
    }

    public void OnButtonEarnAds()
    {
        OnEarnAds?.Invoke();
        Hide();
    }

    public void OnButtonUse()
    {
        OnUseItem?.Invoke();
        Hide();
    }

    public void OnButtonPurchaseIAPBundle()
    {
        IAPManager.instance.PurchaseIAP(IAPConstant.idle_bundle, (success) =>
        {
            if(success)
            {
                SaveManager.Instance.Data.PurchaseIdleBundle();
                OnUseItem?.Invoke();
                Hide();
            }
         
        });
    }

    public override void Hide(Action<bool> hideComplete = null)
    {
        base.Hide(hideComplete);
        GamePlayController.instance.SetPauseGameplay(false);
    }
}