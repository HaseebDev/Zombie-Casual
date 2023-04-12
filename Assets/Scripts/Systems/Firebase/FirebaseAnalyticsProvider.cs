using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

public class FirebaseAnalyticsProvider : IAnalyticsProvider
{
    public void Initialize(params object[] _params)
    {
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
    }

    public void LogEvent(string eventName, List<LogEventParam> _params = null)
    {
        var _converted = ConvertParams(_params);
        string paramDebug = "";
        if (_params != null)
        {
            FirebaseAnalytics.LogEvent(eventName, _converted.ToArray());
            foreach (var _singleParam in _params)
            {
                paramDebug += $"par:{_singleParam.key}-{_singleParam.value} \n";
            }
        }
        else
            FirebaseAnalytics.LogEvent(eventName);

        //Debug.Log($"[FirebaseAnalytics] event:{eventName} par:{paramDebug}");
    }

    public void LogEvent(string eventName, LogEventParam _singleParam = null)
    {
        var _converted = ConvertSingleParam(_singleParam);
        if (_converted != null)
            FirebaseAnalytics.LogEvent(eventName, _converted);
        else
            FirebaseAnalytics.LogEvent(eventName);
        //Debug.Log($"[FirebaseAnalytics] event:{eventName} par:{_singleParam.key}-{_singleParam.value}");
    }

    List<Parameter> ConvertParams(List<LogEventParam> _myParams)
    {
        List<Parameter> result = null;
        if (_myParams != null && _myParams.Count > 0)
        {
            result = new List<Parameter>();

            foreach (var p in _myParams)
            {
                var conveted = ConvertSingleParam(p);
                if (conveted != null)
                    result.Add(conveted);
            }

        }
        return result;
    }

    Parameter ConvertSingleParam(LogEventParam p)
    {

        Parameter result = null;
        if (p == null || p.value == null)
            return null;

        if (p.value.GetType() == typeof(string))
            result = (new Parameter(p.key, (string)p.value));
        else if (p.value.GetType() == typeof(double))
            result = (new Parameter(p.key, (double)p.value));
        else if (p.value.GetType() == typeof(long))
            result = (new Parameter(p.key, (long)p.value));

        return result;
    }
}
