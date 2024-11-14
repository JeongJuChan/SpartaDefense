using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockSlotByStageData", menuName = "SO/UnlockSlotByStageData")]
public class UnlockSlotByStageDataSO : ListDataSO<UnlockSlotByStageData>
{
    private Dictionary<int, UnlockSlotByStageData> unlockSlotDataDict = new Dictionary<int, UnlockSlotByStageData>();
    private Dictionary<int, int> unlockSlotStageDict = new Dictionary<int, int>();

    public UnlockSlotByStageData GetUnlockSlotData(int index)
    {
        if (unlockSlotDataDict.Count == 0)
        {
            InitDict();
        }

        if (!unlockSlotDataDict.ContainsKey(index))
        {
            return default;
        }

        return unlockSlotDataDict[index];
    }

    public List<UnlockSlotByStageData> GetUnlockSlotDatas()
    {
        return datas;
    }

    public int GetStageIndex(int forgeNum)
    {
        if (unlockSlotStageDict.Count == 0)
        {
            InitDict();
        }

        return unlockSlotStageDict[forgeNum];
    }

    public override void InitDict()
    {
        unlockSlotDataDict.Clear();
        unlockSlotStageDict.Clear();

        foreach (var data in datas)
        {
            if (unlockSlotDataDict.Count == 0)
            {
                for (int i = 1; i <= data.forgeNum; i++)
                {
                    unlockSlotStageDict.Add(i, data.stageIndex);
                }
            }

            if (!unlockSlotDataDict.ContainsKey(data.stageIndex))
            {
                unlockSlotDataDict.Add(data.stageIndex, data);
            }

            unlockSlotStageDict[data.forgeNum] = data.stageIndex;
        }
    }
}
