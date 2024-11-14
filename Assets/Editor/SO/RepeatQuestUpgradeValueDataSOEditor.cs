using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RepeatQuestUpgradeValueDataSO))]
public class RepeatQuestUpgradeValueDataSOEditor : ListDataSOEditor<RepeatQuestUpgradeValueDataSO>
{
    public static void LoadCSVToSO(RepeatQuestUpgradeValueDataSO repeatQuestUpgradeValueDataSO, TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);
        int[] repeatQuestUpgradeValues = new int[rows.Length - 1];

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            repeatQuestUpgradeValues[i - 1] = int.Parse(elements[1].Trim('\r'));
        }

        repeatQuestUpgradeValueDataSO.SetRepeatQuestUpgradeValues(repeatQuestUpgradeValues);
        EditorUtility.SetDirty(repeatQuestUpgradeValueDataSO);
    }

    protected override void ClearDatas()
    {
    }

    protected override void LoadCSV(TextAsset csv)
    {
    }
}
