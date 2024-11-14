using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrainingSoldierDataSO))]
public class TrainingSoldierDataSOEditor : DataSOEditor<TrainingSoldierDataSO>
{
    public static void LoadCSVToSO(TrainingSoldierDataSO trainingSoldierDataSO, TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[1].Split(',');

        int baseExp = int.Parse(elements[1]);
        int expIncrement = int.Parse(elements[2]);
        int baseItemAmount = int.Parse(elements[3]);
        int itemIncrement = int.Parse(elements[4].Trim('\r'));

        trainingSoldierDataSO.SetData(new TrainingSoldierData(baseExp, expIncrement, baseItemAmount, itemIncrement));
        trainingSoldierDataSO.InitData();
        EditorUtility.SetDirty(trainingSoldierDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        string[] elements = rows[1].Split(',');

        int baseExp = int.Parse(elements[1]);
        int expIncrement = int.Parse(elements[2]);
        int baseItemAmount = int.Parse(elements[3]);
        int itemIncrement = int.Parse(elements[4].Trim('\r'));

        dataSO.SetData(new TrainingSoldierData(baseExp, expIncrement, baseItemAmount, itemIncrement));

        EditorUtility.SetDirty(dataSO);
    }
}
