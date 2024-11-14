using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BossRewardIncrementDataSO))]
public class BossRewardIncrementDataSOEditor : DataSOEditor<BossRewardIncrementDataSO>
{
    public static void LoadCSVToSO(BossRewardIncrementDataSO bossRewardIncrementDataSO, TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[4].Split(',');

        int goldIncrement = int.Parse(elements[10]);
        int goldIncrementPercent = int.Parse(elements[11]);
        int forgeTicketIncrement = int.Parse(elements[13]);
        int forgeTicketIncrementPercent = int.Parse(elements[14].Trim('\r'));

        bossRewardIncrementDataSO.SetData(new MonsterRewardIncrementData(goldIncrement, goldIncrementPercent, forgeTicketIncrement,
            forgeTicketIncrementPercent));

        EditorUtility.SetDirty(bossRewardIncrementDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        string[] elements = rows[4].Split(',');

        int goldIncrement = int.Parse(elements[10]);
        int goldIncrementPercent = int.Parse(elements[11]);
        int forgeTicketIncrement = int.Parse(elements[13]);
        int forgeTicketIncrementPercent = int.Parse(elements[14].Trim('\r'));

        dataSO.SetData(new MonsterRewardIncrementData(goldIncrement, goldIncrementPercent, forgeTicketIncrement,
            forgeTicketIncrementPercent));


        EditorUtility.SetDirty(dataSO);
    }
}
