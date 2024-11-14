using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class StraightProjectile : HeroProjectile, IMoveStraight, IPoolable<StraightProjectile>
{
    private Action<StraightProjectile> returnAction;

    /*public event Func<float> GetSpeed;
    public event Func<IDamagable> OnGetDamagable;
    public event Func<SlotInfo> OnGetSlotInfo;
    public event Action OnReturnToPool;*/

    //[SerializeField] private float rotationMod = 30f;

    [SerializeField] private float maxDuration = 2f;

    public void Initialize(Action<StraightProjectile> returnAction)
    {
        this.returnAction = returnAction;
    }

    public void ReturnToPool()
    {
        returnAction?.Invoke(this);
    }

    public void MoveStraight(Monster target)
    {
        StartCoroutine(CoMoveStraight(target));
    }

    private IEnumerator CoMoveStraight(Monster target)
    {
        UpdateSpeed();

        Transform targetTransform = target.GetDamagePivot();

        float elapsedTime = Time.deltaTime;
        Vector3 myOffsetPos = myTransform.position;

        Vector3 direction = (targetTransform.position - myTransform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 45f;

        float angledifference = Vector2.SignedAngle(transform.right, direction);

        Quaternion targetingRoation = Quaternion.FromToRotation(transform.right, direction);
        bool isReached = myTransform.position.x > targetTransform.position.x;

        while (!isReached)
        {
            if (target.isDead || elapsedTime >= maxDuration)
            {
                damagable = null;
                ReturnToPool();
                yield break;
            }

            direction = (targetTransform.position - myTransform.position).normalized;
            Vector2 newPos = new Vector2(myTransform.position.x, myTransform.position.y) + new Vector2(direction.x, direction.y) * speed * Time.deltaTime;
            myTransform.position = newPos;

            elapsedTime += Time.deltaTime;
            myTransform.SetPositionAndRotation(newPos, Quaternion.Euler(0, 0, angledifference - 45f));

            isReached = myTransform.position.x >= targetTransform.position.x;

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
