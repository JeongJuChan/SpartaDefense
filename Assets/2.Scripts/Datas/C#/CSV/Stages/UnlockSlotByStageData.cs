using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UnlockSlotByStageData
{
    public int stageIndex;
    public int forgeNum;
    public int skillNum;

    public UnlockSlotByStageData(int stageIndex, int forgeNum, int skillNum)
    {
        this.stageIndex = stageIndex;
        this.forgeNum = forgeNum;
        this.skillNum = skillNum;
    }
}
