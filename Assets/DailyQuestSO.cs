using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/DailyQuest", fileName = "DailyQuest")]
public class DailyQuestSO : ListDatasSO<DailyQuestData, DailyQuestSlotData>
{
    public List<DailyQuestData> GetDailyQuestDatas()
    {
        return datas_1;
    }

    public List<DailyQuestSlotData> GetDailyQuestSlotDatas()
    {
        return datas_2;
    }


    public override void InitDict_T1()
    {

    }

    public override void InitDict_T2()
    {
    }
}
