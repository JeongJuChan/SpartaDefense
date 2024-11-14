using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterResourceHandler
{
    private Dictionary<int, GameObject> monsterResourceDict = new Dictionary<int, GameObject>();
    private const string MONSTER_PATH = "Characters/Monsters";

    private Action<int, Monster[]> OnUpdateMonstersIndex;

    private Func<DungeonType, DungeonBossData> OnGetDungeonBossData;
    private Func<int, MonsterCoreInfoData> OnGetMonsterCoreData;

    public MonsterResourceHandler(Action<int, Monster[]> updateMonstersCoreData, 
        Func<DungeonType, DungeonBossData> getDungeonBossData, Func<int, MonsterCoreInfoData> getCoreInfoData)
    {
        OnUpdateMonstersIndex += updateMonstersCoreData;
        OnGetDungeonBossData += getDungeonBossData;
        OnGetMonsterCoreData += getCoreInfoData;
    }

    public GameObject GetResource(int index)
    {
        if (monsterResourceDict.ContainsKey(index))
        {
            return monsterResourceDict[index];
        }

        return null;
    }

    public void LoadStageMonsters(int mainStageNum)
    {
        Monster[] monsters = Resources.LoadAll<Monster>($"{MONSTER_PATH}/Stage{mainStageNum}");

        OnUpdateMonstersIndex?.Invoke(mainStageNum, monsters);

        foreach (var monster in monsters)
        {
            if (!monsterResourceDict.ContainsKey(monster.index))
            {
                monsterResourceDict.Add(monster.index, monster.gameObject);
            }
        }
    }

    public void LoadDungeonMonsters(DungeonType dungeonType)
    {
        DungeonBossData dungeonBossData = OnGetDungeonBossData.Invoke(dungeonType);

        int index = dungeonBossData.index;
        string monsterName = OnGetMonsterCoreData.Invoke(dungeonBossData.index).coreInfoData.name;

        Monster monster = Resources.Load<Monster>($"{MONSTER_PATH}/{dungeonType}/{monsterName}");

        if (!monsterResourceDict.ContainsKey(dungeonBossData.index))
        {
            monsterResourceDict.Add(dungeonBossData.index, monster.gameObject);
            monster.SetIndex(dungeonBossData.index);
        }
    }

    
}