using Keiwando.BigInteger;
using System.Collections;
using UnityEngine;

public class ColleagueEnforceStone : Monster
{
    [SerializeField] private GameObject shadowObject;
    private BigInteger maxHp;

    private RewardMovingController rewardMovingController;

    private float colliderWidth;
    private Vector2 offsetPos;

    private const int UNIT_DECREMENT = 10;

    private BigInteger preHp;

    private readonly int IS_STUNNED_HASH = AnimatorParameters.IS_STUNNED_HASH;


    private WaitForSeconds appearWaitForSeconds;

    protected override void Awake()
    {
        base.Awake();
        rewardMovingController = FindAnyObjectByType<RewardMovingController>();
        isMove = false;
        boxCollider.enabled = false;
        shadowObject.SetActive(false);
        colliderWidth = GetColliderWidth();
        offsetPos = new Vector2(transform.position.x - colliderWidth  * Consts.HALF, transform.position.y);
        appearWaitForSeconds = CoroutineUtility.GetWaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
    }

    protected override void OnEnable()
    {
        InvokeDefaultEnableEvent();
        maxHp = monsterData.monsterUpgradableData.health;
        preHp = maxHp;
        transform.position = offsetPos;

        StartCoroutine(CoWaitForAppearing());
    }

    private IEnumerator CoWaitForAppearing()
    {
        yield return appearWaitForSeconds;
        boxCollider.enabled = true;
        shadowObject.SetActive(true);
        isInvincible = true;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        animator.SetBool(IS_STUNNED_HASH, true);
    }

    protected override void OnDisable()
    {
        boxCollider.enabled = false;
        shadowObject.SetActive(false);
        PlayDyingAnimation(false);
    }

    protected override void PlayDyingAnimation(bool isPlaying)
    {
        if (isPlaying)
        {
            animator.SetBool(IS_STUNNED_HASH, false);
        }
        base.PlayDyingAnimation(isPlaying);
    }

    public override void TakeDamage(BigInteger damage)
    {
        if (!isDead && !isInvincible)
        {
            BigInteger currentHp = monsterData.monsterUpgradableData.health - damage;
            currentHp = currentHp < 0 ? 0 : currentHp;

            if (preHp != maxHp)
            {
                BigInteger preUnit = preHp * Consts.PERCENT_DIVIDE_VALUE / (maxHp * UNIT_DECREMENT);
                BigInteger currenUnit = currentHp * Consts.PERCENT_DIVIDE_VALUE / (maxHp * UNIT_DECREMENT);

                if (currenUnit < preUnit)
                {
                    int count = int.Parse((preUnit - currenUnit).ToString());
                    rewardMovingController.RequestMovingCurrency(count, RewardType.ColleagueLevelUpStone, skillPivot.position);
                }
            }

            if (currentHp == 0)
            {
                rewardMovingController.RequestMovingCurrency(1, RewardType.ColleagueLevelUpStone, skillPivot.position);
            }

            preHp = currentHp;
        }

        base.TakeDamage(damage);
    }
}
