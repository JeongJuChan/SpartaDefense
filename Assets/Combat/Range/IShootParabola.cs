using UnityEngine;

public interface IShootParabola
{
    /// <summary>
    /// 포물선으로 쏨
    /// </summary>
    /// <param name="initTargetingPos">첫 타게팅 위치</param>
    /// <param name="damagable">데미지 입을 타겟</param>
    void ShootParabola(Vector2 initTargetingPos, Monster damagable);
}