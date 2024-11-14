using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ParabolaProjectile : HeroProjectile, IMoveParabola, IPoolable<ParabolaProjectile>
{
    private Action<ParabolaProjectile> returnAction;

    /*public event Func<float> GetSpeed;
    public event Func<IDamagable> OnGetDamagable;
    public event Func<SlotInfo> OnGetSlotInfo;
    public event Action OnReturnToPool;*/

    public void Initialize(Action<ParabolaProjectile> returnAction)
    {
        this.returnAction = returnAction;
    }

    public void ReturnToPool()
    {
        returnAction?.Invoke(this);
    }
    public void MoveParabola(Vector2 targetingPos, Monster target)
    {
        StartCoroutine(CoMoveParabola(targetingPos, target));
    }

    private IEnumerator CoMoveParabola(Vector2 targetingPos, Monster target)
    {
        UpdateSpeed();
        Vector2 myOffsetPos = myTransform.position;

        Transform targetTransform = target.GetDamagePivot();

        float offsetDistance = targetTransform.position.x - myOffsetPos.x;
        float distance = offsetDistance;
        float ratio = 0f;

        while (ratio < 1f)
        {
            if (target.isDead)
            {
                damagable = null;
                break;  
            }

            distance -= speed * Time.deltaTime;

            targetingPos.x = (myOffsetPos.x + targetTransform.position.x) * Consts.HALF;

            ratio = 1 - (distance / offsetDistance);
            Vector2 firstPos = Vector2.Lerp(myOffsetPos, targetingPos, ratio);
            Vector2 secondPos = Vector2.Lerp(targetingPos, targetTransform.position, ratio);

            Vector2 lastPos = Vector2.Lerp(firstPos, secondPos, ratio);

            Vector3 toDirection = new Vector3(lastPos.x, lastPos.y, 0) - myTransform.position;

            Quaternion targetingRotation = Quaternion.FromToRotation(Vector3.right, toDirection);
            myTransform.SetPositionAndRotation(lastPos, targetingRotation);

            yield return null;
        }

        if (damagable != null && !target.isDead)
        {
            Vector2 effectPos = target.GetDamageTextPivot().position;
            OnAttacked?.Invoke(slotInfo, damagable, effectPos);
            Vector2 modifiedVec = new Vector2(effectPos.x, effectPos.y);
            OnEffectSpawned?.Invoke(EffectType.Hit, modifiedVec);
            damagable = null;
        }

        ReturnToPool();
    }
}
