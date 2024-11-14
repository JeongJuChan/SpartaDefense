using JetBrains.Annotations;
using Keiwando.BigInteger;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonBossData", menuName = "SO/DungeonBossData")]
public class DungeonBossDataSO : ListDataSO<DungeonBossCSVData>
{
    private Dictionary<DungeonType, DungeonBossData> dungeonBossDataDict = new Dictionary<DungeonType, DungeonBossData>();
    private Dictionary<int, DungeonBossData> dungeonBossDataIndexDict = new Dictionary<int, DungeonBossData>();

    public DungeonBossData GetDungeonBossData(DungeonType dungeonType)
    {
        if (!dungeonBossDataDict.ContainsKey(dungeonType))
        {
            InitDict();
        }

        return dungeonBossDataDict[dungeonType];
    }

    public DungeonBossData GetDungeonBossData(int index)
    {
        if (!dungeonBossDataIndexDict.ContainsKey(index))
        {
            InitDict();
        }

        return dungeonBossDataIndexDict[index];
    }

    public override void InitDict()
    {
        dungeonBossDataDict.Clear();
        dungeonBossDataIndexDict.Clear();

        foreach (DungeonBossCSVData data in datas)
        {
            MonsterUpgradableCSVData monsterUpgradableCSVData = data.monsterUpgradableCSVData;
            MonsterUpgradableData monsterUpgradableData = new MonsterUpgradableData(new BigInteger(monsterUpgradableCSVData.health), 
                new BigInteger(monsterUpgradableCSVData.damage), monsterUpgradableCSVData.attackSpeedRate, monsterUpgradableCSVData.speed);
            DungeonBossData dungeonBossData = new DungeonBossData(data.index, data.dungeonType, data.dungeonName, data.count, data.waveCount,
                data.duration, monsterUpgradableData, new BigInteger(data.dungeonRewardStr));

            if (!dungeonBossDataDict.ContainsKey(data.dungeonType))
            {
                dungeonBossDataDict.Add(data.dungeonType, dungeonBossData);
            }

            if (!dungeonBossDataIndexDict.ContainsKey(data.index))
            {
                dungeonBossDataIndexDict.Add(data.index, dungeonBossData);
            }
        }
    }
}
