using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColleagueStatDataSO))]
public class ColleagueStatDataSOEditor : ListDataSOEditor<ColleagueStatDataSO>
{
    public static void LoadCSVToSO(ColleagueStatDataSO colleagueStatDataSO, TextAsset csv)
    {
        colleagueStatDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        List<ColleagueStatData> colleagueStatDatas = new List<ColleagueStatData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int index = int.Parse(elements[0]);
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[1]);
            ColleagueType colleagueType = EnumUtility.GetEqualValue<ColleagueType>(elements[2]);
            int maxLevel = int.Parse(elements[3]);
            int damage = int.Parse(elements[4]);
            int hp = int.Parse(elements[5]);
            int defense = int.Parse(elements[6]);
            int statIncrementPerLevel = int.Parse(elements[7]);

            colleagueStatDatas.Add(new ColleagueStatData(index, rank, colleagueType, maxLevel, damage, hp, defense, 
                statIncrementPerLevel));
        }

        colleagueStatDataSO.AddDatas(colleagueStatDatas);
        EditorUtility.SetDirty(colleagueStatDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        List<ColleagueStatData> colleagueStatDatas = new List<ColleagueStatData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int index = int.Parse(elements[0]);
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[1]);
            ColleagueType colleagueType = EnumUtility.GetEqualValue<ColleagueType>(elements[2]);
            int maxLevel = int.Parse(elements[3]);
            int damage = int.Parse(elements[4]);
            int hp = int.Parse(elements[5]);
            int defense = int.Parse(elements[6]);
            int statIncrementPerLevel = int.Parse(elements[7]);

            colleagueStatDatas.Add(new ColleagueStatData(index, rank, colleagueType, maxLevel, damage, hp, defense,
                statIncrementPerLevel));
        }

        dataSO.AddDatas(colleagueStatDatas);
        EditorUtility.SetDirty(dataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }
}
