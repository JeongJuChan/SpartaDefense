using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CastleDoorRankProbabilityData
{
    public int castleDoorLevel;
    public int[] rankProbabilities;

    public CastleDoorRankProbabilityData(int castleDoorLevel, int[] rankProbabilities)
    {
        this.castleDoorLevel = castleDoorLevel;
        this.rankProbabilities = rankProbabilities;
    }
}
