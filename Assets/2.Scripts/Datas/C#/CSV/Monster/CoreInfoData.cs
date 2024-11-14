using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CoreInfoData
{
    public int index;
    public string name;

    public CoreInfoData(int index, string name)
    {
        this.index = index;
        this.name = name;
    }
}
