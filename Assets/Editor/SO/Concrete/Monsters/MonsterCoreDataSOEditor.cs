using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonsterCoreInfoDataSO))]
public class MonsterCoreDataSOEditor : ListDataSOEditor<MonsterCoreInfoDataSO>
{
    public static void LoadCSVToSO(MonsterCoreInfoDataSO monsterCoreInfoDataSO, TextAsset csv)
    {
        monsterCoreInfoDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<MonsterCoreInfoData> coreInfoDatas = new List<MonsterCoreInfoData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] element = rows[i].Split(',');
            int index = int.Parse(element[0]);
            string monsterName = element[1];
            MonsterType monsterType = EnumUtility.GetEqualValue<MonsterType>(element[2].Trim('\r'));

            coreInfoDatas.Add(new MonsterCoreInfoData(new CoreInfoData(index, monsterName), monsterType));
        }

        monsterCoreInfoDataSO.AddDatas(coreInfoDatas);
        monsterCoreInfoDataSO.InitDict();
        EditorUtility.SetDirty(monsterCoreInfoDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<MonsterCoreInfoData> coreInfoDatas = new List<MonsterCoreInfoData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] element = rows[i].Split(',');
            int index = int.Parse(element[0]);
            string monsterName = element[1];
            MonsterType monsterType = EnumUtility.GetEqualValue<MonsterType>(element[2].Trim('\r'));

            coreInfoDatas.Add(new MonsterCoreInfoData(new CoreInfoData(index, monsterName), monsterType));
        }

        dataSO.AddDatas(coreInfoDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
