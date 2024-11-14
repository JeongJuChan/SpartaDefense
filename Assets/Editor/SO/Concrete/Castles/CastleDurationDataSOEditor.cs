using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CastleDoorDurationDataSO))]
public class CastleDurationDataSOEditor : ListDataSOEditor<CastleDoorDurationDataSO>
{
    public static void LoadCSVToSO(CastleDoorDurationDataSO castleDoorDurationDataSO, TextAsset csv)
    {
        castleDoorDurationDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<CastleDoorDurationData> castleDurationDatas = new List<CastleDoorDurationData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int level = int.Parse(elements[0]);
            int duration = int.Parse(elements[1]);

            castleDurationDatas.Add(new CastleDoorDurationData(level, duration));
        }

        castleDoorDurationDataSO.AddDatas(castleDurationDatas);
        castleDoorDurationDataSO.InitDict();
        EditorUtility.SetDirty(castleDoorDurationDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<CastleDoorDurationData> castleDurationDatas = new List<CastleDoorDurationData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int level = int.Parse(elements[0]);
            int duration = int.Parse(elements[1]);

            castleDurationDatas.Add(new CastleDoorDurationData(level, duration));
        }

        dataSO.AddDatas(castleDurationDatas);

        EditorUtility.SetDirty(dataSO);
    }
}
