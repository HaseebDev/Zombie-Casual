using System;
using UnityEngine;

public class ShopCategoryManager : MonoBehaviour
{
    private void Start()
    {
        this.OnCategoryChanged(ShopCategory.HEROES);
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.LEVEL_UP, new Action(OnLevelUp));
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.LEVEL_UP, new Action(OnLevelUp));
    }

    private void OnLevelUp()
    {
        this.OnCategoryChanged(ShopCategory.HEROES);
    }
    

    public void ChangeModeBtnClicked(ShopCategory shopCategory)
    {
        this.OnCategoryChanged(shopCategory);
    }

    public Action<ShopCategory> OnCategoryChanged;

    public void OnButtonFreeAddOn()
    {
        var _airDropRewards = DesignHelper.GetListDailyFreeAdsOnDesign();
        if (_airDropRewards != null)
        {
            AdsManager.instance.ShowAdsReward((complete, amount) =>
            {
                if (complete)
                {
                    TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_REWARD_SIMPLE, false, null, _airDropRewards, true, false);
                    InGameCanvas.instance?._gamePannelView.ResetAddOnPannel();
                }
            });
        }
    }
}