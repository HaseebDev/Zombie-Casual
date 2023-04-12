using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AnalyticsConstant;

[RequireComponent(typeof(Button))]
public class AnalyticsEventListener : MonoBehaviour
{
    public ANALYTICS_ENUM _analyticEnum;

    private Button button;
    private void Awake()
    {
        button = this.GetComponent<Button>();
    }
    private void Start()
    {
        if (!button)
            button = this.GetComponent<Button>();

        button?.onClick.AddListener(LogEvent);
    }

    private void OnDestroy()
    {
        button?.onClick.RemoveListener(LogEvent);
    }

    public void LogEvent()
    {
        int campaignLevel = SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).CurrentLevel;
        if (campaignLevel <= AnalyticsConstant.MAX_TRACKING_LEVEL)
        {
            string eventName = getEventNameByLevel(_analyticEnum, campaignLevel);
            AnalyticsManager.instance.LogEvent(eventName, new LogEventParam("level", campaignLevel));

        }

    }
}
