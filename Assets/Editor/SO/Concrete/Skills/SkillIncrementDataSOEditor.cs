using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillIncrementDataSO))]
public class SkillIncrementDataSOEditor : ListDataSOEditor<SkillIncrementDataSO>
{
    public static void LoadCSVToSO(SkillIncrementDataSO skillIncrementDataSO, TextAsset csv)
    {
        skillIncrementDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<SkillIncrementData> skillIncrementDatas = new List<SkillIncrementData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            int index = int.Parse(elements[0]);
            string name = elements[1];
            SkillTargetingType skillTargetingType = EnumUtility.GetEqualValue<SkillTargetingType>(elements[3]);
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[4]);
            int maxExpIncrement = int.Parse(elements[5]);
            int maxExpIncrementLimit = int.Parse(elements[6]);
            float additionalStatPerecentIncrement = float.Parse(elements[7].Trim('%'));
            float damageIncrement = float.Parse(elements[8].Trim('%', '\r'));

            SkillIncrementData skillIncrementData = new SkillIncrementData(index, name, skillTargetingType, rank, maxExpIncrement,
                maxExpIncrementLimit, additionalStatPerecentIncrement, damageIncrement);

            skillIncrementDatas.Add(skillIncrementData);
        }

        skillIncrementDataSO.AddDatas(skillIncrementDatas);
        skillIncrementDataSO.InitDict();
        EditorUtility.SetDirty(skillIncrementDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<SkillIncrementData> skillIncrementDatas = new List<SkillIncrementData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            int index = int.Parse(elements[0]);
            string name = elements[1];
            SkillTargetingType skillTargetingType = EnumUtility.GetEqualValue<SkillTargetingType>(elements[3]);
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[4]);
            int maxExpIncrement = int.Parse(elements[5]);
            int maxExpIncrementLimit = int.Parse(elements[6]);
            float additionalStatPerecentIncrement = float.Parse(elements[7].Trim('%'));
            float damageIncrement = float.Parse(elements[8].Trim('%', '\r'));

            SkillIncrementData skillIncrementData = new SkillIncrementData(index, name, skillTargetingType, rank, maxExpIncrement,
                maxExpIncrementLimit, additionalStatPerecentIncrement, damageIncrement);

            skillIncrementDatas.Add(skillIncrementData);
        }

        dataSO.AddDatas(skillIncrementDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
