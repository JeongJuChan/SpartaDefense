using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColleagueEncyclopediaIncrementDataSO))]
public class ColleagueEncyclopediaIncrementDataSOEditor : ListDataSOEditor<ColleagueEncyclopediaIncrementDataSO>
{
    public static void LoadCSVToSO(ColleagueEncyclopediaIncrementDataSO colleagueEncyclopediaIncrementDataSO, TextAsset csv)
    {
        colleagueEncyclopediaIncrementDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<ColleagueEncyclopediaIncrementData> colleagueEncyclopediaIncrementDatas = new List<ColleagueEncyclopediaIncrementData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            ColleagueEncyclopediaType colleagueEncyclopediaType = EnumUtility.GetEqualValue<ColleagueEncyclopediaType>(elements[0]);
            int level = int.Parse(elements[2]);
            int increment = int.Parse(elements[7]);
            int goalLevel = int.Parse(elements[8].Trim('\r'));

            colleagueEncyclopediaIncrementDatas.Add(new ColleagueEncyclopediaIncrementData(colleagueEncyclopediaType, level, increment, goalLevel));
        }

        colleagueEncyclopediaIncrementDataSO.AddDatas(colleagueEncyclopediaIncrementDatas);
        EditorUtility.SetDirty(colleagueEncyclopediaIncrementDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<ColleagueEncyclopediaIncrementData> colleagueEncyclopediaIncrementDatas = new List<ColleagueEncyclopediaIncrementData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            ColleagueEncyclopediaType colleagueEncyclopediaType = EnumUtility.GetEqualValue<ColleagueEncyclopediaType>(elements[0]);
            int level = int.Parse(elements[2]);
            int increment = int.Parse(elements[7]);
            int goalLevel = int.Parse(elements[8].Trim('\r'));

            colleagueEncyclopediaIncrementDatas.Add(new ColleagueEncyclopediaIncrementData(colleagueEncyclopediaType, level, increment, goalLevel));
        }

        dataSO.AddDatas(colleagueEncyclopediaIncrementDatas);
        EditorUtility.SetDirty(dataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();    
    }
}
