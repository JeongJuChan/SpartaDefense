using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ColleagueEncyclopediaData
{
    public ColleagueEncyclopediaType colleagueEncyclopediaType;
    public string colleagueEncyclopediaNameKR;
    public int level;
    public ColleagueType[] colleagueTypes;
    public StatType statType;

    public ColleagueEncyclopediaData(ColleagueEncyclopediaType colleagueEncyclopediaType, string colleagueEncyclopediaNameKR,
        int level, ColleagueType[] colleagueTypes, StatType statType)
    {
        this.colleagueEncyclopediaType = colleagueEncyclopediaType;
        this.colleagueEncyclopediaNameKR = colleagueEncyclopediaNameKR;
        this.level = level;
        this.colleagueTypes = colleagueTypes;
        this.statType = statType;
    }
}
