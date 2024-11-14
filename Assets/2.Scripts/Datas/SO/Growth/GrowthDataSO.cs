using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrowthDataSO", menuName = "SO/Growth/GrowthData")]
public class GrowthDataSO : ListDataSO<GrowthData>
{
    private Dictionary<int, GrowthData> growthDataDictByTrainingLevel = new Dictionary<int, GrowthData>();

    public GrowthData GetGrowthData(int level)
    {
        if (!growthDataDictByTrainingLevel.ContainsKey(level))
        {
            return default;
        }

        return growthDataDictByTrainingLevel[level];
    }

    public override void InitDict()
    {
        foreach (GrowthData data in datas)
        {
            if (!growthDataDictByTrainingLevel.ContainsKey(data.growthLevel))
            {
                growthDataDictByTrainingLevel.Add(data.growthLevel, data);
            }
        }
    }

    public int GetGrowthDatasLength()
    {
        return growthDataDictByTrainingLevel.Count;
    }
}
