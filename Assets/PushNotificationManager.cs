using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_ANDROID
using UnityEngine.Android;
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class PushNotificationManager : MonoBehaviorSingleton<PushNotificationManager>
{
    Dictionary<string, PushNotesDataSO> dataDic;
    Dictionary<string, bool> rewardRecieved;

    public event Action OnApplicationPauseEvent;

    public int hasPermissionChecked
    {
        get { return PlayerPrefs.GetInt("HasPermissionChecked_" + Application.productName, 0); }
        set { PlayerPrefs.SetInt("HasPermissionChecked_" + Application.productName, value); }
    }
#if UNITY_EDITOR
    public bool GetIsPlayerStartInMainScene()
    {
        return dataDic == null;
    }
#endif 


    public void StartInit()
    {
        DontDestroyOnLoad(gameObject);
        Initialize();
    }


    private void Initialize()
    {
        SetCollections();

        Debug.Log(hasPermissionChecked);

        // 최초 한 번만 권한 체크
        if (hasPermissionChecked == 0)
        {
            CheckNotificationPermission();
            hasPermissionChecked = 1;
        }

#if UNITY_ANDROID
        // 게임시작 모든 알람 지우기
        AndroidNotificationCenter.CancelAllNotifications();
        AndroidNotificationCenter.CancelAllScheduledNotifications();
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif

        LoadDatas();
    }

    private void SetCollections()
    {
        dataDic = new Dictionary<string, PushNotesDataSO>();
        rewardRecieved = new Dictionary<string, bool>();
    }

    private void LoadDatas()
    {
        PushNotesDataSO[] datas = Resources.LoadAll<PushNotesDataSO>("ScriptableObjects/PushNotesDataSO");

        foreach (PushNotesDataSO data in datas)
        {
            dataDic[data.name] = data;
        }

        LoadRewardRecieved();
    }

    private void LoadRewardRecieved()
    {
        foreach (KeyValuePair<string, PushNotesDataSO> kvp in dataDic)
        {
            rewardRecieved[kvp.Key] = ES3.KeyExists($"PushRewardRecieved_{kvp.Key}") ? ES3.Load<bool>($"PushRewardRecieved_{kvp.Key}") : false;
        }
    }

    private void CheckNotificationPermission()
    {
#if UNITY_ANDROID
        Debug.Log($"Permission.HasUserAuthorizedPermission(\"android.permission.POST_NOTIFICATIONS\") {Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS")}");
        // 푸시 알림 권한이 허용되어 있는지 확인
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            // 권한이 허용되지 않은 경우 권한 요청
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
#elif UNITY_IOS
        // iOS의 경우 권한 요청
        StartCoroutine(RequestAuthorization());
#endif
    }

#if UNITY_IOS
    public IEnumerator<string> RequestAuthorization()
    {
        using (var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            }
        }
    }
#endif
    private void OnApplicationPause(bool pause)
    {
#if UNITY_ANDROID
        if (pause)
        {
            // 알림 예약
            if (Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                OnApplicationPauseEvent?.Invoke();
                SendLocalNotification();
            }

            SaveRewardRecieved();
        }
        else
        {
            // 알림 예약 제거
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.CancelAllScheduledNotifications();
        }
#elif UNITY_IOS
        if (pause)
        {
            // 알림 예약
            OnApplicationPauseEvent?.Invoke();
            SendLocalNotification();
            SaveRewardRecieved();
        }
        else
        {
            // 알림 예약 제거
            iOSNotificationCenter.RemoveAllScheduledNotifications();
        }
#endif
    }

    public void SendLocalNotification(string title, string desc, int minutes)
    {
#if UNITY_ANDROID
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Channel",
            Importance = Importance.Default,
            Description = "Description",
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        AndroidNotificationCenter.SendNotification(new AndroidNotification(title, desc, DateTime.Now.AddMinutes(minutes)), "channel_id");
#elif UNITY_IOS
        var notification = new iOSNotification()
        {
            Title = title,
            Body = desc,
            // Subtitle = "Subtitle",
            ShowInForeground = true,
            ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = new TimeSpan(0, minutes, 0),
                Repeats = false
            }
        };

        iOSNotificationCenter.ScheduleNotification(notification);

#endif
    }

    private void SendLocalNotification()
    {
#if UNITY_ANDROID
        // Android에서만 사용되는 푸시 채널 설정
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Channel",
            Importance = Importance.Default,
            Description = "Description",
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        foreach (KeyValuePair<string, PushNotesDataSO> kvp in dataDic)
        {
            if (rewardRecieved[kvp.Key]) continue;

            Debug.Log($"Push: {kvp.Key}");

            AndroidNotificationCenter.SendNotification(
            new AndroidNotification(kvp.Value.Title, kvp.Value.Desc, DateTime.Now.AddHours(kvp.Value.PushTime)), "channel_id"); // AddHours(1) : 1시간 후 알림
        }
#elif UNITY_IOS
        // iOS에서만 사용되는 푸시 채널 설정

        foreach (KeyValuePair<string, PushNotesDataSO> kvp in dataDic)
        {
            if (rewardRecieved[kvp.Key]) continue;

            Debug.Log($"Push: {kvp.Key}");

            var notification = new iOSNotification()
            {
                Title = kvp.Value.Title,
                Body = kvp.Value.Desc,
                // Subtitle = "Subtitle",
                ShowInForeground = true,
                ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
                CategoryIdentifier = "category_a",
                ThreadIdentifier = "thread1",
                Trigger = new iOSNotificationTimeIntervalTrigger()
                {
                    TimeInterval = new TimeSpan(0, kvp.Value.PushTime, 0),
                    Repeats = false
                }
            };

            iOSNotificationCenter.ScheduleNotification(notification);
        }
#endif
    }

    public List<PushNotesDataSO> GetUnrecievedRewardDatas(int hour)
    {
        List<PushNotesDataSO> pushDatas = new List<PushNotesDataSO>();

        foreach (KeyValuePair<string, PushNotesDataSO> kvp in dataDic)
        {
            if (!rewardRecieved[kvp.Key] && kvp.Value.PushTime <= hour)
            {
                pushDatas.Add(kvp.Value);
            }
        }

        return pushDatas;
    }

    public void SetRewardRecieved(string dataName)
    {
        rewardRecieved[dataName] = true;
    }

    private void SaveRewardRecieved()
    {
        foreach (KeyValuePair<string, bool> kvp in rewardRecieved)
        {
            ES3.Save($"PushRewardRecieved_{kvp.Key}", kvp.Value);
        }
    }
}
