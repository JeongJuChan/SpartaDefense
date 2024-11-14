using Keiwando.BigInteger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/SlotStatData", fileName = "SlotStatData")]
public class SlotStatDataSO : ListDataSO<SlotStatCSVData>
{
    private Dictionary<Rank, SlotStatData> slotStatDataDict = new Dictionary<Rank, SlotStatData>();

    public SlotStatData GetEquipmentData(Rank rank)
    {
        if (slotStatDataDict.Count == 0)
        {
            InitDict();
        }

        if (slotStatDataDict.ContainsKey(rank))
        {
            return slotStatDataDict[rank];
        }

        return default;
    }

    public override void InitDict()
    {
        slotStatDataDict.Clear();

        foreach (var data in datas)
        {
            SlotEquipmentStatData slotEquipmentStatData =
                new SlotEquipmentStatData(new BigInteger(data.slotEquipmentStatCSVData.mainDamage), new BigInteger(data.slotEquipmentStatCSVData.hp),
                new BigInteger(data.slotEquipmentStatCSVData.defense), new SlotEquipmentStatDataSave());

            SlotStatData slotStatData = new SlotStatData(data.rank, data.level, slotEquipmentStatData, 
                new BigInteger(data.gold), new BigInteger(data.exp), data.increment, data.variationPercent,
                data.heroDamagePercent, new SlotStatDataSave());

            if (!slotStatDataDict.ContainsKey(data.rank))
            {
                slotStatDataDict.Add(data.rank, slotStatData);
            }
        }
    }
}
