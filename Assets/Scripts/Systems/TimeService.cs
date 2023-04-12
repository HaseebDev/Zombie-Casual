using QuickType.ServerTime;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Time;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Control time service
/// </summary>
public class TimeService : BaseSystem<TimeService>
{
    public static TimeService instance;

    private void Awake()
    {
        instance = this;
    }

    public const float MIN_SEC = 60;
    public const float HOUR_SEC = 3600;
    public const float DAY_SEC = 86400;
    public const float FRAME_SEC = 0.02f;
    public static float FETCH_SERVER_TIME_DURATION = 300;

    private float _lastUpTimeFromServer = 0;
    private float _timeUnity = 0f;
    private long _offsetTimeVsServer = 0;
    private float _fetchTimer = 0f;

    public override void Initialize(params object[] pars)
    {
        base.Initialize(pars);
        GetServerTime();
    }

    public long GetCurrentTimeStamp(bool forceLocal = false)
    {
        long unixTime = 0;
        bool forceGetServerTime = forceLocal;
        if (forceGetServerTime)
        {
            GetServerTime();
        }

        if (_timeUnity > 0 && _serverTimer > 0)
        {
            var diff = (long) (SpeedHackProofTime.unscaledTime - _timeUnity);
            unixTime = _serverTimer + diff;
        }
        else if (_offsetTimeVsServer > 0)
        {
            unixTime = GetLocalTimeStamp() + _offsetTimeVsServer;
        }
        else
        {
            unixTime = GetLocalTimeStamp();
        }

        return unixTime;
    }


    public long GetLocalTimeStamp()
    {
        long unixTime = 0;
        DateTime foo = DateTime.UtcNow;
        unixTime = ((DateTimeOffset) foo).ToUnixTimeSeconds();
        return unixTime;
    }

    public long GetTimeStampBeginDay()
    {
        long unixTime = 0;
        DateTime today = UnixTimestampToDateTime(GetCurrentTimeStamp()).Date;
        unixTime = ((DateTimeOffset) today).ToUnixTimeSeconds();
        return unixTime;
    }

    public static long ParseDate(string Date)
    {
        long timeStamp = 0;
        string inString = Date;
        DateTime dateValue;
        if (DateTime.TryParse(inString, out dateValue))
            Debug.Log($"Converted '{inString}' to {dateValue}.");
        else
        {
            Debug.LogError($"Unable to convert '{inString}' to a date.");
            return -1;
        }

        timeStamp = ((DateTimeOffset) dateValue).ToUnixTimeSeconds();

        return timeStamp;
    }

    public static string FormatTimeSpan(double deltaSecs, bool showSecond = true)
    {
        string result = "";
        var ts = TimeSpan.FromSeconds(deltaSecs);

        if (ts.Days > 0)
            result = $"{ts.Days}d {ts.Hours}h {ts.Minutes}m";
        else if (ts.Hours > 0)
        {
            result = $"{ts.Hours}h {ts.Minutes}m {ts.Seconds}s";
            if (!showSecond)
            {
                result = $"{ts.Hours}h {ts.Minutes}m";
            }
        }
        else
        {
            result = $"{ts.Minutes}m {ts.Seconds}s";
            if (!showSecond)
            {
                result = $"{ts.Minutes}m";
            }
        }

        return result;
    }

    public static string FormatTimeSpanShortly(double deltaSecs)
    {
        string result = "";
        var ts = TimeSpan.FromSeconds(deltaSecs);

        if (ts.Days > 0)
            result =
                $"{ts.Days.ToString("D2")}d {ts.Hours.ToString("D2")} : {ts.Minutes.ToString("D2")} : {ts.Seconds}";
        else if (ts.Hours > 0)
            result = $"{ts.Hours.ToString("D2")} : {ts.Minutes.ToString("D2")} : {ts.Seconds.ToString("D2")}";
        else
            result = $"{ts.Minutes.ToString("D2")} : {ts.Seconds.ToString("D2")}";

        return result;
    }

    private void Update()
    {
        if (_offsetTimeVsServer != 0)
        {
            _fetchTimer += Time.deltaTime;
            if (_fetchTimer % 60 >= FETCH_SERVER_TIME_DURATION)
            {
                GetServerTime();
                _fetchTimer = 0f;
            }
        }
    }

    public override void UpdateSystem(float _deltaTime)
    {
        // if (IsSyncedServerTime)
        // {
        //     timerTemp += Time.fixedUnscaledDeltaTime;
        //     CurrentServerTS = LastServerTs + (int) (timerTemp % 60);
        //
        //     // Logwin.Log("Server Ts:", $"{CurrentServerTS}", "Time Service");
        //     // Logwin.Log("Local Ts:", $"{GetLocalTimeStamp()}", "Time Service");
        // }
    }

    public long GetNextDayTSofMonth(long curTs, int day)
    {
        var norDay = UnixTimestampToDateTime(GetCurrentTimeStamp());
        var specificDay = norDay;
        if (norDay.Day < day)
            specificDay = new DateTime(norDay.Year, norDay.Month, day);
        else
        {
            norDay = norDay.AddMonths(1);
            specificDay = new DateTime(norDay.Year, norDay.Month, day);
        }

        var timeStamp = ((DateTimeOffset) specificDay).ToUnixTimeSeconds();
        return timeStamp;
    }

    #region server time

    public static string timeURL = @"http://worldtimeapi.org/api/ip?fields=unixtime";
    public ServerTimeData currentServerTimeData { get; private set; }

    public bool IsSyncedServerTime
    {
        get { return currentServerTimeData != null; }
    }

    public long CurrentServerTS { get; private set; }
    private long _serverTimer = 0L;

    private long LastServerTs;
    private float timerTemp = 0;
    private bool isGetTime = false;

    public void GetServerTime(Action<long> callback = null)
    {
        StartCoroutine(GetServerTimeCoroutine(callback));
    }

    public IEnumerator GetServerTimeCoroutine(Action<long> callback = null)
    {
        if (!isGetTime)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(timeURL))
            {
                isGetTime = true;
                webRequest.certificateHandler = new ForceAcceptAllCertificate();
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError)
                {
                    //  Debug.Log(": Error: " + webRequest.error);
                }
                else
                {
                    // Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                    try
                    {
                        var jsonTime = webRequest.downloadHandler.text;
                        currentServerTimeData = ServerTimeData.FromJson(jsonTime);
                    }
                    catch (Exception ex)
                    {
                        //   Debug.LogError($"Parse time exception {ex}");
                        currentServerTimeData = null;
                    }

                    if (currentServerTimeData != null)
                    {
                        _fetchTimer = 0f;
                        this._offsetTimeVsServer = currentServerTimeData.Unixtime - GetLocalTimeStamp();
                        _timeUnity = SpeedHackProofTime.unscaledTime;
                        _serverTimer = currentServerTimeData.Unixtime;
                    }
                    else
                    {
                        callback?.Invoke(-1);
                    }
                }

                isGetTime = false;
            }
        }
    }

    #endregion

    #region Utils

    public static DateTime UnixTimestampToDateTime(double unixTime)
    {
        DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        long unixTimeStampInTicks = (long) (unixTime * TimeSpan.TicksPerSecond);
        return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
    }

    #endregion

    public DateTime GetCurrentDateTime()
    {
        var norDay = UnixTimestampToDateTime(GetCurrentTimeStamp());
        return norDay;
    }
}