using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GrowthDataSO))]
public class GrowthDataSOEditor : ListDataSOEditor<GrowthDataSO>
{
    public static void LoadCSVToSO(GrowthDataSO growthDataSO, TextAsset csv)
    {
        growthDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        List<GrowthData> growthDatas = new List<GrowthData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            int trainingLevel = int.Parse(elements[0]);
            int desiredForgeLevel = int.Parse(elements[1]);
            int levelUpMax = int.Parse(elements[2]);
            int currencyBasePerLevel = int.Parse(elements[3]);
            int currencyIncrementPerLevel = int.Parse(elements[4]);
            int[] incrementsPerLevel = new int[] { int.Parse(elements[5]), int.Parse(elements[6]), int.Parse(elements[7].Trim('\r')) };

            growthDatas.Add(new GrowthData(trainingLevel, desiredForgeLevel, levelUpMax, currencyBasePerLevel, 
                currencyIncrementPerLevel, incrementsPerLevel));
        }

        growthDataSO.AddDatas(growthDatas);
        growthDataSO.InitDict();
        EditorUtility.SetDirty(growthDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        dataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        List<GrowthData> growthDatas = new List<GrowthData>();

        for(int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');

            int trainingLevel = int.Parse(elements[0]);
            int desiredForgeLevel = int.Parse(elements[1]);
            int levelUpMax = int.Parse(elements[2]);
            int currencyBasePerLevel = int.Parse(elements[3]);
            int currencyIncrementPerLevel = int.Parse(elements[4]);
            int[] incrementsPerLevel = new int[] { int.Parse(elements[5]), int.Parse(elements[6]), int.Parse(elements[7].Trim('\r')) };

            growthDatas.Add(new GrowthData(trainingLevel, desiredForgeLevel, levelUpMax, currencyBasePerLevel,
                currencyIncrementPerLevel, incrementsPerLevel));
        }

        dataSO.AddDatas(growthDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
