using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColleagueEncyclopediaData", menuName = "SO/Encyclopedia/ColleagueEncyclopediaData")]
public class ColleagueEncyclopediaDataSO : ListDataSO<ColleagueEncyclopediaData>
{
    private Dictionary<ColleagueEncyclopediaType, ColleagueEncyclopediaData> colleagueEncyclopediaDataDict = 
        new Dictionary<ColleagueEncyclopediaType, ColleagueEncyclopediaData>();

    public override void InitDict()
    {
        foreach (ColleagueEncyclopediaData data in datas)
        {
            if (!colleagueEncyclopediaDataDict.ContainsKey(data.colleagueEncyclopediaType))
            {
                colleagueEncyclopediaDataDict.Add(data.colleagueEncyclopediaType, data);
            }
        }
    }
}
