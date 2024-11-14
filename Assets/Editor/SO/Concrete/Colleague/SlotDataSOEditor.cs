using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColleagueDataSO))]
public class SlotDataSOEditor : ListDataSOEditor<ColleagueDataSO>
{
    public static void LoadCSVToSO(ColleagueDataSO slotDataSO, TextAsset csv)
    {
        slotDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<ColleagueData> slotDatas = new List<ColleagueData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int index = int.Parse(elements[0]);
            int skillIndex = int.Parse(elements[1]);
            string equipmentName = elements[2];
            string nickName = elements[3];
            ColleagueType slotType = EnumUtility.GetEqualValue<ColleagueType>(elements[4]);
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[5]);
            ShootingType shootingType = EnumUtility.GetEqualValue<ShootingType>(elements[6].Trim('\r'));

            ColleagueInfo slotInfo = new ColleagueInfo(rank, slotType);

            slotDatas.Add(new ColleagueData(index, skillIndex, equipmentName, nickName, slotInfo, shootingType));
        }

        slotDataSO.AddDatas(slotDatas);
        slotDataSO.InitDict();
        EditorUtility.SetDirty(slotDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<ColleagueData> slotDatas = new List<ColleagueData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int index = int.Parse(elements[0]);
            int skillIndex = int.Parse(elements[1]);
            string colleagueName = elements[2];
            string nickName = elements[3];
            ColleagueType slotType = EnumUtility.GetEqualValue<ColleagueType>(elements[4]);
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[5]);
            ShootingType shootingType = EnumUtility.GetEqualValue<ShootingType>(elements[6].Trim('\r'));

            ColleagueInfo slotInfo = new ColleagueInfo(rank, slotType);

            slotDatas.Add(new ColleagueData(index, skillIndex, colleagueName, nickName, slotInfo, shootingType));
        }

        dataSO.AddDatas(slotDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
