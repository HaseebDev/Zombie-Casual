using System;
using System.Collections;
using System.Collections.Generic;
using com.datld.data;
using MEC;
using QuickEngine.Extensions;
using UnityEngine;
using UnityExtensions.Localization;

public class LocalizeController : BaseSystem<LocalizeController>
{
    public static LocalizeController Instance = null;

    public const string language_vi = "vi";
    public const string language_en = "en-US";
    public string currentLanguage = language_en;

    private SettingData _settingData;

    private void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveManager.Instance.Data != null);
        _settingData = SaveManager.Instance.Data.SettingData;
    }

    private string GetSavedLanguage()
    {
        return _settingData.Language;
    }

    public override IEnumerator<float> InitializeCoroutineHandler()
    {
        TaskResult isComplete = TaskResult.Failure;

        LocalizationManager.LoadMetaAsync((t) =>
        {
            if (t == TaskResult.Success)
            {
                LocalizationManager.LoadLanguageAsync(language_en, _ =>
                {
                    Debug.Log("Complete load localize, result " + _);
                    isComplete = _;
                }, true);
            }
        }, true);

        // string savedLanguage = GetSavedLanguage();
        // currentLanguage = savedLanguage.IsNullOrEmpty() ? GetDefaultLanguageType() : savedLanguage;
        // SaveCurrentLanguage();

        while (isComplete != TaskResult.Success)
        {
            yield return Timing.WaitForOneFrame;
        }

        OnInitializeComplete?.Invoke(true);
        IsInited = true;
    }

    private string GetDefaultLanguageType()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Vietnamese:
                return language_vi;
            default:
                return language_en;
        }
    }

    public void SwitchLanguage()
    {
        if (currentLanguage == language_en)
            currentLanguage = language_vi;
        else
        {
            currentLanguage = language_en;
        }

        SaveCurrentLanguage();
        MasterCanvas.CurrentMasterCanvas.ShowHUDLoadingView(true);
        LocalizationManager.LoadLanguageAsync(currentLanguage, (t) =>
        {
            if (t == TaskResult.Success)
            {
                MasterCanvas.CurrentMasterCanvas.ShowHUDLoadingView(false);
            }
        }, true);
    }

    private void SaveCurrentLanguage()
    {
        _settingData.Language = currentLanguage;
    }

    public static string GetText(string id, params object[] arg)
    {
        string text = LocalizationManager.GetText(id);

        if (arg != null)
        {
            if (text == null)
            {
                return "NF " + id;
            }

            return string.Format(text, arg);
        }

        return text;
    }
}