using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterCoreInfoData", menuName = "SO/MonsterCoreInfoData")]
public class MonsterCoreInfoDataSO : ListDataSO<MonsterCoreInfoData>
{
    private Dictionary<int, MonsterCoreInfoData> coreInfoDataDict = new Dictionary<int, MonsterCoreInfoData>();

    public MonsterCoreInfoData GetCoreInfoData(int index)
    {
        if (coreInfoDataDict.Count == 0)
        {
            InitDict();
        }

        if (coreInfoDataDict.ContainsKey(index))
        {
            return coreInfoDataDict[index];
        }

        return default;
    }

    public override void InitDict()
    {
        coreInfoDataDict.Clear();

        foreach (MonsterCoreInfoData data in datas)
        {
            if (!coreInfoDataDict.ContainsKey(data.coreInfoData.index))
            {
                coreInfoDataDict.Add(data.coreInfoData.index, data);
            }
        }
    }
}
