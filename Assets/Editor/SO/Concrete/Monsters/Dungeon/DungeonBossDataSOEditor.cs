using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonBossDataSO))]
public class DungeonBossDataSOEditor : ListDataSOEditor<DungeonBossDataSO>
{
    public static void LoadCSVToSO(DungeonBossDataSO dungeonBossDataSO, TextAsset csv)
    {
        dungeonBossDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<DungeonBossCSVData> dungeonBossCSVDatas = new List<DungeonBossCSVData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            if (elements[0] == "")
            {
                continue;
            }

            int index = int.Parse(elements[0]);
            DungeonType dungeonType = EnumUtility.GetEqualValue<DungeonType>(elements[1]);
            string dungeonName = elements[2];
            int count = int.Parse(elements[3]);
            int waveCount = int.Parse(elements[4]);
            int duration = int.Parse(elements[5]);
            string damage = elements[7];
            string health = elements[8];
            string dungeonReward = elements[9].Trim('\r');

            dungeonBossCSVDatas.Add(new DungeonBossCSVData(index, dungeonType, dungeonName, count, waveCount, duration, new MonsterUpgradableCSVData(health, damage, 1, 1), dungeonReward));
        }

        dungeonBossDataSO.AddDatas(dungeonBossCSVDatas);
        dungeonBossDataSO.InitDict();
        EditorUtility.SetDirty(dungeonBossDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<DungeonBossCSVData> dungeonBossCSVDatas = new List<DungeonBossCSVData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            if (elements[0] == "")
            {
                continue;
            }

            int index = int.Parse(elements[0]);
            DungeonType dungeonType = EnumUtility.GetEqualValue<DungeonType>(elements[1]);
            string dungeonName = elements[2];
            int count = int.Parse(elements[3]);
            int waveCount = int.Parse(elements[4]);
            int duration = int.Parse(elements[5]);
            string damage = elements[7];
            string health = elements[8];
            string dungeonReward = elements[9].Trim('\r');

            dungeonBossCSVDatas.Add(new DungeonBossCSVData(index, dungeonType, dungeonName, count, waveCount, duration, new MonsterUpgradableCSVData(health, damage, 1, 1), dungeonReward));
        }

        dataSO.AddDatas(dungeonBossCSVDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
