using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MonsterCSVData
{
    public CoreInfoData coreInfoData;
    public MonsterUpgradableCSVData monsterUpgradableCSVData;

    public MonsterCSVData(CoreInfoData coreInfoData, MonsterUpgradableCSVData monsterUpgradableCSVData)
    {
        this.coreInfoData = coreInfoData;
        this.monsterUpgradableCSVData = monsterUpgradableCSVData;
    }
}
