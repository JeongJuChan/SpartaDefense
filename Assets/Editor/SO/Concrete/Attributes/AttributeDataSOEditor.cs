using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AttributeDataSO))]
public class AttributeDataSOEditor : ListDataSOEditor<AttributeDataSO>
{
    public static void LoadCSVToSO(AttributeDataSO attributeDataSO, TextAsset csv)
    {
        attributeDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<AttributeData> castleDoorRankProbabilityDatas = new List<AttributeData>();

        AttributeRankProbabilityData attributeRankProbability = new AttributeRankProbabilityData();
        AttributeAppliedData attributeAppliedDatas = new AttributeAppliedData();

        int attributeLength = Enum.GetValues(typeof(AttributeType)).Length;
        Rank preRank = Rank.None;
        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[0]);

            if (rank != Rank.None)
            {
                int[] attributeProbabilityDatas = new int[attributeLength];

                preRank = rank;
                attributeProbabilityDatas[0] = (int)(float.Parse(elements[1].Trim('%')) * Consts.PERCENT_DIVIDE_VALUE);
                attributeProbabilityDatas[1] = (int)(float.Parse(elements[2].Trim('%')) * Consts.PERCENT_DIVIDE_VALUE);
                attributeProbabilityDatas[2] = (int)(float.Parse(elements[3].Trim('%')) * Consts.PERCENT_DIVIDE_VALUE);
                attributeProbabilityDatas[3] = (int)(float.Parse(elements[4].Trim('%')) * Consts.PERCENT_DIVIDE_VALUE);
                attributeProbabilityDatas[4] = (int)(float.Parse(elements[5].Trim('%', '\r')) * Consts.PERCENT_DIVIDE_VALUE);

                attributeRankProbability = new AttributeRankProbabilityData(attributeProbabilityDatas);
            }
            else
            {
                float[] attributeProbabilityStats = new float[attributeLength - 1];
                attributeProbabilityStats[0] = float.Parse(elements[2]);
                attributeProbabilityStats[1] = float.Parse(elements[3]);
                attributeProbabilityStats[2] = float.Parse(elements[4]);
                attributeProbabilityStats[3] = float.Parse(elements[5].Trim('\r'));

                attributeAppliedDatas = new AttributeAppliedData(attributeProbabilityStats);
            }

            if (preRank != Rank.None && attributeAppliedDatas.attributeAppliedStat != null && attributeAppliedDatas.attributeAppliedStat[2] != 0f)
            {
                castleDoorRankProbabilityDatas.Add(new AttributeData(preRank, attributeRankProbability, attributeAppliedDatas));
                preRank = Rank.None;
                attributeAppliedDatas = new AttributeAppliedData();
            }
        }

        attributeDataSO.AddDatas(castleDoorRankProbabilityDatas);
        attributeDataSO.InitDict();
        EditorUtility.SetDirty(attributeDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        SetTarget();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<AttributeData> castleDoorRankProbabilityDatas = new List<AttributeData>();

        AttributeRankProbabilityData attributeRankProbability = new AttributeRankProbabilityData();
        AttributeAppliedData attributeAppliedDatas = new AttributeAppliedData();

        int attributeLength = Enum.GetValues(typeof(AttributeType)).Length;
        Rank preRank = Rank.None;
        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            Rank rank = EnumUtility.GetEqualValue<Rank>(elements[0]);

            if (rank != Rank.None)
            {
                int[] attributeProbabilityDatas = new int[attributeLength];

                preRank = rank;
                attributeProbabilityDatas[0] = (int)(float.Parse(elements[1].Trim('%')) * Consts.PERCENT_DIVIDE_VALUE);
                attributeProbabilityDatas[1] = (int)(float.Parse(elements[2].Trim('%')) * Consts.PERCENT_DIVIDE_VALUE);
                attributeProbabilityDatas[2] = (int)(float.Parse(elements[3].Trim('%')) * Consts.PERCENT_DIVIDE_VALUE);
                attributeProbabilityDatas[3] = (int)(float.Parse(elements[4].Trim('%')) * Consts.PERCENT_DIVIDE_VALUE);
                attributeProbabilityDatas[4] = (int)(float.Parse(elements[5].Trim('%')) * Consts.PERCENT_DIVIDE_VALUE);
                attributeProbabilityDatas[5] = (int)(float.Parse(elements[6].Trim('%')) * Consts.PERCENT_DIVIDE_VALUE);
                attributeProbabilityDatas[6] = (int)(float.Parse(elements[7].Trim('%', '\r')) * Consts.PERCENT_DIVIDE_VALUE);

                attributeRankProbability = new AttributeRankProbabilityData(attributeProbabilityDatas);
            }
            else
            {
                float[] attributeProbabilityStats = new float[attributeLength - 1];
                attributeProbabilityStats[0] = float.Parse(elements[2]);
                attributeProbabilityStats[1] = float.Parse(elements[3]);
                attributeProbabilityStats[2] = float.Parse(elements[4]);
                attributeProbabilityStats[3] = float.Parse(elements[5]);
                attributeProbabilityStats[4] = float.Parse(elements[6]);
                attributeProbabilityStats[5] = float.Parse(elements[7].Trim('\r'));

                attributeAppliedDatas = new AttributeAppliedData(attributeProbabilityStats);
            }

            if (preRank != Rank.None && attributeAppliedDatas.attributeAppliedStat != null && attributeAppliedDatas.attributeAppliedStat[2] != 0f)
            {
                castleDoorRankProbabilityDatas.Add(new AttributeData(preRank, attributeRankProbability, attributeAppliedDatas));
                preRank = Rank.None;
                attributeAppliedDatas = new AttributeAppliedData();
            }
        }

        dataSO.AddDatas(castleDoorRankProbabilityDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
