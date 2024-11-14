using Keiwando.BigInteger;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeProbabilityUIPanel : AccelerationUI
{
    [SerializeField] private CastleDoorLevelDataSO castleDoorLevelDataSO;
    [SerializeField] private CastleDoorDurationDataSO castleDoorDurationDataSO;

    [SerializeField] private Button forgeInfoButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private Image goldImage;
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;

    [SerializeField] private RankProbabilityUIPanel[] rankProbabilityUIPanels;

    [SerializeField] private Button levelUpButton;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TextMeshProUGUI goldAmountText;

    [SerializeField] private ForgeExpUIElement forgeExpUIPrefab;
    [SerializeField] private Transform forgeExpUIPrefabParent;

    [SerializeField] private GameObject forgeProbabilityInsidePanel;

    [SerializeField] private Button otherDisableButton;

    [SerializeField] private GameObject arrowImagePanel;
    [SerializeField] private GameObject nextLevelPanel;
    [SerializeField] private GameObject forgeLevelUpExpPanel;
    [SerializeField] private RemainTimeViewer reaminTimeViewer;
    [SerializeField] private GameObject purchaseButtonPanel;
    [SerializeField] private GameObject levelUpButtonPanel;
    [SerializeField] private GameObject maxLevelText;
    [SerializeField] private GameObject[] nextProbabilityPanels;

    [SerializeField] private Button lockButton;

    public event Func<ForgeLevelData> OnGetForgeLevelData;

    private ForgeLevelData castleLevelData;

    public event Action OnTryLevelUp;
    public event Func<int, CastleDoorRankProbabilityData> OnGetCastleDoorRankProbabilityData;
    public event Func<int> OnGetCastleDoorRankProbabilityCount;
    public event Action OnAddExp;

    private Dictionary<int, string> rankKrIndexDict = new Dictionary<int, string>();

    private List<ForgeExpUIElement> forgeExpUIElements = new List<ForgeExpUIElement>();
    private int currentActiveExpElementIndex;

    private BigInteger purchaseAmount;

    private CastleDoorLevelData castleDoorLevelData;

    [SerializeField] private RemainTimeViewer timeCostViewer;


    [SerializeField] protected Image accelerationTicketImage;
    [SerializeField] protected TextMeshProUGUI accelerationTicketText;

    [SerializeField] private AccelerationUIPopup accelerationUIPopup;

    [SerializeField] private StepGuide stepGuide;

    private bool isCastleLevelUpOpened;

    private FeatureType featureType;
    private int unlockCount;

    private QuestManager questManager;

    public override void Init()
    {
        base.Init();
        forgeInfoButton.GetComponent<GuideController>().Initialize();

        questManager = QuestManager.instance;

        forgeInfoButton.onClick.AddListener(() =>
        {
            ActiveSelf(true);

            stepGuide.NextStep(0);

            DialogManager.instance.HideDialog();

            questManager.UpdateCount(EventQuestType.CastleDoorUpgradeButtonTouch, 1, -1);

            if (isLevelUpProcessing)
            {
                NotificationManager.instance.SetNotification(RedDotIDType.ForgeLevelUp, false);
                return;
            }
            if (currentActiveExpElementIndex >= castleLevelData.maxExp)
            {
                levelUpButton.interactable = true;
                purchaseButton.interactable = false;

                NotificationManager.instance.SetNotification(RedDotIDType.ForgeLevelUp, true);
            }
            else
            {
                CheckPurchaseButtonInteractable();
            }
        });

        forgeInfoButton.onClick.AddListener(UpdateQuestEvent);

        closeButton.onClick.AddListener(() => ActiveSelf(false));
        levelUpButton.onClick.AddListener(OnClickLevelUpButton);

        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.CastleDoorUpgradeButtonTouch,
            () => QuestManager.instance.UpdateCount(EventQuestType.CastleDoorUpgradeButtonTouch,
            PlayerPrefs.HasKey(Consts.HAS_FORGE_PROBABILITY_TOUCHED) ? 1 : 0, -1));
        UnlockData unlockData = ResourceManager.instance.unlockDataSO.GetUnlockData(FeatureID.CastleDoorProbabilityButton);

        lockButton.onClick.AddListener(SendLockMessage);
        featureType = unlockData.featureType;
        unlockCount = unlockData.count;

        UnlockManager.Instance.RegisterFeature(new UnlockableFeature(unlockData.featureType, unlockData.featureID, unlockData.count,
            UnlockPanel));

        isCastleLevelUpOpened = ES3.Load<bool>(Consts.IS_CASTLE_DOOR_LEVEL_UP_OPENED, false, ES3.settings);
        if (isCastleLevelUpOpened)
        {
            UnlockPanel();
        }

        PushNotificationManager.instance.OnApplicationPauseEvent += RegisterPushMessage;
        Currency accelerationTicketCurrency = CurrencyManager.instance.GetCurrency(CurrencyType.AccelerationTicket);
        accelerationTicketCurrency.OnCurrencyChange += UpdateAccelerationTicketText;

        UpdateAccelerationTicketText(accelerationTicketCurrency.GetCurrencyValue());
        accelerationTicketImage.sprite = accelerationTicketCurrency.GetIcon();

        Currency goldCurrency = CurrencyManager.instance.GetCurrency(CurrencyType.Gold);
        goldCurrency.OnCurrencyChange += UpdateGoldText;

        goldImage.sprite = goldCurrency.GetIcon();

        castleDoorLevelData = castleDoorLevelDataSO.GetData();

        purchaseAmount = new BigInteger(castleDoorLevelData.baseGold + castleDoorLevelData.increment);

        purchaseAmount += purchaseAmount * castleDoorLevelData.incrementPercent / Consts.PERCENT_DIVIDE_VALUE;

        purchaseButton.onClick.AddListener(OnClickPurchaseButton);


        dailyTimeCalculator.OnUpdateDailyRewardTime += reaminTimeViewer.UpdateRemainTime;

        InitRankIndexDict();
        UpdateLevelText();
        UpdateGoldAmountText();
        InitRankProbabilityPanelsBackgroundImage();
        UpdatePurchaseAmount();

        otherDisableButton.onClick.AddListener(() => ActiveSelf(false));
        ActiveSelf(false);

        accelerationButton.onClick.AddListener(OnClickAccelerationButton);
        advertisementButton.onClick.AddListener(OnClickAdvertisementButton);
        accelerationUIPopup.OnGetUsableAccelerationTicketCount += GetUserableTicketCount;
        accelerationUIPopup.OnUseTicket += AccelerateWaitDuration;
        OnUpdateAccelerationTicketCount += accelerationUIPopup.SetTicketCount;
        accelerationUIPopup.OnGetAccelTimeText += ConvertTime;

        accelerationUIPopup.Init();

        accelerationPanel.SetActive(false);

        maxLevelText.SetActive(false);

        Initialize();

        castleLevelData = OnGetForgeLevelData.Invoke();

        isLevelUpProcessing = ES3.Load<bool>(Consts.IS_LEVEL_UP_PROCESSING_FORGE_PROBABILITY, false, ES3.settings);


        if (isLevelUpProcessing)
        {
            string[] startDay = ES3.Load<string[]>($"{Consts.ACCELERATION_START_DAY_FORGE_PROBABILITY}_{name}", Date.GetDaySplit(), ES3.settings);

            string[] accelerationCompleteTime = ES3.Load<string[]>(Consts.ACCELERATION_COMPLETE_TIME_FORGE_PROBABILITY, ES3.settings);

            int completeSecond = int.Parse(accelerationCompleteTime[2]);
            int additionalMinute = completeSecond / Consts.MAX_SECOND;
            accelerationCompleteTime[2] = (completeSecond % Consts.MAX_SECOND).ToString();
            int completeMinute = int.Parse(accelerationCompleteTime[1]) + additionalMinute;
            int additionalHour = completeMinute / Consts.MAX_MINUTE;
            accelerationCompleteTime[1] = (completeMinute % Consts.MAX_MINUTE).ToString();
            int completeHour = int.Parse(accelerationCompleteTime[0]) + additionalHour;
            int dayDiff = completeHour / Consts.MAX_HOUR;
            accelerationCompleteTime[0] = (completeHour % Consts.MAX_HOUR).ToString();

            dailyTimeCalculator.InitCompleteTime(int.Parse(accelerationCompleteTime[0]), int.Parse(accelerationCompleteTime[1]),
                int.Parse(accelerationCompleteTime[2]));
            currentActiveExpElementIndex = 0;
            levelUpButton.gameObject.SetActive(false);
            reaminTimeViewer.gameObject.SetActive(true);
            purchaseButtonPanel.SetActive(false);
            forgeLevelUpExpPanel.SetActive(false);
            timeCostViewer.gameObject.SetActive(false);
            accelerationPanel.SetActive(true);
            NotificationManager.instance.SetNotification(RedDotIDType.ForgeLevelUp, false);
            OnUpdateCoroutineState?.Invoke(this, true, OnLevelUpTimerFinished);
            Debug.Log($"몇 분? {dailyTimeCalculator.GetRewardMinutesByTime()}");
        }
    }

    private void UnlockPanel()
    {
        lockButton.gameObject.SetActive(false);
        ES3.Save<bool>(Consts.IS_CASTLE_DOOR_LEVEL_UP_OPENED, true, ES3.settings);
        ES3.StoreCachedFile();
    }

    private void SendLockMessage()
    {
        UnlockManager.Instance.ToastLockMessage(featureType, unlockCount);
    }

    private void UpdateAccelerationTicketText(BigInteger accelerationTicket)
    {
        CurrencyManager.instance.GetCurrencyValue(CurrencyType.AccelerationTicket);
        accelerationTicketText.text = accelerationTicket.ToString();
    }

    private void ShowTimeCost()
    {
        currentTimeCost = castleDoorDurationDataSO.GetCastleDoorDurationData(OnGetForgeLevelData.Invoke().forgeLevel).duration;
        string time = dailyTimeCalculator.ConvertTime(0, currentTimeCost, 0);
        timeCostViewer.UpdateRemainTime(time);
        timeCostViewer.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        if (OnGetForgeLevelData != null)
        {
            castleLevelData = OnGetForgeLevelData.Invoke();
        }

        // if (currentActiveExpElementIndex >= castleLevelData.maxExp)
        // {
        //     stepGuide.NextStep();
        // }
    }

    public void StartInit()
    {
        InstantiateForgeUIElement();
        UpdateCastleProbabilityTexts();
        TryLockLevelUp();
        currentTimeCost = castleLevelData.forgeLevel * Consts.MAX_MINUTE / 2;
    }

    private void TryLockLevelUp()
    {
        if (castleLevelData.forgeLevel >= OnGetCastleDoorRankProbabilityCount.Invoke())
        {
            arrowImagePanel.SetActive(false);
            nextLevelPanel.SetActive(false);
            forgeLevelUpExpPanel.SetActive(false);
            levelUpButtonPanel.SetActive(false);
            purchaseButtonPanel.SetActive(false);
            maxLevelText.SetActive(true);
            foreach (GameObject panel in nextProbabilityPanels)
            {
                panel.SetActive(false);
            }
        }
    }


    private void CheckPurchaseButtonInteractable()
    {
        if (currentActiveExpElementIndex >= castleLevelData.maxExp)
        {
            levelUpButton.interactable = true;
            purchaseButton.gameObject.SetActive(false);
            ShowTimeCost();
            NotificationManager.instance.SetNotification(RedDotIDType.ForgeLevelUp, true);
            NotificationManager.instance.SetNotification(RedDotIDType.ForgePurchase, false);
            return;
        }

        if (CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gold) >= purchaseAmount)
        {
            purchaseButton.interactable = true;
            NotificationManager.instance.SetNotification(RedDotIDType.ForgePurchase, true);
        }
        else
        {
            purchaseButton.interactable = false;
            NotificationManager.instance.SetNotification(RedDotIDType.ForgePurchase, false);
        }
    }

    private void InitRankProbabilityPanelsBackgroundImage()
    {
        Rank[] ranks = (Rank[])Enum.GetValues(typeof(Rank));

        for (int i = 0; i < ranks.Length - 1; i++)
        {
            Sprite rankSprite = ResourceManager.instance.rank.GetRankBackgroundSprite(ranks[i] + 1);
            rankProbabilityUIPanels[i].InitSprites(rankSprite);
        }
    }
    private void InitRankIndexDict()
    {
        for (int i = 0; i < rankProbabilityUIPanels.Length; i++)
        {
            rankKrIndexDict.Add(i, EnumToKRManager.instance.GetEnumToKR((Rank)(i + 1)));
        }
    }

    private void UpdateQuestEvent()
    {
        QuestManager.instance.UpdateCount(EventQuestType.CastleDoorUpgradeButtonTouch, 1, -1);
        PlayerPrefs.SetInt(Consts.HAS_FORGE_PROBABILITY_TOUCHED, 1);
        forgeInfoButton.onClick.RemoveListener(UpdateQuestEvent);
    }

    private void UpdateGoldText(BigInteger gold)
    {
        CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gold);
        goldText.text = gold.ToString();

        CheckPurchaseButtonInteractable();

        if (currentActiveExpElementIndex >= castleLevelData.maxExp)
        {
            if (isCastleLevelUpOpened)
            {
                NotificationManager.instance.SetNotification(RedDotIDType.ForgeLevelUpButton, NotificationManager.instance.GetNotificationState(RedDotIDType.ForgeLevelUp));
            }
            return;
        }

        if (gold >= purchaseAmount)
        {
            if (isLevelUpProcessing)
            {
                NotificationManager.instance.SetNotification(RedDotIDType.ForgePurchase, false);
            }
            else if (currentActiveExpElementIndex < castleLevelData.maxExp)
            {
                NotificationManager.instance.SetNotification(RedDotIDType.ForgePurchase, true);
                if (isCastleLevelUpOpened)
                {
                    NotificationManager.instance.SetNotification(RedDotIDType.ForgeLevelUpButton, NotificationManager.instance.GetNotificationState(RedDotIDType.ForgeLevelUp));
                }
            }
        }
        else
        {
            if (isCastleLevelUpOpened)
            {
                NotificationManager.instance.SetNotification(RedDotIDType.ForgeLevelUpButton, NotificationManager.instance.GetNotificationState(RedDotIDType.ForgeLevelUp));
            }
        }
    }

    public void UpdateLevelText()
    {
        if (OnGetForgeLevelData == null)
        {
            return;
        }

        castleLevelData = OnGetForgeLevelData.Invoke();

        currentLevelText.text = $"현재 Lv.{castleLevelData.forgeLevel}";
        nextLevelText.text = $"다음 Lv.{castleLevelData.forgeLevel + 1}";
    }

    private void InstantiateForgeUIElement()
    {
        while (forgeExpUIElements.Count < castleLevelData.maxExp)
        {
            forgeExpUIElements.Add(Instantiate(forgeExpUIPrefab, forgeExpUIPrefabParent));
        }
    }

    private string GetRankKrByIndex(int index)
    {
        return rankKrIndexDict[index];
    }

    private void OnClickLevelUpButton()
    {
        if (currentActiveExpElementIndex < castleLevelData.maxExp)
        {
            return;
        }

        currentActiveExpElementIndex = 0;
        levelUpButton.gameObject.SetActive(false);
        reaminTimeViewer.gameObject.SetActive(true);
        purchaseButtonPanel.SetActive(false);
        forgeLevelUpExpPanel.SetActive(false);
        timeCostViewer.gameObject.SetActive(false);
        accelerationPanel.SetActive(true);
        SetReaminTime();
        NotificationManager.instance.SetNotification(RedDotIDType.ForgeLevelUp, false);
        NotificationManager.instance.SetNotification(RedDotIDType.ForgeLevelUpButton, false);

        stepGuide.NextStep(2);

        SaveDatas();
    }

    private void SetReaminTime()
    {
        string[] accelerationCompleteTime = Date.GetTimeSplit();
        accelerationCompleteTime[1] = (int.Parse(accelerationCompleteTime[1]) + currentTimeCost).ToString();
        //TODO : 시트 넣어서 바꾸기
        dailyTimeCalculator.InitCompleteTime(int.Parse(accelerationCompleteTime[0]), int.Parse(accelerationCompleteTime[1]),
            int.Parse(accelerationCompleteTime[2]));
        isLevelUpProcessing = true;
        OnUpdateCoroutineState?.Invoke(this, true, OnLevelUpTimerFinished);

        Debug.Log($"{accelerationCompleteTime[0]} : {accelerationCompleteTime[1]} : {accelerationCompleteTime[2]}");

        ES3.Save<string[]>(Consts.ACCELERATION_COMPLETE_TIME_FORGE_PROBABILITY, accelerationCompleteTime, ES3.settings);
        ES3.Save<string[]>($"{Consts.ACCELERATION_START_DAY_FORGE_PROBABILITY}_{name}", Date.GetDaySplit(), ES3.settings);
        ES3.Save<bool>(Consts.IS_LEVEL_UP_PROCESSING_FORGE_PROBABILITY, isLevelUpProcessing, ES3.settings);
        ES3.StoreCachedFile();
    }

    private void OnLevelUpTimerFinished()
    {
        isLevelUpProcessing = false;
        OnUpdateCoroutineState?.Invoke(this, false, null);
        OnTryLevelUp?.Invoke();
        InstantiateForgeUIElement();
        ResetExpElements();
        UpdateLevelText();
        UpdateCastleProbabilityTexts();
        UpdatePurchaseAmount();
        levelUpButton.gameObject.SetActive(true);
        purchaseButtonPanel.gameObject.SetActive(true);
        purchaseButton.gameObject.SetActive(true);
        reaminTimeViewer.gameObject.SetActive(false);
        timeCostViewer.gameObject.SetActive(false);
        forgeLevelUpExpPanel.SetActive(true);
        accelerationPanel.SetActive(false);
        UpdateLevelUpState(false);
        TryLockLevelUp();
        currentActiveExpElementIndex = 0;
        currentTimeCost = castleLevelData.forgeLevel * Consts.MAX_MINUTE / 2;
        ES3.Save<bool>(Consts.IS_LEVEL_UP_PROCESSING_FORGE_PROBABILITY, isLevelUpProcessing, ES3.settings);
        NotificationManager.instance.SetNotification(RedDotIDType.ForgeLevelUp, false);
        SaveDatas();
    }

    private void UpdateCastleProbabilityTexts()
    {
        CastleDoorRankProbabilityData currentCastleDoorRankProbabilityData =
            OnGetCastleDoorRankProbabilityData.Invoke(castleLevelData.forgeLevel);

        CastleDoorRankProbabilityData nextCastleDoorRankProbabilityData =
            OnGetCastleDoorRankProbabilityData.Invoke(castleLevelData.forgeLevel + 1);

        for (int i = 0; i < rankProbabilityUIPanels.Length; i++)
        {
            int nextLevelProability = nextCastleDoorRankProbabilityData.castleDoorLevel == 0 ?
                0 : nextCastleDoorRankProbabilityData.rankProbabilities[i];

            string rankKr = GetRankKrByIndex(i);
            rankProbabilityUIPanels[i].UpdateCurrentRankProbabilityTexts(rankKr,
                currentCastleDoorRankProbabilityData.rankProbabilities[i]);

            rankProbabilityUIPanels[i].UpdateNextRankProbabilityTexts(rankKr, nextLevelProability);
        }
    }

    private void OnClickPurchaseButton()
    {
        BigInteger currentGold = CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gold);
        if (currentGold >= purchaseAmount)
        {
            // TODO: 여기 초록색 바 개수 세이브하기
            forgeExpUIElements[currentActiveExpElementIndex].ActiveFillImage(true);
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gold, -purchaseAmount);
            currentActiveExpElementIndex++;

            OnAddExp?.Invoke();
            castleLevelData = OnGetForgeLevelData.Invoke();
            UpdateLevelText();
            InstantiateForgeUIElement();
            UpdateCastleProbabilityTexts();

            CheckPurchaseButtonInteractable();

            SaveDatas();

            Debug.Log($"{castleLevelData.currentExp}, {castleLevelData.maxExp}");

            if (currentActiveExpElementIndex >= castleLevelData.maxExp)
            {
                levelUpButton.interactable = true;
                purchaseButton.interactable = false;
                NotificationManager.instance.SetNotification(RedDotIDType.ForgeLevelUp, true);

                stepGuide.NextStep(1);
            }
        }
        else
        {
            return;
        }
    }

    private void UpdateLevelUpState(bool isActive)
    {
        CheckPurchaseButtonInteractable();
        levelUpButton.interactable = isActive;
    }

    private void ResetExpElements()
    {
        currentActiveExpElementIndex = 0;
        foreach (var expUIElement in forgeExpUIElements)
        {
            expUIElement.ActiveFillImage(false);
        }
    }

    private void UpdatePurchaseAmount()
    {
        if (castleLevelData.forgeLevel - 1 > 0)
        {
            purchaseAmount = castleDoorLevelData.baseGold + castleDoorLevelData.increment * castleLevelData.forgeLevel;
            purchaseAmount += purchaseAmount * castleDoorLevelData.incrementPercent / Consts.PERCENT_DIVIDE_VALUE;
        }
        UpdateGoldAmountText();
    }

    private void UpdateGoldAmountText()
    {
        goldAmountText.text = purchaseAmount.ToString();
    }

    private void ActiveSelf(bool isActive)
    {
        if (isActive)
        {
            if (GameManager.instance.isInitializing)
            {
                LoadDatas();
                CheckPurchaseButtonInteractable();

                if (isLevelUpProcessing)
                {
                    OnUpdateCoroutineState?.Invoke(this, true, OnLevelUpTimerFinished);
                }
            }
        }
        else
        {
            OnUpdateCoroutineState?.Invoke(this, false, null);
        }

        forgeProbabilityInsidePanel.SetActive(isActive);
        OnChangePopupState?.Invoke(isActive);
        otherDisableButton.gameObject.SetActive(isActive);
    }

    private void OnClickAccelerationButton()
    {
        BigInteger count = GetUserableTicketCount();
        if (count == null)
        {
            return;
        }

        stepGuide.NextStep(3);
        OnUpdateAccelerationTicketCount?.Invoke(count);
    }

    private void OnClickAdvertisementButton()
    {
        AdsManager.instance.ShowRewardedAd($"{CurrencyType.AccelerationTicket}_CastleDoor", (reward, adInfo) =>
        {
            OnPlayAdvertisementFinished();
            Firebase.Analytics.FirebaseAnalytics.LogEvent($"ad__CastleDoor_{CurrencyType.AccelerationTicket}");
            bool isAdsAvailable = AdsManager.instance.GetAdCount($"{CurrencyType.AccelerationTicket}_CastleDoor") < 3;
            advertisementButton.enabled = isAdsAvailable;
            advertisementText.text = $"30분 스킵\n{3 - AdsManager.instance.GetAdCount($"{CurrencyType.AccelerationTicket}_CastleDoor")} / 3";
        });
    }

    private void OnPlayAdvertisementFinished()
    {
        AccelerateWaitDuration(6 * Consts.MINUTE_PER_TICKET);
    }

    public override void AccelerateWaitDuration(BigInteger amount)
    {
        dailyTimeCalculator.UpdateCompleteTime(0, int.Parse(amount.ToString()), 0, Consts.ACCELERATION_COMPLETE_TIME_FORGE_PROBABILITY);
    }

    private void SaveDatas()
    {
        ES3.Save<int>(Consts.CURRNET_ACTIVE_EXP_ELEMENT_INDEX, currentActiveExpElementIndex, ES3.settings);
        ES3.StoreCachedFile();
    }

    private void LoadDatas()
    {
        currentActiveExpElementIndex = ES3.Load<int>(Consts.CURRNET_ACTIVE_EXP_ELEMENT_INDEX, 0);

        for (int i = 0; i < currentActiveExpElementIndex; i++)
        {
            forgeExpUIElements[i].ActiveFillImage(true);
        }
    }

    private void RegisterPushMessage()
    {
        if (isLevelUpProcessing)
        {
            PushNotificationManager.instance.SendLocalNotification("성문 레벨업 알림",
                "성문이 레벨업 되었습니다.\n지금 접속하여 높은 등급의 영웅을 얻어보세요!", dailyTimeCalculator.GetRewardMinutesByTime());
        }
    }

    public int GetCastleDoorProbailityLevel()
    {
        return castleLevelData.forgeLevel;
    }

}
