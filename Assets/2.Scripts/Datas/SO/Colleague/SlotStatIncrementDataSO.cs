using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlotStatIncrementData", menuName = "SO/SlotStatIncrementData")]
public class SlotStatIncrementDataSO : ListDataSO<SlotStatIncrementData>
{
    private Dictionary<Rank, SlotStatIncrementData> slotStatIncrementDataDict = new Dictionary<Rank, SlotStatIncrementData>();

    public override void InitDict()
    {
        slotStatIncrementDataDict.Clear();

        foreach (SlotStatIncrementData data in datas)
        {
            if (!slotStatIncrementDataDict.ContainsKey(data.rank))
            {
                slotStatIncrementDataDict.Add(data.rank, data);
            }
        }
    }
}
