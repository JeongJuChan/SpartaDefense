using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hero : MonoBehaviour, IIndex
{
    [Header("TargetDatas")]
    protected List<Monster> targets = new List<Monster>();
    protected Func<List<Monster>> OnGetTargetFunc;

    [Header("Animation")]
    protected Animator animator;

    protected ColleagueData colleagueData;

    [Header("ShootingDatas")]
    protected Transform firePivot;
    protected int projectileCount = 1;
    protected float quaterDistanceY;
    protected Vector3 myPos;
    protected MonsterSpawnData monsterSpawnData;
    protected AttackAnimationEventHandler attackEventHandler;

    protected SpriteRenderer spriteRenderer;

    protected readonly int attackHash = AnimatorParameters.IS_ATTACK_HASH;

    [field: SerializeField] public int index { get; protected set; }

    protected Monster targetMonster;

    protected SkillEffect skillEffect;

    protected float attackSpeedRate;
    protected float offsetAttackSpeedRate = 1f;

    protected readonly int ATTACK_SPEED_RATE_HASH = AnimatorParameters.ATTACK_SPEED_RATE_HASH;

    #region UnityMethods
    protected virtual void Awake()
    {
        attackEventHandler = GetComponentInChildren<AttackAnimationEventHandler>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        skillEffect = GetComponentInChildren<SkillEffect>();
    }

    public void SetUsingSkillParticleColor()
    {
        skillEffect = GetComponentInChildren<SkillEffect>();
        skillEffect.SetColor(ResourceManager.instance.rank.GetRankBackgroundSprite(colleagueData.colleagueInfo.rank).texture.GetPixel(64, 64));
    }

    public void SetUsingSkillParticlePosY(float posY)
    {
        skillEffect.SetParticlePosY(posY);
    }

    public void PlayUsingSkillParticle()
    {
        skillEffect.Play();
    }

    protected void Start()
    {
        attackEventHandler.AddAttackAction(Shoot);
    }

    protected void OnDestroy()
    {
        attackEventHandler.RemoveAction(Shoot);
    }
    #endregion

    public void SetDirectionData(MonsterSpawnData spawnData)
    {
        monsterSpawnData = new MonsterSpawnData(spawnData.SpawnMinPosition, spawnData.SpawnMaxPosition);

        float halfPositionY = Consts.HALF * (monsterSpawnData.SpawnMinPosition.y + monsterSpawnData.SpawnMaxPosition.y);
        float halfDistanceY = monsterSpawnData.SpawnMaxPosition.y - halfPositionY;
        quaterDistanceY = Consts.HALF * halfDistanceY;
    }

    public void UpdateMyPos()
    {
        myPos = transform.position;
    }

    public void TryShoot()
    {
        bool isTargetExist = OnGetTargetFunc().Count != 0;
        animator.SetBool(attackHash, isTargetExist);
    }

    protected virtual void Shoot()
    {
        if (colleagueData.colleagueInfo.rank == Rank.None)
        {
            return;
        }

        targets = OnGetTargetFunc.Invoke();
        ShootTarget();
    }

    protected void SetTargetMonster()
    {
        if (targets.Count == 0)
        {
            return;
        }

        int index = UnityEngine.Random.Range(0, targets.Count);

        if (targets.Count > index)
        {
            targetMonster = targets[index];
        }
        else
        {
            SetTargetMonster();
        }
    }

    protected void SetValidTarget()
    {
        if (TrySetTarget())
        {

        }
        else
        {
            return;
        }
    }

    public void ResetTarget()
    {
        targetMonster = null;
    }

    protected abstract void ShootTarget();

    #region TargetingMethods
    public void SetGetTargetFunc(Func<List<Monster>> func)
    {
        OnGetTargetFunc = func;
    }

    protected bool TrySetTarget()
    {
        targets = OnGetTargetFunc?.Invoke();
        if (targets.Count == 0)
        {
            return false;
        }

        return true;
    }
    #endregion

    public ColleagueInfo GetSlotInfo()
    {
        return colleagueData.colleagueInfo;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public int GetIndex()
    {
        return colleagueData.index;
    }

    public ColleagueType GetColleagueType()
    {
        return GetColleagueInfo().colleagueType;
    }

    public ColleagueInfo GetColleagueInfo()
    {
        return colleagueData.colleagueInfo;
    }

    public void SetColleagueData(ColleagueData colleagueData)
    {
        this.colleagueData = colleagueData;
    }

    public void UpdateSlotHeroData(SlotHeroData slotHeroData)
    {
        animator.runtimeAnimatorController = slotHeroData.runtimeAnimatorController;
        spriteRenderer.sprite = slotHeroData.defaultSprite;
    }

    public void SetFirePivot(Transform pivot)
    {
        firePivot = pivot;
    }

    public void UpdateAttackSpeedRate(float attributeAttackSpeedRate)
    {
        attackSpeedRate = offsetAttackSpeedRate + offsetAttackSpeedRate * attributeAttackSpeedRate * Consts.PERCENT_MUTIPLY_VALUE;
        animator.SetFloat(ATTACK_SPEED_RATE_HASH, attackSpeedRate);
    }
}

