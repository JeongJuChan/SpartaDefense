using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColleagueEncyclopediaIncrementData", menuName = "SO/Encyclopedia/ColleagueEncyclopediaIncrementData")]
public class ColleagueEncyclopediaIncrementDataSO : ListDataSO<ColleagueEncyclopediaIncrementData>
{
    private Dictionary<ColleagueEncyclopediaType, ColleagueEncyclopediaIncrementData> colleagueEncyclopediaIncrementDataDict = 
        new Dictionary<ColleagueEncyclopediaType, ColleagueEncyclopediaIncrementData>();

    public override void InitDict()
    {
        foreach (ColleagueEncyclopediaIncrementData data in datas)
        {
            if (colleagueEncyclopediaIncrementDataDict.ContainsKey(data.colleagueEncyclopediaType))
            {
                colleagueEncyclopediaIncrementDataDict.Add(data.colleagueEncyclopediaType, data);
            }
        }
    }
}
