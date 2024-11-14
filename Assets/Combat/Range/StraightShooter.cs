using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightShooter : Hero, IShootStraight
{
    private Func<ColleagueInfo, Vector3, Quaternion, StraightProjectile> OnSpawnProjectile;

    public void ShootStraight(Vector2 initTargetingPos, Monster target)
    {
        StraightProjectile projectile = GetProjectile(initTargetingPos);
        projectile.SetTarget(target);
        if (target == null)
        {
            projectile.ReturnToPool();
            return;
        }
        projectile.MoveStraight(target);
    }

    protected override void ShootTarget()
    {
        if (projectileCount > 1)
        {
            if (targets.Count > projectileCount)
            {
                ShootForSingleTarget();

                List<Monster> newTargets = new List<Monster>(targets);
                for (int i = 1; i < projectileCount; i++)
                {
                    int index = UnityEngine.Random.Range(0, newTargets.Count);
                    Monster monster = newTargets[index];
                    ShootStraight(myPos, monster);
                    newTargets.Remove(monster);
                }
            }
            else
            {
                if (targets.Count <= 0)
                {
                    return;
                }

                ShootForSingleTarget();

                for (int i = 1; i < projectileCount && i < targets.Count; i++)
                {
                    Monster monster = targets[i];
                    ShootStraight(myPos, monster);
                }
            }
        }
        else
        {
            ShootForSingleTarget();
        }
    }

    private void ShootForSingleTarget()
    {
        if (targetMonster == null || targetMonster.isDead)
        {
            SetTargetMonster();
        }

        ShootStraight(myPos, targetMonster);
    }

    #region ProjectileMethods
    protected StraightProjectile GetProjectile(Vector3 initTargetingPos)
    {
        Quaternion initRotation = Quaternion.FromToRotation(Vector3.right, initTargetingPos - myPos);

        StraightProjectile projectile = OnSpawnProjectile?.Invoke(colleagueData.colleagueInfo, firePivot.position, initRotation);
        return projectile;
    }

    public void SetSpawnAction(Func<ColleagueInfo, Vector3, Quaternion, StraightProjectile> spawnProjectileAction)
    {
        OnSpawnProjectile = spawnProjectileAction;
    }
    #endregion
}
