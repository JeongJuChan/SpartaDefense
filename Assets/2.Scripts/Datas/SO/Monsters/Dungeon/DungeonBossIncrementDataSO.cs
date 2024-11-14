using Keiwando.BigInteger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonBossIncrementData", menuName = "SO/DungeonBossIncrementData")]
public class DungeonBossIncrementDataSO : ListDataSO<DungeonBossIncrementCSVData>
{
    private Dictionary<DungeonType, DungeonBossIncrementData> dungeonBossDataDict = new Dictionary<DungeonType, DungeonBossIncrementData>();

    public DungeonBossIncrementData GetDungeonBossIncrementData(DungeonType dungeonType)
    {
        if (dungeonBossDataDict.Count == 0)
        {
            InitDict();
        }

        return dungeonBossDataDict[dungeonType];
    }

    public override void InitDict()
    {
        dungeonBossDataDict.Clear();

        foreach (var data in datas)
        {
            DungeonBossIncrementData dungeonBossIncrementData = new DungeonBossIncrementData(data.dungeonType, new BigInteger(data.damage),
                new BigInteger(data.health), new BigInteger(data.dungeonRewardCSVData), data.battlePercentMod, data.rewardPercentMod);

            if (!dungeonBossDataDict.ContainsKey(data.dungeonType))
            {
                dungeonBossDataDict.Add(data.dungeonType, dungeonBossIncrementData);
            }
        }
    }
}
