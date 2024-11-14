using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterObjectPooler : IPooler<Monster>
{
    private Dictionary<int, ObjectPool<Monster>> poolDict = new Dictionary<int, ObjectPool<Monster>>();

    private Transform parent;

    private Action<Vector2> OnDead;

    private Func<int, MonsterData> OnGetMonsterDataFunc;

    private Action<int> OnCastleAttacked;

    public int initCount { get; private set; }

    public int maxCount { get; private set; }

    private float monsterSpawnPosX;
    private float obstaclePosX;

    private float arriveDuration;

    public MonsterObjectPooler(float obstaclePosX, float monsterSpawnPosX, float arriveDuration, Transform parent, (int, int) poolCountMod,
        Func<int, MonsterData> OnGetMonsterDataFunc, Action<Vector2> OnDead, Action<int> OnCastleAttacked)
    {
        this.obstaclePosX = obstaclePosX;
        this.monsterSpawnPosX = monsterSpawnPosX;
        this.arriveDuration = arriveDuration;
        this.parent = parent;
        this.OnGetMonsterDataFunc += OnGetMonsterDataFunc;
        this.OnDead = OnDead;
        this.OnCastleAttacked = OnCastleAttacked;
        initCount = poolCountMod.Item1;
        maxCount = poolCountMod.Item2;
    }

    #region IPoolerInterfaceMethods
    public Monster Pool(int prefabIndex, Vector3 position, Quaternion quaternion)
    {
        Monster monster = poolDict[prefabIndex].Pool(position, quaternion);
        monster.gameObject.SetActive(true);
        return monster;
    }

    public void AddPoolInfo(int index, int poolCount)
    {
        if (!poolDict.ContainsKey(index))
        {
            GameObject go = ResourceManager.instance.monster.GetResource(index);
            poolDict.Add(index, new ObjectPool<Monster>(FactoryMethod, go, initCount * poolCount, maxCount * poolCount, parent));
        }
    }

    public void RemovePoolInfo(int index)
    {
        if (poolDict.ContainsKey(index))
        {
            poolDict.Remove(index);
        }
    }

    public Monster FactoryMethod(GameObject go)
    {
        Monster monster = UnityEngine.Object.Instantiate(go).GetComponent<Monster>();
        monster.name = go.name;
        monster.OnResetData += ResetMonsterBaseData;
        monster.OnDeadCallback += OnDead;
        monster.OnAttacked += OnCastleAttacked;
        monster.InitSpeed(monsterSpawnPosX, obstaclePosX, arriveDuration);
        ResetMonsterBaseData(monster);
        return monster;
    }

    #endregion
    private void ResetMonsterBaseData(Monster monster)
    {
        MonsterData monsterBaseData = OnGetMonsterDataFunc.Invoke(monster.index);
        monster.SetMonsterBaseData(monsterBaseData);
    }

    public void UpdatePoolCount(int index, int maxCountMod)
    {
        poolDict[index].UpdateMaxCount(maxCountMod);
    }

    public void UpdatePoolInfo(int index)
    {
        throw new NotImplementedException();
    }
}
