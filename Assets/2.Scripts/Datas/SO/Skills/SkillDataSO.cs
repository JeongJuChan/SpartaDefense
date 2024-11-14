using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/SkillData", fileName = "SkillData")]
public class SkillDataSO : ListDataSO<SkillData>
{
    private Dictionary<int, SkillData> skillDataDict = new Dictionary<int, SkillData>();
    private Dictionary<Rank, int> skillRankIndexDataDict = new Dictionary<Rank, int>();

    public List<SkillData> GetSkillDatas()
    {
        return datas;
    }

    public List<SkillData> GetSkillDatas(Rank rank)
    {
        if (skillDataDict.Count == 0)
        {
            InitDict();
        }

        List<SkillData> skillDatas = new List<SkillData>(datas);

        skillDatas.RemoveAll(x => x.rank != rank);

        return skillDatas;
    }

    public SkillData GetSkillData(int index)
    {
        if (skillDataDict.Count == 0)
        {
            InitDict();
        }

        if (skillDataDict.ContainsKey(index))
        {
            return skillDataDict[index];
        }

        return default;
    }

    public override void InitDict()
    {
        skillDataDict.Clear();

        foreach (SkillData data in datas)
        {
            if (!skillDataDict.ContainsKey(data.index))
            {
                skillDataDict.Add(data.index, data);
            }

            if (!skillRankIndexDataDict.ContainsKey(data.rank))
            {
                skillRankIndexDataDict.Add(data.rank, data.index);
            }
        }
    }
}
