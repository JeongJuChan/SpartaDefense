using Keiwando.BigInteger;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(CastleProgressionDataSO))]
public class CastleProgressionDataSOEditor : ListDataSOEditor<CastleProgressionDataSO>
{
    public static void LoadCSVToSO(CastleProgressionDataSO castleProgressionDataSO, TextAsset csv)
    {
        castleProgressionDataSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<CastleProgressionData> castleProgressionDatas = new List<CastleProgressionData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int level = int.Parse(elements[0]);
            string name = elements[1];
            int RequiredCharacterLevel = int.Parse(elements[2]);
            int ForgeSlotOpen = int.Parse(elements[3]);
            int StageClear = int.Parse(elements[4]);
            int BaseHP = int.Parse(elements[5]);
            int BaseAttack = int.Parse(elements[6]);
            int BaseDefense = int.Parse(elements[7]);

            Sprite castleSprite = Resources.Load<Sprite>("Sprites/Castles/CastleLv_" + level);

            CastleProgressionData castleProgressionData = new CastleProgressionData(level, name, RequiredCharacterLevel, ForgeSlotOpen, StageClear, BaseHP, BaseAttack, BaseDefense, castleSprite);
            castleProgressionDatas.Add(castleProgressionData);
        };

        castleProgressionDataSO.AddDatas(castleProgressionDatas);
        castleProgressionDataSO.InitDict();
        EditorUtility.SetDirty(castleProgressionDataSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<CastleProgressionData> castleProgressionDatas = new List<CastleProgressionData>();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] elements = rows[i].Split(',');
            int level = int.Parse(elements[0]);
            string name = elements[1];
            int RequiredCharacterLevel = int.Parse(elements[2]);
            int ForgeSlotOpen = int.Parse(elements[3]);
            int StageClear = int.Parse(elements[4]);
            int BaseHP = int.Parse(elements[5]);
            int BaseAttack = int.Parse(elements[6]);
            int BaseDefense = int.Parse(elements[7]);

            Sprite castleSprite = Resources.Load<Sprite>("Sprites/Castles/CastleLv_" + level);

            CastleProgressionData castleProgressionData = new CastleProgressionData(level, name, RequiredCharacterLevel, ForgeSlotOpen, StageClear, BaseHP, BaseAttack, BaseDefense, castleSprite);
            castleProgressionDatas.Add(castleProgressionData);
        };

        dataSO.AddDatas(castleProgressionDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
