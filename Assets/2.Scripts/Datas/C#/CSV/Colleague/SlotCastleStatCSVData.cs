using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SlotEquipmentStatCSVData
{
    public string mainDamage;
    public string hp;
    public string defense;

    public SlotEquipmentStatCSVData(string mainDamage, string hp, string defense)
    {
        this.mainDamage = mainDamage;
        this.hp = hp;
        this.defense = defense;
    }
}
