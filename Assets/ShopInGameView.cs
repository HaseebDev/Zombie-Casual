using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using DG.Tweening;
using QuickType.Shop;

[Serializable]
public class InGameShopCategory
{
    public INGAME_SHOP type;
    public RectTransform _rect;
}

public class ShopInGameView : MonoBehaviour
{
    public ScrollRect _scrollRect;
    public MyPopup popup;

    public List<InGameShopCategory> _listShopCategory;
    private ShopItemUI _currentActiveShining;

    public List<BaseShop> Shops;
    private bool _isPausedBefore = false;

    public void ScrollTo(INGAME_SHOP type = INGAME_SHOP.NONE)
    {
        if (type == INGAME_SHOP.NONE)
            return;

        var target = _listShopCategory.FirstOrDefault(x => x.type == type);
        if (target != null)
            _scrollRect.content.localPosition = _scrollRect.GetSnapToPositionToBringChildIntoView(target._rect);
    }

    private void OnEnable()
    {
        _currentActiveShining?.ActiveShiny(false);
        
        foreach (var VARIABLE in Shops)
        {
            VARIABLE.ResetLayer();
        }
    }

    public void HighLightPackage(ShopDesignElement shopDesignElement)
    {
        DOVirtual.DelayedCall(0.1f, () =>
        {
            if (_currentActiveShining != null)
            {
                if (shopDesignElement.Id == _currentActiveShining.ShopDesignElement.Id)
                {
                    _currentActiveShining?.ActiveShiny(true);
                    return;
                }
            }

            var allShopItems = GetComponentsInChildren<ShopItemUI>().ToList();

            _currentActiveShining = allShopItems.Find(x => x.ShopDesignElement.Id == shopDesignElement.Id);
            _currentActiveShining?.ActiveShiny(true);
        });
    }

    public void Show(INGAME_SHOP targetCategory)
    {
        _isPausedBefore = GamePlayController.instance.IsPausedGame;
        
        GamePlayController.instance.SetPauseGameplay(true);
        gameObject.SetActive(true);
        ScrollTo(targetCategory);
    }

    public void Hide()
    {
        if(!_isPausedBefore)
            GamePlayController.instance.SetPauseGameplay(false);
        popup.Hide();
    }
}
