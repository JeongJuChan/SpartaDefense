using System;

[Serializable]
public struct ForgeLevelData
{
    public int forgeLevel;
    public int currentExp;
    public int maxExp;

    public ForgeLevelData(int forgeLevel, int currentExp, int maxExp)
    {
        this.forgeLevel = forgeLevel;
        this.currentExp = currentExp;
        this.maxExp = maxExp;
    }
}