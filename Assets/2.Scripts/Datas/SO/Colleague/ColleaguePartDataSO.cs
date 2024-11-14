using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColleaguePartData", menuName = "SO/Colleague/ColleaguePartData")]
public class ColleaguePartDataSO : ListDataSO<ColleaguePartData>
{
    private Dictionary<Rank, Dictionary<int, int>> colleaguePartDataDict = new Dictionary<Rank, Dictionary<int, int>>();

    public int GetColleaguePartCost(Rank rank, int currentStar)
    {
        if (colleaguePartDataDict.Count == 0)
        {
            InitDict();
        }

        if (!colleaguePartDataDict.ContainsKey(rank))
        {
            return default;
        }

        if (!colleaguePartDataDict[rank].ContainsKey(currentStar))
        {
            return default;
        }

        return colleaguePartDataDict[rank][currentStar];
    }

    public override void InitDict()
    {
        foreach (ColleaguePartData data in datas)
        {
            if (!colleaguePartDataDict.ContainsKey(data.rank))
            {
                colleaguePartDataDict.Add(data.rank, new Dictionary<int, int>());
            }

            colleaguePartDataDict[data.rank].Add(data.currentStar, data.colleaguePartCost);
        }
    }
}
