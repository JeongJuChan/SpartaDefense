using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentSummon : Summon
{
    private HashSet<EquipmentData> hashSummonedItems;
    private List<EquipmentData> summonedItems;
    private Dictionary<EquipmentData, int> summonedCounts;

    public EquipmentSummon(SummonDataSO data) : base(data)
    {
        resultUI = UIManager.instance.GetUIElement<UI_SummonResult>();

        hashSummonedItems = new HashSet<EquipmentData>();
        summonedItems = new List<EquipmentData>();
        summonedCounts = new Dictionary<EquipmentData, int>();

        type = SummonType.Equipment;
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
            var equipmentTypeData = Enum.GetValues(typeof(EquipmentType));

            EquipmentType type = (EquipmentType)equipmentTypeData.GetValue(GetRandomInt(equipmentTypeData.Length));

            int rankNum = GetRandomInt(ranks.Length);
            Rank rank = ranks[rankNum];

            int index = GetRandomInt(EquipmentManager.instance.GetEquipmentCountOfRank(type, rank));

            string name = $"{type}_{rank}_{index}";
            EquipmentData equipment = EquipmentManager.instance.GetData(name);

            summonedItems.Add(equipment);
            hashSummonedItems.Add(equipment);
            if (summonedCounts.ContainsKey(equipment)) summonedCounts[equipment]++;
            else summonedCounts[equipment] = 1;
        }

        foreach (EquipmentData equipment in summonedItems)
        {
            resultUI.AddSlot(equipment);
        }

        foreach (EquipmentData equipment in hashSummonedItems)
        {
            EquipmentManager.instance.UpdateEquipmentCount(equipment, summonedCounts[equipment]);
        }

        EquipmentManager.instance.AllSort();

        //QuestManager.instance.UpdateCount(QuestType.EquipmentSummonCount, quantity);
        DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.Forge_Equipment, quantity);

        resultUI.ShowSlots();
    }

    protected override void SummonItem(CurrencyType currencyType, int quantity, int price, int quantityNum)
    {
    }
}
