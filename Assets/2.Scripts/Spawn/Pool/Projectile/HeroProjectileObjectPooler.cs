using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroProjectileObjectPooler<T> : IPooler<T> where T : HeroProjectile, IPoolable<T>
{
    protected Dictionary<int, ObjectPool<T>> poolDict = new Dictionary<int, ObjectPool<T>>();

    public int initCount { get; protected set; }
    public int maxCount { get; protected set; }

    private Transform parent;

    private Action<EffectType, Vector2> OnSpawnEffect;

    private float maxRangeX;
    //private float remainTime;

    private Vector2 effectAdjustVec;

    private Action<ColleagueInfo, IDamagable, Vector2> OnMonsterAttacked;

    private ColleagueInfo slotInfo;

    private Func<int, ColleagueData> OnGetSlotDataByIndex;

    private Dictionary<ColleagueInfo, ObjectPool<T>> projectilePoolDict = new Dictionary<ColleagueInfo, ObjectPool<T>>();

    public HeroProjectileObjectPooler(float maxRangeX, int initCount, int maxCount, Transform parent, Action<EffectType, Vector2> OnSpawnEffect, 
        Vector2 effectAdjustVec, Action<ColleagueInfo, IDamagable, Vector2> OnMonsterAttacked, Func<int, ColleagueData> OnGetSlotDataByIndex)
    {
        this.parent = parent;
        //this.remainTime = remainTime;
        this.initCount = initCount;
        this.maxCount = maxCount;
        this.OnSpawnEffect = OnSpawnEffect;
        this.maxRangeX = maxRangeX;
        this.effectAdjustVec = effectAdjustVec;
        this.OnMonsterAttacked = OnMonsterAttacked;
        this.OnGetSlotDataByIndex = OnGetSlotDataByIndex;
    }

    #region IPoolerInterfaceMethods
    public T Pool(int prefabIndex, Vector3 position, Quaternion quaternion)
    {
        return poolDict[prefabIndex].Pool(position, quaternion);
    }

    public void AddPoolInfo(int index, int poolCount)
    {
        if (!poolDict.ContainsKey(index))
        {
            ColleagueInfo colleagueInfo = OnGetSlotDataByIndex.Invoke(index).colleagueInfo;
            GameObject go = ResourceManager.instance.heroProjectile.GetResource(colleagueInfo);
            ColleagueData slotData = OnGetSlotDataByIndex.Invoke(index);
            slotInfo = slotData.colleagueInfo;
            ColleagueInfo projectileColleagueInfo = go.GetComponent<HeroProjectile>().GetColleagueInfo();
            if (!projectilePoolDict.ContainsKey(projectileColleagueInfo))
            {
                poolDict.Add(index, new ObjectPool<T>(FactoryMethod, go, initCount, maxCount, parent));
                projectilePoolDict.Add(projectileColleagueInfo, poolDict[index]);
            }
        }
    }

    public void UpdatePoolInfo(int index)
    {
        if (poolDict.ContainsKey(index))
        {
            ColleagueInfo colleagueInfo = OnGetSlotDataByIndex.Invoke(index).colleagueInfo;
            GameObject go = ResourceManager.instance.heroProjectile.GetResource(colleagueInfo);
            ColleagueData slotData = OnGetSlotDataByIndex.Invoke(index);
            slotInfo = slotData.colleagueInfo;
            ColleagueInfo projectileColleagueInfo = go.GetComponent<HeroProjectile>().GetColleagueInfo();
            if (!projectilePoolDict.ContainsKey(projectileColleagueInfo))
            {
                projectilePoolDict.Add(projectileColleagueInfo, new ObjectPool<T>(FactoryMethod, go, initCount, maxCount, parent));
            }

            poolDict[index] = projectilePoolDict[projectileColleagueInfo];
        }
    }

    public void RemovePoolInfo(int index)
    {
        if (poolDict.ContainsKey(index))
        {
            poolDict.Remove(index);
        }
    }

    public T FactoryMethod(GameObject go)
    {
        T t = UnityEngine.Object.Instantiate(go).GetComponent<T>();
        t.name = go.name;
        t.Init(maxRangeX);
        t.OnEffectSpawned += OnSpawnEffect;
        t.OnAttacked += OnMonsterAttacked;
        t.SetSlotInfo(slotInfo);
        return t;
    }

    public void UpdatePoolCount(int index, int maxCountMod)
    {
        poolDict[index].UpdateMaxCount(maxCountMod);
    }
    #endregion
}
