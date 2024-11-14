using System;
using System.Collections.Generic;

public class SkillUpgradeDataHandler
{
    private Func<int, SkillIncrementData> OnGetSkillIncrementData;
    private Dictionary<int, SkillUpgradableData> skillUpgradableDataDict = new Dictionary<int, SkillUpgradableData>();

    public Func<int, SkillData> OnGetSkillData;

    public event Action<int> OnUpdateColleagueUIPopup;

    public SkillUpgradeDataHandler(Func<int, SkillIncrementData> OnGetSkillIncrementData, Func<int, SkillData> OnGetSkillData)
    {
        this.OnGetSkillIncrementData += OnGetSkillIncrementData;
        this.OnGetSkillData += OnGetSkillData;
    }

    public SkillUpgradableData GetSkillUpgradableData(int index)
    {
        return skillUpgradableDataDict[index];
    }

    public void UpdateSkillUpgradableData(int index)
    {
        SkillData skillData = OnGetSkillData(index);
        SkillUpgradableData skillUpgradableData = skillData.skillUpgradableData;

        if (!skillUpgradableDataDict.ContainsKey(index))
        {
            skillUpgradableDataDict.Add(index, skillUpgradableData);
        }

        skillUpgradableDataDict[index] = skillUpgradableData;
    }

    public void UpdateSkillUpgradableData(int index, int skillIndex, int starCount)
    {
        SkillIncrementData skillIncrementData = OnGetSkillIncrementData.Invoke(skillIndex);

        SkillData skillData = OnGetSkillData(skillIndex);
        SkillUpgradableData skillUpgradableData = skillData.skillUpgradableData;
        skillUpgradableData.damagePerecent += skillIncrementData.damageIncrement * starCount;
        skillUpgradableDataDict[skillIndex] = skillUpgradableData;
        OnUpdateColleagueUIPopup?.Invoke(index);
    }

    private SkillUpgradableData GetSkillUpgradedOnce(int index, SkillUpgradableData skillUpgradableData)
    {
        SkillIncrementData skillIncrementData = OnGetSkillIncrementData.Invoke(index);

        return skillUpgradableData;
    }

    private SkillUpgradableData UpgradeData(SkillUpgradableData skillUpgradableData, SkillIncrementData skillIncrementData)
    {
        EncyclopediaDataHandler.Instance.SlotLevelChangeEvent(EncyclopediaType.Skill, $"{skillIncrementData.rank}");
        DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.LevelUP_Skill, 1);

        return skillUpgradableData;
    }
}
