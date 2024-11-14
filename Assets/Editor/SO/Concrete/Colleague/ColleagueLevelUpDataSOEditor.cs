using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColleagueLevelUpDataSO))]
public class ColleagueLevelUpDataSOEditor : ListDataSOEditor<ColleagueLevelUpDataSO>
{
    public static void LoadCSVToSO(ColleagueLevelUpDataSO colleagueLevelUpDataSO, TextAsset csv)
    {
        colleagueLevelUpDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        List<ColleagueLevelUpData> colleagueEnforceDatas = new List<ColleagueLevelUpData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[0]);
            int baseColleagueEnforceCost = int.Parse(elements[1]);
            int colleagueEnforceIncrement = int.Parse(elements[2]);

            colleagueEnforceDatas.Add(new ColleagueLevelUpData(rank, baseColleagueEnforceCost, colleagueEnforceIncrement));
        }

        colleagueLevelUpDataSO.AddDatas(colleagueEnforceDatas);

        EditorUtility.SetDirty(colleagueLevelUpDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<ColleagueLevelUpData> colleagueEnforceDatas = new List<ColleagueLevelUpData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[0]);
            int baseColleagueEnforceCost = int.Parse(elements[1]);
            int colleagueEnforceIncrement = int.Parse(elements[2]);

            colleagueEnforceDatas.Add(new ColleagueLevelUpData(rank, baseColleagueEnforceCost, colleagueEnforceIncrement));
        }

        dataSO.AddDatas(colleagueEnforceDatas);

        EditorUtility.SetDirty(dataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }
}
