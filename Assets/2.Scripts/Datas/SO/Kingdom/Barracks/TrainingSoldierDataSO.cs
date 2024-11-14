using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainingSoldierData", menuName = "SO/TrainingSoldierData")]
public class TrainingSoldierDataSO : DataSO<TrainingSoldierData>
{
    public TrainingSoldierData GetData()
    {
        return data;
    }
}
