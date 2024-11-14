using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserLevelData", menuName = "SO/UserLevelData")]
public class UserLevelDataSO : DataSO<UserLevelData>
{
    public UserLevelData GetData()
    {
        return data;
    }
}
