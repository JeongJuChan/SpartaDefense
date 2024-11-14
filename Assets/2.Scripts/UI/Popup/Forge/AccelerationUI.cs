using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccelerationUI : CoroutinableUI
{
    protected DailyTimeCalculator dailyTimeCalculator;

    public Action<BigInteger> OnUpdateAccelerationTicketCount;

    [SerializeField] protected GameObject accelerationPanel;

    [SerializeField] protected TextMeshProUGUI timeCostText;
    [SerializeField] protected Button advertisementButton;
    [SerializeField] protected TextMeshProUGUI advertisementText;
    [SerializeField] protected Button accelerationButton;


    protected int currentTimeCost;

    protected bool isLevelUpProcessing;

    public virtual void Init()
    {
        dailyTimeCalculator = new DailyTimeCalculator();
        OnUpdateCoroutineState += dailyTimeCalculator.ToggleCalculateRemainTime;
    }

    public string ConvertTime(BigInteger minutes)
    {
        return dailyTimeCalculator.ConvertTime(0, int.Parse(minutes.ToString()), 0);
    }

    public virtual void AccelerateWaitDuration(BigInteger amount)
    {
        dailyTimeCalculator.UpdateCompleteTime(0, int.Parse(amount.ToString()), 0);
    }

    public BigInteger GetUserableTicketCount()
    {
        BigInteger count = dailyTimeCalculator.GetUseableTicketCount();
        BigInteger accelerationTicketCount = CurrencyManager.instance.GetCurrencyValue(CurrencyType.AccelerationTicket);
        count = accelerationTicketCount - count > 0 ? count : accelerationTicketCount;
        return count;
    }

}
