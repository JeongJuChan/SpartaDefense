using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ColleagueEncyclopediaIncrementData
{
    public ColleagueEncyclopediaType colleagueEncyclopediaType;
    public int level;
    public int increment;
    public int goalLevelEachElement;

    public ColleagueEncyclopediaIncrementData(ColleagueEncyclopediaType colleagueEncyclopediaType, int level, int increment, 
        int goalLevelEachElement)
    {
        this.colleagueEncyclopediaType = colleagueEncyclopediaType;
        this.level = level;
        this.increment = increment;
        this.goalLevelEachElement = goalLevelEachElement;
    }
}
