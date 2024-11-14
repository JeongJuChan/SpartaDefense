using Keiwando.BigInteger;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUIPanel : MonoBehaviour
{
    [SerializeField] private RedDotIDType redDotIDType;
    [SerializeField] private Button entranceButton;
    [SerializeField] private Button adsEntranceButton;
    [SerializeField] private TextMeshProUGUI adsEntranceButtonText;
    [SerializeField] private DungeonType dungeonType;
    [SerializeField] private Image dungeonRewardImage;
    [SerializeField] private Image dungeonCostImage;
    [SerializeField] private Image dungeonBackgroundImage;
    [SerializeField] private TextMeshProUGUI dungeonNameText;
    private PairCurrencyTypeData pairCurrencyTypeData;

    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private GameObject lockPanel;
    [SerializeField] private FeatureID featureID;

    public event Func<DungeonType, int> OnGetDailyRewardCurrencyCount;

    public event Action<DungeonType, PairCurrencyTypeData, string> OnOpenDungeonChoicePopup;

    private NotificationManager notificationManager;
    private string dungeonName;
    private bool isLocked = true;

    public void Init(Func<DungeonType, string> GetDungeonName, Func<DungeonType, PairCurrencyTypeData> getPairCurrencyType, 
        Sprite dungeonBackground)
    {
        entranceButton.onClick.AddListener(ShowDungeonChoiceUIPopup);
        adsEntranceButton.onClick.AddListener(ShowAds);

        pairCurrencyTypeData = getPairCurrencyType.Invoke(dungeonType);
        notificationManager = NotificationManager.instance;
        Currency rewardCurrency = CurrencyManager.instance.GetCurrency(pairCurrencyTypeData.rewardCurrencyType);
        dungeonRewardImage.sprite = rewardCurrency.GetIcon();

        dungeonName = GetDungeonName.Invoke(dungeonType);
        InitDungeonName();
        Currency costCurrency = CurrencyManager.instance.GetCurrency(pairCurrencyTypeData.costCurrencyType);
        dungeonCostImage.sprite = costCurrency.GetIcon();
        dungeonBackgroundImage.sprite = dungeonBackground;
        costCurrency.OnCurrencyChange += UpdateCurrency;
    }

    public void InitUnlock()
    {
        isLocked = ES3.Load($"{dungeonType}{Consts.IS_LOCKED}", true, ES3.settings);

        if (isLocked)
        {
            UnlockData unlockData = ResourceManager.instance.unlockDataSO.GetUnlockData(featureID);

            UnlockManager.Instance.RegisterFeature(new UnlockableFeature(unlockData.featureType, unlockData.featureID, unlockData.count, () =>
            {
                isLocked = false;
                
                ES3.Save($"{dungeonType}{Consts.IS_LOCKED}", isLocked, ES3.settings);

                ES3.StoreCachedFile();
                lockPanel.SetActive(isLocked);
            }));
        }


        lockPanel.SetActive(isLocked);
    }

    public bool CheckDungeonAvailability()
    {
        return CurrencyManager.instance.GetCurrencyValue(pairCurrencyTypeData.costCurrencyType) > 0;
    }

    private void InitDungeonName()
    {
        dungeonNameText.text = dungeonName;
    }

    private void ShowDungeonChoiceUIPopup()
    {
        OnOpenDungeonChoicePopup?.Invoke(dungeonType, pairCurrencyTypeData, dungeonName);
    }

    private void ShowAds()
    {
        // Show ads
        AdsManager.instance.ShowRewardedAd($"Ads_{dungeonType}", (reward, adInfo) =>
        {
            CurrencyManager.instance.TryUpdateCurrency(pairCurrencyTypeData.costCurrencyType, 1);
            Firebase.Analytics.FirebaseAnalytics.LogEvent($"ad_dun_{dungeonType}");
        });
    }

    private void UpdateCurrency(BigInteger currencyAmount)
    {
        currencyText.text = $"{currencyAmount}/{OnGetDailyRewardCurrencyCount.Invoke(dungeonType)}";
        notificationManager.SetNotification(redDotIDType, currencyAmount > 0 ? true : false);
        notificationManager.SetNotification(RedDotIDType.ShowDungeonButton, notificationManager.GetNotificationState(RedDotIDType.ShowDungeonButton) || currencyAmount > 0 ? true : false);

        if (currencyAmount <= 0)
        {
            adsEntranceButton.gameObject.SetActive(true);
            entranceButton.gameObject.SetActive(false);
            adsEntranceButtonText.text = $"열쇠 x1\n({3 - (AdsManager.instance.GetAdCount($"Ads_{dungeonType}"))}/3)";
        }
        else
        {
            adsEntranceButton.gameObject.SetActive(false);
            entranceButton.gameObject.SetActive(true);
        }
    }
}
