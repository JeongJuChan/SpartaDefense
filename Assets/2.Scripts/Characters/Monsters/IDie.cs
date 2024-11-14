using System;

public interface IDie
{
    event Action OnDead;
    void Die(bool isCausedByBattle);
}