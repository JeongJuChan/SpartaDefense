using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class HeroProjectile : MonoBehaviour, IIndex
{
    [Header("MoveFields")]
    [SerializeField] protected float arriveTime = 1f;
    private float maxRangeX;
    private float totalMapDistanceX;
    protected float speed;
    protected Transform myTransform;
    public Action<EffectType, Vector2> OnEffectSpawned;
    public Action<ColleagueInfo, IDamagable, Vector2> OnAttacked;

    [field: SerializeField] public int index { get; private set; }
    [field: SerializeField] public int heroIndex { get; private set; }

    [Header("Physics2D")]
    private Rigidbody2D rigid;
    private BoxCollider2D boxCollider2D;

    protected IDamagable damagable;

    protected ColleagueInfo slotInfo;

    #region UnityMethods
    protected void Awake()
    {
        myTransform = GetComponent<Transform>();
        InitPhysicsSetting();
    }
    #endregion

    #region MoveSettingMethods

    public void Init(float maxRangeX)
    {
        this.maxRangeX = maxRangeX;
    }
    public void ChangeArriveTime(float arriveTime)
    {
        this.arriveTime = arriveTime;
        UpdateSpeed();
    }

    protected void UpdateSpeed()
    {
        totalMapDistanceX = maxRangeX - myTransform.position.x;
        speed = totalMapDistanceX / arriveTime;
    }
    
    #endregion

    #region PhysicsSettingsMethods
    private void InitPhysicsSetting()
    {
        rigid = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        rigid.bodyType = RigidbodyType2D.Kinematic;
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        boxCollider2D.isTrigger = true;
    }
    #endregion

    public void SetTarget(IDamagable target)
    {
        this.damagable = target;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void SetSlotInfo(ColleagueInfo slotInfo)
    {
        this.slotInfo = slotInfo;
    }

    public ColleagueInfo GetColleagueInfo()
    {
        return slotInfo;
    }
}
