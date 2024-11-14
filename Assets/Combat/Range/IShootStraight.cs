using UnityEngine;

public interface IShootStraight
{
    /// <summary>
    /// 직선형 투사체 발사에 사용되는 인터페이스
    /// </summary>
    /// <param name="initTargetingPos">시작 타겟 방향</param>
    /// <param name="target">타겟의 트랜스폼</param>
    void ShootStraight(Vector2 initTargetingPos, Monster target);
}