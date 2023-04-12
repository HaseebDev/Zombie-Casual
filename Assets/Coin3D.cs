using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Ez.Pooly;
using MEC;

public class Coin3D : MonoBehaviour
{
    public CurrencyType CurrencyType;

    public void PlayAnimCollectInGame(Action OnComplete, Action PlaySFX)
    {
        float timeFX = 0.5f;
        Vector2 screenPos = Vector2.zero;
        var cam = GamePlayController.instance.GetMainCamera();
        if (cam != null)
            screenPos = cam.WorldToScreenPoint(transform.position);
        Vector2 endPos = InGameCanvas.instance._gamePannelView.iconCurrency.transform.position;
        ResourceManager.instance.GetCurrencySprite(CurrencyType, sp =>
        {
            if (sp != null)
            {
                InGameCanvas.instance.ShowMovingSprite(sp, screenPos, endPos, Vector2.one * 80, timeFX, false, () =>
                {
                    EventSystemServiceStatic.DispatchAll(EVENT_NAME.RESET_GAME_HUD);
                    OnComplete?.Invoke();
                });

                Timing.CallDelayed(timeFX * 0.7f, () => { PlaySFX?.Invoke(); });
            }

        });
    }

    public void PlayAnimCollectCustom(Camera cam, Vector2 targetScreenPos, Action OnComplete, float timeFx = 0.5f,
        Ease easeType = Ease.Linear, Transform parent = null)
    {
        Vector2 screenPos = cam.WorldToScreenPoint(transform.position);
        //Vector2 endPos = InGameCanvas.instance._gamePannelView.iconCurrency.transform.position;
        ResourceManager.instance.GetCurrencySprite(CurrencyType,
            sp =>
            {
                MainMenuCanvas.instance.ShowMovingSprite(sp, screenPos, targetScreenPos, Vector2.one * 80, timeFx,
                    false, () => { OnComplete?.Invoke(); }, Ease.InOutSine, parent);
            });
    }
}