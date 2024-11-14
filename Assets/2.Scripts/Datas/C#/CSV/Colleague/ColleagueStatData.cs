using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ColleagueStatData
{
    public int index;
    public Rank rank;
    public ColleagueType colleagueType;
    public int maxLevel;
    public int baseDamage;
    public int baseHealth;
    public int baseDefense;
    public int statIncrementPerLevel;

    public ColleagueStatData(int index, Rank rank, ColleagueType colleagueType, int maxLevel, int baseDamage, int baseHp, int baseDefense, 
        int statIncrementPerLevel)
    {
        this.index = index;
        this.rank = rank;
        this.colleagueType = colleagueType;
        this.maxLevel = maxLevel;
        this.baseDamage = baseDamage;
        this.baseHealth = baseHp;
        this.baseDefense = baseDefense;
        this.statIncrementPerLevel = statIncrementPerLevel;
    }
}
