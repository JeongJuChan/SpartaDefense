using Keiwando.BigInteger;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ColleagueManager : MonoBehaviour
{
    [SerializeField] private ColleagueDataSO colleagueDataSO;
    [SerializeField] private ColleagueStatDataSO colleagueStatDataSO;
    [SerializeField] private ColleaguePartDataSO colleaguePartDataSO;
    [SerializeField] private ColleagueLevelUpDataSO colleagueEnforceDataSO;

    public event Action<int, ColleagueUpgradableData, bool> OnUpdateColleagueUI;

    [SerializeField] private int maxStarCount = 10;
    [SerializeField] private int starUnitCount = 5;

    public event Action<ColleagueUpgradableData> OnLevelUpEnded;
    public event Action<int> OnUpdateColleagueStats;
    public event Action<ColleagueUpgradableData, int> OnUpdateStarUI;
    public event Action<int, ColleagueUpgradableData, int> OnUpdateStarUIForBottomUIElement;
    public event Action<ColleagueUpgradableData, int> OnUpdateColleagueAdvanceText;
    public event Action<int, int, int> OnColleagueAdvanced;
    
    public event Action<int> OnUpdateSkillUpgradableData;

    private ColleagueUpgradableDataHandler colleagueDataHandler;
    public event Func<int, SkillUpgradableData> OnGetSkillUpgradableData;

    public void Init()
    {
        colleagueDataHandler = new ColleagueUpgradableDataHandler(colleagueStatDataSO.GetColleagueStatData, 
            colleaguePartDataSO.GetColleaguePartCost, maxStarCount, colleagueEnforceDataSO.GetLevelUpData);
        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.ColleagueLevelUp,
            () => { QuestManager.instance.UpdateCount(EventQuestType.ColleagueSummonCount_1, 0, -1); });
        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.ColleagueAdvancement,
            () => { QuestManager.instance.UpdateCount(EventQuestType.ColleagueAdvancement, PlayerPrefs.GetInt(Consts.HAS_COLLEAGUE_ADVANCED, 0), -1); });
    }

    public void OnColleagueSummoned(int index, int count, bool isLastColleague)
    {
        colleagueDataHandler.UpdateColleagueData(index, count, isLastColleague);
        OnUpdateSkillUpgradableData?.Invoke(colleagueDataSO.GetColleagueData(index).skillIndex);
        OnUpdateColleagueUI?.Invoke(index, colleagueDataHandler.GetColleagueUpgradableData(index), isLastColleague);
    }

    public void OnColleagueSummoned(ColleagueUpgradableData colleagueUpgradableData)
    {
        int index = colleagueUpgradableData.index;
        ColleagueData colleagueData = colleagueDataSO.GetColleagueData(index);
        int skillIndex = colleagueDataSO.GetColleagueData(index).skillIndex;
        colleagueDataHandler.UpdateColleagueData(colleagueUpgradableData);
        OnUpdateSkillUpgradableData?.Invoke(colleagueDataSO.GetColleagueData(index).skillIndex);
        OnColleagueAdvanced?.Invoke(index, skillIndex, colleagueUpgradableData.starCount);
        OnUpdateColleagueUI?.Invoke(index, colleagueDataHandler.GetColleagueUpgradableData(index), false);
        OnUpdateStarUIForBottomUIElement?.Invoke(index, colleagueUpgradableData, starUnitCount);
        //StatDataHandler.Instance.UpdateSkillPower(colleagueData.colleagueInfo.colleagueType,
        //    (int)OnGetSkillUpgradableData.Invoke(skillIndex).damagePerecent, colleagueUpgradableData);
    }

    public BigInteger GetColleaguePower(int index)
    {
        return colleagueDataHandler.GetPower(index);
    }

    public Dictionary<int, ColleagueUpgradableData> GetColleagueUpgradableDataDict()
    {
        return colleagueDataHandler.GetColleagueUpgradableDataDict();
    }

    public bool GetIsColleagueAdvanceable(int index)
    {
        return colleagueDataHandler.GetIsAdvanceable(index);
    }

    public void AdvanceColleague(int index)
    {
        colleagueDataHandler.AdvanceColleague(index);
        ColleagueUpgradableData colleagueUpgradableData = GetColleagueUpgradableData(index);

        ColleagueData colleagueData = colleagueDataSO.GetColleagueData(index);

        OnColleagueAdvanced.Invoke(index, colleagueData.skillIndex, colleagueUpgradableData.starCount);

        int advanceCost = GetAdvanceCost(index, colleagueUpgradableData);
        OnUpdateColleagueAdvanceText?.Invoke(colleagueUpgradableData, advanceCost);

        OnUpdateStarUI?.Invoke(colleagueUpgradableData, starUnitCount);
        OnUpdateStarUIForBottomUIElement?.Invoke(index, colleagueUpgradableData, starUnitCount);

        PlayerPrefs.SetInt(Consts.HAS_COLLEAGUE_ADVANCED, 1);
        QuestManager.instance.UpdateCount(EventQuestType.ColleagueAdvancement, PlayerPrefs.GetInt(Consts.HAS_COLLEAGUE_ADVANCED, 0), -1);

        //StatDataHandler.Instance.UpdateSkillPower(colleagueData.colleagueInfo.colleagueType, 
        //    (int)OnGetSkillUpgradableData.Invoke(colleagueData.skillIndex).damagePerecent, colleagueUpgradableData);
    }

    public int GetAdvanceCost(int index, ColleagueUpgradableData colleagueUpgradableData)
    {
        Rank rank = colleagueStatDataSO.GetColleagueStatData(index).rank;
        int advanceCost = colleaguePartDataSO.GetColleaguePartCost(rank, colleagueUpgradableData.starCount);
        
        return advanceCost;
    }

    public ColleagueUpgradableData GetColleagueUpgradableData(int index)
    {
        return colleagueDataHandler.GetColleagueUpgradableData(index);
    }

    public bool GetIsColleagueLevelUpPossibleState(int index)
    {
        return colleagueDataHandler.GetIsColleagueLevelUpState(index);
    }

    public void LevelUpColleague(int index)
    {
        ColleagueStatData colleagueStatData = colleagueStatDataSO.GetColleagueStatData(index);
        ColleagueUpgradableData preColleagueUpgradableData = GetColleagueUpgradableData(index);
        colleagueDataHandler.LevelUpColleague(index);
        ColleagueUpgradableData currentColleagueUpgradableData = GetColleagueUpgradableData(index);
        OnLevelUpEnded?.Invoke(currentColleagueUpgradableData);

        ColleagueUpgradableData colleagueUpgradableData = currentColleagueUpgradableData;
        colleagueUpgradableData.damage -= preColleagueUpgradableData.damage;
        colleagueUpgradableData.health -= preColleagueUpgradableData.health;
        colleagueUpgradableData.defense -= preColleagueUpgradableData.defense;

        StatDataHandler.Instance.ModifyStats(ArithmeticStatType.Base, colleagueStatData.colleagueType, colleagueUpgradableData, true);
        QuestManager.instance.UpdateCount(EventQuestType.ColleagueLevelUp, 1, -1);
        DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.LevelUP_Colleague, 1);
    }

    public BigInteger GetTotalLevelUpCost(Rank rank, int level)
    {
        return colleagueDataHandler.GetTotalLevelUpCost(rank, level);
    }

    public int GetMaxLevel(int index)
    {
        return colleagueDataHandler.GetMaxLevel(index);
    }
}
