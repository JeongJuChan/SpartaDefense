using System;

[Serializable]
public struct AttributeRankProbabilityData
{
    public int[] attributeProbabilityDatas;

    public AttributeRankProbabilityData(int[] attributeProbabilityDatas)
    {
        this.attributeProbabilityDatas = attributeProbabilityDatas;
    }
}