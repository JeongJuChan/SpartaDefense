using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/MonsterIncrementData", fileName = "MonsterIncrementData")]
public class MonsterIncrementDataSO : DataSO<MonsterIncrementData>
{
    public MonsterIncrementData GetData()
    {
        return data;
    }
}
