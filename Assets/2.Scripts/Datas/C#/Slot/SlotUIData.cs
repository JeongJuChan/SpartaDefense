using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SlotUIData
{
    public string nickName;
    public string rankKR;
    public Color rankColor;
    public EquipmentType equipmentType;
    public string equipmentName;
    public SlotStatData slotStatData;
    public Sprite rankSprite;
    public Sprite mainSprite;
    public Dictionary<int, float> attributeStatDict;

    public SlotUIData(string nickName, string rankKR, Color rankColor, EquipmentType equipmentType, string equipmentName,
        SlotStatData slotStatData, Sprite rankSprite, Sprite mainSprite, Dictionary<int, float> attributeStatDict)
    {
        this.nickName = nickName;
        this.rankKR = rankKR;
        this.rankColor = rankColor;
        this.equipmentType = equipmentType;
        this.equipmentName = equipmentName;
        this.slotStatData = slotStatData;
        this.rankSprite = rankSprite;
        this.mainSprite = mainSprite;
        this.attributeStatDict = attributeStatDict;
    }

    public void SetSlotData(SlotUIData slotdata)
    {
        nickName = slotdata.nickName;
        rankKR = EnumToKRManager.instance.GetEnumToKR(slotdata.slotStatData.rank);
        rankColor = ResourceManager.instance.rank.GetRankColor(slotdata.slotStatData.rank);
        equipmentType = slotdata.equipmentType;
        equipmentName = slotdata.nickName;
        rankSprite = ResourceManager.instance.rank.GetRankBackgroundSprite(slotdata.slotStatData.rank);
        mainSprite = ResourceManager.instance.forgeEquipmentResourceDataHandler.GetForgeEquipmentSprite(
            new ForgeEquipmentInfo(slotdata.equipmentType, slotdata.slotStatData.rank));
    }

    public void DeleteNewSlotData()
    {
        if (ES3.KeyExists($"{equipmentType}{Consts.SLOT_NICKNAME}{false}", ES3.settings))
        {
            ES3.DeleteKey($"{equipmentType}{Consts.SLOT_NICKNAME}{false}", ES3.settings);
        }
        if (ES3.KeyExists($"{equipmentType}{Consts.SLOT_STAT_DATA}{false}", ES3.settings))
        {
            ES3.DeleteKey($"{equipmentType}{Consts.SLOT_STAT_DATA}{false}", ES3.settings);
        }
        if (ES3.KeyExists($"{equipmentType}{Consts.SLOT_ATTRIBUTE_STAT_DICT}{false}", ES3.settings))
        {
            ES3.DeleteKey($"{equipmentType}{Consts.SLOT_ATTRIBUTE_STAT_DICT}{false}", ES3.settings);
        }
    }

    public void SaveDatas(bool isCurrentSlot)
    {
        if (equipmentType == EquipmentType.None)
        {
            DeleteNewSlotData();
            return;
        }

        slotStatData.SaveDatas(equipmentType, isCurrentSlot);

        //ES3.Save<string>($"{equipmentType}{Consts.SLOT_RANK}{isCurrentSlot}", nickName, ES3.settings);
        ES3.Save<string>($"{equipmentType}{Consts.SLOT_NICKNAME}{isCurrentSlot}", nickName, ES3.settings);
        ES3.Save<SlotStatData>($"{equipmentType}{Consts.SLOT_STAT_DATA}{isCurrentSlot}", slotStatData, ES3.settings);
        ES3.Save<Dictionary<int, float>>($"{equipmentType}{Consts.SLOT_ATTRIBUTE_STAT_DICT}{isCurrentSlot}", attributeStatDict, ES3.settings);

        ES3.StoreCachedFile();
    }

    public void LoadDatas(bool isCurrentSlot)
    {
        if (!ES3.KeyExists($"{equipmentType}{Consts.SLOT_NICKNAME}{isCurrentSlot}"))
        {
            return;
        }

        nickName = ES3.Load<string>($"{equipmentType}{Consts.SLOT_NICKNAME}{isCurrentSlot}", ES3.settings);
        slotStatData = ES3.Load<SlotStatData>($"{equipmentType}{Consts.SLOT_STAT_DATA}{isCurrentSlot}", ES3.settings);
        attributeStatDict = ES3.Load<Dictionary<int, float>>($"{equipmentType}{Consts.SLOT_ATTRIBUTE_STAT_DICT}{isCurrentSlot}", ES3.settings);

        slotStatData.LoadDatas(equipmentType, isCurrentSlot);
    }
}
