using UnityEngine;

[CreateAssetMenu(fileName = "CastleDoorLevelData", menuName = "SO/CastleDoorLevelData")]
public class CastleDoorLevelDataSO : DataSO<CastleDoorLevelData>
{
    public CastleDoorLevelData GetData()
    {
        return data;
    }
}
