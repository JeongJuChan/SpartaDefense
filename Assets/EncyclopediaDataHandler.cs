using System;
using System.Collections.Generic;
using UnityEngine;

public class EncyclopediaDataHandler : Singleton<EncyclopediaDataHandler>
{
    public event Action<EncyclopediaType, string> OnSlotLevelChangeEvent;
    private Dictionary<string, EncyclopediaCategoryData> equipmentCategoryDatas;
    private Dictionary<string, EncyclopediaCategoryData> skillCategoryDatas;
    private Dictionary<ColleagueEncyclopediaType, ColleagueEncyclopediaData> colleagueEncyclopediaDataDict;
    private Dictionary<ColleagueEncyclopediaType, Dictionary<int, ColleagueEncyclopediaIncrementData>> colleagueEncyclopediaIncrementDataDict;

    public void Init()
    {
        OnSlotLevelChangeEvent = null;
        equipmentCategoryDatas = new Dictionary<string, EncyclopediaCategoryData>();
        skillCategoryDatas = new Dictionary<string, EncyclopediaCategoryData>();
        colleagueEncyclopediaDataDict = new Dictionary<ColleagueEncyclopediaType, ColleagueEncyclopediaData>();
        colleagueEncyclopediaIncrementDataDict = 
            new Dictionary<ColleagueEncyclopediaType, Dictionary<int, ColleagueEncyclopediaIncrementData>>();

        // 데이터 불러오기
        /*LoadData_Equipment();
        LoadData_Skill();*/
        LoadData_Colleague();
    }

    public void SlotLevelChangeEvent(EncyclopediaType type, string key)
    {
        OnSlotLevelChangeEvent?.Invoke(type, key);
    }

    public List<EncyclopediaCategoryData> GetCategoryDatas(EncyclopediaType type)
    {
        switch (type)
        {
            case EncyclopediaType.Equipment:
                return new List<EncyclopediaCategoryData>(equipmentCategoryDatas.Values);
            case EncyclopediaType.Skill:
                return new List<EncyclopediaCategoryData>(skillCategoryDatas.Values);
            default:
                return null;
        }
    }

    public Dictionary<ColleagueEncyclopediaType, ColleagueEncyclopediaData> GetColleagueEncyclopediaDatas()
    {
        return colleagueEncyclopediaDataDict;
    }

    public ColleagueEncyclopediaIncrementData GetColleagueEncyclopediaIncrementData(ColleagueEncyclopediaType colleagueEncyclopediaType, int level)
    {
        if (!colleagueEncyclopediaIncrementDataDict.ContainsKey(colleagueEncyclopediaType))
        {
            return default;
        }

        if (!colleagueEncyclopediaIncrementDataDict[colleagueEncyclopediaType].ContainsKey(level))
        {
            return default;
        }

        return colleagueEncyclopediaIncrementDataDict[colleagueEncyclopediaType][level];
    }

    private void LoadData_Equipment()
    {
        TextAsset equipmentData = Resources.Load<TextAsset>("CSV/Encyclopedia/EquipmentData");
        string[] equipmentDataLines = equipmentData.text.Split('\n');

        for (int i = 1; i < equipmentDataLines.Length; i++)
        {
            List<EncyclopediaSlotData> slotDatas = new List<EncyclopediaSlotData>();

            string line = equipmentDataLines[i];
            string[] data = line.Split(',');
            string categoryName = data[0];
            Enum.TryParse<StatType>(data[1], out StatType abilityType);
            int levelPerAbility = int.Parse(data[2]);
            Enum.TryParse<ArithmeticStatType>(data[3], out ArithmeticStatType arithmeticStatType);


            for (int j = 0; j < 4; j++)
            {
                string slotName = $"{categoryName}_{j}";
                slotDatas.Add(new EncyclopediaSlotData(slotName));
            }

            equipmentCategoryDatas.Add(categoryName, new EncyclopediaCategoryData(categoryName, abilityType, levelPerAbility, arithmeticStatType, slotDatas));
        }
    }

    private void LoadData_Skill()
    {
        TextAsset skillData = Resources.Load<TextAsset>("CSV/Encyclopedia/SkillData");
        string[] skillDataLines = skillData.text.Split('\n');

        for (int i = 1; i < skillDataLines.Length; i++)
        {
            List<EncyclopediaSlotData> slotDatas = new List<EncyclopediaSlotData>();

            string line = skillDataLines[i];
            string[] data = line.Split(',');
            string categoryName = data[0];
            Enum.TryParse<StatType>(data[1], out StatType abilityType);
            int levelPerAbility = int.Parse(data[2]);
            Enum.TryParse<ArithmeticStatType>(data[3], out ArithmeticStatType arithmeticStatType);

            var skillDatas = ResourceManager.instance.skill.skillDataSO.GetSkillDatas(Enum.Parse<Rank>(categoryName));

            for (int j = 0; j < skillDatas.Count; j++)
            {
                slotDatas.Add(new EncyclopediaSlotData($"{skillDatas[j].index}_{categoryName}"));
            }

            skillCategoryDatas.Add(categoryName, new EncyclopediaCategoryData(categoryName, abilityType, levelPerAbility, arithmeticStatType, slotDatas));
        }
    }

    private void LoadData_Colleague()
    {
        TextAsset colleagueEncyclopediaData = Resources.Load<TextAsset>("CSV/Encyclopedia/ColleagueEncyclopediaData CSV");
        string[] colleagueDataLines = colleagueEncyclopediaData.text.Split('\n');

        ColleagueEncyclopediaType preColleagueEncyclopedia = ColleagueEncyclopediaType.None;

        for (int i = 1; i < colleagueDataLines.Length; i++)
        {
            string[] elements = colleagueDataLines[i].Split(',');
            ColleagueEncyclopediaType colleagueEncyclopediaType = EnumUtility.GetEqualValue<ColleagueEncyclopediaType>(elements[0]);
            if (preColleagueEncyclopedia != colleagueEncyclopediaType)
            {
                string colleagueEncyclopediaNameKR = elements[1];
                ColleagueType colleagueType1 = EnumUtility.GetEqualValue<ColleagueType>(elements[3]);
                ColleagueType colleagueType2 = EnumUtility.GetEqualValue<ColleagueType>(elements[4]);
                ColleagueType colleagueType3 = EnumUtility.GetEqualValue<ColleagueType>(elements[5]);
                StatType statType = EnumUtility.GetEqualValue<StatType>(elements[6]);

                ColleagueType[] colleagueTypes = new ColleagueType[3];
                colleagueTypes[0] = colleagueType1;
                colleagueTypes[1] = colleagueType2;
                colleagueTypes[2] = colleagueType3;

                if (!colleagueEncyclopediaDataDict.ContainsKey(colleagueEncyclopediaType))
                {
                    colleagueEncyclopediaDataDict.Add(colleagueEncyclopediaType, 
                        new ColleagueEncyclopediaData(colleagueEncyclopediaType, colleagueEncyclopediaNameKR, 0,
                    colleagueTypes, statType));
                }

                preColleagueEncyclopedia = colleagueEncyclopediaType;
            }

            int level = int.Parse(elements[2]);
            int increment = int.Parse(elements[7]);
            int goalLevel = int.Parse(elements[8].Trim('\r'));

            if (!colleagueEncyclopediaIncrementDataDict.ContainsKey(colleagueEncyclopediaType))
            {
                colleagueEncyclopediaIncrementDataDict.Add(colleagueEncyclopediaType, new Dictionary<int, ColleagueEncyclopediaIncrementData>());
            }

            if (!colleagueEncyclopediaIncrementDataDict[colleagueEncyclopediaType].ContainsKey(level))
            {
                colleagueEncyclopediaIncrementDataDict[colleagueEncyclopediaType].Add(level, new ColleagueEncyclopediaIncrementData(
                    colleagueEncyclopediaType, level, increment, goalLevel));
            }
        }
    }
}
