using System;

[Serializable]
public struct GrowthData
{
    public int growthLevel;
    public int desiredForgeLevel;
    public int levelUpMax;
    public int currencyBasePerGrowthLevel;
    public int currencyIncrementPerGrowthLevel;
    public int[] incrementsPerGrowthLevel;

    public GrowthData(int growthLevel, int desiredForgeLevel, int levelUpMax, int currencyBasePerGrowthLevel, 
        int currencyIncrementPerGrowthLevel, int[] incrementsPerGrowthLevel)
    {
        this.growthLevel = growthLevel;
        this.desiredForgeLevel = desiredForgeLevel;
        this.levelUpMax = levelUpMax;
        this.currencyBasePerGrowthLevel = currencyBasePerGrowthLevel;
        this.currencyIncrementPerGrowthLevel = currencyIncrementPerGrowthLevel;
        this.incrementsPerGrowthLevel = incrementsPerGrowthLevel;
    }
}
