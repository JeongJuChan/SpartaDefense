using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PairCurrencyTypeData
{
    public CurrencyType costCurrencyType;
    public CurrencyType rewardCurrencyType;

    public PairCurrencyTypeData(CurrencyType costCurrencyType, CurrencyType rewardCurrencyType)
    {
        this.costCurrencyType = costCurrencyType;
        this.rewardCurrencyType = rewardCurrencyType;
    }
}
