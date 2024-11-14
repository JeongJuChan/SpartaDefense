using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillDataHandler
{
    private Dictionary<int, SkillResourceData> skillResourceDataDict = new Dictionary<int, SkillResourceData>();
    private const string TEMPLATE_PATH = "Skills";
    private Func<List<SkillData>> OnGetSkillDatas;
    public SkillDataSO skillDataSO { get; private set; }

    public SkillDataHandler(Func<List<SkillData>> OnGetSkillDatas, SkillDataSO skillDataSO)
    {
        this.OnGetSkillDatas = OnGetSkillDatas;
        this.skillDataSO = skillDataSO;
        InitDict();
    }

    public Dictionary<int, SkillResourceData> GetSkillResourceDict()
    {
        if (skillResourceDataDict.Count == 0)
        {
            InitDict();
        }

        return skillResourceDataDict;
    }

    public SkillResourceData GetSkillResourceData(int index)
    {
        if (skillResourceDataDict.ContainsKey(index))
        {
            return skillResourceDataDict[index];
        }

        return default;
    }

    private void InitDict()
    {
        List<SkillData> skillDatas = new List<SkillData>(OnGetSkillDatas.Invoke());

        skillDatas.Sort((x, y) =>
        {
            return x.rank.CompareTo(y.rank);
        });

        for (int i = 0; i < skillDatas.Count; i++)
        {
            string skillPath = $"{TEMPLATE_PATH}/{skillDatas[i].skillName}";
            Skill skill = Resources.Load<Skill>(skillPath);
            Sprite skillIcon = Resources.Load<Sprite>(skillPath);
            SkillResourceData skillResourceData = new SkillResourceData(skillIcon, skill);
            skillResourceDataDict.Add(skillDatas[i].index, skillResourceData);
        }
    }
}