using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BarracksLevelData", menuName = "SO/BarracksLevelData")]
public class BarracksLevelDataSO : DataSO<BarracksLevelData>
{
    public BarracksLevelData GetData()
    {
        return data;
    }
}
