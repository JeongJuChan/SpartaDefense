using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AttributeData
{
    public Rank rank;
    public AttributeRankProbabilityData rankProbability;
    public AttributeAppliedData attributeAppliedData;

    public AttributeData(Rank rank, AttributeRankProbabilityData rankProbability, AttributeAppliedData attributeAppliedData)
    {
        this.rank = rank;
        this.rankProbability = rankProbability;
        this.attributeAppliedData = attributeAppliedData;
    }
}
