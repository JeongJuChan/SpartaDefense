using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/CastleDoorRankProbabilityData", fileName = "CastleDoorRankProbabilityData")]
public class CastleDoorRankProbabilityDataSO : ListDataSO<CastleDoorRankProbabilityData>
{
    private Dictionary<int, CastleDoorRankProbabilityData> castleDoorRankProbabilityDict = 
        new Dictionary<int, CastleDoorRankProbabilityData>();

    private RankRoller rankRoller;

    public event Func<int, Rank> OnGetRandomRank;

    public Rank GetRandomRank(int level)
    {
        if (castleDoorRankProbabilityDict.Count == 0)
        {
            InitDict();
        }

        if (rankRoller == null)
        {
            InitRankRoller();
        }

        return OnGetRandomRank.Invoke(level);
    }

    public Rank GetRandomRank(int level, bool isEquipmentTypeSummoned)
    {
        if (!isEquipmentTypeSummoned)
        {
            return Rank.Common;
        }

        if (castleDoorRankProbabilityDict.Count == 0)
        {
            InitDict();
        }

        if (rankRoller == null)
        {
            InitRankRoller();
        }

        return OnGetRandomRank.Invoke(level);
    }

    private void InitRankRoller()
    {
        rankRoller = new RankRoller(this, GetCastleDoorRankProbabilityData);
    }

    public CastleDoorRankProbabilityData GetCastleDoorRankProbabilityData(int level)
    {
        if (castleDoorRankProbabilityDict.Count == 0)
        {
            InitDict();
        }

        if (castleDoorRankProbabilityDict.ContainsKey(level))
        {
            return castleDoorRankProbabilityDict[level];
        }

        return default;
    }

    public int GetCastleDataCount()
    {
        return datas.Count;
    }

    public override void InitDict()
    {
        castleDoorRankProbabilityDict.Clear();

        foreach (var data in datas)
        {
            if (!castleDoorRankProbabilityDict.ContainsKey(data.castleDoorLevel))
            {
                castleDoorRankProbabilityDict.Add(data.castleDoorLevel, data);
            }
        }
    }
}
