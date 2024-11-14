using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColleaguePartDataSO))]
public class ColleaguePartDataSOEditor : ListDataSOEditor<ColleaguePartDataSO>
{
    public static void LoadCSVToSO(ColleaguePartDataSO colleaguePartDataSO, TextAsset csv)
    {
        colleaguePartDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        List<ColleaguePartData> colleaguePartDatas = new List<ColleaguePartData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[0]);
            int starCount = int.Parse(elements[1]);
            int colleaguePartCost = int.Parse(elements[2]);

            colleaguePartDatas.Add(new ColleaguePartData(rank, starCount, colleaguePartCost));
        }

        colleaguePartDataSO.AddDatas(colleaguePartDatas);

        EditorUtility.SetDirty(colleaguePartDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        List<ColleaguePartData> colleaguePartDatas = new List<ColleaguePartData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[0]);
            int starCount = int.Parse(elements[1]);
            int colleaguePartCost = int.Parse(elements[2]);

            colleaguePartDatas.Add(new ColleaguePartData(rank, starCount, colleaguePartCost));
        }

        dataSO.AddDatas(colleaguePartDatas);

        EditorUtility.SetDirty(dataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }
}
