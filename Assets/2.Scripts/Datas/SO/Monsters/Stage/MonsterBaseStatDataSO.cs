using Keiwando.BigInteger;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/MonsterBaseStatData", fileName = "MonsterBaseStatData")]
public class MonsterBaseStatDataSO : DataSO<MonsterUpgradableCSVData>
{
    protected MonsterUpgradableData monsterBaseStatData = default;

    public MonsterUpgradableData GetData()
    {
        if (monsterBaseStatData.damage == null)
        {
            InitData();
        }

        return monsterBaseStatData;
    }

    public override void InitData()
    {
        monsterBaseStatData = new MonsterUpgradableData(new BigInteger(data.health), new BigInteger(data.damage), data.attackSpeedRate,
            data.speed);
    }
}
