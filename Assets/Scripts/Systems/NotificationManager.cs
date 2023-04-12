using UnityEngine;
//1
using System;
using System.Collections;
using com.datld.data; //for DateTime
//2
using NotificationSamples;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    //1
    private GameNotificationsManager manager;

    //2
    public const string ChannelId = "game_channel0";

    //3
    public const string ReminderChannelId = "reminder_channel1";

    //4
    public const string NewsChannelId = "news_channel2";
    private SettingData _settingData;

    void Awake()
    {
        Instance = this;
        manager = GetComponent<GameNotificationsManager>();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveManager.Instance.Data != null);
        _settingData = SaveManager.Instance.Data.SettingData;

        //1
        var c1 = new GameNotificationChannel(ChannelId, "Game Alerts", "Game notifications");
        //2
        var c2 = new GameNotificationChannel(NewsChannelId, "News", "News and Events");
        //3
        var c3 = new GameNotificationChannel(ReminderChannelId, "Reminders", "Reminder notifications");
        //4
        manager.Initialize(c1, c2, c3);

        ScheduleReminderNotification();
    }

    public void ScheduleReminderNotification()
    {
        string title = "Zombie is coming!";
        string body = "The Zombies are rising again! Stand up and take back your city!";
        DateTime deliverTime = DateTime.Now.AddHours(24 * 3);
        string channel = ReminderChannelId;

        CancelNotification(NotificationID.reminder);
        SendNotification(NotificationID.reminder, title, body, deliverTime, channelId: channel,
            smallIcon: NotificationIconID.reminder_small_icon,
            largeIcon: NotificationIconID.reminder_large_icon);
    }

    public void ScheduleChest(ChestType chestType, float receiveHours)
    {
        string title = "";
        string body = "";
        int notiId = 0;

        switch (chestType)
        {
            case ChestType.RARE:
                if (!_settingData.IsRareChestNotificationOn)
                    return;
                
                title = "Free Rare chest available";
                notiId = NotificationID.chestRare;
                body = "Rare chest available. Open now!";
                break;
            case ChestType.LEGENDARY:
                if (!_settingData.IsLegendaryChestNotificationOn)
                    return;
                
                title = "Free Legendary chest available";
                notiId = NotificationID.chestLegendary;
                body = "Legendary chest available. Open now!";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(chestType), chestType, null);
        }

        DateTime deliverTime = DateTime.Now.AddHours(receiveHours);
        string channel = ChannelId;

        CancelNotification(notiId);
        SendNotification(notiId, title, body, deliverTime, channelId: channel,
            smallIcon: NotificationIconID.reminder_small_icon,
            largeIcon: NotificationIconID.reminder_large_icon);
    }

    public void ScheduleNewStuff()
    {
        if (!_settingData.IsFreeStuffNotificationOn)
            return;
        
        string title = "Free Stuffs available";
        string body = "Free rewards are ready for you!";
        DateTime deliverTime = DateTime.Now.AddHours(24);
        string channel = ReminderChannelId;

        CancelNotification(NotificationID.freeStuff);
        SendNotification(NotificationID.freeStuff, title, body, deliverTime, channelId: channel,
            smallIcon: NotificationIconID.reminder_small_icon,
            largeIcon: NotificationIconID.reminder_large_icon);
    }

    public void CancelNotification(int id)
    {
        manager.Platform.CancelNotification(id);
    }

    public void SwitchRareChestNotificationToggle(bool isOn)
    {
        _settingData.IsRareChestNotificationOn = isOn;

        if (!isOn)
        {
            CancelNotification(NotificationID.chestRare);
        }
    }

    public void SwitchLegendaryChestNotificationToggle(bool isOn)
    {
        _settingData.IsLegendaryChestNotificationOn = isOn;

        if (!isOn)
        {
            CancelNotification(NotificationID.chestLegendary);
        }
    }

    public void SwitchFreeStuffNotificationToggle(bool isOn)
    {
        _settingData.IsFreeStuffNotificationOn = isOn;

        if (!isOn)
        {
            CancelNotification(NotificationID.freeStuff);
        }
    }

    //1
    public void SendNotification(int? id, string title, string body, DateTime deliveryTime, int? badgeNumber = null,
        bool reschedule = false, string channelId = null,
        string smallIcon = null, string largeIcon = null)
    {
        //2
        IGameNotification notification = manager.CreateNotification();
        //3
        if (notification == null)
        {
            return;
        }

        notification.Id = id;
        //4
        notification.Title = title;
        //5
        notification.Body = body;
        //6
        notification.Group = !string.IsNullOrEmpty(channelId) ? channelId : ChannelId;
        //7
        notification.DeliveryTime = deliveryTime;
        //8
        notification.SmallIcon = smallIcon;
        //9
        notification.LargeIcon = largeIcon;

        //10
        if (badgeNumber != null)
        {
            notification.BadgeNumber = badgeNumber;
        }

        //11
        PendingNotification notificationToDisplay = manager.ScheduleNotification(notification);
        //12
        notificationToDisplay.Reschedule = reschedule;
    }
}