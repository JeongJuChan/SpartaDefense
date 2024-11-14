using Keiwando.BigInteger;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingElement : CoroutinableUI
{
    [SerializeField] private RemainTimer remainTimer;
    [SerializeField] private Button trainingButton;
    [SerializeField] private Button accelerationButton;
    [SerializeField] private Button getRewardButton;
    [SerializeField] private Button startTimerButton;

    [SerializeField] private Image getRewardImage;
    //[SerializeField] private Image startTimerImage;

    [SerializeField] private TextMeshProUGUI soldierLevelText;
    [SerializeField] private TextMeshProUGUI amountText;

    public event Action<BigInteger> OnUpdateAccelerationTicketCount;

    private DailyTimeCalculator dailyTimeCalculator;

    private bool isTrainingState;

    private int rewardCost;

    public event Action<TrainingElement> OnAccelerationButtonClicked;

    private int rewardMaxAmount;
    private int currentRewardAmount;

    private int timerDuraion = 1;

    public event Action<int> OnRewardReceived;

    private int secondUnitByReward;

    private bool isTrainingDisabled;

    private void OnEnable()
    {
        if (isTrainingState)
        {
            UpdateTrainingInfo();
        }
    }

    private void UpdateTrainingInfo()
    {
        getRewardButton.gameObject.SetActive(true);
        startTimerButton.gameObject.SetActive(false);

        if (ES3.KeyExists($"{Consts.TRAINING_START_TIME}_{name}"))
        {
            string[] trainingStartTime = ES3.Load<string[]>($"{Consts.TRAINING_START_TIME}_{name}", ES3.settings);
            string[] currentTime = null;
            if (trainingStartTime != null)
            {
                string[] startDay = ES3.Load<string[]>($"{Consts.TRAINING_START_DAY}_{name}", Date.GetDaySplit(), ES3.settings);
                int trainingStartTotalSecond = dailyTimeCalculator.GetSecondsByTime(int.Parse(trainingStartTime[0]),
                    int.Parse(trainingStartTime[1]), int.Parse(trainingStartTime[2]));
                currentTime = Date.GetTimeSplit();
                int nowTotalSecond = dailyTimeCalculator.GetSecondsByTime(int.Parse(currentTime[0]), int.Parse(currentTime[1]), int.Parse(currentTime[2]));

                int dayDiffHour = dailyTimeCalculator.GetHourByDayDiff(startDay);

                if (int.Parse(currentTime[0]) != 12)
                {
                    nowTotalSecond += dailyTimeCalculator.GetSecondsByTime(dayDiffHour, 0, 0);
                }
                else
                {
                    nowTotalSecond += dailyTimeCalculator.GetSecondsByTime(dayDiffHour / 2, 0, 0);
                }

                int abtractSecond = nowTotalSecond - trainingStartTotalSecond;
                int maxTime = dailyTimeCalculator.GetSecondsByTime(0, timerDuraion * 25, 0);
                if (abtractSecond >= maxTime)
                {
                    currentRewardAmount = rewardMaxAmount - 1;
                    remainTimer.ActivateTimerText(false);
                    OnTrainingFinished();
                }
                else
                {
                    int additionalReward = abtractSecond / secondUnitByReward;

                    int currentReward = additionalReward + currentRewardAmount;

                    if (currentReward >= rewardMaxAmount)
                    {
                        currentRewardAmount = rewardMaxAmount - 1;
                        EnableGetRewardButton(currentRewardAmount != 0);
                        remainTimer.ActivateTimerText(false);
                        OnTrainingFinished();
                        return;
                    }

                    currentRewardAmount = currentReward;
                    int currentRemainTime = secondUnitByReward - (abtractSecond % secondUnitByReward);

                    EnableGetRewardButton(currentRewardAmount != 0);

                    int currentRemainMinute = currentRemainTime / Consts.MAX_SECOND;
                    int currentRemainSecond = currentRemainTime % Consts.MAX_SECOND;

                    int currentSecond = int.Parse(currentTime[2]) + currentRemainSecond;
                    int currentMinute = int.Parse(currentTime[1]) + currentRemainMinute;
                    int currentHour = int.Parse(currentTime[0]);

                    /*currentSecond = currentSecond % Consts.MAX_SECOND;
                    currentMinute += currentSecond / Consts.MAX_SECOND;
                    currentMinute = currentMinute % Consts.MAX_MINUTE;
                    currentHour += currentMinute / Consts.MAX_MINUTE;*/

                    string[] time = DateTime.Now.ToString().Split('-', '/', ':');
                    if (time[1] == "AM")
                    {
                        currentHour -= Consts.MAX_HOUR;
                    }

                    UpdateRewardAmountText();
                    dailyTimeCalculator.InitCompleteTime(currentHour, currentMinute, currentSecond);

                    //ES3.Save<string[]>($"{Consts.TRANING_COMPLETE_TIME}_{name}", currentTime, ES3.settings);
                    //ES3.StoreCachedFile();

                    remainTimer.ActivateTimerText(true);

                    isTrainingState = true;

                    dailyTimeCalculator.ToggleCalculateRemainTime(this, true, OnTrainingFinished);
                }
            }

        }
    }

    private void OnDisable()
    {
        if (isTrainingState)
        {
            isTrainingDisabled = true;
            //string dateTime = DateTime.Now.ToString();
            //ES3.Save<string[]>($"{Consts.TRAINING_START_DAY}_{name}", dailyTimeCalculator.GetDaySplit(dateTime), ES3.settings);
            //ES3.Save<string[]>($"{Consts.TRAINING_START_TIME}_{name}", dailyTimeCalculator.GetTimeSplit(dateTime), ES3.settings);
            //ES3.StoreCachedFile();
            dailyTimeCalculator.ToggleCalculateRemainTime(this, false, OnTrainingFinished);
        }
    }

    private void SendPushMessage()
    {
        if (currentRewardAmount < rewardMaxAmount)
        {
            if (isTrainingState)
            {
                int rewardDiff = rewardMaxAmount - currentRewardAmount - 1;
                rewardDiff = rewardDiff < 0 ? 0 : rewardDiff;
                int lastMinute = rewardDiff * timerDuraion + dailyTimeCalculator.GetRewardMinutesByTime();
                PushNotificationManager.instance.SendLocalNotification("훈련소 보상 알림", $"병사 훈련이 완료되었습니다.\n 지금 접속하여 훈련 보상을 받아보세요!", lastMinute);
            }
        }
    }

    public void SetCurrencyIcon(Sprite currencyIcon)
    {
        getRewardImage.sprite = currencyIcon;
        //startTimerImage.sprite = currencyIcon;
    }

    public void Init()
    {
        dailyTimeCalculator = new DailyTimeCalculator();
        startTimerButton.onClick.AddListener(OnClickStartTimerButton);
        getRewardButton.onClick.AddListener(OnClickGetRewardButton);

        PushNotificationManager.instance.OnApplicationPauseEvent += SendPushMessage;

        secondUnitByReward = dailyTimeCalculator.GetSecondsByTime(0, timerDuraion, 0);

        isTrainingState = ES3.Load<bool>($"{Consts.IS_TRANING_STATE}_{name}", false, ES3.settings);
        currentRewardAmount = ES3.Load<int>($"{Consts.TRAINING_REWARD_COUNT}_{name}", 0, ES3.settings);
        dailyTimeCalculator.OnUpdateDailyRewardTime += remainTimer.UpdateTime;

        remainTimer.OnGetRemainSecond += dailyTimeCalculator.GetRewardSecondsByTime;

        getRewardButton.gameObject.SetActive(false);
        startTimerButton.gameObject.SetActive(true);

        remainTimer.ActivateTimerText(false);

        UpdateRewardAmountText();

        dailyTimeCalculator.GetRewardSecondsByTime();

        remainTimer.SetOffsetRemainTime(secondUnitByReward);

        if (isTrainingState)
        {
            startTimerButton.gameObject.SetActive(false);
            getRewardButton.gameObject.SetActive(true);
            trainingButton.gameObject.SetActive(false);
            /*string[] accelerationCompleteTime = ES3.Load<string[]>($"{Consts.TRANING_COMPLETE_TIME}_{name}", ES3.settings);
            int hour = int.Parse(accelerationCompleteTime[0]);
            int minute = int.Parse(accelerationCompleteTime[1]);
            int second = int.Parse(accelerationCompleteTime[2]);
            dailyTimeCalculator.InitCompleteTime(hour, minute, second);
            NotificationManager.instance.SetNotification(RedDotIDType.ForgeLevelUp, false);
            */
        }
    }

    public void SetMaxRewardAmount(int maxRewardAmount)
    {
        rewardMaxAmount = maxRewardAmount;
    }

    public void AccelerateWaitDuration(BigInteger amount)
    {
        dailyTimeCalculator.UpdateCompleteTime(0, int.Parse(amount.ToString()), 0, Consts.ACCELERATION_COMPLETE_TIME_FORGE_PROBABILITY);
    }

    public string ConvertTime(BigInteger minutes)
    {
        return dailyTimeCalculator.ConvertTime(0, int.Parse(minutes.ToString()), 0);
    }

    public BigInteger GetUserableTicketCount()
    {
        BigInteger count = dailyTimeCalculator.GetUseableTicketCount();
        BigInteger accelerationTicketCount = CurrencyManager.instance.GetCurrencyValue(CurrencyType.AccelerationTicket);
        count = accelerationTicketCount - count > 0 ? count : accelerationTicketCount;
        return count;
    }

    private void OnClickStartTimerButton()
    {
        QuestManager.instance.UpdateCount(EventQuestType.Training, 1, -1);
        EnableGetRewardButton(false);
        remainTimer.ActivateTimerText(true);
        startTimerButton.gameObject.SetActive(false);
        getRewardButton.gameObject.SetActive(true);
        StartTimer();
    }

    private void EnableGetRewardButton(bool isActive)
    {
        getRewardButton.interactable = isActive;
    }

    private void StartTimer()
    {
        ES3.Save<string[]>($"{Consts.TRAINING_START_TIME}_{name}", Date.GetTimeSplit(), ES3.settings);
        ES3.Save<string[]>($"{Consts.TRAINING_START_DAY}_{name}", Date.GetDaySplit(), ES3.settings);
        string[] accelerationCompleteTime = Date.GetTimeSplit();
        accelerationCompleteTime[1] = (int.Parse(accelerationCompleteTime[1]) + timerDuraion).ToString();

        dailyTimeCalculator.InitCompleteTime(int.Parse(accelerationCompleteTime[0]), int.Parse(accelerationCompleteTime[1]),
            int.Parse(accelerationCompleteTime[2]));
        isTrainingState = true;

        dailyTimeCalculator.ToggleCalculateRemainTime(this, true, OnTrainingFinished);

        Debug.Log($"{accelerationCompleteTime[0]} : {accelerationCompleteTime[1]} : {accelerationCompleteTime[2]}");

        ES3.Save<string[]>($"{Consts.TRANING_COMPLETE_TIME}_{name}", accelerationCompleteTime, ES3.settings);
        ES3.Save<bool>($"{Consts.IS_TRANING_STATE}_{name}", isTrainingState, ES3.settings);
        ES3.StoreCachedFile();
    }

    private void OnClickAccelerationButton()
    {
        /*OnAccelerationButtonClicked?.Invoke(this);
        BigInteger count = GetUserableTicketCount();
        OnUpdateAccelerationTicketCount?.Invoke(count);*/
    }

    private void OnClickGetRewardButton()
    {
        OnRewardReceived?.Invoke(currentRewardAmount);
        if (currentRewardAmount == rewardMaxAmount)
        {
            //remainTimer.ActivateSlider(false);
            startTimerButton.gameObject.SetActive(true);
            getRewardButton.gameObject.SetActive(false);
        }
        else
        {
            remainTimer.UpdateRemainTimeSlider();
            //remainTimer.ActivateSlider(true);
        }
        currentRewardAmount = 0;
        EnableGetRewardButton(false);
        ES3.Save<int>($"{Consts.TRAINING_REWARD_COUNT}_{name}", currentRewardAmount, ES3.settings);
        ES3.StoreCachedFile();
        UpdateRewardAmountText();
        //dailyTimeCalculator.ToggleCalculateRemainTime(this, false, null);
    }

    private void OnTrainingFinished()
    {
        if (rewardMaxAmount > currentRewardAmount)
        {
            currentRewardAmount++;
            EnableGetRewardButton(true);
            ES3.Save<int>($"{Consts.TRAINING_REWARD_COUNT}_{name}", currentRewardAmount, ES3.settings);
        }

        UpdateRewardAmountText();

        if (currentRewardAmount == rewardMaxAmount)
        {
            //remainTimer.ActivateSlider(false);
            dailyTimeCalculator.ToggleCalculateRemainTime(this, false, null);
        }
        else
        {
            startTimerButton.gameObject.SetActive(false);
            getRewardButton.gameObject.SetActive(true);
            if (!isTrainingDisabled)
            {
                StartTimer();
            }
            else
            {
                dailyTimeCalculator.ToggleCalculateRemainTime(this, true, OnTrainingFinished);
                isTrainingDisabled = false;
            }
        }

        ES3.Save<bool>($"{Consts.IS_TRANING_STATE}_{name}", isTrainingState, ES3.settings);
        ES3.StoreCachedFile();
    }
    public void UpdateSoldierLevel(int level)
    {
        //soldierLevelText.text = $"병사\nLv.{level}";
        soldierLevelText.text = $"병사";
    }

    private void UpdateRewardAmountText()
    {
        amountText.text = $"획득량\n{currentRewardAmount}/{rewardMaxAmount}";
    }
}