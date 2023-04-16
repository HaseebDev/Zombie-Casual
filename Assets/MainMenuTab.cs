using System;
using System.Collections;
using System.Collections.Generic;
using UIScripts.Main_Menu;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

[Serializable]
public class HUDReminder
{
    public EnumHUD hud;
    public ReminderUI _reminder;
}


public class MainMenuTab : MonoBehaviour
{
    public static MainMenuTab Instance;

    private ButtonTab currentTab;

    [Header("Bottom Menu")] public ButtonTab btnHero;
    public ButtonTab btnEvent;
    public ButtonTab btnHome;
    public ButtonTab btnResearch;
    public ButtonTab btnShop;
    //public GameObject 

    public List<HUDReminder> _HUDreminders;

    public GameObject lockTalentPanel;

    public GameObject lockShop;

    public GameObject lockEquipment;
    // public LocalizedTMPTextParam tooltipText;

    private EnumHUD CurrentEnumHUD = EnumHUD.NONE;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SwitchHomeTab(EnumHUD.HUD_HOME);
        // tooltipText.UpdateParams(DesignHelper.GetConfigDesign(GameConstant.UNLOCK_TALENT_LEVEL).Value);
        // GetComponent<HorizontalLayoutGroup>().enabled = false;
    }

    public void ResetLayer()
    {
        lockTalentPanel.SetActive(!DesignHelper.IsRequirementAvailable(EnumHUD.HUD_TALENT.ToString()));
        lockShop.SetActive(!DesignHelper.IsRequirementAvailable(EnumHUD.HUD_SHOP.ToString()));
        lockEquipment.SetActive(!DesignHelper.IsRequirementAvailable(EnumHUD.HUD_EQUIPMENT.ToString()));
    }


    private void SwitchHomeTab(EnumHUD tab)
    {
        UnSelectCurrentTab();
        MiniGameController.instance?.SetPauseMiniGame(tab != EnumHUD.HUD_HOME);
        switch (tab)
        {
            case EnumHUD.HUD_EQUIPMENT:
                currentTab = btnHero;
                break;
            case EnumHUD.HUD_HERO:
                currentTab = btnHero;
                // OnButtonHero();
                break;
            case EnumHUD.NONE:
                currentTab = btnEvent;
                // OnButtonArmory();
                break;
            case EnumHUD.HUD_TALENT:
                currentTab = btnResearch;
                // OnButtonResearch();
                break;
            case EnumHUD.HUD_SHOP:
                currentTab = btnShop;
                // OnButtonShop();
                break;
            case EnumHUD.HUD_HOME:
                currentTab = btnHome;
                break;
            default:
                break;
        }

        currentTab.Select();
    }

    private void UnSelectCurrentTab()
    {
        currentTab?.UnSelect();
    }

    public void OnButtonHero()
    {
        var unlockRequire = DesignHelper.GetUnlockRequirementLevel(EnumHUD.HUD_EQUIPMENT.ToString());
        if (unlockRequire.Item1)
        {
            if (!IsDuplicateHUD(EnumHUD.HUD_EQUIPMENT))
            {
                MainMenuCanvas.instance.HideAllHUD(EnumHUD.HUD_EQUIPMENT);
                MainMenuCanvas.instance.ShowHUDForce(EnumHUD.HUD_EQUIPMENT, false);
                CurrentEnumHUD = EnumHUD.HUD_EQUIPMENT;
            }
        }
        else
        {
            MainMenuCanvas.instance.ShowFloatingTextNotify(LocalizeController.GetText(LOCALIZE_ID_PREF.UNLOCK_IN_LV,
                unlockRequire.Item2));
        }
    }

    public void OnButtonEvent()
    {
        MainMenuCanvas.instance.ShowFloatingTextNotify(LOCALIZE_ID_PREF.COMING_SOON.AsLocalizeString());
    }

    public void OnHomeButton()
    {
        if (!IsDuplicateHUD(EnumHUD.HUD_HOME))
        {
            MainMenuCanvas.instance.HideAllHUD(EnumHUD.HUD_HOME);
            
            MainMenuCanvas.instance.ShowHUDForce(EnumHUD.HUD_HOME, false);
            CurrentEnumHUD = EnumHUD.HUD_HOME;
        }
    }

    public void OnButtonResearch()
    {
        var unlockTalentDesign = DesignHelper.GetUnlockRequirementLevel(EnumHUD.HUD_TALENT.ToString());
        if (unlockTalentDesign.Item1)
        {
            if (!IsDuplicateHUD(EnumHUD.HUD_TALENT))
            {
                MainMenuCanvas.instance.HideAllHUD(EnumHUD.HUD_TALENT);
                MainMenuCanvas.instance.ShowHUDForce(EnumHUD.HUD_TALENT, false);
                CurrentEnumHUD = EnumHUD.HUD_TALENT;
            }
        }
        else
        {
            MainMenuCanvas.instance.ShowFloatingTextNotify(LocalizeController.GetText(LOCALIZE_ID_PREF.UNLOCK_IN_LV,
                unlockTalentDesign.Item2));
        }
    }

    public void OnButtonShop()
    {
        var unlockTalentDesign = DesignHelper.GetUnlockRequirementLevel(EnumHUD.HUD_SHOP.ToString());
        if (unlockTalentDesign.Item1)
        {
            if (!IsDuplicateHUD(EnumHUD.HUD_SHOP))
            {
                Action callback = () => HUDShop.Instance.FocusOnHighlight();

                MainMenuCanvas.instance.HideAllHUD(EnumHUD.HUD_SHOP);
                MainMenuCanvas.instance.ShowHUDForce(EnumHUD.HUD_SHOP, false, null, callback);
                CurrentEnumHUD = EnumHUD.HUD_SHOP;
            }
        }
        else
        {
            MainMenuCanvas.instance.ShowFloatingTextNotify(LocalizeController.GetText(LOCALIZE_ID_PREF.UNLOCK_IN_LV,
                unlockTalentDesign.Item2));
        }
    }

    public void OnShowHUD(EnumHUD hud)
    {
        //SwitchHomeTab(hud);
        switch (hud)
        {
            case EnumHUD.NONE:
            case EnumHUD.HUD_LOADING_GAME:
            case EnumHUD.HUD_LOADING:
            case EnumHUD.HUD_IDLE_OFFLINE_REWARD:
                Show();
                break;
            case EnumHUD.HUD_HOME:
            case EnumHUD.HUD_SHOP:
            case EnumHUD.HUD_EQUIPMENT:
            case EnumHUD.HUD_TALENT:
                SwitchHomeTab(hud);
                Show();
                break;
            case EnumHUD.HUD_HERO:
            case EnumHUD.HUD_FUSION:
            case EnumHUD.HUD_FUSION_RESULT:
            case EnumHUD.HUD_STAR_REWARD:
                Hide();
                break;
        }
    }

    public void Show()
    {
        gameObject?.SetActive(true);
    }

    public void Hide()
    {
        gameObject?.SetActive(false);
    }

    public void ShowReminder(EnumHUD hud, bool isShow, int quantity)
    {
        foreach (var hudReminder in _HUDreminders)
        {
            bool canActive = true;

            switch (hudReminder.hud)
            {
                case EnumHUD.HUD_EQUIPMENT:
                    if (lockEquipment.activeInHierarchy)
                        canActive = false;
                    break;
                case EnumHUD.HUD_SHOP:
                    if (lockShop.activeInHierarchy)
                        canActive = false;
                    break;
                case EnumHUD.HUD_TALENT:
                    if (lockTalentPanel.activeInHierarchy)
                        canActive = false;
                    break;
            }

            if (hudReminder.hud == hud)
            {
                hudReminder._reminder.Load(quantity);
                hudReminder._reminder.Show(isShow);
                if (!canActive)
                    hudReminder._reminder.Show(false);
            }
        }
    }

    private bool IsDuplicateHUD(EnumHUD type)
    {
        if (MasterCanvas.CurrentMasterCanvas.CurrentHUD != null &&
            MasterCanvas.CurrentMasterCanvas.CurrentHUD._hudType != type)
            return false;

        return CurrentEnumHUD == type;
    }
}