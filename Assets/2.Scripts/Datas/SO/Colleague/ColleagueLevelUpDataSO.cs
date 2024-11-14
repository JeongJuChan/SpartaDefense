using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColleagueLevelUpData", menuName = "SO/Colleague/ColleagueLevelUpData")]
public class ColleagueLevelUpDataSO : ListDataSO<ColleagueLevelUpData>
{
    private Dictionary<Rank, ColleagueLevelUpData> colleagueLevelUpDataDict = new Dictionary<Rank, ColleagueLevelUpData>();

    public ColleagueLevelUpData GetLevelUpData(Rank rank)
    {
        if (colleagueLevelUpDataDict.Count == 0)
        {
            InitDict();
        }

        return colleagueLevelUpDataDict[rank];
    }

    public override void InitDict()
    {
        foreach (ColleagueLevelUpData data in datas)
        {
            if (!colleagueLevelUpDataDict.ContainsKey(data.rank))
            {
                colleagueLevelUpDataDict.Add(data.rank, data);
            }
        }
    }
}
