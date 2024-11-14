using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CastleDoorRankProbabilityDataSO))]
public class CastleDoorRankProbabilityDataSOEditor : ListDataSOEditor<CastleDoorRankProbabilityDataSO>
{
    public static void LoadCSVToSO(CastleDoorRankProbabilityDataSO castleDoorRankProbabilityDataSO, TextAsset csv)
    {
        castleDoorRankProbabilityDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<CastleDoorRankProbabilityData> castleDoorRankProbabilityDatas = new List<CastleDoorRankProbabilityData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int level = int.Parse(elements[0]);
            int[] rankProbability = new int[Enum.GetValues(typeof(Rank)).Length - 1];
            for (int j = 1; j <= rankProbability.Length; j++)
            {
                int probability;
                if (j == rankProbability.Length)
                {
                    probability = (int)(decimal.Parse(elements[j].Trim('\r')) * 100);
                }
                else
                {
                    probability = (int)(decimal.Parse(elements[j]) * 100);
                }
                rankProbability[j - 1] = probability;
            }
            castleDoorRankProbabilityDatas.Add(new CastleDoorRankProbabilityData(level, rankProbability));
        }

        castleDoorRankProbabilityDataSO.AddDatas(castleDoorRankProbabilityDatas);
        castleDoorRankProbabilityDataSO.InitDict();
        EditorUtility.SetDirty(castleDoorRankProbabilityDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<CastleDoorRankProbabilityData> castleDoorRankProbabilityDatas = new List<CastleDoorRankProbabilityData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int level = int.Parse(elements[0]);
            int[] rankProbability = new int[Enum.GetValues(typeof(Rank)).Length - 1];
            for (int j = 1; j <= rankProbability.Length; j++)
            {
                int probability;
                if (j == rankProbability.Length)
                {
                    probability = (int)(decimal.Parse(elements[j].Trim('\r')) * 100);
                }
                else
                {
                    probability = (int)(decimal.Parse(elements[j]) * 100);
                }
                rankProbability[j - 1] = probability;
            }
            castleDoorRankProbabilityDatas.Add(new CastleDoorRankProbabilityData(level, rankProbability));
        }

        dataSO.AddDatas(castleDoorRankProbabilityDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }

    
}
