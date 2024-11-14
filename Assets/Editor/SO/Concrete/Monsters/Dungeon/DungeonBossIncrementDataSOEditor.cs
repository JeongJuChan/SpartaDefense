using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonBossIncrementDataSO))]
public class DungeonBossIncrementDataSOEditor : ListDataSOEditor<DungeonBossIncrementDataSO>
{
    private const string INCREMENT_PER_LEVEL = "레벨당"; 

    public static void LoadCSVToSO(DungeonBossIncrementDataSO dungeonBossIncrementDataSO, TextAsset csv)
    {
        dungeonBossIncrementDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<DungeonBossIncrementCSVData> dungeonBossIncrementCSVDatas = new List<DungeonBossIncrementCSVData>();

        DungeonType dungeonType = DungeonType.None;

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            if (elements[1] != "")
            {
                dungeonType = EnumUtility.GetEqualValue<DungeonType>(elements[1]);
            }

            if (elements[6] != INCREMENT_PER_LEVEL)
            {
                continue;
            }
            string damage = elements[7];
            string health = elements[8];
            string reward = elements[9];
            int battlePercentMod = int.Parse(elements[10]);
            int rewardPercentMod = int.Parse(elements[11].Trim('\r'));
            DungeonBossIncrementCSVData dungeonBossIncrementCSVData = new DungeonBossIncrementCSVData(dungeonType, damage, health, reward,
                battlePercentMod, rewardPercentMod);

            dungeonBossIncrementCSVDatas.Add(dungeonBossIncrementCSVData);
        }

        dungeonBossIncrementDataSO.AddDatas(dungeonBossIncrementCSVDatas);
        dungeonBossIncrementDataSO.InitDict();
        EditorUtility.SetDirty(dungeonBossIncrementDataSO);
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<DungeonBossIncrementCSVData> dungeonBossIncrementCSVDatas = new List<DungeonBossIncrementCSVData>();

        DungeonType dungeonType = DungeonType.None;

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            if (elements[1] != "")
            {
                dungeonType = EnumUtility.GetEqualValue<DungeonType>(elements[1]);
            }

            if (elements[6] != INCREMENT_PER_LEVEL)
            {
                continue;
            }
            string damage = elements[7];
            string health = elements[8];
            string reward = elements[9];
            int battlePercentMod = int.Parse(elements[10]);
            int rewardPercentMod = int.Parse(elements[11].Trim('\r'));
            DungeonBossIncrementCSVData dungeonBossIncrementCSVData = new DungeonBossIncrementCSVData(dungeonType, damage, health, reward,
                battlePercentMod, rewardPercentMod);

            dungeonBossIncrementCSVDatas.Add(dungeonBossIncrementCSVData);
        }

        dataSO.AddDatas(dungeonBossIncrementCSVDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

}
