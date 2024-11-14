using Keiwando.BigInteger;
using System;
using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour, IPoolable<Monster>, IDamagable, IDie, IIndex, IHasHpUI, IAlive
{
    public event Action<Monster> OnRemoveTargetAction;

    public bool IsTargetted { get; private set; }
    [field: SerializeField] public int index { get; private set; }

    private Action<Monster> OnReturnAction;

    [SerializeField] protected MonsterData monsterData;

    [SerializeField] protected Transform skillPivot;
    [SerializeField] protected Transform damageTextPivot;

    public event Action OnDamaged;
    public event Action OnDead;
    public event Action<Vector2> OnDeadCallback;
    public event Action<Monster> OnResetData;
    public bool isDead { get; private set; } = false;

    public event Action<int> OnAttacked;

    [field: Header("IHasUI")]
    public event Action<BigInteger, BigInteger> OnUpdateMaxHPUI;
    public event Action<BigInteger> OnUpdateCurrenHPUI;
    public event Action OnResetHPUI;
    public event Action<bool> OnActiveHpUI;
    public event Action OnAlive;

    [Header("Blink")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected float blinkDuration = 0.25f;
    protected BlinkCalculator<Monster> blinkCalculator;

    [SerializeField] protected float totalSpeed;
    protected float offsetSpeed;

    [SerializeField] protected Animator animator;

    protected bool isMove = true;

    protected AttackAnimationEventHandler attackEventHandler;

    protected readonly int ATTACK_HASH = AnimatorParameters.IS_ATTACK_HASH;
    protected readonly int DEAD_HASH = AnimatorParameters.IS_DEAD_HASH;
    protected readonly int ATTACK_SPEED_RATE_HASH = AnimatorParameters.ATTACK_SPEED_RATE_HASH;

    protected WaitForSeconds deadWaitForSeconds;

    protected BoxCollider2D boxCollider;

    [SerializeField] protected bool isInvincible = false;

    private HPUIPanel hpUIPanel;

    private Coroutine preCoroutine;

    #region UnityMethods
    protected virtual void Awake()
    {
        blinkCalculator = new BlinkCalculator<Monster>(this, spriteRenderer, blinkDuration);
        OnAlive += SetDeadFalse;
        boxCollider = GetComponent<BoxCollider2D>();
        hpUIPanel = GetComponentInChildren<HPUIPanel>();
        hpUIPanel.init(this);
        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
    }

    protected virtual void OnEnable()
    {
        isMove = true;
        isInvincible = true;
        InvokeDefaultEnableEvent();

        if (attackEventHandler == null)
        {
            attackEventHandler = GetComponentInChildren<AttackAnimationEventHandler>();
        }

        attackEventHandler.AddAttackAction(OnAttack);
    }

    protected void InvokeDefaultEnableEvent()
    {
        OnResetData?.Invoke(this);
        OnAlive?.Invoke();
        OnActiveHpUI?.Invoke(false);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Consts.WALL_TAG)
        {
            isMove = false;
            PlayAttackAnimation(true);
        }
    }


    protected void Update()
    {
        if (isMove)
        {
            Move();
        }
    }

    private Plane[] planes;
    protected bool CheckBound()
    {
        var point = transform.position;

        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
            {
                return false;
            }
        }
        return true;
    }

    protected virtual void OnDisable()
    {
        PlayDyingAnimation(false);
        attackEventHandler.RemoveAction(OnAttack);
    }

    protected void OnDestroy()
    {
        OnAlive -= SetDeadFalse;
    }
    #endregion

    #region IDamagable
    public virtual void TakeDamage(BigInteger damage)
    {
        if (isInvincible)
        {
            return;
        }

        if (isDead)
        {
            return;
        }

        OnActiveHpUI?.Invoke(true);

        BigInteger health = monsterData.monsterUpgradableData.health - damage;
        monsterData.monsterUpgradableData.health = health < 0 ? 0 : health;

        OnUpdateCurrenHPUI?.Invoke(monsterData.monsterUpgradableData.health);

        if (monsterData.monsterUpgradableData.health != 0)
        {
            OnDamaged?.Invoke();
        }
        else
        {
            Die(true);
        }
    }
    #endregion

    #region IDie
    public void Die(bool isCausedByBattle)
    {
        isDead = true;

        if (isCausedByBattle)
        {
            preCoroutine = StartCoroutine(CoOnDie());

            QuestManager.instance.UpdateCount(QuestType.MonsterKillCount, 1);
            DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.Kill_Monster, 1);
        }
        else
        {
            SetDeadSettings();
            ReturnToPool();
        }
    }

    private void SetDeadSettings()
    {
        isMove = false;
        IsTargetted = false;
        OnRemoveTargetAction?.Invoke(this);
    }
    #endregion

    #region IIndex
    public void SetIndex(int index)
    {
        this.index = index;
    }
    #endregion

    #region IPoolable
    public void Initialize(Action<Monster> returnAction)
    {
        OnReturnAction = returnAction;
    }

    public void ReturnToPool()
    {
        OnResetHPUI?.Invoke();
        OnReturnAction?.Invoke(this);
    }
    #endregion

    #region HeroTargetMethods
    public void SetIsTargettedTrue()
    {
        IsTargetted = true;
    }
    #endregion

    #region InitMethods
    public void SetMonsterBaseData(MonsterData monsterData)
    {
        this.monsterData = monsterData;
        UpdateAttackSpeedRate(monsterData.monsterUpgradableData.attackSpeedRate);
        totalSpeed = offsetSpeed * monsterData.monsterUpgradableData.speed;

        OnUpdateMaxHPUI?.Invoke(monsterData.monsterUpgradableData.health, monsterData.monsterUpgradableData.health);
    }


    public void InitSpeed(float monsterSpawnPosX, float obstaclePosX, float arriveDuration)
    {
        float spawnPosX = monsterSpawnPosX;
        offsetSpeed = (monsterSpawnPosX - obstaclePosX) / arriveDuration;
    }
    #endregion
    #region MoveMethods
    private void Move()
    {
        Vector2 position = transform.position;
        position.x -= totalSpeed * Time.deltaTime;
        transform.position = position;
    }
    #endregion

    #region AttackMethods
    protected void OnAttack()
    {
        OnAttacked?.Invoke(index);
    }

    protected virtual void PlayAttackAnimation(bool isPlaying)
    {
        animator.SetBool(ATTACK_HASH, isPlaying);
    }
    #endregion

    #region OnDieMethods
    private IEnumerator CoOnDie()
    {
        if (deadWaitForSeconds == null)
        {
            deadWaitForSeconds = CoroutineUtility.GetWaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }

        PlayDyingAnimation(true);
        PlayAttackAnimation(false);

        OnActiveHpUI?.Invoke(false);
        SetDeadSettings();
        OnDeadCallback?.Invoke(transform.position);

        yield return deadWaitForSeconds;

        ReturnToPool();
    }

    protected virtual void PlayDyingAnimation(bool isPlaying)
    {
        animator.SetBool(DEAD_HASH, isPlaying);
    }
    #endregion

    public Transform GetSkillPivot()
    {
        return skillPivot;
    }

    public Transform GetDamageTextPivot()
    {
        return damageTextPivot;
    }

    public Transform GetDamagePivot()
    {
        return hpUIPanel.transform;
    }

    public void ToggleInvincible(bool isInvincible)
    {
        this.isInvincible = isInvincible;
    }

    public float GetSpeed()
    {
        return totalSpeed;
    }

    public float GetColliderWidth()
    {
        return (boxCollider.size.x + boxCollider.offset.x) * Consts.HALF;
    }

    private void UpdateAttackSpeedRate(float attackSpeedRate)
    {
        animator.SetFloat(ATTACK_SPEED_RATE_HASH, attackSpeedRate);
    }

    private void SetDeadFalse()
    {
        isDead = false;
    }
}
