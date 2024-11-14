using UnityEngine;

public interface IMoveParabola
{
    /// <summary>
    /// 포물선 방향으로 움직임
    /// </summary>
    /// <param name="targetingPos">타게팅 위치</param>
    /// <param name="target">타겟</param>
    void MoveParabola(Vector2 targetingPos, Monster target);
}