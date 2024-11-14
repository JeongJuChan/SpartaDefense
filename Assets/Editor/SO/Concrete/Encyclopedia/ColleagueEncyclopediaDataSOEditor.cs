using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColleagueEncyclopediaDataSO))]
public class ColleagueEncyclopediaDataSOEditor : ListDataSOEditor<ColleagueEncyclopediaDataSO>
{
    public static void LoadCSVToSO(ColleagueEncyclopediaDataSO colleagueEncyclopediaDataSO, TextAsset csv)
    {
        colleagueEncyclopediaDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<ColleagueEncyclopediaData> colleagueEncyclopediaDatas = new List<ColleagueEncyclopediaData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            ColleagueEncyclopediaType colleagueEncyclopediaType = EnumUtility.GetEqualValue<ColleagueEncyclopediaType>(elements[0]);
            string colleagueEncyclopediaNameKR = elements[1];
            ColleagueType colleagueType1 = EnumUtility.GetEqualValue<ColleagueType>(elements[3]);
            ColleagueType colleagueType2 = EnumUtility.GetEqualValue<ColleagueType>(elements[4]);
            ColleagueType colleagueType3 = EnumUtility.GetEqualValue<ColleagueType>(elements[5]);
            StatType statType = EnumUtility.GetEqualValue<StatType>(elements[6]);

            ColleagueType[] colleagueTypes = new ColleagueType[3];
            colleagueTypes[0] = colleagueType1;
            colleagueTypes[1] = colleagueType2;
            colleagueTypes[2] = colleagueType3;

            colleagueEncyclopediaDatas.Add(new ColleagueEncyclopediaData(colleagueEncyclopediaType, colleagueEncyclopediaNameKR, 0,
                colleagueTypes, statType));
        }

        colleagueEncyclopediaDataSO.AddDatas(colleagueEncyclopediaDatas);
        EditorUtility.SetDirty(colleagueEncyclopediaDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');
        Debug.Log(rows[0]);

        List<ColleagueEncyclopediaData> colleagueEncyclopediaDatas = new List<ColleagueEncyclopediaData>();

        ColleagueEncyclopediaType preColleagueEncyclopedia = ColleagueEncyclopediaType.None;

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            ColleagueEncyclopediaType colleagueEncyclopediaType = EnumUtility.GetEqualValue<ColleagueEncyclopediaType>(elements[0]);
            if (preColleagueEncyclopedia != colleagueEncyclopediaType)
            {
                string colleagueEncyclopediaNameKR = elements[1];
                ColleagueType colleagueType1 = EnumUtility.GetEqualValue<ColleagueType>(elements[3]);
                ColleagueType colleagueType2 = EnumUtility.GetEqualValue<ColleagueType>(elements[4]);
                ColleagueType colleagueType3 = EnumUtility.GetEqualValue<ColleagueType>(elements[5]);
                StatType statType = EnumUtility.GetEqualValue<StatType>(elements[6]);

                ColleagueType[] colleagueTypes = new ColleagueType[3];
                colleagueTypes[0] = colleagueType1;
                colleagueTypes[1] = colleagueType2;
                colleagueTypes[2] = colleagueType3;

                colleagueEncyclopediaDatas.Add(new ColleagueEncyclopediaData(colleagueEncyclopediaType, colleagueEncyclopediaNameKR, 0, 
                    colleagueTypes, statType));

                preColleagueEncyclopedia = colleagueEncyclopediaType;
            }
        }

        dataSO.AddDatas(colleagueEncyclopediaDatas);
        EditorUtility.SetDirty(dataSO);
    }
}
