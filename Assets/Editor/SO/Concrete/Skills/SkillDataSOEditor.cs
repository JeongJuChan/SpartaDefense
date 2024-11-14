using Keiwando.BigInteger;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillDataSO))]
public class SkillDataSOEditor : ListDataSOEditor<SkillDataSO>
{
    public static void LoadCSVToSO(SkillDataSO skillDataSO, TextAsset csv)
    {
        skillDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<SkillData> skillDatas = new List<SkillData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int skillIndex = int.Parse(elements[0]);
            string skillName = elements[1];
            string skillNameKR = elements[2];
            SkillDamageType skillDamageType = EnumUtility.GetEqualValue<SkillDamageType>(elements[3]);
            SkillTargetingType skillTargetingType = EnumUtility.GetEqualValue<SkillTargetingType>(elements[4]);
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[5]);
            string description = elements[6];
            int level = int.Parse(elements[7]);
            float damagePercent = float.Parse(elements[8]);
            int skillTargetCount = int.Parse(elements[9]);
            float skillDuration = float.Parse(elements[10]);
            float additionalStatPercent = float.Parse(elements[11].Trim('%'));
            float coolTime = float.Parse(elements[12]);
            int maxExp = int.Parse(elements[13]);
            bool isVibrated = bool.Parse(elements[14].Trim('\r'));

            SkillUpgradableData skillUpgradableData = new SkillUpgradableData(damagePercent, skillTargetCount, skillDuration, coolTime);
            skillDatas.Add(new SkillData(skillIndex, skillName, skillNameKR, skillDamageType, skillTargetingType, rank, description, isVibrated, skillUpgradableData));
        }

        skillDataSO.AddDatas(skillDatas);
        skillDataSO.InitDict();
        EditorUtility.SetDirty(skillDataSO);
    }
    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<SkillData> skillDatas = new List<SkillData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int skillIndex = int.Parse(elements[0]);
            string skillName = elements[1];
            string skillNameKR = elements[2];
            SkillDamageType skillDamageType = EnumUtility.GetEqualValue<SkillDamageType>(elements[3]);
            SkillTargetingType skillTargetingType = EnumUtility.GetEqualValue<SkillTargetingType>(elements[4]);
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[5]);
            string description = elements[6];
            int level = int.Parse(elements[7]);
            float damagePercent = float.Parse(elements[8]);
            int skillTargetCount = int.Parse(elements[9]);
            float skillDuration = float.Parse(elements[10]);
            float additionalStatPercent = float.Parse(elements[11].Trim('%'));
            float coolTime = float.Parse(elements[12]);
            int maxExp = int.Parse(elements[13]);
            bool isVibrated = bool.Parse(elements[14].Trim('\r'));

            SkillUpgradableData skillUpgradableData = new SkillUpgradableData(damagePercent, skillTargetCount, skillDuration, coolTime);
            skillDatas.Add(new SkillData(skillIndex, skillName, skillNameKR, skillDamageType, skillTargetingType, rank, description, isVibrated, skillUpgradableData));
        }

        dataSO.AddDatas(skillDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
