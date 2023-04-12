using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using System;
using static AnalyticsConstant;

[Serializable]
public class LogEventParam
{
    public string key;
    public object value;
    public LogEventParam(string _key, string _value)
    {
        this.key = _key;
        this.value = _value;
    }
    public LogEventParam(string _key, long _value)
    {
        this.key = _key;
        this.value = _value;
    }
    public LogEventParam(string _key, double _value)
    {
        this.key = _key;
        this.value = _value;
    }
}

public interface IAnalyticsProvider
{
    void Initialize(params object[] _params);
    void LogEvent(string eventName, List<LogEventParam> _params = null);
    void LogEvent(string eventName, LogEventParam _singleParam = null);
}

public class AnalyticsManager : BaseSystem<AnalyticsManager>
{
    public static AnalyticsManager instance;
    public List<IAnalyticsProvider> _listProviders;
    private void Awake()
    {
        instance = this;

    }

    public override void Initialize(params object[] pars)
    {
        base.Initialize(pars);

        _listProviders = new List<IAnalyticsProvider>();
        _listProviders.Add(new FirebaseAnalyticsProvider());

        foreach (var provider in _listProviders)
        {
            provider.Initialize();
        }

    }

    public void LogEvent(string eventName, LogEventParam _singleParam = null)
    {
        foreach (var provider in _listProviders)
        {
            provider.LogEvent(eventName, _singleParam);
        }
    }

    public void LogEvent(ANALYTICS_ENUM enumHUD, LogEventParam _singleParam)
    {
        foreach (var provider in _listProviders)
        {
            provider.LogEvent(getEventName(enumHUD), _singleParam);
        }
    }

    public void LogEvent(string eventName, List<LogEventParam> _params = null)
    {
        foreach (var provider in _listProviders)
        {
            provider.LogEvent(eventName, _params);
        }
    }

    public void LogEvent(ANALYTICS_ENUM enumHUD, List<LogEventParam> _params = null)
    {
        foreach (var provider in _listProviders)
        {
            var eventName = AnalyticsConstant.getEventName(enumHUD);
            if (!string.IsNullOrEmpty(eventName))
                provider.LogEvent(eventName, _params);
        }
    }

    internal void LogEvent()
    {
        throw new NotImplementedException();
    }

    #region Extends 

    public void LogEventDayLoginVsCampaignLevel(int day, int campaignLevel)
    {
        string eventName = $"{getEventName(ANALYTICS_ENUM.DAY_LOGIN_CAMPAIGN_LEVEL)}_day_{day}_level_{campaignLevel}";
        LogEvent(eventName, new List<LogEventParam>()
            {
                new LogEventParam("dayLogin",SaveManager.Instance.Data.MetaData.DayLogin),
                new LogEventParam("campaignLevel",campaignLevel)});
    }

    #endregion
}
