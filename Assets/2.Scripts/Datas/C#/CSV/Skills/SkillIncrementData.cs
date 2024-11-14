using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SkillIncrementData
{
    public int index;
    public string skillName;
    public SkillTargetingType skillTargetingType;
    public Rank rank;
    public int maxExpIncrement;
    public int maxExpIncrementLimit;
    public float addtionalStatPercentIncrement;
    public float damageIncrement;

    public SkillIncrementData(int index, string skillName, SkillTargetingType skillTargetingType, Rank rank, int maxExpIncrement, 
        int maxExpIncrementLimit, float addtionalStatPercentIncrement, float damageIncrement)
    {
        this.index = index;
        this.skillName = skillName;
        this.skillTargetingType = skillTargetingType;
        this.rank = rank;
        this.maxExpIncrement = maxExpIncrement;
        this.maxExpIncrementLimit = maxExpIncrementLimit;
        this.addtionalStatPercentIncrement = addtionalStatPercentIncrement;
        this.damageIncrement = damageIncrement;
    }
}
