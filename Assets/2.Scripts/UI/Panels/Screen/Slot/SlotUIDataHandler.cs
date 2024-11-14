using System.Collections.Generic;
using UnityEngine;

public class SlotUIDataHandler
{
    [Header("Current SlotData")]
    private Dictionary<EquipmentType, SlotUIData> currentSlotUIDataDict = new Dictionary<EquipmentType, SlotUIData>();

    [Header("New SlotData")]
    private Dictionary<EquipmentType, SlotUIData> newSlotUIDataDict = new Dictionary<EquipmentType, SlotUIData>();

    public SlotUIDataHandler(NewSlotUIPanel newSlotUIPanel, CurrentSlotUIPanel currentSlotUIPanel, SlotDetailPanel slotDetailPanel)
    {
/*        newSlotUIPanel.OnGetCurrentSlotUIData += GetCurrentSlotUIData;
        newSlotUIPanel.OnGetNewSlotUIData += GetNewSlotUIData;
        newSlotUIPanel.OnSwapSlotData += SwapSlotPanels;

        newSlotUIPanel.OnUpdateCurrentForgeUI += currentSlotUIPanel.UpdateCurrentStatsText;
        newSlotUIPanel.OnActiveCurrentSlotPanel += currentSlotUIPanel.SetActive;*/

        slotDetailPanel.OnGetSlotStatUIData += GetCurrentSlotUIData;

        /*foreach (EquipmentType equipmentType in System.Enum.GetValues(typeof(EquipmentType)))
        {
            if (equipmentType == EquipmentType.None)
            {
                continue;
            }
            SlotUIData slotUIData = new SlotUIData();
            slotUIData.equipmentType = equipmentType;

            // currentSlotUIDataDict.Add(slotType, slotUIData);
            if (ES3.KeyExists($"{equipmentType}{Consts.SLOT_NICKNAME}", ES3.settings))
            {
                SetCurrentSlotUIData(slotUIData);
            }
        }*/

    }

    public void SwapSlotPanels(SlotUIData newSlotUIData)
    {
        SlotUIData tempSlotUIData = newSlotUIData;
        SlotUIData currentSlotUIData = GetCurrentSlotUIData(newSlotUIData.equipmentType);
        tempSlotUIData = currentSlotUIData;
        currentSlotUIData = newSlotUIData;
        newSlotUIData = tempSlotUIData;

        newSlotUIData.equipmentType = currentSlotUIData.equipmentType;

        SetCurrentSlotUIData(currentSlotUIData);
        SetNewSlotUIData(newSlotUIData);
    }

    public SlotUIData GetNewSlotUIData(EquipmentType equipmentType)
    {
        if (newSlotUIDataDict.ContainsKey(equipmentType))
        {
            return newSlotUIDataDict[equipmentType];
        }

        return default;
    }

    public SlotUIData GetCurrentSlotUIData(EquipmentType equipmentType)
    {
        if (currentSlotUIDataDict.ContainsKey(equipmentType))
        {
            return currentSlotUIDataDict[equipmentType];
        }

        return default;
    }

    public void SetCurrentSlotUIData(SlotUIData slotUIData)
    {
        EquipmentType equipmentType = slotUIData.equipmentType;
        if (!currentSlotUIDataDict.ContainsKey(equipmentType) && equipmentType != EquipmentType.None)
        {
            currentSlotUIDataDict.Add(equipmentType, slotUIData);
        }

        currentSlotUIDataDict[equipmentType] = slotUIData;
    }

    public void SetNewSlotUIData(SlotUIData slotUIData)
    {
        EquipmentType equipmentType = slotUIData.equipmentType;
        if (!newSlotUIDataDict.ContainsKey(equipmentType) && equipmentType != EquipmentType.None)
        {
            newSlotUIDataDict.Add(equipmentType, slotUIData);
        }

        newSlotUIDataDict[equipmentType] = slotUIData;
    }


}