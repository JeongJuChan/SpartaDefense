using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BarracksLevelDataSO))]
public class BarracksLevelDataSOEditor : DataSOEditor<BarracksLevelDataSO>
{
    public static void LoadCSVToSO(BarracksLevelDataSO barracksLevelDataSO, TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[1].Split(',');

        int level = int.Parse(elements[0]);
        int baseExp = int.Parse(elements[2]);
        int increment = int.Parse(elements[3].Trim('\r'));

        barracksLevelDataSO.SetData(new BarracksLevelData(level, 0, baseExp, increment));
        barracksLevelDataSO.InitData();
        EditorUtility.SetDirty(barracksLevelDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        string[] elements = rows[1].Split(',');

        int level = int.Parse(elements[0]);
        int baseExp = int.Parse(elements[2]);
        int increment = int.Parse(elements[3].Trim('\r'));
        dataSO.SetData(new BarracksLevelData(level, 0, baseExp, increment));

        EditorUtility.SetDirty(dataSO);
    }
}
