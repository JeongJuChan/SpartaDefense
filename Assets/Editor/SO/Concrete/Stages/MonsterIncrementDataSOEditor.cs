using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonsterIncrementDataSO))]
public class MonsterIncrementDataSOEditor : DataSOEditor<MonsterIncrementDataSO>
{
    public static void LoadCSVToSO(MonsterIncrementDataSO monsterIncrementDataSO, TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[2].Split(',');
        int hpIncrement = int.Parse(elements[2]);
        int damageIncrement = int.Parse(elements[3]);
        int stageIncrementPercent = int.Parse(elements[6]);
        int hpIncrementPercent = int.Parse(elements[7]);
        int damageIncrementPercent = int.Parse(elements[8]);
        float speedIncrement = float.Parse(elements[9]);

        monsterIncrementDataSO.SetData(new MonsterIncrementData(hpIncrement, damageIncrement, stageIncrementPercent, hpIncrementPercent, damageIncrementPercent, speedIncrement));
        monsterIncrementDataSO.InitData();
        EditorUtility.SetDirty(monsterIncrementDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[2].Split(',');
        int hpIncrement = int.Parse(elements[2]);
        int damageIncrement = int.Parse(elements[3]);
        int stageIncrementPercent = int.Parse(elements[6]);
        int hpIncrementPercent = int.Parse(elements[7]);
        int damageIncrementPercent = int.Parse(elements[8]);
        float speedIncrement = float.Parse(elements[9]);

        dataSO.SetData(new MonsterIncrementData(hpIncrement, damageIncrement, stageIncrementPercent, hpIncrementPercent, damageIncrementPercent, speedIncrement));
        dataSO.InitData();
        EditorUtility.SetDirty(dataSO);
    }
}
