using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MonsterUpgradableData
{
    public BigInteger health;
    public BigInteger damage;
    public float attackSpeedRate;
    public float speed;

    public MonsterUpgradableData(BigInteger health, BigInteger damage, float attackSpeedRate, float speed)
    {
        this.health = health;
        this.damage = damage;
        this.attackSpeedRate = attackSpeedRate;
        this.speed = speed;
    }
}
