using UnityEngine;


#if UNITY_ANDROID
    using UnityEngine.Android;
    using Unity.Notifications.Android;
#elif UNITY_IOS
    using System.Collections;
    using Unity.Notifications.iOS;
#endif

[System.Serializable]
public class NotificationMessage
{
    public string subtitle;
    public string body;
    
    public NotificationMessage(string subtitle, string body)
    {
        this.subtitle = subtitle;
        this.body = body;
    }
}

public class NotificationHandler : MonoBehaviour
{
    private static NotificationMessage[] notificationMessages = {
        new NotificationMessage(
            "🎁 A mysterious gift is waiting for you!",
            "Come back now to claim your free reward and discover exciting new levels! Don't miss out!"
        ),
        new NotificationMessage(
            "Did you miss something? 🔍",
            "A bunch of new hidden objects are waiting to be found! Return now to conquer today's challenge!"
        ),
        new NotificationMessage(
            "⏰ Adventure time!",
            "Rare items have just appeared! Who will be the first to find them? Log in and check now!"
        ),
        new NotificationMessage(
            "The hidden world misses you!",
            "Come back to explore, unlock new stories, and prove your ultimate searching skills!"
        ),
        new NotificationMessage(
            "🔥 Don't miss today's reward!",
            "Every day you log in, a new surprise is waiting for you. Jump back into the game now!"
        )
    };

    void Start()
    {
#if UNITY_ANDROID
        RequestAuthorization();
        RegisterNotificationChannel();
#elif UNITY_IOS
        StartCoroutine(RequestAuthorization());
#endif
    }

    void OnApplicationPause(bool focus)
    {
        if (focus)
        {
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllNotifications();
            SendNotification();
#elif UNITY_IOS
            iOSNotificationCenter.RemoveAllDeliveredNotifications();
            SendNotification();
#endif
        }
    }

#if UNITY_ANDROID
    void RequestAuthorization()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }

    void RegisterNotificationChannel()
    {
        var channel = new AndroidNotificationChannel
        {
            Id = "default_channel",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications"
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    void SendNotification()
    {
        NotificationMessage message = GetRandomNotification();

        var notification = new AndroidNotification
        {
            Title = message.subtitle,
            Text = message.body,
            // FireTime = System.DateTime.Now.AddMinutes(1),
            FireTime = GetNext8PM(),
            // SmallIcon = "icon_0",
            LargeIcon = "icon_1",
            RepeatInterval = new System.TimeSpan(24, 0, 0), // Repeat every 24 hours
        };

        AndroidNotificationCenter.SendNotification(notification, "default_channel");
    }

#elif UNITY_IOS
    public IEnumerator RequestAuthorization()
    {
        using var request = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
        while (!request.IsFinished)
        {
            yield return null;
        }
    }

    void SendNotification()
    {
        NotificationMessage message = GetRandomNotification();
        System.DateTime next8PM = GetNext8PM();
        System.TimeSpan timeUntil8PM = next8PM - System.DateTime.Now;

        var timeTrigger = new iOSNotificationTimeIntervalTrigger
        {
            TimeInterval = timeUntil8PM,
            Repeats = true
        };

        var notification = new iOSNotification
        {
            Identifier = "Lives_full",
            Title = "Hidden objects find puzzle",
            Body = message.body,
            Subtitle = message.subtitle,
            ShowInForeground = true,
            ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Badge,
            CategoryIdentifier = "default_category",
            ThreadIdentifier = "default_thread",
            Trigger = timeTrigger
        };

        iOSNotificationCenter.ScheduleNotification(notification);
    }
#endif

    public static NotificationMessage GetRandomNotification()
    {
        int randomIndex = UnityEngine.Random.Range(0, notificationMessages.Length);
        return notificationMessages[randomIndex];
    }

    System.DateTime GetNext8PM()
    {
        var now = System.DateTime.Now;
        var today8PM = new System.DateTime(now.Year, now.Month, now.Day, 20, 0, 0); // 20:00 = 8 PM

        if (now < today8PM)
        {
            return today8PM; // Hôm nay 8 giờ tối
        }
        else
        {
            return today8PM.AddDays(1); // Mai 8 giờ tối
        }
    }
}