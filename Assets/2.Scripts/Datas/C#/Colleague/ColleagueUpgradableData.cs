using Keiwando.BigInteger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ColleagueUpgradableData
{
    public int index;
    public BigInteger count;
    public int starCount;
    public int level;
    public BigInteger damage;
    public BigInteger health;
    public BigInteger defense;
    public BigInteger power;

    public ColleagueUpgradableData(int index, BigInteger count, int starCount, int level, BigInteger damage, int health, int defense, 
        BigInteger power)
    {
        this.index = index;
        this.count = count;
        this.starCount = starCount;
        this.level = level;
        this.damage = damage;
        this.health = health;
        this.defense = defense;
        this.power = power;
    }
}
