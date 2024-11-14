using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public struct MonsterData
{
    public MonsterCoreInfoData coreInfoData;
    public MonsterUpgradableData monsterUpgradableData;
    
    public MonsterData(MonsterCoreInfoData coreInfoData, MonsterUpgradableData monsterUpgradableData)
    {
        this.coreInfoData = coreInfoData;
        this.monsterUpgradableData = monsterUpgradableData;
    }
}
