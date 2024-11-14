using System.Collections.Generic;
using UnityEngine;

public abstract class UIElementPooler<T> : MonoBehaviour where T : IUIElement
{
    protected Queue<T> inactivePool;
    protected Queue<T> activePool;

    protected abstract void InitPool(T[] uiElements);

    protected virtual void DeActivateActiveElements(int elementCount)
    {
        int destroyCount = activePool.Count - elementCount;
        for (int i = 0; i < destroyCount; i++)
        {
            T element = activePool.Dequeue();
            element.SetActive(false);
            inactivePool.Enqueue(element);
        }
    }
}