using System;
using System.Collections.Generic;

public class SlotStatRoller
{
    private List<EquipmentType> equipments = new List<EquipmentType>();
    private Dictionary<EquipmentType, bool> equipmentIsInstantiatedDict = new Dictionary<EquipmentType, bool>();

    private Func<Rank, AttributeData> OnGetAttributeData;

    private List<EquipmentType> unSummonedEquipments = new List<EquipmentType>();

    public SlotStatRoller(SlotEquipmentForger slotEquipmentForger, Func<Rank, AttributeData> OnGetAttributeData)
    {
        //slotEquipmentForge.OnGetAttributeTypes += GetAttributeType;
        slotEquipmentForger.OnGetRandomEquipmentType += GetRandomEquipmentType;
        slotEquipmentForger.OnGetIsSlotTypeInstantiated += GetIsEquipmentInstantiated;
        this.OnGetAttributeData = OnGetAttributeData;

        EquipmentType[] equipmentTypes = (EquipmentType[])Enum.GetValues(typeof(EquipmentType));
        for (int i = 1; i < equipmentTypes.Length; i++)
        {
            unSummonedEquipments.Add(equipmentTypes[i]);
        }
    }

    public void AddSlotType(EquipmentType equipmentType)
    {
        if (!equipments.Contains(equipmentType) && equipmentType != EquipmentType.None)
        {
            equipments.Add(equipmentType);
            unSummonedEquipments.Remove(equipmentType);
            equipmentIsInstantiatedDict.Add(equipmentType, false);
        }
    }

    public int GetOpenSlotCount()
    {
        return equipments.Count;
    }

    private Dictionary<int, float> GetAttributeType(Rank rank)
    {
        if (rank == Rank.Common || rank == Rank.None)
        {
            return null;
        }

        Dictionary<int, float> attributeStatDict = new Dictionary<int, float>();

        AttributeData attributeData = OnGetAttributeData.Invoke(rank);
        AttributeRankProbabilityData probilityData = attributeData.rankProbability;

        int percent = UnityEngine.Random.Range(0, Consts.PERCENT_TOTAL_VALUE);
        int total = 0;
        for (int i = 0; i < probilityData.attributeProbabilityDatas.Length; i++)
        {
            int tempPercent = percent;
            total += probilityData.attributeProbabilityDatas[i];
            tempPercent -= total;

            if (tempPercent < 0)
            {
                if (i == 0)
                {
                    return null;
                }

                attributeStatDict.Add(i - 1, attributeData.attributeAppliedData.attributeAppliedStat[i - 1]);
                return attributeStatDict;
            }
        }

        return null;
    }

    public bool GetIsEquipmentTypeSummoned(EquipmentType equipmentType)
    {
        return equipments.Contains(equipmentType);
    }

    public EquipmentType GetRandomEquipmentType()
    {
        int count;
        if (unSummonedEquipments.Count != 0)
        {
            count = unSummonedEquipments.Count;
            int index = UnityEngine.Random.Range(0, count);
            return unSummonedEquipments[index];
        }
        else
        {
            count = equipments.Count;
            int index = UnityEngine.Random.Range(0, count);
            return equipments[index];
        }
        /*if (unSummonedEquipments.Count > OnGetHeroes.Invoke().Count)
        {
            foreach (EquipmentType key in OnGetHeroes.Invoke().Keys)
            {
                unSummonedEquipments.Remove(key);
            }

            index = UnityEngine.Random.Range(0, unSummonedEquipments.Count);
            return unSummonedEquipments[index];
        }
        else
        {
            index = UnityEngine.Random.Range(0, equipments.Count);
            return equipments[index];
        }*/
    }

    private bool GetIsEquipmentInstantiated(EquipmentType equipmentType)
    {
        return true /*OnGetHeroes.Invoke().ContainsKey(equipmentType)*/;
    }
}