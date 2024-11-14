using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DailyQuestSO))]
public class DailyQuestSOEditor : ListDatasSOEditor<DailyQuestSO>
{
    protected override void ClearDatas_T1()
    {
        dataSO.ClearDatas_T1();
    }
    protected override void ClearDatas_T2()
    {
        dataSO.ClearDatas_T2();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        if (rows[0].Split(',')[0] == "DailyQuestType")
        {
            List<DailyQuestSlotData> dailyQuestSlotDatas = new List<DailyQuestSlotData>();

            for (int i = 1; i < rows.Length; i++)
            {
                string[] elements = rows[i].Split(',');
                DailyQuestType dailyQuestType = EnumUtility.GetEqualValue<DailyQuestType>(elements[0]);
                string description = elements[1];
                int goalCount = int.Parse(elements[2]);
                RewardType reward = EnumUtility.GetEqualValue<RewardType>(elements[3]);
                int amount = int.Parse(elements[4]);
                int questPoint = int.Parse(elements[5]);

                dailyQuestSlotDatas.Add(new DailyQuestSlotData(dailyQuestType, description, goalCount, reward, amount, questPoint));

            }
            dataSO.AddDatas(dailyQuestSlotDatas);
            dataSO.InitDict_T2();

            EditorUtility.SetDirty(dataSO);
        }
        else
        {
            List<DailyQuestData> dailyQuestDatas = new List<DailyQuestData>();

            List<RewardType> rewardTypes = new List<RewardType>();
            List<int> amounts = new List<int>();
            List<int> questPoints = new List<int>();

            for (int i = 1; i < rows.Length; i++)
            {
                string[] elements = rows[i].Split(',');
                int questPoint = int.Parse(elements[0]);
                RewardType reward = EnumUtility.GetEqualValue<RewardType>(elements[1]);
                int amount = int.Parse(elements[2]);

                rewardTypes.Add(reward);
                amounts.Add(amount);
                questPoints.Add(questPoint);
            }

            dailyQuestDatas.Add(new DailyQuestData(rewardTypes, amounts, questPoints));

            dataSO.AddDatas(dailyQuestDatas);
            dataSO.InitDict_T1();

            EditorUtility.SetDirty(dataSO);
        }

    }
}
