using Keiwando.BigInteger;
using System;

[Serializable]
public struct AttributeAppliedData
{
    public float[] attributeAppliedStat;

    public AttributeAppliedData(float[] attributeAppliedStat)
    {
        this.attributeAppliedStat = attributeAppliedStat;
    }
}