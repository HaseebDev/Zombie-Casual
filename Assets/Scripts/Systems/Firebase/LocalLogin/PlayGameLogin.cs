using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGameLogin : BaseLocalLoginProvider
{
    public override void Initialize(LOCAL_LOGIN_TYPE type)
    {
        base.Initialize(type);
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .RequestServerAuthCode(false /* Don't force refresh */)
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        Debug.Log("PlayGameLogin Init successfully!!!");

    }

    public override void Login(Action<bool, string> loginResponse)
    {
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate((success, message) =>
            {
                Debug.Log("Authentication Message: " + message);
                Debug.Log($"PlayGameLogin Login result:{success}");
                // handle success or failure
                if (success)
                {
                    Debug.Log("Authentication Google Play Game successful");
                    string userInfo = "Username: " + Social.localUser.userName +
                        "\nUser ID: " + Social.localUser.id +
                        "\nIsUnderage: " + Social.localUser.underage;
                    Debug.Log(userInfo);

                    this.AuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                    loginResponse?.Invoke(true, AuthCode);
                }
                else
                {
                    loginResponse?.Invoke(false, null);

                }
            });
        }
        else
        {
            this.AuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
            loginResponse?.Invoke(true, AuthCode);
        }

    }

    public override void Logout(Action<bool> callback)
    {
        PlayGamesPlatform.Instance.SignOut();
        if (Social.localUser.authenticated == false)
        {

        }
    }
}
