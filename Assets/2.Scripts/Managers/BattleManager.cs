using Keiwando.BigInteger;
using System;
using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private SlotStatDataSO slotStatDataSO;
    [SerializeField] private MonsterBaseStatDataSO monsterBaseDataSO;
    [SerializeField] private AttributeDataSO attributeDataSO;
    [SerializeField] private ColleagueDataSO colleagueDataSO;
    [SerializeField] private SkillDataSO skillDataSO;

    public event Func<SlotEquipmentStatData> OnGetCaslteData;

    private Castle castle;

    public event Func<ColleagueType, BigInteger> OnGetDamage;

    public event Func<int, MonsterData> OnGetMonsterData;

    private const int DIVIDE_VALUE = 2;

    private const float PERCENT = Consts.PERCENT_DIVIDE_VALUE;

    private const int DEFAULT_CRITICAL_MOD = 2;

    public event Action<BigInteger, DamageType, int, Vector2> OnSpawnDamageUI;

    private float vibrateDuration = 0.1f;
    private bool isVibratingProgress;

    private bool isCastleDamageable = true;
    private bool isMonsterDamageable = true;

    private StageController stageController;
    private DungeonController dungeonController;

    private StatDataHandler statDataHandler;

    private float criticalProbability;
    private float criticalMultiplication;

    public void Init()
    {
        castle = FindAnyObjectByType<Castle>();
        statDataHandler = StatDataHandler.Instance;
        castle.OnChangeMonsterUndamageable += () => ChangeMonsterDamageable(false);
        castle.OnChangeCastleUndamageable += () => ChangeCastleDamageable(false);
        stageController = FindAnyObjectByType<StageController>();
        stageController.OnChangeMonsterDamageableState += ChangeMonsterDamageable;
        stageController.OnChangeCastleDamageableState += ChangeCastleDamageable;
        dungeonController = FindAnyObjectByType<DungeonController>();
        dungeonController.OnChangeCastleDamageableState += ChangeCastleDamageable;
        statDataHandler.AddAttributeEvent(AttributeType.CriticalProbability, UpdateCriticalProbabilityValue);
        statDataHandler.AddAttributeEvent(AttributeType.CriticalMultiplication, UpdateCriticalMultiplicationValue);
    }

    private void UpdateCriticalProbabilityValue(float criticalProbability)
    {
        this.criticalProbability = criticalProbability;
    }

    private void UpdateCriticalMultiplicationValue(float criticalMultiplication)
    {
        this.criticalMultiplication = criticalMultiplication;
    }

    public void OnMonsterAttacked(ColleagueInfo slotInfo, IDamagable monster, Vector2 pos)
    {
        if (!isMonsterDamageable)
        {
            return;
        }

        BigInteger damage = statDataHandler.GetDamage(slotInfo.colleagueType);

        float criticalRandom = UnityEngine.Random.Range(0, PERCENT);
        bool isCriticalApplying = criticalRandom < criticalProbability;

        if (isCriticalApplying)
        {
            damage = DEFAULT_CRITICAL_MOD * damage;
            if (criticalMultiplication > Mathf.Epsilon)
            {
                damage = damage + (int)(damage.ToFloat() * criticalMultiplication) / Consts.PERCENT_DIVIDE_VALUE;
            }

            Debug.Log($"치명타 데미지 : {damage}");
        }

        if (monster != null)
        {
            DamageType damageType = isCriticalApplying ? DamageType.Critical : DamageType.Normal;
            monster.TakeDamage(damage);
            OnSpawnDamageUI?.Invoke(damage, damageType, 1, pos);
        }
    }

    public void OnMonsterAttacked(float damagePercent, IDamagable monster, Vector2 pos, bool isVibrated, BigInteger damage)
    {
        if (!isMonsterDamageable)
        {
            return;
        }

        BigInteger totalDamage = (int)(damage.ToFloat() * damagePercent);

        if (!isVibratingProgress && isVibrated)
        {
            isVibratingProgress = true;
            StartCoroutine(CoShakeCamera());
        }

        if (monster != null)
        {
            monster.TakeDamage(totalDamage);
            OnSpawnDamageUI?.Invoke(totalDamage, DamageType.Normal, 1, pos);
        }
    }

    private IEnumerator CoShakeCamera()
    {
        float elapsedTime = Time.deltaTime;

        UIAnimations.Instance.ShakeCamera();

        while (elapsedTime < vibrateDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isVibratingProgress = false;
    }

    public void OnCastleAttacked(int monsterIndex)
    {
        if (!isCastleDamageable)
        {
            return;
        }

        MonsterData monsterBaseData = OnGetMonsterData.Invoke(monsterIndex);
        BigInteger totalDamage = CalculateDamage(monsterBaseData.monsterUpgradableData.damage, OnGetCaslteData.Invoke().defense);

        if (totalDamage <= 0)
        {
            return;
        }

        castle.TakeDamage(totalDamage);
        OnSpawnDamageUI?.Invoke(totalDamage, DamageType.Castle, 1, castle.GetDamageTextPivot().position);
    }

    private BigInteger CalculateDamage(BigInteger damage, BigInteger defense)
    {
        return damage - (defense / DIVIDE_VALUE);
    }

    private void ChangeMonsterDamageable(bool isDamageable)
    {
        isMonsterDamageable = isDamageable;
    }

    private void ChangeCastleDamageable(bool isDamageable)
    {
        isCastleDamageable = isDamageable;
    }

}
