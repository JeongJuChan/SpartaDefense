using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ColleaguePartData
{
    public Rank rank;
    public int currentStar;
    public int colleaguePartCost;

    public ColleaguePartData(Rank rank, int currentStar, int colleaguePartCost)
    {
        this.rank = rank;
        this.currentStar = currentStar;
        this.colleaguePartCost = colleaguePartCost;
    }
}
