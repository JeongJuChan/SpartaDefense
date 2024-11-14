using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CastleDoorDurationData", menuName = "SO/CastleDoorDurationData")]
public class CastleDoorDurationDataSO : ListDataSO<CastleDoorDurationData>
{
    private Dictionary<int, CastleDoorDurationData> castleDurationDict = new Dictionary<int, CastleDoorDurationData>();

    public CastleDoorDurationData GetCastleDoorDurationData(int level)
    {
        if (castleDurationDict.Count == 0)
        {
            InitDict();
        }

        return castleDurationDict[level];
    }

    public override void InitDict()
    {
        foreach (CastleDoorDurationData data in datas)
        {
            if (!castleDurationDict.ContainsKey(data.level))
            {
                castleDurationDict.Add(data.level, data);
            }
        }
    }
}
