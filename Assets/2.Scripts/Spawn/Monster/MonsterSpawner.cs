using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private IPooler<Monster> stageMonsterPooler;
    private IPooler<Monster> dungeonMonsterPooler;

    [SerializeField] private MonsterCoreInfoDataSO monsterCoreInfoDataSO;
    [SerializeField] private MonsterBaseStatDataSO monsterBaseDataSO;
    [SerializeField] private MonsterBaseStatDataSO bossBaseDataSO;
    [SerializeField] private MonsterCountDataSO monsterCountDataSO;
    [SerializeField] private DungeonBossDataSO dungeonBossDataSO;

    private UpgradeMonsterDataHandler upgradeDataHandler;

    private MonsterSpawnData monsterSpawnData;

    private StageController stageController;

    private BattleManager battleManager;

    private DungeonController dungeonController;

    [SerializeField] private float outsideOfScreenModifiedX = 1;

    public Action<Monster> OnSpawned;

    [SerializeField] private float arriveDuration = 5f;

    [SerializeField] private Transform obstacleTransform;

    private bool isMonsterPoolFirstTime = true;
    private bool isDungeonBossPoolFirstTime = true;

    private HashSet<int> mainStageSet = new HashSet<int>();
    private HashSet<int> dungeonSet = new HashSet<int>();

    public void Init()
    {
        battleManager = FindAnyObjectByType<BattleManager>();
        upgradeDataHandler = FindAnyObjectByType<UpgradeMonsterDataHandler>();
        InitSpawnAction();
    }

    #region UnityMethods

    private void OnDestroy()
    {
        stageController.OnSpawnAction -= Spawn;
    }
    #endregion

    #region MonsterPoolMethods
    private void UpdateMonsterPool(int mainStageNum)
    {
        if (isMonsterPoolFirstTime)
        {
            isMonsterPoolFirstTime = false;
            (int, int) poolCountMod = monsterCountDataSO.GetPoolCountMod();
            stageMonsterPooler = new MonsterObjectPooler(obstacleTransform.position.x, monsterSpawnData.SpawnMinPosition.x, arriveDuration, transform, poolCountMod, 
                upgradeDataHandler.GetMonsterData, stageController.TryGoNextRoutineStage, battleManager.OnCastleAttacked);
        }

        if (mainStageSet.Contains(mainStageNum))
        {
            return;
        }

        mainStageSet.Add(mainStageNum);
        Dictionary<CoreInfoData, int> maxCountDict = monsterCountDataSO.GetMainStageMonsterMaxCountData(mainStageNum);
        Dictionary<string, CoreInfoData> monstersCoreDataDict = monsterCountDataSO.GetStageMonsterCoreDataSet(mainStageNum);
        foreach (var monsterCoreData in monstersCoreDataDict.Values)
        {
            if (maxCountDict.ContainsKey(monsterCoreData))
            {
                stageMonsterPooler.AddPoolInfo(monsterCoreData.index, maxCountDict[monsterCoreData]);
            }
        }
    }

    private void UpdateMonsterPool(DungeonType dungeonType)
    {
        DungeonBossData dungeonBossData;

        if (isDungeonBossPoolFirstTime)
        {
            isDungeonBossPoolFirstTime = false;
            dungeonBossData = dungeonBossDataSO.GetDungeonBossData(DungeonType.GoldDungeon);

            (int, int) poolCountMod = (1, dungeonBossData.count);
            dungeonMonsterPooler = new MonsterObjectPooler(obstacleTransform.position.x, monsterSpawnData.SpawnMinPosition.x, arriveDuration, transform, poolCountMod,
                upgradeDataHandler.GetMonsterData, dungeonController.TryClearDungeon, battleManager.OnCastleAttacked);
        }

        dungeonBossData = dungeonBossDataSO.GetDungeonBossData(dungeonType);
        if (dungeonSet.Contains(dungeonBossData.index))
        {
            return;
        }

        dungeonSet.Add(dungeonBossData.index);

        //MonsterData monsterData = monsterBaseDataSO.GetMonsterBaseData(dungeonBossData.index);
        dungeonMonsterPooler.AddPoolInfo(dungeonBossData.index, 1);
    }
    #endregion

    public void SetSpawnData(MonsterSpawnData monsterSpawnData)
    {
        this.monsterSpawnData = monsterSpawnData;
    }

    private void Spawn(int prefabIndex)
    {
        float monsterPosX = monsterSpawnData.SpawnMinPosition.x + outsideOfScreenModifiedX;

        MonsterType monsterType = monsterCoreInfoDataSO.GetCoreInfoData(prefabIndex).monsterType;

        float monsterPosY;
        if (monsterType != MonsterType.Normal)
        {
            monsterPosY = (monsterSpawnData.SpawnMinPosition.y + monsterSpawnData.SpawnMaxPosition.y) * Consts.HALF - 1;
        }
        else
        {
            monsterPosY = UnityEngine.Random.Range(monsterSpawnData.SpawnMinPosition.y, monsterSpawnData.SpawnMaxPosition.y);
        }

        Spawn(prefabIndex, new Vector2(monsterPosX, monsterPosY));
    }

    private void SpawnDungeonBoss(int prefabIndex, DungeonType dungeonType)
    {
        Vector2 monsterPos = GetDungeonMonsterPosition(dungeonType);
        SpawnDungeonBoss(dungeonType, prefabIndex, monsterPos);
    }

    private Vector2 GetDungeonMonsterPosition(DungeonType dungeonType)
    {
        Vector2 monsterPos = Vector2.zero;

        if (dungeonType == DungeonType.ColleagueLevelUpStoneDungeon)
        {
            monsterPos.x = monsterSpawnData.SpawnMinPosition.x;
        }
        else
        {
            monsterPos.x = monsterSpawnData.SpawnMinPosition.x + outsideOfScreenModifiedX;
        }

        if (dungeonType == DungeonType.GoldDungeon)
        {
            monsterPos.y = UnityEngine.Random.Range(monsterSpawnData.SpawnMinPosition.y, monsterSpawnData.SpawnMaxPosition.y);
        }
        else
        {
            monsterPos.y = (monsterSpawnData.SpawnMinPosition.y + monsterSpawnData.SpawnMaxPosition.y) * Consts.HALF - 1;
        }

        return monsterPos;
    }

    private void Spawn(int prefabIndex, Vector2 position)
    {
        Monster monster = stageMonsterPooler.Pool(prefabIndex, position, Quaternion.identity);
        OnSpawned?.Invoke(monster);
    }

    private void SpawnDungeonBoss(DungeonType dungeonType, int prefabIndex, Vector2 position)
    {
        Monster monster = dungeonMonsterPooler.Pool(prefabIndex, position, Quaternion.identity);
        if (dungeonType == DungeonType.ColleagueLevelUpStoneDungeon)
        {
            monster.transform.position = new Vector2(monster.transform.position.x - monster.GetColliderWidth(), monster.transform.position.y);
        }

        OnSpawned?.Invoke(monster);
    }

    private void InitSpawnAction()
    {
        stageController = FindAnyObjectByType<StageController>();
        stageController.OnSpawnAction += Spawn;
        stageController.OnMainStageUpdated += UpdateMonsterPool;

        dungeonController = FindAnyObjectByType<DungeonController>();
        dungeonController.OnSpawnDungeonBoss += SpawnDungeonBoss;
        
        dungeonController.OnUpdateDungeonPool += UpdateMonsterPool;
    }
}
