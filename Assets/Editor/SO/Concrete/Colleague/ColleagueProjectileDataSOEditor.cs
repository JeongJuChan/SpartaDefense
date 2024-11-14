using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColleagueProjectileDataSO))]
public class ColleagueProjectileDataSOEditor : ListDataSOEditor<ColleagueProjectileDataSO>
{
    public static void LoadCSVToSO(ColleagueProjectileDataSO colleagueProjectileDataSO, TextAsset csv)
    {
        colleagueProjectileDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        List<ColleagueProjectileData> colleagueProjectileDatas = new List<ColleagueProjectileData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int index = int.Parse(elements[0]);
            string name = elements[1];
            ColleagueType colleagueType = EnumUtility.GetEqualValue<ColleagueType>(elements[2]);
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[3].Trim('\r'));

            colleagueProjectileDatas.Add(new ColleagueProjectileData(index, name, colleagueType, rank));
        }

        colleagueProjectileDataSO.AddDatas(colleagueProjectileDatas);
        colleagueProjectileDataSO.InitDict();

        EditorUtility.SetDirty(colleagueProjectileDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        List<ColleagueProjectileData> colleagueProjectileDatas = new List<ColleagueProjectileData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int index = int.Parse(elements[0]);
            string name = elements[1];
            ColleagueType colleagueType = EnumUtility.GetEqualValue<ColleagueType>(elements[2]);
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[3]);

            colleagueProjectileDatas.Add(new ColleagueProjectileData(index, name, colleagueType, rank));
        }

        dataSO.AddDatas(colleagueProjectileDatas);

        EditorUtility.SetDirty(dataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }
}
