using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ColleagueLevelUpData
{
    public Rank rank;
    public int baseColleagueLevelUpCost;
    public int colleagueLevelUpIncrement;

    public ColleagueLevelUpData(Rank rank, int baseColleagueLevelUpCost, int colleagueLevelUpIncrement)
    {
        this.rank = rank;
        this.baseColleagueLevelUpCost = baseColleagueLevelUpCost;
        this.colleagueLevelUpIncrement = colleagueLevelUpIncrement;
    }
}
