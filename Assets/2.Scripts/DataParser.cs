using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Keiwando.BigInteger;
using UnityEngine;

public struct EquipmentLevelData
{
    Dictionary<StatType, Dictionary<string, Dictionary<string, string>>> equipmentLevelDatas;

    public EquipmentLevelData(Dictionary<StatType, Dictionary<string, Dictionary<string, string>>> equipmentLevelDatas)
    {
        this.equipmentLevelDatas = equipmentLevelDatas;
    }

    public CurrentEquipmentLevelData GetStatIncrease(StatType equipType, string grade)
    {
        if (!equipmentLevelDatas.ContainsKey(equipType) || !equipmentLevelDatas[equipType].ContainsKey(grade))
        {
            Debug.LogError($"Data not found for type: {equipType}, grade: {grade}");
            return default(CurrentEquipmentLevelData);
        }

        var data = equipmentLevelDatas[equipType][grade];
        return new CurrentEquipmentLevelData(
            new BigInteger(data["statIncrease"]),
            new BigInteger(data["currencyIncrease"]),
            new BigInteger(data["startStat"]),
            new BigInteger(data["startCurrency"])
        );
    }
}

[Serializable]
public struct OfflineRewardConfig
{
    public int StartGates { get; private set; }
    public int GatesPerLevel { get; private set; }
    public float GatesFactorPercent { get; private set; }
    public int BaseGold { get; private set; }
    public int GoldPerLevel { get; private set; }
    public float GoldFactorPercent { get; private set; }
    public int BaseLevelUpStone { get; private set; }
    public int LevelUpStonePerLevel { get; private set; }
    public float LevelUpStoneFactorPercent { get; private set; }
    public int MaxTime { get; private set; }
    public int MinTime { get; private set; }

    public OfflineRewardConfig(string csvLine)
    {
        string[] rows = csvLine.Split('\n');
        string[] values = rows[1].Split(',');
        StartGates = int.Parse(values[0]);
        GatesPerLevel = int.Parse(values[1]);
        GatesFactorPercent = float.Parse(values[2]);
        BaseGold = int.Parse(values[3]);
        GoldPerLevel = int.Parse(values[4]);
        GoldFactorPercent = float.Parse(values[5]);
        BaseLevelUpStone = int.Parse(values[6]);
        LevelUpStonePerLevel = int.Parse(values[7]);
        LevelUpStoneFactorPercent = float.Parse(values[8]);
        MaxTime = int.Parse(values[9]);
        MinTime = int.Parse(values[10]);
    }
}

public class DataParser
{
    public static QuestDatas ParseQuestData(TextAsset data)
    {
        if (data == null)
        {
            throw new ArgumentNullException("data", "Provided TextAsset for quest data is null.");
        }

        var questDatas = new Dictionary<int, QuestInfoData>();
        string[] lines = data.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++) // start at 1 to skip header if present
        {
            string[] fields = lines[i].Split(',');


            try
            {
                QuestInfoData questData = new QuestInfoData
                {
                    Index = int.Parse(fields[0].Trim()),
                    Description = fields[1].Trim(),
                    QuestType = (QuestType)Enum.Parse(typeof(QuestType), fields[2].Trim(), true),
                    EventQuestType = ChangeEventQuestType(fields[3].Trim()),
                    GoalCount = int.Parse(fields[4].Trim()),
                    Importance = int.Parse(fields[5].Trim()),
                    RewardType = (RewardType)Enum.Parse(typeof(RewardType), fields[6].Trim(), true),
                    RewardAmount = int.Parse(fields[7].Trim()),
                    CountResetNeeded = bool.Parse(fields[8].Trim()),
                    IstheFirstQuestOftheType = bool.Parse(fields[9].Trim())
                };

                questDatas.Add(questData.Index, questData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse line {i + 1}: {ex.Message}");
            }
        }

        return new QuestDatas(questDatas);
    }

    private static EventQuestType ChangeEventQuestType(string type)
    {
        return string.IsNullOrEmpty(type) ? EventQuestType.None : (EventQuestType)Enum.Parse(typeof(EventQuestType), type, true);
    }


    public static EquipmentLevelData ParseLevelData(TextAsset data)
    {
        var equipmentLevelDatas = new Dictionary<StatType, Dictionary<string, Dictionary<string, string>>>
    {
        { StatType.Damage, new Dictionary<string, Dictionary<string, string>>() },
        { StatType.HP, new Dictionary<string, Dictionary<string, string>>() }
        // { StatType.Defense, new Dictionary<string, Dictionary<string, string>>() }
    };


        string[] lines = data.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');
            if (fields.Length >= 9)
            {
                var DamageEquipType = StatType.Damage;
                var HPEquipType = StatType.HP;
                var grade = fields[0]; // 등급을 나타내는 필드

                var DamageGradeData = new Dictionary<string, string>
            {
                {"statIncrease", fields[1]},
                {"currencyIncrease", fields[2]},
                {"startStat", fields[3]},
                {"startCurrency", fields[4]}
                // 여기에 더 많은 필드 추가 가능
            };
                var HPGradeData = new Dictionary<string, string>
            {
                {"statIncrease", fields[5]},
                {"currencyIncrease", fields[6]},
                {"startStat", fields[7]},
                {"startCurrency", fields[8]}
                // 여기에 더 많은 필드 추가 가능
            };


                // equipmentLevelDatas[weaponEquiptype].TryAdd(grade, weaponGradeData);
                // Debug.Log("흐음 weapon " + equipmentLevelDatas[weaponEquipType].TryAdd(grade,  weaponGradeData));
                // Debug.Log("흐음 garment " + equipmentLevelDatas[garmentEquipType].TryAdd(grade, garmentGradeData));

                if (!equipmentLevelDatas[DamageEquipType].ContainsKey(grade))
                {
                    equipmentLevelDatas[DamageEquipType].Add(grade, DamageGradeData);
                }
                if (!equipmentLevelDatas[HPEquipType].ContainsKey(grade))
                {
                    equipmentLevelDatas[HPEquipType].Add(grade, HPGradeData);
                }

            }
        }

        return new EquipmentLevelData(equipmentLevelDatas);
    }
    public static Dictionary<Rank, double> ParseOptionGradePercentageData(TextAsset data)
    {
        var gradePercentage = new Dictionary<Rank, double>();

        string[] lines = data.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');

            Rank grade = Enum.Parse<Rank>(fields[0]);
            double percentage = double.Parse(fields[1]);

            gradePercentage[grade] = percentage;
        }

        return gradePercentage;
    }

    public static Dictionary<AbilityOptionEffectType, double> ParseOptionEffectTypePercentageData(TextAsset data)
    {
        var gradePercentage = new Dictionary<AbilityOptionEffectType, double>();

        string[] lines = data.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');

            AbilityOptionEffectType grade = Enum.Parse<AbilityOptionEffectType>(fields[0]);
            double percentage = double.Parse(fields[1]);

            gradePercentage[grade] = percentage;
        }

        return gradePercentage;
    }

    public static Dictionary<AbilityOptionEffectType, Dictionary<Rank, (float min, float max, ArithmeticStatType type)>> ParseOptionPercentageData(TextAsset data)
    {
        var effectIncreaseRanges = new Dictionary<AbilityOptionEffectType, Dictionary<Rank, (float min, float max, ArithmeticStatType type)>>();

        // 데이터를 줄 단위로 분리
        string[] lines = data.text.Split('\n');

        // 첫 번째 줄은 등급의 헤더 정보를 포함하므로, 등급을 분리하여 저장
        Rank[] ranks = lines[0].Split(',')[1..].Select(s => Enum.TryParse(s, out Rank grade) ? grade : default).ToArray();
        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');

            AbilityOptionEffectType effectType = Enum.Parse<AbilityOptionEffectType>(fields[0]);
            ArithmeticStatType statType = Enum.Parse<ArithmeticStatType>(fields[7]);

            if (!effectIncreaseRanges.ContainsKey(effectType))
            {
                effectIncreaseRanges[effectType] = new Dictionary<Rank, (float min, float max, ArithmeticStatType type)>();
            }

            for (int j = 1; j < fields.Length - 1; j++)
            {
                // 각 등급별 최소값과 최대값을 파싱
                string[] range = fields[j].Split('~');
                float min = float.Parse(range[0]);
                float max = float.Parse(range[1]);


                effectIncreaseRanges[effectType][ranks[j - 1]] = (min, max, statType);
            }
        }

        return effectIncreaseRanges;
    }
}
