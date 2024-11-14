using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MonsterCoreInfoData
{
    public CoreInfoData coreInfoData;
    public MonsterType monsterType;

    public MonsterCoreInfoData(CoreInfoData coreInfoData, MonsterType monsterType)
    {
        this.coreInfoData = coreInfoData;
        this.monsterType = monsterType;
    }
}
