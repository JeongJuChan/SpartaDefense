using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaShooter : Hero, IShootParabola
{
    private Func<ColleagueInfo, Vector3, Quaternion, ParabolaProjectile> OnSpawnProjectile;

    public void ShootParabola(Vector2 targetingPos, Monster target)
    {
        ParabolaProjectile projectile = GetProjectile(targetingPos);
        projectile.SetTarget(target);
        projectile.MoveParabola(targetingPos, target);
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
                    Shoot(monster);
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
                    Shoot(monster);
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

        Shoot(targetMonster);
    }

    protected void Shoot(Monster monster)
    {
        Vector3 targetPos;
        if (monster == null)
        {
            return;
        }    
        Transform target = monster.transform;
        targetPos = target.position;

        Vector2 targetPoint;
        float halfPosX = Consts.HALF * (myPos.x + target.position.x);
        float abtractHeight = myPos.y - target.position.y;

        if (abtractHeight >= 0)
        {
            targetPoint = new Vector2(halfPosX, myPos.y + quaterDistanceY);
        }
        else
        {
            float distanceY = abtractHeight >= 0 ? abtractHeight : -abtractHeight;
            distanceY = distanceY < quaterDistanceY ? quaterDistanceY : distanceY;
            targetPoint = new Vector2(halfPosX, targetPos.y + distanceY);
        }

        ShootParabola(targetPoint, monster);
    }

    public void SetSpawnAction(Func<ColleagueInfo, Vector3, Quaternion, ParabolaProjectile> spawnProjectileAction)
    {
        OnSpawnProjectile = spawnProjectileAction;
    }
    
    #region ProjectileMethods
    protected ParabolaProjectile GetProjectile(Vector3 initTargetingPos)
    {
        Quaternion initRotation = Quaternion.FromToRotation(Vector3.right, initTargetingPos - myPos);

        ParabolaProjectile projectile = OnSpawnProjectile?.Invoke(colleagueData.colleagueInfo, firePivot.position, initRotation);
        return projectile;
    }
    #endregion
}
