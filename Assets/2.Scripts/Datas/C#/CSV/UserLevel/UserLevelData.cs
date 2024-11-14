using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UserLevelData
{
    public int level;
    public int maxExp;
    public int baseExp;
    public int increment;

    public UserLevelData(int level, int maxExp, int baseExp, int increment)
    {
        this.level = level;
        this.maxExp = maxExp;
        this.baseExp = baseExp;
        this.increment = increment;
    }
}
