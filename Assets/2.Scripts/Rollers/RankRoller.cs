using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankRoller
{
    private Rank[] ranks;

    private Func<int, CastleDoorRankProbabilityData> OnGetCastleDoorRankProbabilityData;

    private readonly int TOTAL_PROBABILITY;

    public RankRoller(CastleDoorRankProbabilityDataSO castleDoorRankProbabilityDataSO, Func<int, CastleDoorRankProbabilityData> OnGetCastleDoorRankProbabilityData)
    {
        InitRank();
        castleDoorRankProbabilityDataSO.OnGetRandomRank += GetRandomRank;
        this.OnGetCastleDoorRankProbabilityData = OnGetCastleDoorRankProbabilityData;

        CastleDoorRankProbabilityData castleDoorRankProbabilityData = OnGetCastleDoorRankProbabilityData.Invoke(1);
        foreach (int probability in castleDoorRankProbabilityData.rankProbabilities)
        {
            TOTAL_PROBABILITY += probability;
        }

    }

    private Rank GetRandomRank(int level)
    {
        int index = 0;
        CastleDoorRankProbabilityData castleDoorRankProbabilityData =
            OnGetCastleDoorRankProbabilityData.Invoke(level);
        float percent = UnityEngine.Random.Range(0, TOTAL_PROBABILITY);

        foreach (var probablity in castleDoorRankProbabilityData.rankProbabilities)
        {
            percent -= probablity;
            if (percent < 0)
            {
                Rank rank = GetRank(index);
                return rank;
            }
            else
            { 
                index++;
            }
        }
        return default;
    }

    private Rank GetRank(int index)
    {
        return ranks[index];
    }

    private void InitRank()
    {
        Rank[] ranks = (Rank[])Enum.GetValues(typeof(Rank));
        this.ranks = new Rank[ranks.Length - 1];
        for (int i = 1; i < ranks.Length; i++)
        {
            this.ranks[i - 1] = ranks[i];
        }
    }
}
