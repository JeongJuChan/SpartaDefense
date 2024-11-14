using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class DailyRewardController : MonoBehaviour
{
    public DailyTimeCalculator dailyTimeCalculator { get; private set; }

    private Dictionary<DungeonType, (CurrencyType, int)> dungeonDailyRewardCountDict;

    private string[] rewardTime;
    private string[] rewardDay;

    [SerializeField] private int rewardHour = 12;
    [SerializeField] private int rewardMinute = 0;
    [SerializeField] private int rewardSecond = 0;

    public void Init()
    {
        dailyTimeCalculator = new DailyTimeCalculator();

        dailyTimeCalculator.InitCompleteTime(rewardHour, rewardMinute, rewardSecond);

        dungeonDailyRewardCountDict = new Dictionary<DungeonType, (CurrencyType, int)>()
        {
            { DungeonType.GoldDungeon, (CurrencyType.GoldDungeonTicket, 2) },
            { DungeonType.GemDungeon, (CurrencyType.GemDungeonTicket, 2) },
            { DungeonType.ForgeTicketDungeon, (CurrencyType.ForgeDungeonTicket, 2) },
            { DungeonType.ColleagueLevelUpStoneDungeon, (CurrencyType.ColleagueLevelUpDungeonTicket, 2) },
        };

        LoadSaveDatas();
    }

    private void Update()
    {
        if (GetIsGetRewardPossible())
        {
            GiveReward();
        }
    }

    private void LoadSaveDatas()
    {
        if (!ES3.KeyExists(Consts.REWARD_DAY, ES3.settings))
        {
            GiveReward();
            return;
        }
        else
        {
            if (!ES3.KeyExists(Consts.REWARD_TIME, ES3.settings))
            {
                GiveReward();
                return;
            }
            else
            {
                rewardDay = ES3.Load<string[]>(Consts.REWARD_DAY, ES3.settings);
                rewardTime = ES3.Load<string[]>(Consts.REWARD_TIME, ES3.settings);

                if (GetIsGetRewardPossible())
                {
                    GiveReward();
                }
            }
        }
    }

    private bool GetIsGetRewardPossible()
    {
        return dailyTimeCalculator.GetIsRewardPossible(rewardDay, rewardTime);
    }

    private void GiveReward()
    {
        foreach (var currencyTuple in dungeonDailyRewardCountDict.Values)
        {
            CurrencyManager.instance.TryUpdateCurrency(currencyTuple.Item1, currencyTuple.Item2);
        }

        rewardDay = Date.GetDaySplit();
        rewardTime = Date.GetTimeSplit();
        ES3.Save<string[]>(Consts.REWARD_DAY, rewardDay, ES3.settings);
        ES3.Save<string[]>(Consts.REWARD_TIME, rewardTime, ES3.settings);

        ES3.StoreCachedFile();
    }

    public int GetDungeonDailyRewardCount(DungeonType dungeonType)
    {
        return dungeonDailyRewardCountDict[dungeonType].Item2;
    }
}
