using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Localization;

public class HUDMail : BaseHUD
{
    [SerializeField] private TMP_InputField _titleMail;
    [SerializeField] private TMP_InputField _contentMail;

    private void ShowNotify(string text)
    {
        MasterCanvas.CurrentMasterCanvas.ShowFloatingTextNotify(text, duration: 1.5f);
    }

    public override void ResetLayers()
    {
        base.ResetLayers();
        _titleMail.text = "";
        _contentMail.text = "";
    }

    public void StartSendMail()
    {
        if (_titleMail.text.Length > 5 &&
            _contentMail.text.Length > 50)
        {
            SendSupportMail(_titleMail.text, _contentMail.text);
            ShowNotify( LocalizeController.GetText("MAIL_THANK"));
            Hide();
        }
        else
        {
            ShowNotify(LocalizeController.GetText("MAIL_ERROR"));
        }
    }

    public static string CreateSystemInfo()
    {
        string result = "";
        AddString(ref result, $"Device id: {SystemInfo.deviceUniqueIdentifier}");
        AddString(ref result, $"User id: {SystemInfo.deviceUniqueIdentifier}");
        AddString(ref result, $"Local time: {DateTime.Now}");
        AddString(ref result, $"Server time: {DateTime.UtcNow}");
        AddString(ref result, $"App version: {Application.version}");
        AddString(ref result, $"Unity version: {Application.unityVersion}");
        AddString(ref result, $"Device: {SystemInfo.deviceName}");
        AddString(ref result, $"OS: {SystemInfo.operatingSystem}");
        AddString(ref result, $"Device type: {SystemInfo.deviceType}");
        AddString(ref result, $"Device model: {SystemInfo.deviceModel}");

        return result;
    }

    private static void AddString(ref string result, string content)
    {
        result += content + "\n";
    }

    public static void SendSupportMail(string title, string content,string email = "nhnpro.nguyenhoang@gmail.com")
    {
        string subject = MyEscapeURL(title);
        string body = MyEscapeURL(CreateSystemInfo() + "\n" + "Issue: " + content);

        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    public static string MyEscapeURL(string URL)
    {
        return WWW.EscapeURL(URL).Replace("+", "%20");
    }
}