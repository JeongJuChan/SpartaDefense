using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DungeonBossData
{
    public int index;
    public DungeonType dungeonType;
    public string dungeonName;
    public int count;
    public int waveCount;
    public int duration;
    public MonsterUpgradableData monsterUpgradableData;
    public BigInteger dungeonReward;

    public DungeonBossData(int index, DungeonType dungeonType, string dungeonName, int count, int waveCount, int duration, MonsterUpgradableData monsterUpgradableData,
        BigInteger dungeonReward)
    {
        this.index = index;
        this.dungeonType = dungeonType;
        this.dungeonName = dungeonName;
        this.count = count;
        this.waveCount = waveCount;
        this.duration = duration;
        this.monsterUpgradableData = monsterUpgradableData;
        this.dungeonReward = dungeonReward;
    }
}
