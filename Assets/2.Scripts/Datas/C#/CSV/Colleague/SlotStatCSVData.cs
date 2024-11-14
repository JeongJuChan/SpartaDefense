using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SlotStatCSVData
{
    public Rank rank;
    public int level;
    public SlotEquipmentStatCSVData slotEquipmentStatCSVData;
    public string gold;
    public string exp;
    public int increment;
    public int variationPercent;
    public int heroDamagePercent;

    public SlotStatCSVData(Rank rank, int level, SlotEquipmentStatCSVData slotEquipmentStatCSVData, string gold, string exp, 
        int increment, int variationPercent, int heroDamagePercent)
    {
        this.rank = rank;
        this.level = level;
        this.slotEquipmentStatCSVData = slotEquipmentStatCSVData;
        this.gold = gold;
        this.exp = exp;
        this.increment = increment;
        this.variationPercent = variationPercent;
        this.heroDamagePercent = heroDamagePercent;
    }
}
