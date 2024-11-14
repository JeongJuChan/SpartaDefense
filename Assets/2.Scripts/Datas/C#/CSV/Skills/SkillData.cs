using System;

[Serializable]
public struct SkillData : ISummonable
{
    public int index;
    public string skillName;
    public string skillNameKR;
    public SkillDamageType skillDamageType;
    public SkillTargetingType skillTargetingType;
    public Rank rank;
    public string description;
    public bool isVibrated;
    public SkillUpgradableData skillUpgradableData;

    public SkillData(int index, string skillName, string skillNameKR, SkillDamageType skillDamageType, SkillTargetingType skillTargetingType, Rank rank,
         string description, bool isVibrated, SkillUpgradableData skillUpgradableData)
    {
        this.index = index;
        this.skillName = skillName;
        this.skillNameKR = skillNameKR;
        this.skillDamageType = skillDamageType;
        this.skillTargetingType = skillTargetingType;
        this.rank = rank;
        this.description = description;
        this.isVibrated = isVibrated;
        this.skillUpgradableData = skillUpgradableData;
    }
}
