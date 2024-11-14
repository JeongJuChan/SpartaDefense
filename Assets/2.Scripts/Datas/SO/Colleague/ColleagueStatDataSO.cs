using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColleagueStatData", menuName = "SO/Colleague/ColleagueStatData")]
public class ColleagueStatDataSO : ListDataSO<ColleagueStatData>
{
    private Dictionary<int, ColleagueStatData> colleagueStatDataDict = new Dictionary<int, ColleagueStatData>();

    public ColleagueStatData GetColleagueStatData(int index)
    {
        if (colleagueStatDataDict.Count == 0)
        {
            InitDict();
        }

        return colleagueStatDataDict[index];
    }

    public override void InitDict()
    {
        foreach (ColleagueStatData data in datas)
        {
            if (!colleagueStatDataDict.ContainsKey(data.index))
            {
                colleagueStatDataDict.Add(data.index, data);
            }
        }
    }
}
