using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DailyQuestSlotData
{
    public DailyQuestType type;
    public string Description;
    public int currentCount;
    public int GoalCount;
    public RewardType reward;
    public int amount;
    public int QuestPoint;
    public bool isCompleted;
    public bool isRewarded;

    public DailyQuestSlotData(DailyQuestType type, string description, int goalCount, RewardType reward, int amount, int questPoint)
    {
        this.type = type;
        Description = description;
        GoalCount = goalCount;
        this.reward = reward;
        this.amount = amount;
        QuestPoint = questPoint;

        currentCount = ES3.Load<int>($"{type}_CurrentCount", 0);
        isCompleted = ES3.Load<bool>($"{type}_IsCompleted", false);
        isRewarded = ES3.Load<bool>($"{type}_IsRewarded", false);
    }
}
