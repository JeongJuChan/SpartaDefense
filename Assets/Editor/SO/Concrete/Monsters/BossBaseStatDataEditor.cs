using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BossBaseStatDataSO))]
public class BossBaseStatDataEditor : DataSOEditor<BossBaseStatDataSO>
{
    public static void LoadCSVToSO(BossBaseStatDataSO bossBaseStatDataSO, TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[3].Split(',');

        bossBaseStatDataSO.SetData(new MonsterUpgradableCSVData(elements[2], elements[3], float.Parse(elements[4]), float.Parse(elements[5].Trim('\r'))));
        bossBaseStatDataSO.InitData();
        EditorUtility.SetDirty(bossBaseStatDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[3].Split(',');

        dataSO.SetData(new MonsterUpgradableCSVData(elements[2], elements[3], float.Parse(elements[4]), float.Parse(elements[5].Trim('\r'))));
        dataSO.InitData();
        EditorUtility.SetDirty(dataSO);
    }
}
