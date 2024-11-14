using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatInfoPanel : MonoBehaviour, UIInitNeeded
{
    [SerializeField] private SlotDetailAttributePanel[] playerStatInfoSlots;
    [SerializeField] private SlotDetailAttributePanel playertotalStatInfoSlot;
    [SerializeField] private Button closeButton;

    public void Init()
    {
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));

        StatDataHandler.Instance.OnUpdateEquipmentStatData += UpdatePlayerStatInfo;

        foreach (SlotDetailAttributePanel slotDetailAttributePanel in playerStatInfoSlots)
        {
            slotDetailAttributePanel.SetActive(false);
        }

        gameObject.SetActive(false);

        StatDataHandler.Instance.OnUpdateAttributeText += UpdateAttributeTypeStatText;
        StatDataHandler.Instance.OnUpdateTotalPower += UpdateTotalPower;
    }

    private void UpdatePlayerStatInfo(SlotEquipmentStatData equipmentStatData)
    {
        playerStatInfoSlots[0].SetActive(true);
        playerStatInfoSlots[1].SetActive(true);
        playerStatInfoSlots[2].SetActive(true);

        BigInteger damage = equipmentStatData.mainDamage == null ? 0 : equipmentStatData.mainDamage;
        BigInteger health = equipmentStatData.health == null ? 0 : equipmentStatData.health;
        BigInteger defense = equipmentStatData.defense == null ? 0 : equipmentStatData.defense;
        playerStatInfoSlots[0].UpdateAttributeText(EnumToKRManager.instance.GetEnumToKR(StatType.Damage), damage);
        playerStatInfoSlots[1].UpdateAttributeText(EnumToKRManager.instance.GetEnumToKR(StatType.HP), health);
        playerStatInfoSlots[2].UpdateAttributeText(EnumToKRManager.instance.GetEnumToKR(StatType.Defense), defense);

        /*int index = 3;
        AttributeType attributeType;
        AttributeAppliedData attributeAppliedData = StatViewerHelper.instance.GetAttributeAppliedData();
        for (int i = 1; i < Enum.GetValues(typeof(AttributeType)).Length; i++)
        {
            attributeType = (AttributeType)i;
            playerStatInfoSlots[index].SetActive(true);
            playerStatInfoSlots[index].UpdateAttributeText(EnumToKR.GetAttributeTypeKR(attributeType), attributeAppliedData.attributeAppliedStat[i - 1]);
            index++;
        }*/
    }

    private void UpdateTotalPower(BigInteger totalPower)
    {
        playertotalStatInfoSlot.SetActive(true);
        playertotalStatInfoSlot.UpdateAttributeText("Total", totalPower);
    }

    private void UpdateAttributeTypeStatText(AttributeType attributeType, float percent)
    {
        playerStatInfoSlots[(int)attributeType + 2].SetActive(percent != 0);
        playerStatInfoSlots[(int)attributeType + 2].UpdateAttributeText(EnumToKRManager.instance.GetEnumToKR(attributeType), percent);
    }
}