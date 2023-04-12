using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFirebaseLogin
{
    //local login
    string FirebaseAuthID { get; set; }
    BaseLocalLoginProvider currentLocalProvider { get; }
    void Initialize();

    void Prepare(BaseLocalLoginProvider localLogin, Action<bool> callback);
    void LoginFireBase(Action<bool, string> callback);
    void LogoutFireBase(Action<bool> callback);
}


public class FirebaseLogin : BaseSystem<FirebaseLogin>, IFirebaseLogin
{
    private string _firebaseAuthID = "NOT_LOGIN_YET";
    private BaseLocalLoginProvider _currentLocalProvider;

    public string FirebaseAuthID { get => _firebaseAuthID; set => _firebaseAuthID = value; }
    public Firebase.Auth.FirebaseAuth auth { get; private set; }
    public BaseLocalLoginProvider currentLocalProvider { get => _currentLocalProvider; }

    public bool EnableFireBaseLogin()
    {
        return false;
    }

    public void Initialize()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void Prepare(BaseLocalLoginProvider localLogin, Action<bool> callback)
    {
        Debug.Log($"FirebaseLogin Prepare: {localLogin.Type} - {localLogin.AuthCode}");
        _currentLocalProvider = localLogin;
        if (_currentLocalProvider.Type == LOCAL_LOGIN_TYPE.EMAIL)
        {
            LocalEmailLogin emailLogin = (LocalEmailLogin)_currentLocalProvider;
            CreateNewAccount(emailLogin.Email, emailLogin.PassWord, callback);
        }
        else
        {
            callback?.Invoke(true);
        }
    }

    public void LoginFireBase(Action<bool, string> callback)
    {
        switch (currentLocalProvider.Type)
        {
            case LOCAL_LOGIN_TYPE.NONE:
                break;
            case LOCAL_LOGIN_TYPE.PLAY_GAME:
                LoginWithGooglePlayGame(currentLocalProvider, callback);
                break;
            case LOCAL_LOGIN_TYPE.GAME_CENTER:
                LoginWithGameCenter(currentLocalProvider, callback);
                break;
            case LOCAL_LOGIN_TYPE.EMAIL:
                LoginWithEmail(currentLocalProvider, callback);
                break;
            default:
                break;
        }
    }

    public void LogoutFireBase(Action<bool> callback)
    {
        if (auth != null)
        {
            auth.SignOut();
            callback?.Invoke(true);
        }
        else
        {
            callback?.Invoke(false);
        }
    }

    #region Login Play Game
    private void LoginWithGooglePlayGame(BaseLocalLoginProvider localData, Action<bool, string> callback)
    {
        try
        {
            Debug.Log($"LoginWithGooglePlayGame with :{localData.AuthCode}");
            bool loginSuccess = true;
            Firebase.Auth.Credential credential =
                Firebase.Auth.PlayGamesAuthProvider.GetCredential(localData.AuthCode);
            auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("[GooglePlayGame] SignInWithCredentialAsync was canceled.");
                    loginSuccess = false;
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("[GooglePlayGame] SignInWithCredentialAsync encountered an error: " + task.Exception);
                    loginSuccess = false;
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("[GooglePlayGame] User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);

                if (loginSuccess)
                    this.FirebaseAuthID = newUser.UserId;
                callback?.Invoke(loginSuccess, this.FirebaseAuthID);
            });
        }
        catch (Exception ex)
        {
            Debug.LogError($"[GooglePlayGame] LoginWithGooglePlayGame ERROR!!!! {ex}");
            callback?.Invoke(false, null);
        }

    }

    #endregion

    #region Login Game Center
    private void LoginWithGameCenter(BaseLocalLoginProvider localData, Action<bool, string> callback)
    {

    }
    #endregion

    #region Email login

    private void CreateNewAccount(string email, string password, Action<bool> callback)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                callback?.Invoke(false);
                return;
            }
            if (task.IsFaulted)
            {
                //Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                if (task.Exception.ToString().Contains("another account"))
                {
                    callback?.Invoke(true);
                }
                else
                    callback?.Invoke(false);

                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            callback?.Invoke(true);
        });
    }

    private void LoginWithEmail(BaseLocalLoginProvider localData, Action<bool, string> callback)
    {
        LocalEmailLogin emailLogin = (LocalEmailLogin)localData;
        auth.SignInWithEmailAndPasswordAsync(emailLogin.Email, emailLogin.PassWord).ContinueWith(task =>
        {
            bool taskSuccess = true;
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                taskSuccess = false;
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                taskSuccess = false;
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            if (taskSuccess)
                this.FirebaseAuthID = newUser.UserId;

            callback?.Invoke(taskSuccess, this.FirebaseAuthID);
        });

    }

    #endregion

}
