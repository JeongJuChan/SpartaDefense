using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroProjectileSpawner : MonoBehaviour
{
    private HeroProjectileObjectPooler<ParabolaProjectile> parabolaProjectilePooler;
    private HeroProjectileObjectPooler<StraightProjectile> straightProjectilePooler;

    [SerializeField] private SlotStatDataSO slotStatDataSO;
    [SerializeField] private ColleagueDataSO slotDataSO;

    private float maxRangeX;

    private CastleClan castleClan;

    [Header("Effect")]
    //[SerializeField] private float remainTime = 0.5f;
    private Vector2 effectAdjustVec;
    [SerializeField] private EffectSpawner effectSpawner;

    private BattleManager battleManager;

    [SerializeField] private int initCountMod = 5;
    [SerializeField] private int maxCountMod = 10;

    private NewSlotUIPanel newSlotUIPanel;

    private event Action<ColleagueInfo> OnChangeHeroProjectile;

    private HashSet<int> projectilePoolInstantiatedSet = new HashSet<int>();

    #region UnityMethods
    private void Start()
    {
        SetHeroesShootAction();
    }
    #endregion

    public void Init()
    {
        battleManager = FindAnyObjectByType<BattleManager>();
        newSlotUIPanel = FindAnyObjectByType<NewSlotUIPanel>(FindObjectsInactive.Include);
        //newSlotUIPanel.OnHeroChanged += UpdateProjectilePool;
        castleClan = FindAnyObjectByType<CastleClan>();
        castleClan.OnUpdateColleague += UpdateProjectilePool;
        OnChangeHeroProjectile += ResourceManager.instance.heroProjectile.LoadSlotTypeProjectile;
        ForgeManager.instance.OnUpdateRtanArrow += UpdateRtanProjectilePool;
    }

    #region SpawnMethods

    protected ParabolaProjectile SpawnParabolaProjectile(ColleagueInfo slotInfo, Vector3 position, Quaternion quaternion)
    {
        int index = slotDataSO.GetSlotData(slotInfo).index;

        //int i = index / Consts.PERCENT_DIVIDE_VALUE;
        //index = i * Consts.PERCENT_DIVIDE_VALUE;
        /*if (slotInfo.colleagueType != ColleagueType.Rtan)
        {
        }*/

        ParabolaProjectile projectile = parabolaProjectilePooler.Pool(index, position, quaternion);
        return projectile;
    }

    protected StraightProjectile SpawnStraightProjectile(ColleagueInfo slotInfo, Vector3 position, Quaternion quaternion)
    {
        int index = slotDataSO.GetSlotData(slotInfo).index;

        //int i = index / Consts.PERCENT_DIVIDE_VALUE;
        //index = i * Consts.PERCENT_DIVIDE_VALUE;
        /*if (slotInfo.colleagueType != ColleagueType.Rtan)
        {
        }*/

        StraightProjectile projectile = straightProjectilePooler.Pool(index, position, quaternion);
        return projectile;
    }

    #endregion

    #region InitSpawnMethods
    public void SetProjectileData(float maxRangeX)
    {
        this.maxRangeX = maxRangeX;
    }

    protected void UpdateProjectilePool(ColleagueInfo slotInfo)
    {
        if (castleClan.TryGetShooter(slotInfo.colleagueType, out ParabolaShooter parabolaShooter))
        {
            if (parabolaProjectilePooler == null)
            {
                parabolaProjectilePooler = new HeroProjectileObjectPooler<ParabolaProjectile>(maxRangeX, initCountMod, maxCountMod, transform, effectSpawner.Spawn, effectAdjustVec,
                    battleManager.OnMonsterAttacked, slotDataSO.GetColleagueData);
            }

            parabolaShooter.SetSpawnAction(SpawnParabolaProjectile);
            UpdatePooler(parabolaProjectilePooler, slotInfo);
        }
        else if (castleClan.TryGetShooter(slotInfo.colleagueType, out StraightShooter striaghtShooter))
        {
            if (straightProjectilePooler == null)
            {
                straightProjectilePooler = new HeroProjectileObjectPooler<StraightProjectile>(maxRangeX, initCountMod, maxCountMod, transform, effectSpawner.Spawn, effectAdjustVec,
                        battleManager.OnMonsterAttacked, slotDataSO.GetColleagueData);
            }

            striaghtShooter.SetSpawnAction(SpawnStraightProjectile);
            UpdatePooler(straightProjectilePooler, slotInfo);
        }
    }

    private void UpdatePooler<T>(IPooler<T> projectilePooler, ColleagueInfo slotInfo) where T : HeroProjectile, IPoolable<T>
    {
        OnChangeHeroProjectile?.Invoke(slotInfo);

        int index = slotDataSO.GetSlotData(slotInfo).index;

        /*if (slotInfo.colleagueType == ColleagueType.Rtan)
        {
            projectilePooler.AddPoolInfo(index, slotDataSO.defaultPoolCount);
        }
        else*/
        {
            //int i = index / Consts.PERCENT_DIVIDE_VALUE;
            //int result = i * Consts.PERCENT_DIVIDE_VALUE;

            if (projectilePoolInstantiatedSet.Contains(index))
            {
                return;
            }
            else
            {
                projectilePoolInstantiatedSet.Add(index);
            }

            projectilePooler.AddPoolInfo(index, slotDataSO.defaultPoolCount);
        }
    }

    public void UpdateRtanProjectilePool(Rank rank)
    {
        ColleagueInfo slotInfo = new ColleagueInfo(Rank.Rare, ColleagueType.Rtan_Rare);

        if (castleClan.TryGetShooter(ColleagueType.Rtan_Rare, out ParabolaShooter parabolaShooter))
        {
            if (parabolaProjectilePooler == null)
            {
                parabolaProjectilePooler = new HeroProjectileObjectPooler<ParabolaProjectile>(maxRangeX, initCountMod, maxCountMod, transform, effectSpawner.Spawn, effectAdjustVec,
                    battleManager.OnMonsterAttacked, slotDataSO.GetColleagueData);
                parabolaShooter.SetSpawnAction(SpawnParabolaProjectile);
            }

            UpdatePooler(parabolaProjectilePooler, slotInfo, rank);
        }
    }

    private void UpdatePooler<T>(IPooler<T> projectilePooler, ColleagueInfo slotInfo, Rank rank) where T : HeroProjectile, IPoolable<T>
    {
        ResourceManager.instance.heroProjectile.LoadSlotTypeProjectile(slotInfo, rank);

        int index = slotDataSO.GetSlotData(slotInfo).index;

        if (!projectilePoolInstantiatedSet.Contains(index))
        {
            projectilePoolInstantiatedSet.Add(index);
        }

        projectilePooler.UpdatePoolInfo(index);
    }

    private void SetHeroesShootAction()
    {
        foreach (Hero hero in castleClan.GetHeroes().Values)
        {
            if (hero.TryGetComponent(out ParabolaShooter parabolaShooter))
            {
                parabolaShooter.SetSpawnAction(SpawnParabolaProjectile);
            }
            else if (hero.TryGetComponent(out StraightShooter straightShooter))
            {
                straightShooter.SetSpawnAction(SpawnStraightProjectile);
            }
        }
    }
    #endregion
}
