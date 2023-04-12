using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ez.Pooly;
using System;
using System.Linq;
using DG.Tweening;
using Doozy.Engine.UI;
using Ez.Pooly;
using UnityEngine.UI;
using MEC;
using UnityEngine.AddressableAssets;

public class BaseParentHUD : MonoBehaviour, IParentHud
{
    public Transform holder;
    public Transform _floatingTextTrf;
    public Canvas canvas { get; private set; }
    public CanvasGroup _hudLoadingViewDefault;
    public CanvasGroup _hudLoadingViewHUD;

    protected static float _lastOpenHUDTime = -1f;

    private string[] useHUDLoading = new[]
    {
        "HUDEquipment",
        "HUDShop",
    };

    public bool ShowHUDLoadingView(bool enable, string hudName = "", float timeFX = 0.3f)
    {
        bool useHUDLoadingBool = useHUDLoading.Contains(hudName);

        if (useHUDLoadingBool)
            _hudLoadingViewHUD.gameObject.SetActive(enable);
        else
        {
            _hudLoadingViewDefault.gameObject.SetActive(enable);
        }

        if (!enable)
        {
            _hudLoadingViewHUD.gameObject.SetActive(enable);
            _hudLoadingViewDefault.gameObject.SetActive(enable);
        }

        return useHUDLoadingBool;

        // if (enable)
        // {
        //     // _hudLoadingView.alpha = 0f;
        //     _hudLoadingView.alpha = 1;
        //     // _hudLoadingView.DOFade(1.0f, timeFX).SetEase(Ease.Linear);
        // }
        // else
        // {
        //     _hudLoadingView.alpha = 0;
        //     _hudLoadingView.gameObject.SetActiveIfNot(false);
        //
        //     // _hudLoadingView.alpha = 1f;
        //     // _hudLoadingView.DOFade(0f, timeFX).SetEase(Ease.Linear).OnComplete(() =>
        //     // {
        //     //     _hudLoadingView.gameObject.SetActiveIfNot(false);
        //     // });
        // }
    }

    protected Dictionary<EnumHUD, BaseHUD> _dictHUD;
    private BaseHUD currentHUD;
    public BaseHUD CurrentHUD => currentHUD;
    protected Action<EnumHUD> onShowHUD;
    protected Action<EnumHUD> onResetLayer;
    protected UIFloatingTextNotify _uIFloatingText;

    private void Start()
    {
        if (_hudLoadingViewDefault != null && _hudLoadingViewHUD != null)
        {
            _hudLoadingViewDefault.gameObject.SetActiveIfNot(false);
            _hudLoadingViewHUD.gameObject.SetActiveIfNot(false);
        }
    }

    public virtual void Initialize()
    {
        canvas = GetComponent<Canvas>();
        _dictHUD = new Dictionary<EnumHUD, BaseHUD>();
    }

    // public virtual BaseHUD ShowDialogHUD(EnumHUD _type, bool hideCurrentHUD = true, Action<bool> ShowComplete = null,
    //     params object[] args)
    // {
    //     if (hideCurrentHUD && currentHUD != null)
    //     {
    //         if (currentHUD._hudType == _type)
    //             return null;
    //
    //         HideHUD(currentHUD._hudType);
    //     }
    //
    //     BaseHUD _hud = null;
    //     dictDialogHUD.TryGetValue(_type, out _hud);
    //
    //     if (_hud == null)
    //         return ShowHUD(_type, hideCurrentHUD, ShowComplete, args);
    //
    //     _hud.transform.SetAsLastSibling();
    //     _hud.PreInit(_type, this, args);
    //     _hud.Show(ShowComplete);
    //     currentHUD = _hud;
    //
    //     onShowHUD?.Invoke(currentHUD._hudType);
    //     return _hud;
    // }

    protected bool CanOpenNewHUD()
    {
        return Time.time - _lastOpenHUDTime > 0.1f;
    }

    public virtual void ShowHUD(EnumHUD _type, bool hideCurrentHUD = true, Action<bool> ShowComplete = null,
        params object[] args)
    {
        // _lastOpenHUDTime = Time.time;
        StartCoroutine(ShowHUDAsync(_type, hideCurrentHUD, ShowComplete, args));
    }

    public virtual void ShowHUDForce(EnumHUD _type, bool hideCurrentHUD = true, Action<bool> ShowComplete = null,
        params object[] args)
    {
        StartCoroutine(ShowHUDAsync(_type, hideCurrentHUD, ShowComplete, args));
    }


    IEnumerator ShowHUDAsync(EnumHUD _type, bool hideCurrentHUD = true, Action<bool> ShowComplete = null,
        params object[] args)
    {
        bool showLoading = false;
        if (hideCurrentHUD && currentHUD != null)
        {
            if (currentHUD._hudType == _type)
            {
                ShowComplete?.Invoke(true);
                //return null;asdawsdawd
            }
            else
            {
                HideHUD(currentHUD._hudType);
            }
        }
        else
        {
            if (currentHUD != null && currentHUD._hudType == _type)
            {
                ShowComplete?.Invoke(true);
            }
        }

        BaseHUD _hud = null;
        _dictHUD.TryGetValue(_type, out _hud);
        if (_hud == null)
        {
            var hudName = POOLY_PREF.GetHUDPrefabByType(_type);
            bool isContains = Pooly.IsContainKey(hudName);
            if (isContains)
            {
                showLoading = true;
                ShowHUDLoadingView(true, hudName);
                _hud = Pooly.Spawn(hudName, Vector3.zero, Quaternion.identity).GetComponent<BaseHUD>();
                // yield return new WaitForSeconds(0.1f);
            }
            else
            {
                showLoading = true;
                ShowHUDLoadingView(true, hudName);
                // yield return Timing.WaitForSeconds(0.3f);
                //_hud = Pooly.Spawn(hudName, Vector3.zero, Quaternion.identity).GetComponent<BaseHUD>();

                var op = Addressables.LoadAssetAsync<GameObject>(hudName);
                while (!op.IsDone)
                    yield return new WaitForEndOfFrame();

                _hud = Pooly.Spawn(op.Result.transform, Vector3.zero, Quaternion.identity).GetComponent<BaseHUD>();
                // yield return new WaitForSeconds(0.1f);
            }

            _hud.transform.SetParent(GetHolder());
            _hud.transform.localPosition = Vector3.zero;
            _hud.transform.localScale = Vector3.one;

            if (!_dictHUD.ContainsKey(_type))
                _dictHUD.Add(_type, _hud);
        }

        _hud.transform.SetAsLastSibling();
        _hud.PreInit(_type, this, args);
        _hud.Show(ShowComplete);

        currentHUD = _hud;
        onShowHUD?.Invoke(currentHUD._hudType);

        if (showLoading)
        {
            // if (isMenuHUD)
            // yield return new WaitForSeconds(0.25f);
            ShowHUDLoadingView(false);
        }
    }

    public Transform GetHolder()
    {
        return holder == null ? transform : holder;
    }

    public virtual BaseHUD HideHUD(EnumHUD _type, Action<bool> HideComplete = null, bool hideInstantly = false)
    {
        BaseHUD _hud = null;
        _dictHUD.TryGetValue(_type, out _hud);
        if (_hud)
        {
            if (hideInstantly)
            {
                _hud.HideInstantly();
                HideComplete?.Invoke(true);
            }
            else
                _hud.Hide(HideComplete);
        }

        return _hud;
    }

    public virtual BaseHUD GetHUD(EnumHUD _type)
    {
        BaseHUD result = null;
        _dictHUD.TryGetValue(_type, out result);

        return result;
    }

    #region FLoating Text

    public virtual void ShowMovingSprite(Sprite sp, Vector2 startPos, Vector2 endPos, Vector2 size, float duration,
        bool UseNativeSize = true, Action OnComplete = null, Ease easeType = Ease.InOutSine, Transform parent = null)
    {
        Image floating = Pooly.Spawn<Image>(POOLY_PREF.FLOATING_SPRITE, startPos, Quaternion.identity, GetHolder());
        floating.sprite = sp;
        floating.transform.localScale = Vector3.one;
        if (UseNativeSize)
            floating.SetNativeSize();
        else
            floating.GetComponent<RectTransform>().sizeDelta = size;

        if (parent != null)
        {
            floating.transform.SetParent(parent);
            floating.transform.localScale = Vector3.one;
        }


        floating.rectTransform.DOMove(endPos, duration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            Pooly.Despawn(floating.transform);
        });
        //floating.SetColorAlpha(1);
        //floating.DOFade(0, duration * 0.1f).SetEase(easeType).SetDelay(duration * 0.9f).OnComplete(() =>
        //{
        //    Pooly.Despawn(floating.transform);
        //    OnComplete?.Invoke();
        //});
    }

    public virtual void ShowFloatingText(Vector3 screenPos, string content, int size, float moveHeight, Color color,
        float duration = 0.3f)
    {
        UIFloatingText _UIFloatingText =
            Pooly.Spawn<UIFloatingText>(POOLY_PREF.FLOATING_TEXT, Vector3.zero, Quaternion.identity, GetHolder());
        _UIFloatingText.transform.SetParent(GetHolder());
        _UIFloatingText.Initialize(content, screenPos, size, color, moveHeight, duration);
        _UIFloatingText.Show();
    }

    public virtual void ShowFloatingText(FloatingTextData _data)
    {
        if (!GameMaster.instance.OptmizationController.Data.EnableTextDmg)
            return;
        UIFloatingText _UIFloatingText =
            Pooly.Spawn<UIFloatingText>(POOLY_PREF.FLOATING_TEXT, Vector3.zero, Quaternion.identity, GetHolder());
        _UIFloatingText.transform.SetParent(GetHolder());
        _UIFloatingText.Initialize(_data.Content, _data.ScreenPos, _data.Size, _data.Color, _data.MoveHeight,
            _data.Duration);
        _UIFloatingText.Show();
        _UIFloatingText.transform.SetAsLastSibling();
    }

    public virtual void ShowFloatingTextNotify(string content, int size = 50, float moveHeight = 200,
        float duration = 0.7f, bool toUpper = true, bool trim = true)
    {
        moveHeight = Utils.ConvertToMatchHeightRatio(moveHeight);
        if (_uIFloatingText == null || _uIFloatingText.gameObject == null)
        {
            var prefab = Resources.Load("UI/UIFloatingTextNotify") as GameObject;
            var tempGameobject = Instantiate(prefab, Vector3.zero, Quaternion.identity, GetHolder());
            _uIFloatingText = tempGameobject.GetComponent<UIFloatingTextNotify>();
        }


        _uIFloatingText.transform.SetParent(GetHolder());
        _uIFloatingText.transform.SetAsLastSibling();
        _uIFloatingText.Initialize(content, size, moveHeight, duration, toUpper, trim);
        _uIFloatingText.Show();
    }

    public void CleanUp()
    {
        foreach (var VARIABLE in _dictHUD)
        {
            VARIABLE.Value.CleanUp();
        }
    }

    #endregion

    #region MenuStacks

    public Stack<EnumHUD> stackMenu { get; private set; }

    Stack<EnumHUD> IParentHud.MenuStacks => menuStacks;

    private Stack<EnumHUD> menuStacks = new Stack<EnumHUD>();

    public void EnqueueStacks(EnumHUD hud)
    {
        menuStacks.Push(hud);
    }

    public void DequeueStacks(bool isRefresh)
    {
        if (menuStacks.Count <= 0)
            return;
        menuStacks.Pop();
        //get top
        var arr = menuStacks.ToArray();
        if (arr != null && arr.Length >= 1)
        {
            var topOne = menuStacks.Peek();
            if (topOne != EnumHUD.NONE)
            {
                var topHUD = GetHUD(topOne);
                currentHUD = topHUD;
                if (isRefresh)
                    topHUD?.ResetLayers();
            }
        }
    }

    public void OnResetLayer(EnumHUD type)
    {
        onResetLayer?.Invoke(type);
    }

    #endregion
}