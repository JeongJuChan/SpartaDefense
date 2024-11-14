using Keiwando.BigInteger;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SlotStatDataSO))]
public class SlotStatDataSOEditor : ListDataSOEditor<SlotStatDataSO>
{
    public static void LoadCSVToSO(SlotStatDataSO slotStatDataSO, TextAsset csv)
    {
        slotStatDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<SlotStatCSVData> slotStatCSVDatas = new List<SlotStatCSVData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[0]);
            int level = int.Parse(elements[1]);
            string hp = elements[2];
            string damage = elements[3];
            string defense = elements[4];
            string gold = elements[5];
            string exp = elements[6];
            int increment = int.Parse(elements[7]);
            int variation = int.Parse(elements[8]);
            int heroDamagePercent = int.Parse(elements[9].Trim('%', '\r'));

            SlotEquipmentStatCSVData slotEquipmentStatData = new SlotEquipmentStatCSVData(damage, hp, defense);

            slotStatCSVDatas.Add(new SlotStatCSVData(rank, level, slotEquipmentStatData, gold, exp, increment, variation, heroDamagePercent));
        }

        slotStatDataSO.AddDatas(slotStatCSVDatas);
        slotStatDataSO.InitDict();
        EditorUtility.SetDirty(slotStatDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<SlotStatCSVData> slotStatCSVDatas = new List<SlotStatCSVData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[0]);
            int level = int.Parse(elements[1]);
            string hp = elements[2];
            string damage = elements[3];
            string defense = elements[4];
            string gold = elements[5];
            string exp = elements[6];
            int increment = int.Parse(elements[7]);
            int variation = int.Parse(elements[8]);
            int heroDamagePercent = int.Parse(elements[9].Trim('%', '\r'));

            SlotEquipmentStatCSVData slotEquipmentStatData = new SlotEquipmentStatCSVData(damage, hp, defense);

            slotStatCSVDatas.Add(new SlotStatCSVData(rank, level, slotEquipmentStatData, gold, exp, increment, variation, heroDamagePercent));
        }

        dataSO.AddDatas(slotStatCSVDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
