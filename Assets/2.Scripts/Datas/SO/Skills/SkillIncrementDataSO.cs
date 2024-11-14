using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "SkillIncrementData", menuName = "SO/SkillIncrementData")]
public class SkillIncrementDataSO : ListDataSO<SkillIncrementData>
{
    private Dictionary<int, SkillIncrementData> skillIncrementDataDict = new Dictionary<int, SkillIncrementData>();

    public SkillIncrementData GetSkillIncrementData(int index)
    {
        if (skillIncrementDataDict.Count == 0)
        {
            InitDict();
        }

        return skillIncrementDataDict[index];
    }

    public override void InitDict()
    {
        skillIncrementDataDict.Clear();

        foreach (SkillIncrementData data in datas)
        {
            if (!skillIncrementDataDict.ContainsKey(data.index))
            {
                skillIncrementDataDict.Add(data.index, data);
            }
        }
    }
}
