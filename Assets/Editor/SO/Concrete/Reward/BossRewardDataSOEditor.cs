using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BossRewardDataSO))]
public class BossRewardDataSOEditor : DataSOEditor<BossRewardDataSO>
{
    public static void LoadCSVToSO(BossRewardDataSO bossRewardDataSO, TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[3].Split(',');

        bossRewardDataSO.SetData(new MonsterRewardData(int.Parse(elements[10]), int.Parse(elements[12]), int.Parse(elements[13].Trim('%')),
            int.Parse(elements[15])));
        bossRewardDataSO.InitData();
        EditorUtility.SetDirty(bossRewardDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[3].Split(',');

        dataSO.SetData(new MonsterRewardData(int.Parse(elements[10]), int.Parse(elements[12]), int.Parse(elements[13].Trim('%')),
            int.Parse(elements[15])));
        dataSO.InitData();
        EditorUtility.SetDirty(dataSO);
    }
}
