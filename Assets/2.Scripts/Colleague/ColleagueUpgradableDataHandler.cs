using Keiwando.BigInteger;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ColleagueUpgradableDataHandler
{
    private Dictionary<int, ColleagueUpgradableData> colleagueUpgradableDataDict = new Dictionary<int, ColleagueUpgradableData>();
    private Func<int, ColleagueStatData> OnGetColleagueStatData;
    private Func<Rank, int, int> OnGetColleaguePartCost;
    private Func<Rank, ColleagueLevelUpData> OnGetColleagueLevelUpData;

    private int maxStarCount;

    private UI_Colleague ui_Colleague;

    public ColleagueUpgradableDataHandler(Func<int, ColleagueStatData> OnGetColleagueStatData, 
        Func<Rank, int, int> OnGetColleaguePartCost, int maxStarCount, Func<Rank, ColleagueLevelUpData> OnGetColleagueLevelUpData)
    {
        this.OnGetColleagueStatData = OnGetColleagueStatData;
        this.OnGetColleaguePartCost = OnGetColleaguePartCost;
        this.OnGetColleagueLevelUpData = OnGetColleagueLevelUpData;
        this.maxStarCount = maxStarCount;

        ui_Colleague = UIManager.instance.GetUIElement<UI_Colleague>();
    }

    public void UpdateColleagueData(int index, int count, bool isLastColleague)
    {
        ColleagueStatData colleagueStatData = OnGetColleagueStatData.Invoke(index);

        if (!colleagueUpgradableDataDict.ContainsKey(index))
        {
            colleagueUpgradableDataDict.Add(index, new ColleagueUpgradableData(index, 0, 0, 1, colleagueStatData.baseDamage,
                colleagueStatData.baseHealth, colleagueStatData.baseDefense, 0));

            //EncyclopediaDataHandler.Instance.SlotLevelChangeEvent(EncyclopediaType.Colleague, colleagueStatData.colleagueType.ToString());
        }

        ColleagueUpgradableData colleagueUpgradableData = GetColleagueUpgradableData(index);
        colleagueUpgradableData = UpdateColleagueDefaultStats(colleagueStatData, colleagueUpgradableData);
        colleagueUpgradableData.count += count;
        colleagueUpgradableData.power = GetPowerUpdated(colleagueUpgradableData);
        colleagueUpgradableDataDict[index] = colleagueUpgradableData;
        ui_Colleague.UpdateEncyclopediaSlots(colleagueStatData.colleagueType, colleagueUpgradableData.level);
        SaveColleagueUpgradableData(index);
    }

    private void SaveColleagueUpgradableData(int index)
    {
        ES3.Save<ColleagueUpgradableData>($"{index}_{Consts.COLLEAGUE_UPGRADABLE_DATA}", colleagueUpgradableDataDict[index], ES3.settings);
        ES3.Save<string>($"{index}_{Consts.COLLEAGUE_UPGRADABLE_COUNT}", colleagueUpgradableDataDict[index].count.ToString(), ES3.settings);
        ES3.Save<string>($"{index}_{Consts.COLLEAGUE_UPGRADABLE_DAMAGE}", colleagueUpgradableDataDict[index].damage.ToString(), ES3.settings);
        ES3.Save<string>($"{index}_{Consts.COLLEAGUE_UPGRADABLE_HEALTH}", colleagueUpgradableDataDict[index].health.ToString(), ES3.settings);
        ES3.Save<string>($"{index}_{Consts.COLLEAGUE_UPGRADABLE_DEFENSE}", colleagueUpgradableDataDict[index].defense.ToString(), ES3.settings);
        ES3.Save<string>($"{index}_{Consts.COLLEAGUE_UPGRADABLE_POWER}", colleagueUpgradableDataDict[index].power.ToString(), ES3.settings);
        ES3.StoreCachedFile();
    }

    public void UpdateColleagueData(ColleagueUpgradableData colleagueUpgradableData)
    {
        int index = colleagueUpgradableData.index;

        if (!colleagueUpgradableDataDict.ContainsKey(index))
        {
            colleagueUpgradableDataDict.Add(index, colleagueUpgradableData);
        }

        colleagueUpgradableDataDict[index] = colleagueUpgradableData;

        ColleagueType colleagueType = OnGetColleagueStatData.Invoke(index).colleagueType;
        UIManager.instance.GetUIElement<UI_Colleague>().UpdateEncyclopediaSlots(colleagueType, colleagueUpgradableData.level);
    }

    public void LevelUpColleague(int index)
    {
        ColleagueStatData colleagueStatData = OnGetColleagueStatData.Invoke(index);
        Rank rank = colleagueStatData.rank;
        ColleagueUpgradableData colleagueUpgradableData = GetColleagueUpgradableData(index);
        ColleagueLevelUpData colleagueLevelUpData = OnGetColleagueLevelUpData.Invoke(rank);
        BigInteger totalCost = GetTotalLevelUpCost(colleagueLevelUpData, colleagueUpgradableData.level);
        CurrencyManager.instance.TryUpdateCurrency(CurrencyType.ColleagueLevelUpStone, -totalCost);
        colleagueUpgradableData.level++;
        ui_Colleague.UpdateEncyclopediaSlots(colleagueStatData.colleagueType, colleagueUpgradableData.level);
        colleagueUpgradableData = UpdateColleagueDefaultStats(colleagueStatData, colleagueUpgradableData);
        //EncyclopediaDataHandler.Instance.SlotLevelChangeEvent(EncyclopediaType.Colleague, colleagueStatData.colleagueType.ToString());
        colleagueUpgradableData.power = GetPowerUpdated(colleagueUpgradableData);
        //BigInteger totalPower = StatDataHandler.Instance.GetColleaguePower(colleagueStatData.colleagueType);
        ui_Colleague.UpdatePowerText(colleagueUpgradableData.power);
        colleagueUpgradableDataDict[index] = colleagueUpgradableData;
        SaveColleagueUpgradableData(index);
    }

    private ColleagueUpgradableData UpdateColleagueDefaultStats(ColleagueStatData colleagueStatData, ColleagueUpgradableData colleagueUpgradableData)
    {
        colleagueUpgradableData.damage = colleagueStatData.baseDamage + colleagueStatData.statIncrementPerLevel * colleagueUpgradableData.level;
        colleagueUpgradableData.health = colleagueStatData.baseHealth + colleagueStatData.statIncrementPerLevel * colleagueUpgradableData.level;
        colleagueUpgradableData.defense = colleagueStatData.baseDefense + colleagueStatData.statIncrementPerLevel * colleagueUpgradableData.level;
        return colleagueUpgradableData;
    }

    public int GetMaxLevel(int index)
    {
        return OnGetColleagueStatData.Invoke(index).maxLevel;
    }

    public bool GetIsColleagueLevelUpState(int index)
    {
        Rank rank = OnGetColleagueStatData.Invoke(index).rank;
        ColleagueUpgradableData colleagueUpgradableData = GetColleagueUpgradableData(index);
        ColleagueStatData colleagueStatData = OnGetColleagueStatData.Invoke(index);
        if (colleagueUpgradableData.level >= colleagueStatData.maxLevel)
        {
            return false;
        }
        ColleagueLevelUpData colleagueLevelUpData = OnGetColleagueLevelUpData.Invoke(rank);
        BigInteger totalCost = GetTotalLevelUpCost(colleagueLevelUpData, colleagueUpgradableData.level);
        BigInteger enforceStoneCount = CurrencyManager.instance.GetCurrencyValue(CurrencyType.ColleagueLevelUpStone);
        return totalCost <= enforceStoneCount;
    }

    public BigInteger GetTotalLevelUpCost(Rank rank, int level)
    {
        ColleagueLevelUpData colleagueLevelUpData = OnGetColleagueLevelUpData.Invoke(rank);
        return GetTotalLevelUpCost(colleagueLevelUpData, level);
    }

    public BigInteger GetTotalLevelUpCost(ColleagueLevelUpData colleagueLevelUpData, int level)
    {
        return colleagueLevelUpData.baseColleagueLevelUpCost + (level * colleagueLevelUpData.colleagueLevelUpIncrement);
    }

    public bool GetIsAdvanceable(int index)
    {
        if (!colleagueUpgradableDataDict.ContainsKey(index))
        {
            return false;
        }

        ColleagueUpgradableData colleagueUpgradableData = GetColleagueUpgradableData(index);

        if (maxStarCount <= colleagueUpgradableData.starCount)
        {
            return false;
        }

        int cost = GetAdvanceCost(index, colleagueUpgradableData);

        if (colleagueUpgradableData.count < cost)
        {
            return false;
        }

        return true;
    }

    public void AdvanceColleague(int index)
    {
        ColleagueUpgradableData colleagueUpgradableData = GetColleagueUpgradableData(index);
        int cost = GetAdvanceCost(index, colleagueUpgradableData);
        colleagueUpgradableData.count -= cost;
        colleagueUpgradableData.starCount++;
        colleagueUpgradableDataDict[index] = colleagueUpgradableData;
        SaveColleagueUpgradableData(index);
    }

    public ColleagueUpgradableData GetColleagueUpgradableData(int index)
    {
        if (colleagueUpgradableDataDict.ContainsKey(index))
        {
            return colleagueUpgradableDataDict[index];
        }

        return default;
    }

    public BigInteger GetPower(int index)
    {
        return GetColleagueUpgradableData(index).power;
    }

    public Dictionary<int, ColleagueUpgradableData> GetColleagueUpgradableDataDict()
    {
        return colleagueUpgradableDataDict;
    }

    private int GetAdvanceCost(int index, ColleagueUpgradableData colleagueUpgradableData)
    {
        Rank rank = OnGetColleagueStatData.Invoke(index).rank;
        int cost = OnGetColleaguePartCost.Invoke(rank, colleagueUpgradableData.starCount);
        return cost;
    }

    private BigInteger GetPowerUpdated(ColleagueUpgradableData colleagueUpgradableData)
    {
        return colleagueUpgradableData.damage + colleagueUpgradableData.health + colleagueUpgradableData.defense;
    }
}
