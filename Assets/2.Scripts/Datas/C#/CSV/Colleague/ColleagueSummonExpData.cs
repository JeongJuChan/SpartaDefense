using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ColleagueSummonExpData
{
    public int level;
    public int exp;

    public ColleagueSummonExpData(int level, int exp)
    {
        this.level = level;
        this.exp = exp;
    }
}
