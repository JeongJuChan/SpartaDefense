using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColleagueSummonExpDataSO))]
public class ColleagueSummonExpDataSOEditor : ListDataSOEditor<ColleagueSummonExpDataSO>
{
    public static void LoadCSVToSO(ColleagueSummonExpDataSO colleagueSummonExpDataSO, TextAsset csv)
    {
        colleagueSummonExpDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        List<ColleagueSummonExpData> colleagueSummonExpDatas = new List<ColleagueSummonExpData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int level = int.Parse(elements[0]);
            int exp = int.Parse(elements[1]);
            colleagueSummonExpDatas.Add(new ColleagueSummonExpData(level, exp));
        }

        colleagueSummonExpDataSO.AddDatas(colleagueSummonExpDatas);
        colleagueSummonExpDataSO.InitDict();
        EditorUtility.SetDirty(colleagueSummonExpDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        List<ColleagueSummonExpData> colleagueSummonExpDatas = new List<ColleagueSummonExpData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int level = int.Parse(elements[0]);
            int exp = int.Parse(elements[1]);
            colleagueSummonExpDatas.Add(new ColleagueSummonExpData(level, exp));
        }

        dataSO.AddDatas(colleagueSummonExpDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
