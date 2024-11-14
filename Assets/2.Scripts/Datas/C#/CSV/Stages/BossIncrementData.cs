using System;

[Serializable]
public struct BossIncrementData
{
    public int hpIncrement;
    public int damageIncrement;
    public int stageIncrementPercent;
    public int hpIncrementPercent;
    public int damageIncrementPercent;

    public BossIncrementData(int hpIncrement, int damageIncrement, int stageIncrementPercent, int hpIncrementPercent, int damageIncrementPercent)
    {
        this.hpIncrement = hpIncrement;
        this.damageIncrement = damageIncrement;
        this.stageIncrementPercent = stageIncrementPercent;
        this.hpIncrementPercent = hpIncrementPercent;
        this.damageIncrementPercent = damageIncrementPercent;
    }
}
