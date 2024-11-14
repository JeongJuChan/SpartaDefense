using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColleagueSummonExpData", menuName = "SO/Colleague/ColleagueSummonExpData")]
public class ColleagueSummonExpDataSO : ListDataSO<ColleagueSummonExpData>
{
    private Dictionary<int, ColleagueSummonExpData> colleagueSummonExpDict = new Dictionary<int, ColleagueSummonExpData>();

    public ColleagueSummonExpData GetData(int level)
    {
        if (colleagueSummonExpDict.Count == 0)
        {
            InitDict();
        }

        return colleagueSummonExpDict[level];
    }

    public override void InitDict()
    {
        foreach (ColleagueSummonExpData data in datas)
        {
            if (!colleagueSummonExpDict.ContainsKey(data.level))
            {
                colleagueSummonExpDict.Add(data.level, data);
            }
        }
    }
}
