using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DailyQuestData
{
    public List<RewardType> QuestPointPerRewards;
    public List<int> QuestPointPerRewardAmount;
    public List<int> QuestPointPerRewardPoints;

    public DailyQuestData(List<RewardType> questPointPerRewards, List<int> questPointPerRewardAmount, List<int> questPointPerRewardPoints)
    {
        QuestPointPerRewards = questPointPerRewards;
        QuestPointPerRewardAmount = questPointPerRewardAmount;
        QuestPointPerRewardPoints = questPointPerRewardPoints;
    }
}
