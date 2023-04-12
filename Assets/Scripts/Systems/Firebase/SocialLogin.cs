using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialLogin : BaseSystem<SocialLogin>
{
    public static SocialLogin instance;

    //temp turn off social Login Feature
    public bool IsEnable()
    {
        //var countLogin = PlayerPrefs.GetInt(PLAYER_PREF.COUNT_LOGIN, 0);
        //return countLogin >= 2;

        return false;
    }

    private void Awake()
    {
        instance = this;
    }

    public LOCAL_LOGIN_TYPE _typeLocalLogin { get; private set; }
    public BaseLocalLoginProvider _localLogin { get; private set; }
    public FirebaseLogin _firebaseLogin { get; private set; }
    public string FirebaseAuthID { get; private set; }

    public override void Initialize(params object[] pars)
    {
        if (IsEnable())
        {
            base.Initialize(pars);
            InitLocalLogin();

            FirebaseAuthID = "";
            _firebaseLogin = new FirebaseLogin();
            _firebaseLogin.Initialize();
        }
        else
        {
            Debug.Log("[SocialLogin] Temp turn off Social Login!!!!");
        }

    }

    public bool IsSignedInFireBase()
    {
        return !string.IsNullOrEmpty(FirebaseAuthID);
    }

    public bool IsSignedInLocal()
    {
        bool result = false;
        result = _localLogin.IsSignedIn;
        return result;
    }

    #region Local login
    public void InitLocalLogin()
    {
#if UNITY_EDITOR
        _localLogin = new LocalEmailLogin();
        _typeLocalLogin = LOCAL_LOGIN_TYPE.EMAIL;
#elif UNITY_ANDROID
        _localLogin = new PlayGameLogin();
       _typeLocalLogin = LOCAL_LOGIN_TYPE.PLAY_GAME;
#elif UNITY_IOS
          _localLogin = new GameCenterLogin();
      _typeLocalLogin = LOCAL_LOGIN_TYPE.GAME_CENTER;
#endif
        _localLogin.Initialize(_typeLocalLogin);
    }

    public void LocalLogin(Action<bool, string> callback)
    {
        _localLogin?.Login(callback);
    }

    public void LocalLogout(Action<bool> callback)
    {
        _localLogin.Logout(callback);
    }

    #endregion

    #region FirebaseLogin

    public void LoginFirebase(Action<bool, string> callback)
    {
        Timing.RunCoroutine(FlowFirebaseLogin(callback));
    }

    IEnumerator<float> FlowFirebaseLogin(Action<bool, string> callback)
    {
        bool waitingTask = false;
        //check login local first
        if (!_localLogin.IsSignedIn)
        {
            waitingTask = true;
            _localLogin.Login((success, authID) =>
            {
                waitingTask = false;
            });
            while (waitingTask)
            {
                yield return Timing.WaitForOneFrame;
            }
        }

        //then login firebase
        waitingTask = true;
        bool prepareSuccess = false;
        _firebaseLogin.Prepare(_localLogin, (success) =>
        {
            waitingTask = false;
            prepareSuccess = success;
        });
        while (waitingTask)
        {
            yield return Timing.WaitForOneFrame;
        }

        if (prepareSuccess)
            _firebaseLogin.LoginFireBase((success, authID) =>
            {
                if (success)
                {
                    this.FirebaseAuthID = authID;
                }

                callback?.Invoke(success, authID);

            });
        else
            callback?.Invoke(false, null);
    }

    #endregion

}
