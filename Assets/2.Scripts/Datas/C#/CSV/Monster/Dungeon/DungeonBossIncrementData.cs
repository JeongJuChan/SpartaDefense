using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DungeonBossIncrementData
{
    public DungeonType dungeonType;
    public BigInteger damage;
    public BigInteger health;
    public BigInteger dungeonReward;
    public int battlePercentMod;
    public int rewardPercentMod;

    public DungeonBossIncrementData(DungeonType dungeonType, BigInteger damage, BigInteger health, BigInteger dungeonReward,
        int battlePercentMod, int rewardPercentMod)
    {
        this.dungeonType = dungeonType;
        this.damage = damage;
        this.health = health;
        this.dungeonReward = dungeonReward;
        this.battlePercentMod = battlePercentMod;
        this.rewardPercentMod = rewardPercentMod;
    }
}
