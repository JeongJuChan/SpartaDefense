using UnityEngine;

public interface IGetResource<T>
{
    public T GetResource(int index);
}