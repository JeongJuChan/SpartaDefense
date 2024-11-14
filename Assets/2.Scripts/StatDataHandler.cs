using System;
using System.Collections.Generic;
using System.Diagnostics;
using Keiwando.BigInteger;
using UnityEngine;

public struct AdditionalStatData
{
    public Dictionary<StatType, BigInteger> Stats;

    public AdditionalStatData(Dictionary<StatType, BigInteger> initialStats)
    {
        Stats = initialStats;
        // foreach (var stat in initialStats)
        // {
        //     Stats[stat.Key] = stat.Value;
        // }
    }

    public void AddOrSubtract(StatType statType, BigInteger value, bool add)
    {
        if (Stats.ContainsKey(statType))
        {

            Stats[statType] = add ? Stats[statType] + value : Stats[statType] - value;

        }
        else
        {
            Stats[statType] = value;
        }
    }

    public void AddOrSubtract(StatType statType, float value, bool add)
    {
        if (Stats.ContainsKey(statType))
        {

            Stats[statType] = add ? Stats[statType] + new BigInteger(value) : Stats[statType] - new BigInteger(value);
        }
        else
        {
            Stats[statType] = new BigInteger(value);
        }
    }
}


public class StatDataHandler : Singleton<StatDataHandler>
{
    // 0 : base, 1 : rate
    private Dictionary<ColleagueType, AdditionalStatData[]> colleagueStatDict = new Dictionary<ColleagueType, AdditionalStatData[]>();
    private Dictionary<AdditionalStatType, AdditionalStatData[]> additionalStatDicts = new Dictionary<AdditionalStatType, AdditionalStatData[]>();

    public event Action<ArithmeticStatType, AdditionalStatData> OnUpdateTotalAdditionalStat;
    public event Action<SlotEquipmentStatData> OnUpdateEquipmentStatData;
    public event Action<BigInteger> OnUpdateTotalPower;

    private Dictionary<ColleagueType, SlotEquipmentStatData> colleagueEquipmentStatDataDict = new Dictionary<ColleagueType, SlotEquipmentStatData>();

    public event Action<SlotEquipmentStatData> OnUpdateCastleStatData;

    private Dictionary<ColleagueType, BigInteger> skillPowerDict = new Dictionary<ColleagueType, BigInteger>();

    private List<ColleagueType> colleagueTypes = new List<ColleagueType>();

    private BigInteger preColleaguePower = 0;
    private BigInteger preAdditionalPower = 0;

    private bool isColleagueLoaded;
    private bool isColleagueAutoEquipState;

    private AttributeDataHandler attributeDataHandler;

    public event Action<AttributeType, float> OnUpdateAttributeText;

    public event Action<BigInteger> OnUpdateColleaguePower;

    public event Action<ColleagueType, ColleagueUpgradableData> OnGetColleagueUpgradableData;

    private Dictionary<ColleagueType, BigInteger> colleaguePowerDict = new Dictionary<ColleagueType, BigInteger>();

    public void Init()
    {
        additionalStatDicts = new Dictionary<AdditionalStatType, AdditionalStatData[]>();
        colleagueStatDict = new Dictionary<ColleagueType, AdditionalStatData[]>();
        colleagueEquipmentStatDataDict = new Dictionary<ColleagueType, SlotEquipmentStatData>();
        attributeDataHandler = new AttributeDataHandler();
        attributeDataHandler.OnUpdateAttributetext += OnAttributeUpdated;
    }

    public void StartInit()
    {
        isColleagueLoaded = true;   
    }

    public void ModifyStat(ArithmeticStatType arithmeticStatType, AdditionalStatType type, StatType statType, BigInteger value, bool isAddition)
    {
        if (!additionalStatDicts.TryGetValue(type, out var stats))
        {
            stats = new AdditionalStatData[2] {
            new AdditionalStatData(new Dictionary<StatType, BigInteger>()),
            new AdditionalStatData(new Dictionary<StatType, BigInteger>())
        };
            additionalStatDicts[type] = stats;
        }

        stats[(int)arithmeticStatType].AddOrSubtract(statType, value, isAddition);
        UpdateTotalStat(true);
    }

    public void ModifyStat(ArithmeticStatType arithmeticStatType, AdditionalStatType type, StatType statType, int value, bool isAddition)
    {
        if (!additionalStatDicts.TryGetValue(type, out var stats))
        {
            stats = new AdditionalStatData[2] {
            new AdditionalStatData(new Dictionary<StatType, BigInteger>()),
            new AdditionalStatData(new Dictionary<StatType, BigInteger>())
        };
            additionalStatDicts[type] = stats;
        }

        stats[(int)arithmeticStatType].AddOrSubtract(statType, new BigInteger(value), isAddition);
        UpdateTotalStat(true);
    }

    public void ModifyStat(ArithmeticStatType arithmeticStatType, AdditionalStatType type, StatType statType, float value, bool isAddition)
    {
        if (!additionalStatDicts.TryGetValue(type, out var stats))
        {
            stats = new AdditionalStatData[2] {
            new AdditionalStatData(new Dictionary<StatType, BigInteger>()),
            new AdditionalStatData(new Dictionary<StatType, BigInteger>())
        };
            additionalStatDicts[type] = stats;
        }

        if (arithmeticStatType == ArithmeticStatType.Rate)
        {
            Console.WriteLine(" ");
        }

        stats[(int)arithmeticStatType].AddOrSubtract(statType, value, isAddition);
        UpdateTotalStat(true);
    }

    public void ModifyStats(ArithmeticStatType arithmeticStatType, AdditionalStatType type,
        List<KeyValuePair<StatType, BigInteger>> values, bool isAddition, bool isShowingPower)
    {
        if (!additionalStatDicts.TryGetValue(type, out var stats))
        {
            stats = new AdditionalStatData[2] {
            new AdditionalStatData(new Dictionary<StatType, BigInteger>()),
            new AdditionalStatData(new Dictionary<StatType, BigInteger>())
        };
            additionalStatDicts[type] = stats;
        }

        foreach (var value in values)
        {
            stats[(int)arithmeticStatType].AddOrSubtract(value.Key, value.Value, isAddition);
        }

        UpdateTotalStat(isShowingPower);
    }

    public BigInteger GetTotalStat(List<KeyValuePair<StatType, BigInteger>> currentValues, List<KeyValuePair<StatType, BigInteger>> newValues)
    {
        return GetTotalStats(currentValues, newValues);
    }

    public void ModifyStats(ArithmeticStatType arithmeticStatType, ColleagueType type, List<KeyValuePair<StatType, BigInteger>> values, bool isAddition, bool isLastColleague)
    {
        if (!colleagueStatDict.TryGetValue(type, out var stats))
        {
            stats = new AdditionalStatData[2] {
            new AdditionalStatData(new Dictionary<StatType, BigInteger>()),
            new AdditionalStatData(new Dictionary<StatType, BigInteger>())
        };
            colleagueStatDict[type] = stats;
        }

        foreach (var value in values)
        {
            stats[(int)arithmeticStatType].AddOrSubtract(value.Key, value.Value, isAddition);
        }

        UpdateTotalStat(type, isLastColleague);
    }

    public void ModifyStats(ArithmeticStatType arithmeticStatType, ColleagueType colleagueType,
        ColleagueUpgradableData colleagueUpgradableData, bool isAddition)
    {
        if (!colleagueStatDict.TryGetValue(colleagueType, out var stats))
        {
            stats = new AdditionalStatData[2] {
            new AdditionalStatData(new Dictionary<StatType, BigInteger>()),
            new AdditionalStatData(new Dictionary<StatType, BigInteger>())
        };
            colleagueStatDict[colleagueType] = stats;
        }

        stats[(int)arithmeticStatType].AddOrSubtract(StatType.Damage, colleagueUpgradableData.damage, isAddition);
        stats[(int)arithmeticStatType].AddOrSubtract(StatType.HP, colleagueUpgradableData.health, isAddition);
        stats[(int)arithmeticStatType].AddOrSubtract(StatType.Defense, colleagueUpgradableData.defense, isAddition);

        UpdateTotalStat(colleagueType, true);
    }

    public AdditionalStatData[] GetAdditionalStatData(AdditionalStatType type)
    {
        if (!additionalStatDicts.TryGetValue(type, out var stats))
        {
            stats = new AdditionalStatData[2]
            {
                new AdditionalStatData(new Dictionary<StatType, BigInteger>(){ { StatType.Damage, 0}, { StatType.Defense, 0}, { StatType.HP, 0} } ),
                new AdditionalStatData(new Dictionary<StatType, BigInteger>(){ { StatType.Damage, 0}, { StatType.Defense, 0}, { StatType.HP, 0} } )
            };

            additionalStatDicts[type] = stats;
        }
        return additionalStatDicts[type];
    }

    public BigInteger GetDamage(ColleagueType colleagueType)
    {
        if (colleagueEquipmentStatDataDict.ContainsKey(colleagueType))
        {
            return colleagueEquipmentStatDataDict[colleagueType].mainDamage;
        }

        return 0;
    }

    public void UpdateSkillPower(ColleagueType colleagueType, int damagePerecent, ColleagueUpgradableData colleagueUpgradableData)
    {
        if (!skillPowerDict.ContainsKey(colleagueType))
        {
            skillPowerDict.Add(colleagueType, 0);
        }

        BigInteger totalPower = colleagueUpgradableData.damage * damagePerecent / Consts.PERCENT_DIVIDE_VALUE;

        if (totalPower == null)
        {
            totalPower = 0;
        }

        skillPowerDict[colleagueType] = totalPower;

        //UpdateColleaguePower(colleagueType, colleagueUpgradableData);
    }

    private void UpdateColleaguePower(ColleagueType colleagueType, ColleagueUpgradableData colleagueUpgradableData)
    {
        if (!colleaguePowerDict.ContainsKey(colleagueType))
        {
            colleaguePowerDict.Add(colleagueType, 0);
        }

        //colleaguePowerDict[colleagueType] = GetColleaguePower(colleagueType, colleagueUpgradableData);

        OnUpdateColleaguePower?.Invoke(colleaguePowerDict[colleagueType]);

        if (isColleagueLoaded)
        {
            ShowPowerToastMessageByColleague();
        }
    }

    private void UpdateColleaguePower(ColleagueType colleagueType, BigInteger power)
    {
        if (!colleaguePowerDict.ContainsKey(colleagueType))
        {
            colleaguePowerDict.Add(colleagueType, 0);
        }

        colleaguePowerDict[colleagueType] = power;

        OnUpdateColleaguePower?.Invoke(colleaguePowerDict[colleagueType]);
    }

    /*public float GetSkillPower(ColleagueType colleagueType)
    {
        if ()
    }*/

    public BigInteger GetTotalStat()
    {
        UpdateColleagueStats();
        BigInteger totalEquipmentPower = GetTotalAdditionalPowerUpdated();
        totalEquipmentPower += GetTotalSkillPower();

        return totalEquipmentPower;
    }

    public BigInteger GetTotalStatDiff()
    {
        UpdateColleagueStats();
        BigInteger totalEquipmentPower = GetTotalAdditionalPowerUpdated();
        totalEquipmentPower += GetTotalSkillPower();

        return totalEquipmentPower;
    }

    private void UpdateTotalStat(bool isShowingPower)
    {
        UpdateColleagueStats();
        BigInteger totalEquipmentPower = GetTotalAdditionalPowerUpdated();
        //totalEquipmentPower += GetTotalSkillPower();

        if (isShowingPower)
        {
            ShowPowerToastMessage(totalEquipmentPower);
        }
        else
        {
            preAdditionalPower = totalEquipmentPower;
        }
    }

    private BigInteger GetTotalSkillPower()
    {
        BigInteger totalPower = 0;
        foreach (BigInteger power in skillPowerDict.Values)
        {
            totalPower += power;
        }

        return totalPower;
    }

    private BigInteger GetTotalStats(List<KeyValuePair<StatType, BigInteger>> currentValues, List<KeyValuePair<StatType, BigInteger>> newValues)
    {
        UpdateColleagueStats();

        if (currentValues == null)
        {
            BigInteger newTotalEquipmentPower = GetTotalAdditionalPowerUpdated(newValues) + GetTotalSkillPower();

            return GetPowerTotal(newTotalEquipmentPower);
        }
        else
        {
            BigInteger totalSkillPower = GetTotalSkillPower();
            BigInteger currentTotalEquipmentPower = GetTotalAdditionalPowerUpdated(currentValues) + totalSkillPower;
            BigInteger newTotalEquipmentPower = GetTotalAdditionalPowerUpdated(newValues) + totalSkillPower;


            return GetPowerTotal(newTotalEquipmentPower) - GetPowerTotal(currentTotalEquipmentPower);
        }
    }

    private void UpdateColleagueStats()
    {
        colleagueTypes.Clear();
        colleagueTypes.AddRange(colleagueEquipmentStatDataDict.Keys);
        UpdateColleaguesStat();
    }

    private void UpdateColleaguesStat()
    {
        for (int i = 0; i < colleagueTypes.Count; i++)
        {
            if (i == colleagueTypes.Count - 1)
            {
                UpdateTotalStat(colleagueTypes[i], true);
            }
            else
            {
                UpdateTotalStat(colleagueTypes[i], false);
            }
        }
    }

    public BigInteger GetTotalAdditionalPowerUpdated()
    {
        BigInteger totalDamage = 0;
        BigInteger totalHp = 0;
        BigInteger totalDefense = 0;

        foreach (SlotEquipmentStatData slotEquipmentStatData in colleagueEquipmentStatDataDict.Values)
        {
            totalHp += slotEquipmentStatData.health;
            totalDefense += slotEquipmentStatData.defense;
        }

        BigInteger totalPercentDamage = 0;
        BigInteger totalPercentHp = 0;
        BigInteger totalPercentDefense = 0;

        AdditionalStatData additionalStatBaseData = GetTotalAdditionalStatData(ArithmeticStatType.Base);
        AdditionalStatData additionalStatRateData = GetTotalAdditionalStatData(ArithmeticStatType.Rate);

        totalDamage += additionalStatBaseData.Stats.ContainsKey(StatType.Damage) ? additionalStatBaseData.Stats[StatType.Damage] : 0;
        BigInteger additionalPercentDamage = additionalStatRateData.Stats.ContainsKey(StatType.Damage) ?
            additionalStatRateData.Stats[StatType.Damage] : 0;
        totalDamage += GetAdditionalPercentStat(totalDamage, additionalPercentDamage);

        totalHp += additionalStatBaseData.Stats.ContainsKey(StatType.HP) ? additionalStatBaseData.Stats[StatType.HP] : 0;
        BigInteger additionalPercentHp = additionalStatRateData.Stats.ContainsKey(StatType.HP) ?
            additionalStatRateData.Stats[StatType.HP] : 0;
        totalHp += GetAdditionalPercentStat(totalHp, additionalPercentHp);

        totalDefense += additionalStatBaseData.Stats.ContainsKey(StatType.Defense) ? additionalStatBaseData.Stats[StatType.Defense] : 0;
        BigInteger additionalPercentDefense = additionalStatRateData.Stats.ContainsKey(StatType.Defense) ?
            additionalStatRateData.Stats[StatType.Defense] : 0;
        totalDefense += GetAdditionalPercentStat(totalDefense, additionalPercentDefense);

        BigInteger totalEquipmentPower = GetPower(totalDamage, totalHp, totalDefense);

        for (int i = 0; i < colleagueTypes.Count; i++)
        {
            UpdateTotalStat(colleagueTypes[i], false);
        }

        OnUpdateCastleStatData?.Invoke(new SlotEquipmentStatData(0, totalHp, totalDefense, new SlotEquipmentStatDataSave()));

        return totalEquipmentPower;
    }

    public BigInteger GetTotalAdditionalPowerUpdated(List<KeyValuePair<StatType, BigInteger>> values)
    {
        BigInteger totalDamage = 0;
        BigInteger totalHp = 0;
        BigInteger totalDefense = 0;

        foreach (SlotEquipmentStatData slotEquipmentStatData in colleagueEquipmentStatDataDict.Values)
        {
            totalHp += slotEquipmentStatData.health;
            totalDefense += slotEquipmentStatData.defense;
        }

        BigInteger totalPercentDamage = 0;
        BigInteger totalPercentHp = 0;
        BigInteger totalPercentDefense = 0;

        AdditionalStatData additionalStatBaseData = GetTotalAdditionalStatData(ArithmeticStatType.Base);
        AdditionalStatData additionalStatRateData = GetTotalAdditionalStatData(ArithmeticStatType.Rate);

        totalDamage += additionalStatBaseData.Stats.ContainsKey(StatType.Damage) ? additionalStatBaseData.Stats[StatType.Damage] : 0;
        BigInteger additionalPercentDamage = additionalStatRateData.Stats.ContainsKey(StatType.Damage) ?
            additionalStatRateData.Stats[StatType.Damage] : 0;
        totalDamage += GetAdditionalPercentStat(totalDamage, additionalPercentDamage);

        totalHp += additionalStatBaseData.Stats.ContainsKey(StatType.HP) ? additionalStatBaseData.Stats[StatType.HP] : 0;
        BigInteger additionalPercentHp = additionalStatRateData.Stats.ContainsKey(StatType.HP) ?
            additionalStatRateData.Stats[StatType.HP] : 0;
        totalHp += GetAdditionalPercentStat(totalHp, additionalPercentHp);

        totalDefense += additionalStatBaseData.Stats.ContainsKey(StatType.Defense) ? additionalStatBaseData.Stats[StatType.Defense] : 0;
        BigInteger additionalPercentDefense = additionalStatRateData.Stats.ContainsKey(StatType.Defense) ?
            additionalStatRateData.Stats[StatType.Defense] : 0;
        totalDefense += GetAdditionalPercentStat(totalDefense, additionalPercentDefense);

        foreach (var value in values)
        {
            switch (value.Key)
            {
                case StatType.Damage:
                    totalDamage += value.Value;
                    break;
                case StatType.HP:
                    totalHp += value.Value;
                    break;
                case StatType.Defense:
                    totalDefense += value.Value;
                    break;
            }
        }

        BigInteger totalEquipmentPower = GetPower(totalDamage, totalHp, totalDefense);

        return totalEquipmentPower;
    }

    public void UpdateTotalStat(ColleagueType colleagueType, bool isLastColleague)
    {
        if (!colleagueStatDict.ContainsKey(colleagueType))
        {
            return;
        }

        AdditionalStatData totalColleagueBaseStatData = GetTotalColleagueStatData(ArithmeticStatType.Base, colleagueType);
        AdditionalStatData totalAdditionalBaseStatData = GetTotalAdditionalStatData(ArithmeticStatType.Base);

        StatType damageType = StatType.Damage;
        StatType hpType = StatType.HP;
        StatType defenseType = StatType.Defense;

        BigInteger totalDamage = totalColleagueBaseStatData.Stats.ContainsKey(damageType) ? totalColleagueBaseStatData.Stats[damageType] : 0;
        BigInteger totalHp = totalColleagueBaseStatData.Stats.ContainsKey(hpType) ? totalColleagueBaseStatData.Stats[hpType] : 0;
        BigInteger totalDefense = totalColleagueBaseStatData.Stats.ContainsKey(defenseType) ? totalColleagueBaseStatData.Stats[defenseType] : 0;

        /*BigInteger colleagueStatValue = GetPower(totalDamage, totalHp, totalDefense);
        if (colleagueStatValue != 0)
        {
            colleagueStatValue += GetSkillPower(colleagueType);
        }
        UpdateColleaguePower(colleagueType, colleagueStatValue);*/

        AdditionalStatData totalAdditionalRateStatData = GetTotalAdditionalStatData(ArithmeticStatType.Rate);
        AdditionalStatData totalColleagueRateStatData = GetTotalColleagueStatData(ArithmeticStatType.Rate);

        BigInteger damagePercent = totalAdditionalRateStatData.Stats.ContainsKey(damageType) ? totalAdditionalRateStatData.Stats[damageType] : 0;
        damagePercent += totalColleagueRateStatData.Stats.ContainsKey(damageType) ? totalColleagueRateStatData.Stats[damageType] : 0;
        totalDamage += GetAdditionalPercentStat(totalDamage, damagePercent);
        totalDamage += totalAdditionalBaseStatData.Stats.ContainsKey(damageType) ? totalAdditionalBaseStatData.Stats[StatType.Damage] : 0;

        BigInteger hpPercent = totalColleagueRateStatData.Stats.ContainsKey(hpType) ? totalColleagueRateStatData.Stats[hpType] : 0;
        BigInteger defensePercent = totalColleagueRateStatData.Stats.ContainsKey(defenseType) ? totalColleagueRateStatData.Stats[defenseType] : 0;
        totalHp += GetAdditionalPercentStat(totalHp, hpPercent);
        totalDefense += GetAdditionalPercentStat(totalDefense, defensePercent);

        SlotEquipmentStatData equipmentStatData = new SlotEquipmentStatData(totalDamage, totalHp, totalDefense, new SlotEquipmentStatDataSave());
        if (!colleagueEquipmentStatDataDict.ContainsKey(colleagueType))
        {
            colleagueEquipmentStatDataDict.Add(colleagueType, new SlotEquipmentStatData(0, 0, 0, new SlotEquipmentStatDataSave()));
        }

        colleagueEquipmentStatDataDict[colleagueType] = equipmentStatData;

        UpdateTotalStatUI();

        if (isColleagueLoaded)
        {
            if (isLastColleague && !isColleagueAutoEquipState)
            {
                ShowPowerToastMessageByColleague();
            }
        }
    }

    /*public BigInteger GetSkillPower(ColleagueType colleagueType)
    {
        if (!skillPowerDict.ContainsKey(colleagueType))
        {
            return 0;
        }

        return skillPowerDict[colleagueType];
    }*/

    private BigInteger GetPower(BigInteger damage, BigInteger health, BigInteger defense)
    {
        return damage + health + defense;
    }

    public BigInteger GetPower(SlotEquipmentStatData slotEquipmentStatData)
    {
        return slotEquipmentStatData.mainDamage + slotEquipmentStatData.health + slotEquipmentStatData.defense;
    }

    private void ShowPowerToastMessage(BigInteger currentAdditionalPower)
    {
        BigInteger diff = currentAdditionalPower - preAdditionalPower;
        UIManager.instance.GetUIElement<UI_Alert>().PowerMessage(diff);
        preAdditionalPower = currentAdditionalPower;
    }

    private BigInteger GetPowerTotal(BigInteger additionalPower)
    {
        BigInteger diff = additionalPower - preAdditionalPower;

        return diff;
    }

    private void ShowPowerToastMessageByColleague()
    {
        BigInteger totalPower = UIManager.instance.GetUIElement<UI_Colleague>().GetTotalPower();

        BigInteger diff = totalPower - preColleaguePower;
        UIManager.instance.GetUIElement<UI_Alert>().PowerMessage(diff);
        preColleaguePower = totalPower;
    }

    private void InitPreColleaguePower()
    {
        BigInteger totalPower = 0;
        foreach (BigInteger power in colleaguePowerDict.Values)
        {
            totalPower += power;
        }

        preColleaguePower = totalPower;
    }

    private void InitPrePower()
    {
        colleagueTypes.Clear();
        colleagueTypes.AddRange(colleagueEquipmentStatDataDict.Keys);
        UpdateColleaguesStat();

        BigInteger totalAdditionalPower = GetTotalAdditionalPowerUpdated();
        BigInteger power = UIManager.instance.GetUIElement<UI_Colleague>().GetTotalPower();
        preAdditionalPower = totalAdditionalPower;
        preColleaguePower = power;
    }

    public void SetColleagueLoaded()
    {
        InitPrePower();
        InitPreColleaguePower();
    }

    public void UpdateIsColleagueAutoEquipState(bool isAutoEquipState)
    {
        isColleagueAutoEquipState = isAutoEquipState;
    }

    private BigInteger GetAdditionalPercentStat(BigInteger totalStat, BigInteger statPercent)
    {
        BigInteger additionalStat;
        string[] str = statPercent.ToString().Split('.');
        if (str.Length == 1)
        {
            additionalStat = totalStat * statPercent / Consts.PERCENT_DIVIDE_VALUE;
        }
        else
        {
            additionalStat = totalStat * statPercent / (Consts.PERCENT_DIVIDE_VALUE * 10);
        }

        if (additionalStat == null)
        {
            additionalStat = 0;
        }

        return additionalStat;
    }

    private AdditionalStatData GetTotalAdditionalStatData(ArithmeticStatType arithmeticStatType)
    {
        AdditionalStatData totalBaseStatData = new AdditionalStatData(new Dictionary<StatType, BigInteger>());

        foreach (var statDataArrays in additionalStatDicts.Values)
        {
            foreach (var stat in statDataArrays[(int)arithmeticStatType].Stats)
            {
                totalBaseStatData.AddOrSubtract(stat.Key, stat.Value, true);
            }
        }

        return totalBaseStatData;
    }

    private AdditionalStatData GetTotalColleagueStatData(ArithmeticStatType arithmeticStatType, ColleagueType colleagueType)
    {
        AdditionalStatData totalBaseStatData = new AdditionalStatData(new Dictionary<StatType, BigInteger>());

        if (colleagueStatDict.ContainsKey(colleagueType))
        {
            foreach (var stat in colleagueStatDict[colleagueType][(int)arithmeticStatType].Stats)
            {
                totalBaseStatData.AddOrSubtract(stat.Key, stat.Value, true);
            }
        }

        return totalBaseStatData;
    }

    private AdditionalStatData GetTotalColleagueStatData(ArithmeticStatType arithmeticStatType)
    {
        AdditionalStatData totalBaseStatData = new AdditionalStatData(new Dictionary<StatType, BigInteger>());

        foreach (var statDataArrays in colleagueStatDict.Values)
        {
            foreach (var stat in statDataArrays[(int)arithmeticStatType].Stats)
            {
                totalBaseStatData.AddOrSubtract(stat.Key, stat.Value, true);
            }
        }

        return totalBaseStatData;
    }

    private void UpdateTotalStatUI()
    {
        BigInteger totalDamage = 0;
        BigInteger totalHealth = 0;
        BigInteger totalDefense = 0;

        foreach (var item in colleagueEquipmentStatDataDict.Values)
        {
            totalDamage += item.mainDamage;
            totalHealth += item.health;
            totalDefense += item.defense;
        }

        AdditionalStatData additionalStatBaseData = GetTotalAdditionalStatData(ArithmeticStatType.Base);
        AdditionalStatData additionalStatRateData = GetTotalAdditionalStatData(ArithmeticStatType.Rate);

        totalHealth += additionalStatBaseData.Stats.ContainsKey(StatType.HP) ? additionalStatBaseData.Stats[StatType.HP] : 0;
        BigInteger additionalPercentHp = additionalStatRateData.Stats.ContainsKey(StatType.HP) ?
            additionalStatRateData.Stats[StatType.HP] : 0;
        totalHealth += GetAdditionalPercentStat(totalHealth, additionalPercentHp);

        totalDefense += additionalStatBaseData.Stats.ContainsKey(StatType.Defense) ? additionalStatBaseData.Stats[StatType.Defense] : 0;
        BigInteger additionalPercentDefense = additionalStatRateData.Stats.ContainsKey(StatType.Defense) ?
            additionalStatRateData.Stats[StatType.Defense] : 0;
        totalDefense += GetAdditionalPercentStat(totalDefense, additionalPercentDefense);

        OnUpdateEquipmentStatData?.Invoke(new SlotEquipmentStatData(totalDamage, totalHealth, totalDefense, default));

        BigInteger totalPower = totalDamage + totalHealth + totalDefense;
        totalPower += GetTotalSkillPower();
        OnUpdateTotalPower?.Invoke(totalPower);
    }

    public BigInteger GetColleaguePower(ColleagueType colleagueType, ColleagueUpgradableData colleagueUpgradableData)
    {
        BigInteger totalPower = GetPower(colleagueUpgradableData.damage, colleagueUpgradableData.health,
            colleagueUpgradableData.defense);
        //totalPower += GetSkillPower(colleagueType);
        return totalPower;
    }

    /*public BigInteger GetColleaguePower(ColleagueType colleagueType)
    {
        if (colleagueStatDict.ContainsKey(colleagueType))
        {
            int baseIndex = (int)ArithmeticStatType.Base;
            BigInteger totalPower = GetPower(colleagueStatDict[colleagueType][baseIndex].Stats[StatType.Damage],
                colleagueStatDict[colleagueType][baseIndex].Stats[StatType.HP], colleagueStatDict[colleagueType][baseIndex].Stats[StatType.Defense]);
            totalPower += GetSkillPower(colleagueType);
            return totalPower;
        }

        return 0;
    }*/

    public void AddAttributeEvent(AttributeType attributeType, Action<float> attributeEvent)
    {
        attributeDataHandler.AddAttributeEvent(attributeType, attributeEvent);
    }

    public void RemoveAttributeEvent(AttributeType attributeType, Action<float> attributeEvent)
    {
        attributeDataHandler.RemoveAttributeEvent(attributeType, attributeEvent);
    }

    private void OnAttributeUpdated(AttributeType attributeType, float percent)
    {
        OnUpdateAttributeText?.Invoke(attributeType, percent);
    }

    public void AddAttributeStat(AttributeType currentAttributeType, float currentValue)
    {
        attributeDataHandler.AddAttributeStat(currentAttributeType, currentValue);
    }

    public void RemoveAttributeStat(AttributeType beforeAttributeType, float beforeValue)
    {
        attributeDataHandler.RemoveAttributeStat(beforeAttributeType, beforeValue);
    }

    public float GetAttributeValue(AttributeType attributeType)
    {
        return attributeDataHandler.GetAttributeAppliedData(attributeType);
    }
}
