using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class AddOnCategoryButton : ShopCategoryButton
{
    [SerializeField] private ReminderUI _reminderUi;
    [SerializeField] private GameObject _lock;
    [SerializeField] private Button _button;

    protected override void OnShopCategoryBtnClicked()
    {
        base.OnShopCategoryBtnClicked();
        UpdateReminder();
    }

    private void UpdateReminder()
    {
        var newAddOn = ReminderManager.HasNewAddOn();
        _reminderUi.Show(false);

        foreach (var adsOn in InGameCanvas.instance._gamePannelView.ListAddOnButton)
        {
            // Debug.LogError("AFTER" + adsOn.Design.SkillId);
            bool has = newAddOn.Item2.Find(x => x.ItemID == adsOn.Design.SkillId) != null;
            adsOn.ShowReminder(has);
        }

        ReminderManager.SaveCurrentAddOnState();
    }


    private void Start()
    {
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.LEVEL_UP, new Action(UpdateLock));
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.ADD_ADON, new Action(UpdateReminderFromAirDrop));
        UpdateLock();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.LEVEL_UP, new Action(UpdateLock));
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.ADD_ADON, new Action(UpdateReminderFromAirDrop));
    }


    private void UpdateReminderFromAirDrop()
    {
        var newAddOn = ReminderManager.HasNewAddOn();
        foreach (var adsOn in InGameCanvas.instance._gamePannelView.ListAddOnButton)
        {
            // Debug.LogError("AFTER" + adsOn.Design.SkillId);
            bool has = newAddOn.Item2.Find(x => x.ItemID == adsOn.Design.SkillId) != null;
            adsOn.ShowReminder(has);
            adsOn.ResetItem();
        }
    }

    private void UpdateLock()
    {
        if (!DesignHelper.IsRequirementAvailable(UnlockRequireId.TAB_ADD_ON))
        {
            _button.interactable = false;
            _lock.SetActive(true);
        }
        else
        {
            _button.interactable = true;
            _lock.SetActive(false);
        }
    }

    public void ShowLockTabAddonText()
    {
        var isUnlockTemp = DesignHelper.GetUnlockRequirementLevel(UnlockRequireId.TAB_ADD_ON);
        bool isUnlock = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).MaxLevel >=
                        (isUnlockTemp.Item2 - 1);

        MasterCanvas.CurrentMasterCanvas.ShowFloatingTextNotify(
            LocalizeController.GetText(LOCALIZE_ID_PREF.UNLOCK_IN_LV, isUnlockTemp.Item2));
    }
}