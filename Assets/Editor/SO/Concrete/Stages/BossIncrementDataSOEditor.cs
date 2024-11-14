using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BossIncrementDataSO))]
public class BossIncrementDataSOEditor : DataSOEditor<BossIncrementDataSO>
{
    public static void LoadCSVToSO(BossIncrementDataSO bossIncrementDataSO, TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[4].Split(',');

        int hpIncrement = int.Parse(elements[2]);
        int damageIncrement = int.Parse(elements[3]);
        int stageIncrementPercent = int.Parse(elements[6]);
        int hpIncrementPercent = int.Parse(elements[7]);
        int damageIncrementPercent = int.Parse(elements[8].Trim('\r'));

        bossIncrementDataSO.SetData(new BossIncrementData(hpIncrement, damageIncrement, stageIncrementPercent, hpIncrementPercent, damageIncrementPercent));
        bossIncrementDataSO.InitData();
        EditorUtility.SetDirty(bossIncrementDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[4].Split(',');

        int hpIncrement = int.Parse(elements[2]);
        int damageIncrement = int.Parse(elements[3]);
        int stageIncrementPercent = int.Parse(elements[6]);
        int hpIncrementPercent = int.Parse(elements[7]);
        int damageIncrementPercent = int.Parse(elements[8].Trim('\r'));

        dataSO.SetData(new BossIncrementData(hpIncrement, damageIncrement, stageIncrementPercent, hpIncrementPercent, damageIncrementPercent));
        dataSO.InitData();
        EditorUtility.SetDirty(dataSO);
    }
}
