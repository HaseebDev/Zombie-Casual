using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum LOCAL_LOGIN_TYPE
{
    NONE = 0,
    PLAY_GAME,
    GAME_CENTER,
    EMAIL
}

public interface ILocalLogin
{
    LOCAL_LOGIN_TYPE loginType { get; set; }
    void Initialize();
    BaseLocalLoginProvider loginProvider { get; set; }
    void LoginLocal(Action<bool, string> callback);
    void LogoutLocal(Action<bool> callback);
}

[Serializable]
public abstract class BaseLocalLoginProvider
{
    public LOCAL_LOGIN_TYPE Type;
    public string AuthCode { get; protected set; }
    public bool IsSignedIn { get; protected set; }

    public virtual void Initialize(LOCAL_LOGIN_TYPE type)
    {
        this.Type = type;
    }

    public abstract void Login(Action<bool, string> loginResponse);

    public abstract void Logout(Action<bool> callback);
}

