using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TrainingSoldierData
{
    public int baseExp;
    public int expIncrement;
    public int baseItemAmount;
    public int itemIncrement;

    public TrainingSoldierData(int baseExp, int expIncrement, int baseItemAmount, int itemIncrement)
    {
        this.baseExp = baseExp;
        this.expIncrement = expIncrement;
        this.baseItemAmount = baseItemAmount;
        this.itemIncrement = itemIncrement;
    }
}
