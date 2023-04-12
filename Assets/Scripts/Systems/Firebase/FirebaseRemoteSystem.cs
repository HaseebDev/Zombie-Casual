using Firebase.Extensions;
using Firebase.RemoteConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseRemoteSystem : BaseSystem<FirebaseRemoteSystem>
{
    public static FirebaseRemoteSystem instance;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    private void Awake()
    {
        instance = this;
    }

    public override void Initialize(params object[] pars)
    {
        base.Initialize(pars);
        FetchFirstTime((success) =>
        {
            Debug.Log($"[Firebase Remote Config] initialize: {success}");
        });
    }


    /// <summary>
    /// サーバとの同期を行います
    /// </summary>
    /// <param name="completionHandler">同期完了時のコールバック</param>
    public void FetchFirstTime(Action<bool> completionHandler)
    {
        // TODO: RELEASE時にここを外す
        //var settings = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Settings;
        //settings.IsDeveloperMode = true;
        //Firebase.RemoteConfig.FirebaseRemoteConfig.Settings = settings;

        var fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(new System.TimeSpan(0));

        fetchTask.ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                completionHandler(false);


            }
            else
            {

            }
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync();//.ActivateFetched();
            completionHandler(true);
        });
    }

    public ConfigValue GetConfigValue(string key)
    {
        return FirebaseRemoteConfig.DefaultInstance.GetValue(key);

    }
}
