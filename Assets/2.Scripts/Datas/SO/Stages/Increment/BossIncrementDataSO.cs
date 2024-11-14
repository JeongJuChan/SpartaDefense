using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/BossIncrementData", fileName = "BossIncrementData")]
public class BossIncrementDataSO : DataSO<BossIncrementData>
{
    public BossIncrementData GetData()
    {
        return data;
    }
}
