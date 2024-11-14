using Keiwando.BigInteger;
using System;

public class SlotStatCalculator
{
    private Func<Rank, SlotStatData> OnGetEquipmentData;
    private int levelVariation;

    public SlotStatCalculator(Func<Rank, SlotStatData> OnGetEquipmentData, int levelVariation)
    {
        this.OnGetEquipmentData = OnGetEquipmentData;
        this.levelVariation = levelVariation;
    }

    public SlotStatData GetSlotStatDataUpdated(Rank rank, int level)
    {
        SlotStatData newSlotStatData = OnGetEquipmentData(rank);
        newSlotStatData.level = level;

        BigInteger increment = (level - 1) * newSlotStatData.increment;

        newSlotStatData.equipmentStatData.mainDamage += increment;
        int variation = UnityEngine.Random.Range(-levelVariation * newSlotStatData.variationPercent, 
            levelVariation * newSlotStatData.variationPercent);
        newSlotStatData.equipmentStatData.mainDamage += newSlotStatData.equipmentStatData.mainDamage * variation / Consts.PERCENT_DIVIDE_VALUE;

        newSlotStatData.equipmentStatData.health += increment;
        variation = UnityEngine.Random.Range(-levelVariation * newSlotStatData.variationPercent, 
            levelVariation * newSlotStatData.variationPercent);
        newSlotStatData.equipmentStatData.health += newSlotStatData.equipmentStatData.health * variation / Consts.PERCENT_DIVIDE_VALUE;

        newSlotStatData.equipmentStatData.defense += increment;
        variation = UnityEngine.Random.Range(-levelVariation * newSlotStatData.variationPercent, 
            levelVariation * newSlotStatData.variationPercent);
        newSlotStatData.equipmentStatData.defense += newSlotStatData.equipmentStatData.defense * variation / Consts.PERCENT_DIVIDE_VALUE;

        newSlotStatData.gold += increment;
        variation = UnityEngine.Random.Range(-levelVariation * newSlotStatData.variationPercent,
            levelVariation * newSlotStatData.variationPercent);
        newSlotStatData.gold += newSlotStatData.gold * variation / Consts.PERCENT_DIVIDE_VALUE;

        newSlotStatData.exp += increment;
        variation = UnityEngine.Random.Range(-levelVariation * newSlotStatData.variationPercent,
            levelVariation * newSlotStatData.variationPercent);
        newSlotStatData.exp += newSlotStatData.exp * variation / Consts.PERCENT_DIVIDE_VALUE;

        return newSlotStatData;
    }

    public SlotStatData GetSlotStatDataSolidUpdated(Rank rank, int level)
    {
        SlotStatData newSlotStatData = OnGetEquipmentData(rank);
        newSlotStatData.level = level;
        return newSlotStatData;
    }
}