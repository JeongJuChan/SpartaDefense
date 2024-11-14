using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSummon : Summon
{
    public event Func<Rank, List<SkillData>> OnGetSkillData;
    public event Action<int, int, bool> OnSummonSkills;

    private HashSet<SkillData> hashSummonedItems;
    private List<SkillData> summonedItems;
    private Dictionary<SkillData, int> summonedCounts;

    public SkillSummon(SummonDataSO data) : base(data)
    {
        resultUI = UIManager.instance.GetUIElement<UI_SummonResult>();

        hashSummonedItems = new HashSet<SkillData>();
        summonedItems = new List<SkillData>();
        summonedCounts = new Dictionary<SkillData, int>();

        type = SummonType.Skill;
        maxLevel = 15;

        ranks = new Rank[100000];
    }

    protected override void SummonItem(int quantity)
    {
        summonedCounts.Clear();
        summonedItems.Clear();
        hashSummonedItems.Clear();

        resultUI.SetResultUI(this.type, () => SmallSummon(), () => LargeSummon());
        resultUI.SetButtonInfo(smallInfo.currencyType, small: smallInfo, large: largeInfo);

        for (int i = 0; i < quantity; i++)
        {
            int rankNum = GetRandomInt(ranks.Length);
            Rank rank = ranks[rankNum];


            var skillDatas = OnGetSkillData.Invoke(rank);

            Debug.Log("SkillDatas Count: " + skillDatas.Count + " Rank: " + rank + " RankNum: " + ranks[rankNum]);
            SkillData skill = skillDatas[GetRandomInt(skillDatas.Count)];

            summonedItems.Add(skill);
            hashSummonedItems.Add(skill);
            if (summonedCounts.ContainsKey(skill)) summonedCounts[skill]++;
            else summonedCounts[skill] = 1;
        }

        foreach (SkillData skill in summonedItems)
        {
            resultUI.AddSlot(skill);
        }
        
        int itemSetCount = hashSummonedItems.Count;

        foreach (SkillData skill in hashSummonedItems)
        {
            itemSetCount--;
            OnSummonSkills?.Invoke(skill.index, summonedCounts[skill], itemSetCount == 0);
        }

        QuestManager.instance.UpdateCount(EventQuestType.ColleagueSummonCount_10, quantity, -1);
        DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.Summon_Colleague, quantity);

        resultUI.ShowSlots();
    }

    protected override void SummonItem(CurrencyType currencyType, int quantity, int price, int quantityNum)
    {
        throw new NotImplementedException();
    }
}