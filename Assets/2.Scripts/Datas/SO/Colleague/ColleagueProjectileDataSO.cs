using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColleagueProjectileData", menuName = "SO/Colleague/ColleagueProjectileData")]
public class ColleagueProjectileDataSO : ListDataSO<ColleagueProjectileData>
{
    private Dictionary<ColleagueType, Dictionary<Rank, ColleagueProjectileData>> colleagueProjectileDataDict = new Dictionary<ColleagueType, Dictionary<Rank, ColleagueProjectileData>>();

    public ColleagueProjectileData GetColleagueProjectileData(ColleagueType colleagueType, Rank rank)
    { 
        if (colleagueProjectileDataDict.Count == 0)
        {
            InitDict();
        }

        return colleagueProjectileDataDict[colleagueType][rank];
    }

    public override void InitDict()
    {
        foreach (ColleagueProjectileData data in datas)
        {
            ColleagueType colleagueType = data.colleagueType;
            Rank rank = data.rank;

            if (!colleagueProjectileDataDict.ContainsKey(colleagueType))
            {
                colleagueProjectileDataDict.Add(colleagueType, new Dictionary<Rank, ColleagueProjectileData>());
            }
            
            if (!colleagueProjectileDataDict[colleagueType].ContainsKey(rank))
            {
                colleagueProjectileDataDict[colleagueType].Add(rank, data);
            }
        }
    }
}
