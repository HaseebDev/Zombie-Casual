using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalEmailLogin : BaseLocalLoginProvider
{
    public string Email;
    public string PassWord;

    public override void Login(Action<bool, string> loginResponse)
    {
        GenerateNewEmailPass();
        loginResponse?.Invoke(true, Email);
    }

    public override void Logout(Action<bool> callback)
    {
        callback?.Invoke(true);
    }

    public void GenerateNewEmailPass()
    {
        Email = $"{SystemInfo.deviceUniqueIdentifier}@test.com";
        PassWord = "123456";
    }
}
