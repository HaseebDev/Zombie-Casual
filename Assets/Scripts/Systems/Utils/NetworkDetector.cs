using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.Networking;

public enum DEBUG_NET_STATE
{
    NONE,
    HAS_NETWORK,
    NO_NETWORK
}

public class NetworkDetector : BaseSystem<NetworkDetector>
{
    public static readonly float CHECK_NET_DURATION = 180;
    public static readonly float CLEAR_CACHE_DURATION = 60;
    public static NetworkDetector instance;
    public bool EnableCheckNet { get; private set; }
    public bool IsReachedInternet { get; private set; }
    private float timerCheckNet = 0f;

    public DEBUG_NET_STATE debugState;

    private bool LastHasInternet = false;
    private float timerClearCache = 0f;


    private void Awake()
    {
        instance = this;

#if !UNITY_EDITOR
        debugState = DEBUG_NET_STATE.NONE;
#endif
    }

    private bool showingDialogNoInternet = false;

    public override void Initialize(params object[] pars)
    {
        base.Initialize(pars);
        EnableCheckNet = false;
        //checkInternetConnection((success) =>
        //{
        //    IsReachedInternet = success;
        //});
    }

    public void checkInternetConnection(Action<bool> action = null, bool autoShowDisconnectPopup = true, bool enableLoading = true)
    {
        #if UNITY_EDITOR
        SROptions.Current.NetState = DEBUG_NET_STATE.HAS_NETWORK;
        #endif
        
        if (SROptions.Current.NetState == DEBUG_NET_STATE.NONE)
        {
            if (LastHasInternet)
            {
                if (enableLoading)
                {
                    TopLayerCanvas.instance.ShowHUDLoading(false);
                }
                // TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
                action?.Invoke(true);
                // Debug.LogError($"[NetworkDetector] HasNetwork: {true}");
            }
            else
                StartCoroutine(checkInternetConnectionCoroutine(action, autoShowDisconnectPopup, enableLoading));
        }
        else
        {
            if (SROptions.Current.NetState == DEBUG_NET_STATE.HAS_NETWORK)
            {
                action?.Invoke(SROptions.Current.NetState == DEBUG_NET_STATE.HAS_NETWORK);
                return;
            }

            if (enableLoading)
            {
                TopLayerCanvas.instance.ShowHUDLoading(true);
            }
            // TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_LOADING, false, null);
            Timing.CallDelayed(1.0f, () =>
            {
                action?.Invoke(SROptions.Current.NetState == DEBUG_NET_STATE.HAS_NETWORK);

                if (SROptions.Current.NetState == DEBUG_NET_STATE.NO_NETWORK)
                {
                    if (autoShowDisconnectPopup)
                    {
                        ShowNoInternetPopup(() =>
                        {
                            showingDialogNoInternet = false;
                        });
                    }
                }

                if (enableLoading)
                {
                    // TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);\
                    TopLayerCanvas.instance.ShowHUDLoading(false);

                }


            });
        }
    }

    IEnumerator checkInternetConnectionCoroutine(Action<bool> action, bool autoShowDisconnectPopup = true, bool enableLoading = true)
    {
        //if (enableLoading)
        //    TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_LOADING);
        //WWW www = new WWW("http://google.com");
        //yield return www;
        //if (www.error != null)
        //{
        //    Debug.LogError($"[NetworkDetector] HasNetwork: {false}");
        //    action?.Invoke(false);
        //    if (autoShowDisconnectPopup)
        //    {
        //        ShowNoInternetPopup(() =>
        //        {
        //            showingDialogNoInternet = false;
        //        });
        //    }

        //    LastHasInternet = false;
        //}
        //else
        //{
        //    Debug.LogError($"[NetworkDetector] HasNetwork: {true}");
        //    action?.Invoke(true);
        //    LastHasInternet = true;
        //}

        //if (enableLoading)
        //    TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);

        if (enableLoading)
        {
            TopLayerCanvas.instance.ShowHUDLoading(true);
        }
            //TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_LOADING);

        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://google.com"))
        {
            webRequest.certificateHandler = new ForceAcceptAllCertificate();
            webRequest.timeout = 5;
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.LogError($"[NetworkDetector] HasNetwork: {false}");
                Debug.Log(": Error: " + webRequest.error);
                action?.Invoke(false);
                if (autoShowDisconnectPopup)
                {
                    ShowNoInternetPopup(() =>
                    {
                        showingDialogNoInternet = false;
                    });
                }
            }
            else
            {
                Debug.LogError($"[NetworkDetector] HasNetwork: {true}");
                action?.Invoke(true);
                LastHasInternet = true;

            }
        }

        if (enableLoading)
        {
            TopLayerCanvas.instance.ShowHUDLoading(false);
        }
           // TopLayerCanvas.instance.HideHUD(EnumHUD.HUD_LOADING);
    }

    private void ShowNoInternetPopup(Action OnHided)
    {
        if (showingDialogNoInternet)
            return;

        TopLayerCanvas.instance.ShowHUD(EnumHUD.HUD_NO_INTERNET, false, null, OnHided);
        showingDialogNoInternet = true;
    }

    public override void UpdateSystem(float _deltaTime)
    {
        base.UpdateSystem(_deltaTime);

        if (EnableCheckNet)
        {
            timerCheckNet += Time.fixedUnscaledDeltaTime;
            if (timerCheckNet >= CHECK_NET_DURATION)
            {
                timerCheckNet = 0f;
                checkInternetConnection((success) =>
                {
                    IsReachedInternet = success;
                });
            }

        }

        timerClearCache += Time.fixedUnscaledDeltaTime;
        if (timerClearCache >= CLEAR_CACHE_DURATION)
        {
            timerClearCache = 0f;
            LastHasInternet = false;
        }
    }

}
