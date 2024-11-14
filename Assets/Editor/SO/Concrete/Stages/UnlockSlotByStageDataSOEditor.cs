using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnlockSlotByStageDataSO))]
public class UnlockSlotByStageDataSOEditor : ListDataSOEditor<UnlockSlotByStageDataSO>
{
    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<UnlockSlotByStageData> unlockSlotByStageDatas = new List<UnlockSlotByStageData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int stageIndex = int.Parse(elements[1]);
            bool isForgeNumEmpty = elements[2] == "";
            bool isSkillNumEmpty = elements[3] == "" || elements[3] == "\r";
            int forgeNum = isForgeNumEmpty ? 0 : int.Parse(elements[2].Trim('번'));
            int skillNum = isSkillNumEmpty ? 0 : int.Parse(elements[3].Trim('번', '\r'));

            if (isForgeNumEmpty && isSkillNumEmpty)
            {
                continue;
            }

            UnlockSlotByStageData unlockSlotByStageData = new UnlockSlotByStageData(stageIndex, forgeNum, skillNum);

            unlockSlotByStageDatas.Add(unlockSlotByStageData);
        }

        dataSO.AddDatas(unlockSlotByStageDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
