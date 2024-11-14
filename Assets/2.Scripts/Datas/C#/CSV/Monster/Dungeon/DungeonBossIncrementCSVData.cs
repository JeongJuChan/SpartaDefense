using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DungeonBossIncrementCSVData
{
    public DungeonType dungeonType;
    public string damage;
    public string health;
    public string dungeonRewardCSVData;
    public int battlePercentMod;
    public int rewardPercentMod;

    public DungeonBossIncrementCSVData(DungeonType dungeonType, string damage, string health, string dungeonRewardCSVData,
        int battlePercentMod, int rewardPercentMod)
    {
        this.dungeonType = dungeonType;
        this.damage = damage;
        this.health = health;
        this.dungeonRewardCSVData = dungeonRewardCSVData;
        this.battlePercentMod = battlePercentMod;
        this.rewardPercentMod = rewardPercentMod;
    }
}
