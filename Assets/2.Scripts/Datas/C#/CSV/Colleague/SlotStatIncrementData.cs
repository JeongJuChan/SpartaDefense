using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SlotStatIncrementData
{
    public Rank rank;
    public int hp;
    public int damage;
    public int defense;
    public int exp;
    public int gold;

    public SlotStatIncrementData(Rank rank, int hp, int damage, int defense, int exp, int gold)
    {
        this.rank = rank;
        this.hp = hp;
        this.damage = damage;
        this.defense = defense;
        this.exp = exp;
        this.gold = gold;
    }
}
