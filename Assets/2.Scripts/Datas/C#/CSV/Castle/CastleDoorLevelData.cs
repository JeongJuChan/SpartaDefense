using System;

[Serializable]
public struct CastleDoorLevelData
{
    public int baseGold;
    public int increment;
    public int incrementPercent;

    public CastleDoorLevelData(int baseGold, int increment, int incrementPercent)
    {
        this.baseGold = baseGold;
        this.increment = increment;
        this.incrementPercent = incrementPercent;
    }
}
