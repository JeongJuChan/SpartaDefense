using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColleagueSummon : Summon
{
    private HashSet<ColleagueData> hashSummonedItems;
    private List<ColleagueData> summonedItems;
    private Dictionary<ColleagueData, int> summonedCounts;


    public event Func<Rank, List<ColleagueData>> OnGetColleagueDatas;

    private Func<int, ColleagueSummonExpData> OnGetData;

    public event Action<int, int, bool> OnSummonColleagues;

    private ColleagueSummonExpData colleagueSummonExpData;

    public ColleagueSummon(SummonDataSO data) : base(data)
    {
        resultUI = UIManager.instance.GetUIElement<UI_SummonResult>();

        hashSummonedItems = new HashSet<ColleagueData>();
        summonedItems = new List<ColleagueData>();
        summonedCounts = new Dictionary<ColleagueData, int>();

        type = SummonType.Colleague;
        maxLevel = 15;

        ranks = new Rank[100000];
    }

    public override void Initialize()
    {
        currentSummonExp = ES3.KeyExists($"{Consts.SUMMON_PREFIX}{type}{Consts.CURRENT_SUMMON_EXP}") ?
            ES3.Load<int>($"{Consts.SUMMON_PREFIX}{type}{Consts.CURRENT_SUMMON_EXP}") : 0;
        currentSummonLevel = ES3.KeyExists($"{Consts.SUMMON_PREFIX}{type}{Consts.CURRENT_SUMMON_LEVEL}") ?
            ES3.Load<int>($"{Consts.SUMMON_PREFIX}{type}{Consts.CURRENT_SUMMON_LEVEL}") : 1;

        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.ColleagueSummonCount_10,
            () => { QuestManager.instance.UpdateCount(EventQuestType.ColleagueSummonCount_10, 0, -1); });
        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.ColleagueSummonCount_1,
            () => { QuestManager.instance.UpdateCount(EventQuestType.ColleagueSummonCount_1, 0, -1); });

        colleagueSummonExpData = OnGetData.Invoke(currentSummonLevel);
        GetProportionData();
        UpdateSummonLevel();
    }

    protected override void UpdateSummonLevel()
    {
        if (maxSummonExp != 0 && currentSummonExp >= maxSummonExp)
        {
            currentSummonExp -= maxSummonExp;
            currentSummonLevel++;
            colleagueSummonExpData = OnGetData.Invoke(currentSummonLevel);
            SetRanks();
        }

        maxSummonExp = colleagueSummonExpData.exp;
        UpdateExpEvents();
    }

    protected override void SummonItem(int quantity)
    {
        summonedCounts.Clear();
        summonedItems.Clear();
        hashSummonedItems.Clear();

        resultUI.SetResultUI(this.type, () => SmallSummon(), () => LargeSummon());
        resultUI.SetButtonInfo(smallInfo.currencyType, small: smallInfo, large: largeInfo);
        SummonNewItems(quantity);
    }

    private void SummonNewItems(int quantity)
    {
        DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.Summon_Colleague, quantity);

        if (quantity < largeInfo.quantity)
        {
            QuestManager.instance.UpdateCount(EventQuestType.ColleagueSummonCount_10, quantity, -1);
            QuestManager.instance.UpdateCount(EventQuestType.ColleagueSummonCount_1, quantity, -1);
        }
        else
        {
            QuestManager.instance.UpdateCount(EventQuestType.ColleagueSummonCount_10, quantity, -1);
            QuestManager.instance.UpdateCount(EventQuestType.ColleagueSummonCount_1, quantity, -1);
        }

        for (int i = 0; i < quantity; i++)
        {
            int rankNum = GetRandomInt(ranks.Length);
            Rank rank = ranks[rankNum];


            var colleagueDatas = OnGetColleagueDatas.Invoke(rank);

            Debug.Log("SkillDatas Count: " + colleagueDatas.Count + " Rank: " + rank + " RankNum: " + ranks[rankNum]);
            ColleagueData colleague = colleagueDatas[GetRandomInt(colleagueDatas.Count)];

            summonedItems.Add(colleague);
            hashSummonedItems.Add(colleague);
            if (summonedCounts.ContainsKey(colleague)) summonedCounts[colleague]++;
            else summonedCounts[colleague] = 1;
        }

        foreach (ColleagueData colleagueData in summonedItems)
        {
            resultUI.AddSlot(colleagueData);
        }

        int itemSetCount = hashSummonedItems.Count;

        foreach (ColleagueData colleagueData in hashSummonedItems)
        {
            itemSetCount--;
            OnSummonColleagues?.Invoke(colleagueData.index, summonedCounts[colleagueData], itemSetCount == 0);
        }

        //QuestManager.instance.UpdateCount(EventQuestType.ColleagueSummonCount_12, quantity, -1);
        DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.Summon_Colleague, quantity);

        resultUI.ShowSlots();
    }

    protected override void SummonItem(CurrencyType currencyType, int quantity, int price, int quantityNum)
    {
        summonedCounts.Clear();
        summonedItems.Clear();
        hashSummonedItems.Clear();

        resultUI.ClearSlots();
        resultUI.SetResultUI(this.type, SmallSummonByType, LargeSummonByType);
        resultUI.SetInfo(smallInfo, largeInfo);
        resultUI.SetCurrentQuantityType(quantityNum);
        //resultUI.SetButtonInfo(currencyType, small: smallInfo, large: largeInfo);

        SummonNewItems(quantity);
    }

    public void SetDataFunc(Func<int, ColleagueSummonExpData> getData)
    {
        OnGetData = getData;
    }
}
