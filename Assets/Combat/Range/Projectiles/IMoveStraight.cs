using UnityEngine;

public interface IMoveStraight
{
    /// <summary>
    /// 직선으로 움직이는 투사체 메서드
    /// </summary>
    /// <param name="target">타겟 트랜스폼</param>
    public void MoveStraight(Monster target);
}