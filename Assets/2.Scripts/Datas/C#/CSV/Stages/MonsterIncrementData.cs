using System;

[Serializable]
public struct MonsterIncrementData
{
    public int hpIncrement;
    public int damageIncrement;
    public int stageIncrementPercent;
    public int hpIncrementPercent;
    public int damageIncrementPercent;
    public float speedIncrement;

    public MonsterIncrementData(int hpIncrement, int damageIncrement, int stageIncrementPercent, int hpIncrementPercent, int damageIncrementPercent, float speedIncrement)
    {
        this.hpIncrement = hpIncrement;
        this.damageIncrement = damageIncrement;
        this.stageIncrementPercent = stageIncrementPercent;
        this.hpIncrementPercent = hpIncrementPercent;
        this.damageIncrementPercent = damageIncrementPercent;
        this.speedIncrement = speedIncrement;
    }
}
