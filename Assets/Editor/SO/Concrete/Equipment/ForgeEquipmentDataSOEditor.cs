using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ForgeEquipmentDataSO))]
public class ForgeEquipmentDataSOEditor : ListDataSOEditor<ForgeEquipmentDataSO>
{
    public static void LoadCSVToSO(ForgeEquipmentDataSO forgeEquipmentDataSO, TextAsset csv)
    {
        forgeEquipmentDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<ForgeEquipmentData> forgeEquipmentDatas = new List<ForgeEquipmentData>();
        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            int index = int.Parse(elements[0]);
            string equipmentName = elements[1];
            string equipmentNameKR = elements[2];
            EquipmentType equipmentType = EnumUtility.GetEqualValue<EquipmentType>(elements[3]);
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[4].Trim('\r'));

            forgeEquipmentDatas.Add(new ForgeEquipmentData(index, equipmentName, equipmentNameKR,
                new ForgeEquipmentInfo(equipmentType, rank)));
        }

        forgeEquipmentDataSO.AddDatas(forgeEquipmentDatas);

        EditorUtility.SetDirty(forgeEquipmentDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<ForgeEquipmentData> forgeEquipmentDatas = new List<ForgeEquipmentData>();
        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            int index = int.Parse(elements[0]);
            string equipmentName = elements[1];
            string equipmentNameKR = elements[2];
            EquipmentType equipmentType = EnumUtility.GetEqualValue<EquipmentType>(elements[3]);
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[4]);

            forgeEquipmentDatas.Add(new ForgeEquipmentData(index, equipmentName, equipmentNameKR,
                new ForgeEquipmentInfo(equipmentType, rank)));
        }

        dataSO.AddDatas(forgeEquipmentDatas);

        EditorUtility.SetDirty(dataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }
}
