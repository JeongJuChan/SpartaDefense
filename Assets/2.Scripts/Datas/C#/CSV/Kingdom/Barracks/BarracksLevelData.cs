using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BarracksLevelData
{
    public int level;
    public int currentExp;
    public int baseMaxExp;
    public int increment;

    public BarracksLevelData(int level, int currentExp, int baseMaxExp, int increment)
    {
        this.level = level;
        this.currentExp = currentExp;
        this.baseMaxExp = baseMaxExp;
        this.increment = increment;
    }
}
