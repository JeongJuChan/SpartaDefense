using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

[CustomEditor(typeof(MonsterCountDataSO))]
public class MonsterCountDataSOEditor : ListDataSOEditor<MonsterCountDataSO>
{
    public static void LoadCSVToSO(MonsterCountDataSO monsterCountDataSO, TextAsset csv)
    {
        monsterCountDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<MonsterCountData> monsterCountDatas = new List<MonsterCountData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] element = rows[i].Split(',');
            int mainStageNum = int.Parse(element[0]);
            int routineNum = int.Parse(element[1]);
            int index = int.Parse(element[2]);
            string monsterName = element[3];
            int monsterCount = int.Parse(element[4]);

            CoreInfoData coreInfoData = new CoreInfoData(index, monsterName);
            monsterCountDatas.Add(new MonsterCountData(mainStageNum, routineNum, coreInfoData, monsterCount));
        }

        monsterCountDataSO.AddDatas(monsterCountDatas);
        monsterCountDataSO.InitDict();
        EditorUtility.SetDirty(monsterCountDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<MonsterCountData> monsterCountDatas = new List<MonsterCountData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] element = rows[i].Split(',');
            int mainStageNum = int.Parse(element[0]);
            int routineNum = int.Parse(element[1]);
            int index = int.Parse(element[2]);
            string monsterName = element[3];
            int monsterCount = int.Parse(element[4]);

            CoreInfoData coreInfoData = new CoreInfoData(index, monsterName);
            monsterCountDatas.Add(new MonsterCountData(mainStageNum, routineNum, coreInfoData, monsterCount));
        }

        dataSO.AddDatas(monsterCountDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
