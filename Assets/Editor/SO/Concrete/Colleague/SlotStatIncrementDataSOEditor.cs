using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotStatIncrementDataSOEditor : ListDataSOEditor<SlotStatIncrementDataSO>
{
    //public static void LoadCSVToSO(TextAsset csv)
    //{
    //    dataSO.ClearDatas();
    //}

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<SlotStatIncrementData> slotStatIncrementData = new List<SlotStatIncrementData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[0]);
            int hp = int.Parse(elements[1]);
            int damage = int.Parse(elements[2]);
            int defense = int.Parse(elements[3]);
            int exp = int.Parse(elements[4]);
            int gold = int.Parse(elements[5]);
        }
    }
}
