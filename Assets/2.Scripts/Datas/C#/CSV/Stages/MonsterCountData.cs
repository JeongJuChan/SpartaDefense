using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MonsterCountData
{
    public int mainStageNum;
    public int routineStageNum;
    public CoreInfoData coreInfoData;
    public int count;

    public MonsterCountData(int mainStageNum, int routineStageNum, CoreInfoData coreInfoData, int count)
    {
        this.mainStageNum = mainStageNum;
        this.routineStageNum = routineStageNum;
        this.coreInfoData = coreInfoData;
        this.count = count;
    }
}
