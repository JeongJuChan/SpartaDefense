using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DungeonBossCSVData
{
    public int index;
    public DungeonType dungeonType;
    public string dungeonName;
    public int count;
    public int waveCount;
    public int duration;
    public MonsterUpgradableCSVData monsterUpgradableCSVData;
    public string dungeonRewardStr;

    public DungeonBossCSVData(int index, DungeonType dungeonType, string dungeonName, int count, int waveCount, int duration, MonsterUpgradableCSVData monsterUpgradableCSVData,
        string dungeonRewardStr)
    {
        this.index = index;
        this.dungeonType = dungeonType;
        this.dungeonName = dungeonName;
        this.count = count;
        this.waveCount = waveCount;
        this.duration = duration;
        this.monsterUpgradableCSVData = monsterUpgradableCSVData;
        this.dungeonRewardStr = dungeonRewardStr;
    }
}
