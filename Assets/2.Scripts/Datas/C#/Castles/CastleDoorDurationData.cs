using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CastleDoorDurationData
{
    public int level;
    public int duration;

    public CastleDoorDurationData(int level, int duration)
    {
        this.level = level;
        this.duration = duration;
    }
}
