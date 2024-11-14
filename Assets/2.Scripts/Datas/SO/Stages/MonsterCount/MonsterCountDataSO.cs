using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/MonsterCountData", fileName = "MonsterCountData")]
public class MonsterCountDataSO : ListDataSO<MonsterCountData>
{
    private Dictionary<(int, int), List<MonsterCountData>> monsterCountDataDict =
        new Dictionary<(int, int), List<MonsterCountData>>();

    private Dictionary<int, Dictionary<string, CoreInfoData>> stageMonsterCoreInfoDict = new Dictionary<int, Dictionary<string, CoreInfoData>>();

    [SerializeField] private int initCountMod = 2;
    [SerializeField] private int maxCountMod = 5;

    [field: SerializeField] public int bossRoutineStage { get; private set; } = 5;
    [field: SerializeField] public int subMaxStage { get; private set; } = 10;

    private int maxMainStageNum;

    public void UpdateMonstersCoreData(int mainStageNum, Monster[] monsters)
    {
        if (monsterCountDataDict.Count == 0)
        {
            InitDict();
        }

        foreach (var monster in monsters)
        {
            if (stageMonsterCoreInfoDict[mainStageNum].ContainsKey(monster.name))
            {
                monster.SetIndex(stageMonsterCoreInfoDict[mainStageNum][monster.name].index);
            }
        }
    }

    public Dictionary<CoreInfoData, int> GetMainStageMonsterMaxCountData(int mainStageNum)
    {
        List<MonsterCountData> monsterCountDatas = GetMainStageMonsterCountDatas(mainStageNum);
        
        monsterCountDatas.Sort(SortUtility.GetSortDescendingCondition());

        Dictionary<CoreInfoData, int> monsterMaxCountDict = new Dictionary<CoreInfoData, int>();

        foreach (var monsterCountData in monsterCountDatas)
        {
            if (!monsterMaxCountDict.ContainsKey(monsterCountData.coreInfoData))
            {
                monsterMaxCountDict.Add(monsterCountData.coreInfoData, monsterCountData.count);
            }
        }

        return monsterMaxCountDict;
    }

    public Dictionary<string, CoreInfoData> GetStageMonsterCoreDataSet(int currentMainStageNum)
    {
        if (monsterCountDataDict.Count == 0)
        {
            InitDict();
        }

        if (stageMonsterCoreInfoDict.ContainsKey(currentMainStageNum))
        {
            return stageMonsterCoreInfoDict[currentMainStageNum];
        }

        return null;
    }

    public void UpdateStageMonsterCoreInfoDict(MonsterCountData monsterCountData, int currentMainStageNum, int currentRoutineStageNum)
    {
        if (!stageMonsterCoreInfoDict.ContainsKey(currentMainStageNum))
        {
            stageMonsterCoreInfoDict.Add(currentMainStageNum, new Dictionary<string, CoreInfoData>());
        }

        if (!stageMonsterCoreInfoDict[currentMainStageNum].ContainsKey(monsterCountData.coreInfoData.name))
        {
            stageMonsterCoreInfoDict[currentMainStageNum].Add(monsterCountData.coreInfoData.name, monsterCountData.coreInfoData);
        }
    }

    public int GetMainMaxStageNum()
    {
        if (monsterCountDataDict.Count == 0)
        {
            InitDict();
        }


        return maxMainStageNum;
    }

    #region MonsterBaseDataMethods
    public List<MonsterCountData> GetMonsterCountDatas(int mainStageNum, int routineStage)
    {
        if (monsterCountDataDict.Count == 0)
        {
            InitDict();
        }

        if (monsterCountDataDict.ContainsKey((mainStageNum, routineStage)))
        {
            return monsterCountDataDict[(mainStageNum, routineStage)];
        }
        else
        {
            return null;
        }
    }

    public List<MonsterCountData> GetMainStageMonsterCountDatas(int mainStageNum)
    {
        if (monsterCountDataDict.Count == 0)
        {
            InitDict();
        }

        List<MonsterCountData> monsterCountDatas = new List<MonsterCountData>();

        int routineStageNum = 1;

        while (true)
        {
            if (monsterCountDataDict.ContainsKey((mainStageNum, routineStageNum)))
            {
                monsterCountDatas.AddRange(monsterCountDataDict[(mainStageNum, routineStageNum)]);
                routineStageNum++;
            }
            else
            {
                return monsterCountDatas;
            }
        }
    }

    public override void InitDict()
    {
        monsterCountDataDict.Clear();

        foreach (var monsterCountData in datas)
        {
            int mainStageNum = monsterCountData.mainStageNum;
            int routineStageNum = monsterCountData.routineStageNum;

            UpdateStageMonsterCoreInfoDict(monsterCountData, mainStageNum, routineStageNum);

            if (!monsterCountDataDict.ContainsKey((mainStageNum, routineStageNum)))
            {
                monsterCountDataDict.Add((mainStageNum, routineStageNum), new List<MonsterCountData>());
            }

            monsterCountDataDict[(mainStageNum, routineStageNum)].Add(monsterCountData);

            maxMainStageNum = mainStageNum;
        }
    }

    public (int, int) GetPoolCountMod()
    {
        return (initCountMod, maxCountMod);
    }
    #endregion
}
