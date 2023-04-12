using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Doozy.Engine.UI;
using System;
using Doozy.Engine.Extensions;
using Ez.Pooly;
using MEC;
using Spine;

public enum EnumHUD
{
    NONE,
    HUD_LOADING_GAME,
    HUD_HOME,
    HUD_LOADING,
    HUD_SHOP,
    HUD_IDLE_OFFLINE_REWARD,
    HUD_EQUIPMENT,
    HUD_COMPLETE_LEVEL_REWWARD,
    HUD_TALENT,
    HUD_HERO,
    HUD_NOT_ENOUGH,
    HUD_ASCEND,
    HUD_REWARD_SIMPLE,
    HUD_FUSION,
    HUD_NOTIFY,
    HUD_REVIVE_DEFEAT,
    HUD_AIR_DROP,
    HUD_SIMULATE_ADS,
    HUD_MERGE_CONFLICT,
    HUD_NO_INTERNET,
    HUD_SHARD_CHAR,
    HUD_ADD_ON_INFO,
    HUD_IDLE_REWARD,
    HUD_ASK_EXIT,
    HUD_PURCHASE_HERO_INGAME,
    HUD_HERO_UNLOCK,
    HUD_STAR_REWARD,
    HUD_IDLE_SKIP_LEVEL,
    HUD_DAILY_FREE_STUFF,
    HUD_MAIL,
    HUD_SETTING,
    HUD_SELECT_CAMPAGIN_LEVEL,
    HUD_PREVIEW_CAMPAIGN_LEVEL,
    HUD_POWER_SAVING_MODE,
    HUD_REMOVE_ADS,
    HUD_FUSION_RESULT,
	HUD_MISSION,
    HUD_RATING
}

public class BaseHUD : MonoBehaviour
{
    protected CanvasGroup _canvasGroup;
    protected Canvas _canvas;
    protected UIView _uiView;

    public EnumHUD _hudType;
    protected IParentHud _parentHUD;

    protected bool _isAddedStack = false;
    protected Action<bool> ShowCallback;
    protected Action<bool> HideCallback;

    public static readonly float TIME_FX = .2f;
    protected bool refreshLastLayer = true;
    protected bool isInit = false;
    private bool _hided = false;

    public virtual void Awake()
    {
        _canvasGroup = this.GetComponent<CanvasGroup>();
        _canvas = this.GetComponent<Canvas>();
        _uiView = this.GetComponent<UIView>();
    }

    public virtual void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        _hided = false;
        _parentHUD = _parent;
        _hudType = type;

        if (_uiView)
        {
            _uiView.OnVisibilityChanged.AddListener(
                new UnityEngine.Events.UnityAction<float>(OnUIViewVisibilityProgress));
        }

        ResetLayers();
    }

    protected void OnUIViewVisibilityProgress(float progress)
    {
        if (progress >= 1)
        {
            ShowCallback?.Invoke(true);
        }
        else if (progress <= 0)
        {
            HideCallback?.Invoke(true);
        }
    }

    public virtual void Init()
    {
        isInit = true;
    }

    public virtual void Show(Action<bool> showComplete = null, bool addStack = true)
    {
        ShowCallback = showComplete;
        gameObject.SetActive(true);

        if (_uiView)
        {
            _uiView.Hide(true);
            _uiView.Show(false);
        }
        else
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0;
                _canvasGroup.DOFade(1, TIME_FX).SetUpdate(true).OnComplete(() => { ShowCallback?.Invoke(true); });
            }
        }

        _isAddedStack = addStack;
        //if (_isAddedStack)
        //    _parentHUD.MenuStack.AddMenu(_hudType);

        this.GetComponent<RectTransform>().FullScreen(true);

        _parentHUD.EnqueueStacks(_hudType);
    }

    protected void LoadAddressableAsset(Action callback)
    {
        List<string> prefabId = GetPreInitAsset();
        if (prefabId == null || prefabId.Count == 0)
        {
            callback?.Invoke();
            return;
        }

        bool needToLoad = false;
        foreach (var id in prefabId)
        {
            if (!Pooly.IsContainKey(id))
            {
                needToLoad = true;
                break;
            }
        }

        if (needToLoad)
            StartCoroutine(LoadAddressableAssetCoroutine(callback));
    }

    protected IEnumerator LoadAddressableAssetCoroutine(Action callback)
    {
        bool showLoading = true;

        List<string> prefabId = GetPreInitAsset();
        MasterCanvas.CurrentMasterCanvas.ShowHUDLoadingView(true);

        int total = 0;

        foreach (var id in prefabId)
        {
            AddressableManager.instance.LoadFromAddressable(id, () =>
            {
                total++;
                if (total == prefabId.Count)
                {
                    showLoading = false;
                }
            });
        }

        while (showLoading)
        {
            Debug.LogError(showLoading);
            yield return Timing.WaitForOneFrame;
        }

        MasterCanvas.CurrentMasterCanvas.ShowHUDLoadingView(false);
        callback?.Invoke();
    }

    protected virtual List<string> GetPreInitAsset()
    {
        return null;
    }

    public virtual void CleanUp()
    {
        // Pooly.Despawn(transform);
    }


    public virtual void Hide(Action<bool> hideComplete = null)
    {
        if(_hided)
            return;
        
        HideCallback = hideComplete;
        if (_uiView)
        {
            _uiView.Hide(false);
        }
        else
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.DOFade(0, TIME_FX).SetUpdate(true).OnComplete(() =>
            {
                gameObject.SetActive(false);
                HideCallback?.Invoke(true);
            });
        }

        _parentHUD.DequeueStacks(refreshLastLayer);
        _hided = true;
    }

    public virtual void HideInstantly()
    {
        _uiView.Hide(true);
        _hided = true;
    }

    public virtual void OnButtonBack()
    {
        //TO DO HANDLE BACK BUTTON

        Hide();
    }

    private void OnDestroy()
    {
        if (_uiView)
        {
            _uiView.OnVisibilityChanged.RemoveAllListeners();
        }
    }

    public virtual void ResetLayers()
    {
        var baseHud = (BaseParentHUD)_parentHUD;
        baseHud?.OnResetLayer(_hudType);
    }
}
