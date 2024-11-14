using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ColleagueData : ISummonable
{
    public int index;
    public int skillIndex;
    public string colleagueName;
    public string nickName;
    public ColleagueInfo colleagueInfo;
    public ShootingType shootingType;

    public ColleagueData(int index, int skillIndex, string colleagueName, string nickName, ColleagueInfo colleagueInfo, ShootingType shootingType)
    {
        this.index = index;
        this.skillIndex = skillIndex;
        this.colleagueName = colleagueName;
        this.nickName = nickName;
        this.colleagueInfo = colleagueInfo;
        this.shootingType = shootingType;
    }
}
