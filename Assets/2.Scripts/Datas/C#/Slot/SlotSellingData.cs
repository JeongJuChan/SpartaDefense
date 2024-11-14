using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SlotSellingData
{
    public BigInteger expAmount;

    public SlotSellingData(BigInteger expAmount)
    {
        this.expAmount = expAmount;
    }
}
