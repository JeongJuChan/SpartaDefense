using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ColleagueProjectileData
{
    public int index;
    public string name;
    public ColleagueType colleagueType;
    public Rank rank;

    public ColleagueProjectileData(int index, string name, ColleagueType colleagueType, Rank rank)
    {
        this.index = index;
        this.name = name;
        this.colleagueType = colleagueType;
        this.rank = rank;
    }
}
