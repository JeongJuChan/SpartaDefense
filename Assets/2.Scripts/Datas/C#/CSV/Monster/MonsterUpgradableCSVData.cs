using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MonsterUpgradableCSVData
{
    public string health;
    public string damage;
    public float attackSpeedRate;
    public float speed;

    public MonsterUpgradableCSVData(string health, string damage, float attackSpeedRate, float speed)
    {
        this.health = health;
        this.damage = damage;
        this.attackSpeedRate = attackSpeedRate;
        this.speed = speed;
    }
}
