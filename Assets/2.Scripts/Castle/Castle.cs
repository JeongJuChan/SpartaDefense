using Keiwando.BigInteger;
using System;
using System.Collections;
using UnityEngine;

public class Castle : MonoBehaviour, IDamagable, IHasHpUI, IDie
{
    [SerializeField] private Transform logsTransform;
    [SerializeField] private Transform wallsTransform;
    [SerializeField] private Transform damageTextPivot;

    [SerializeField] private CastleSpriteSO castleSpriteSO;

    [SerializeField] private SpriteRenderer castleRenderer;

    [SerializeField] private float deadDuration = 0.5f;

    [SerializeField] private int dangerousPercent = 50;

    private WaitForSeconds deadWaitForSeconds;

    private MonsterSpawnData montsterData;

    private SlotEquipmentStatData castleData;

    public event Action OnDamaged;
    public event Action OnDead;

    [field: Header("IHasHPUI")]
    public event Action<BigInteger, BigInteger> OnUpdateMaxHPUI;
    public event Action<BigInteger> OnUpdateCurrenHPUI;
    public event Action OnResetHPUI;

    private BattleManager battleManager;

    [SerializeField] private float healingCycleTime = 1f;

    public bool isDead;

    private SlotEquipmentForger slotEquipmentForger;

    private NewSlotUIPanel newSlotUIPanel;

    private CastleHealEvent castleHealEvent;

    private StageController stageController;

    public event Action OnStartHealEvent;
    public event Action OnStopHealEvent;

    private float healingAmount;

    public Action<float> OnSetCastleHealingAmount;

    private BigInteger maxHealth = new BigInteger(0);

    private bool isHealingState;

    public event Action OnResetSkillCoolTime;
    public event Action<bool> OnActiveHpUI;

    private HPUIPanel hpUIPanel;

    public event Action OnChangeCastleUndamageable;
    public event Action OnChangeMonsterUndamageable;

    private BlinkCalculator<Castle> blinkCalculator;

    [SerializeField] protected float blinkDuration = 0.25f;

    [SerializeField] private float wallDistanceMod = 0.15f;

    [SerializeField] private int vibrateDuration = 500;

#if UNITY_EDITOR
    private bool isInvincible;
#endif

    private bool isCastleDangerous;

    public void Init()
    {
        battleManager = FindAnyObjectByType<BattleManager>();
        /*battleManager.heroesStatDataHandler.OnAddCastleStat += OnCastleStatAdded;
        battleManager.heroesStatDataHandler.OnSubtractCastleStat += OnCastleStatSubtracted;*/
        battleManager.OnGetCaslteData += GetCastleData;

        StatDataHandler.Instance.OnUpdateCastleStatData += UpdateCastleStat;

        castleData = new SlotEquipmentStatData(0, 0, 0, new SlotEquipmentStatDataSave());

        InitSettings();

        slotEquipmentForger = FindObjectOfType<SlotEquipmentForger>();

        //newSlotUIPanel = FindAnyObjectByType<NewSlotUIPanel>(FindObjectsInactive.Include);
        //newSlotUIPanel.OnUpdateRtanAttributes += OnUpdateRtanHealingAttribute;

        StatDataHandler.Instance.AddAttributeEvent(AttributeType.Heal, UpdateHealingAmount);

        castleHealEvent = new CastleHealEvent(this, healingCycleTime);
        castleHealEvent.OnHealCastle += OnHealed;

        hpUIPanel = GetComponentInChildren<HPUIPanel>();
        hpUIPanel.init(this);

        blinkCalculator = new BlinkCalculator<Castle>(this, castleRenderer, blinkDuration);

        //OnDamaged += TryVibrate;

        deadWaitForSeconds = CoroutineUtility.GetWaitForSeconds(deadDuration);
    }

    #region UnityMethods
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TakeDamage(500);
        }
    }
#endif
    #endregion

    #region IDamagable
    public void TakeDamage(BigInteger damage)
    {
        BigInteger health = castleData.health - damage;
        health = health < 0 ? 0 : health;
        castleData.health = health;

        OnDamaged?.Invoke();
        OnUpdateCurrenHPUI?.Invoke(health);

        isCastleDangerous = (castleData.health * Consts.PERCENT_DIVIDE_VALUE / maxHealth) < dangerousPercent;

        if (health <= 0)
        {
            Die(true);
        }
    }
    #endregion

    #region IDie
    public void Die(bool isCausedByBattle)
    {
        if (isCausedByBattle)
        {
            isDead = true;
            OnChangeMonsterUndamageable?.Invoke();
            OnChangeCastleUndamageable?.Invoke();
            OnDead?.Invoke();
        }

        OnResetSkillCoolTime?.Invoke();
        StartCoroutine(WaitForDead());
    }

    private IEnumerator WaitForDead()
    {
        yield return deadWaitForSeconds;
        SetCastleBaseData();
    }
    #endregion

    #region InitSettings
    public void SetMonsterData(MonsterSpawnData spawnData)
    {
        montsterData = spawnData;
    }

    public void UpdateCurrentMaxHealth()
    {
        castleData.health = maxHealth;
        OnUpdateMaxHPUI?.Invoke(maxHealth, castleData.health);
    }

    private void SetCastleBaseData()
    {
        isDead = false;
        castleData.health = maxHealth;
        isCastleDangerous = false;
        OnResetHPUI?.Invoke();
    }

    private void InitSettings()
    {
        SetCastlePosition();
        SetObstclePosition();
    }

    private void SetObstclePosition()
    {
        Sprite sprite = castleRenderer.sprite;
        float unitDistance = (sprite.texture.width - sprite.border.z) / Consts.DEFAULT_PIXEL_PER_UNIT;
        Vector2 castlePos = transform.position;

        float halfUnitDistance = unitDistance * Consts.HALF;
        float offsetDistance = castlePos.x + unitDistance;

        /*Vector2 wallsPosition = wallsTransform.position;
        wallsPosition.x = offsetDistance + halfUnitDistance - wallDistanceMod;
        wallsTransform.position = wallsPosition;*/

        Vector2 logsPosition = logsTransform.position;
        logsPosition.x = offsetDistance + halfUnitDistance * Consts.HALF - wallDistanceMod;
        logsTransform.position = logsPosition;
    }

    private void SetCastlePosition()
    {
        Vector3 castlePosition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        castlePosition = new Vector3(castlePosition.x, montsterData.SpawnMinPosition.y, 0);
        castleRenderer.transform.position = castlePosition;
    }

    #endregion

    private SlotEquipmentStatData GetCastleData()
    {
        return castleData;
    }

    private void UpdateCastleStat(SlotEquipmentStatData slotEquipmentStatData)
    {
        castleData.health += slotEquipmentStatData.health - maxHealth;
        castleData.defense += slotEquipmentStatData.defense - castleData.defense;

        maxHealth = slotEquipmentStatData.health;
        OnUpdateMaxHPUI.Invoke(maxHealth, castleData.health);
    }

    private void OnCastleStatAdded(SlotEquipmentStatData equipmentCastleData)
    {
        castleData.defense += equipmentCastleData.defense;
        castleData.health += equipmentCastleData.health;

        BigInteger preMaxHealth = maxHealth;
        maxHealth = preMaxHealth + equipmentCastleData.health;

        OnUpdateMaxHPUI.Invoke(maxHealth, castleData.health);
    }

    private void OnCastleStatSubtracted(SlotEquipmentStatData equipmentCastleData)
    {
        castleData.defense -= equipmentCastleData.defense;
        castleData.health -= equipmentCastleData.health;

        BigInteger preMaxHealth = maxHealth;
        maxHealth = preMaxHealth - equipmentCastleData.health;

        OnUpdateMaxHPUI.Invoke(maxHealth, castleData.health);
    }

    private void UpdateHealingAmount(float healingAmount)
    {
        this.healingAmount = healingAmount;

        if (isHealingState && healingAmount <= 0f)
        {
            isHealingState = false;
            OnStopHealEvent?.Invoke();
        }
        else if (!isHealingState && healingAmount > 0f)
        {
            isHealingState = true;
            OnStartHealEvent();
        }

        OnSetCastleHealingAmount?.Invoke(healingAmount);
    }

    private void OnHealed(float healAmount)
    {
        castleData.health += (int)(maxHealth.ToFloat() * (healAmount * Consts.PERCENT_MUTIPLY_VALUE));
        castleData.health = castleData.health > maxHealth ? maxHealth : castleData.health;
        OnUpdateCurrenHPUI?.Invoke(castleData.health);
    }

    public Transform GetDamageTextPivot()
    {
        return damageTextPivot;
    }

    public void UpdateSprite(Sprite sprite)
    {
        castleRenderer.sprite = sprite;
        InitSettings();
    }

    public bool GetIsCastleDangerous()
    {
        return isCastleDangerous;
    }

    private void TryVibrate()
    {
        if (isCastleDangerous)
        {
            Vibration.Vibrate(vibrateDuration);
        }
    }
}
