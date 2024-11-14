using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPooler<T> where T : MonoBehaviour, IPoolable<T>
{
    int initCount { get; }
    int maxCount { get; }

    void UpdatePoolCount(int index, int maxCountMod);
    void AddPoolInfo(int index, int poolCount);
    void UpdatePoolInfo(int index);
    void RemovePoolInfo(int index);
    T Pool(int prefabIndex, Vector3 position, Quaternion quaternion);
    T FactoryMethod(GameObject prefab);
}
