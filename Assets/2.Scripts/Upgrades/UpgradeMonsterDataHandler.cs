using Keiwando.BigInteger;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMonsterDataHandler : MonoBehaviour
{
    private StageController stageController;
    private DungeonController dungeonController;

    private MonsterUpgradableData monsterUpgradableData;
    private MonsterUpgradableData bossUpgradableData;
    private Dictionary<int, MonsterCoreInfoData> monsterCoreDataDict = new Dictionary<int, MonsterCoreInfoData>();


    private Dictionary<int, MonsterData> monsterBaseDataDict = new Dictionary<int, MonsterData>();
    private Dictionary<int, MonsterData> monsterDataDict = new Dictionary<int, MonsterData>();
    private Dictionary<DungeonType, DungeonBossData> dungeonBossDataDict = new Dictionary<DungeonType, DungeonBossData>();

    private Dictionary<MonsterType, Func<int, MonsterData>> getMonsterDataDict;

    [SerializeField] private MonsterCoreInfoDataSO monsterCoreInfoDataSO;
    [SerializeField] private MonsterBaseStatDataSO monsterBaseDataSO;
    [SerializeField] private MonsterBaseStatDataSO bossBaseDataSO;
    [SerializeField] private DungeonBossDataSO dungeonBossDataSO;
    [SerializeField] private DungeonBossIncrementDataSO dungeonBossIncrementDataSO;
    [SerializeField] private MonsterCountDataSO monsterCountDataSO;


    [SerializeField] private MonsterRewardDataSO monsterRewardDataSO;
    [SerializeField] private MonsterRewardDataSO bossRewardDataSO;
    [SerializeField] private MonsterRewardIncrementDataSO monsterRewardIncrementDataSO;
    [SerializeField] private MonsterRewardIncrementDataSO bossRewardIncrementDataSO;

    //TODO: 스테이지 컨트롤의 디바이드 메인 맥스 스테이지로 바꾸기
    private int mainMaxStage;
    private int subMaxStage;
    private int speedUpgradeUnitStage;

    private BattleManager battleManager;

    private const float MAX_SPEED = 1.5f;

#if UNITY_EDITOR
    [SerializeField] private int monsterUpgradeCount = 0;
    [SerializeField] private int bossUpgradeCount = 0;
    [SerializeField] private int bossAdditionalUpgradeCount = 0;
    [SerializeField] private int monsterSpeedMod = 0;
#endif

    private int lastSubUpgradeStageAtFirstTime = 9;
    private int lastSubUpgradeStage = 10;

    private int preDifficultyNumForMonster;
    private int preMainStageNumForMonster;
    private int preSubStageNumForMonster;

    private int preDifficultyNumForBoss;
    private int preMainStageNumForBoss;
    private int preSubStageNumForBoss;

    private int normalMonsterGoldRewardTotal;
    private int normalMonsterForgeTicketRewardTotal;
    private int bossMonsterGoldRewardTotal;
    private int bossMonsterForgeTicketRewardTotal;

    private MonsterRewardData normalMonsterRewardData;
    private MonsterRewardData bossMonsterRewardData;

    private MonsterRewardIncrementData monsterRewardIncrementData;
    private MonsterRewardIncrementData bossRewardIncrementData;

    private BigInteger rewardValue = new BigInteger(0);

    public void Init()
    {
        stageController = FindAnyObjectByType<StageController>();
        stageController.OnUpgradeMonster += UpgradeMonster;

        stageController.OnGetNormalMonsterGoldReward += GetMonsterGoldReward;
        stageController.OnGetNormalMonsterForgeTicketReward += GetMonsterForgeTicketReward;
        stageController.OnGetBossGoldReward += GetBossGoldReward;
        stageController.OnGetBossForgeTicketReward += GetBossForgeTicketReward;

        battleManager = FindAnyObjectByType<BattleManager>();
        battleManager.OnGetMonsterData += GetMonsterData;

        stageController.OnUpgradeBoss += UpgradeBoss;
        mainMaxStage = monsterCountDataSO.GetMainMaxStageNum();
        subMaxStage = monsterCountDataSO.subMaxStage;
        speedUpgradeUnitStage = subMaxStage / 2;

        dungeonController = FindAnyObjectByType<DungeonController>();
        dungeonController.OnUpgradeDungeonBoss += UpgradeDungeonBoss;
        dungeonController.OnGetDungeonBossData += GetDungeonBossData;

        RewardManager.instance.offlineRewardCallback += GetTotalSubStage;

        getMonsterDataDict = new Dictionary<MonsterType, Func<int, MonsterData>>()
        {
            { MonsterType.Normal, GetNormalMonsterData },
            { MonsterType.Boss, GetBossMonsterData },
            { MonsterType.DungeonBoss, GetDungeonBossData },
        };

        normalMonsterRewardData = monsterRewardDataSO.GetData();
        bossMonsterRewardData = bossRewardDataSO.GetData();

        monsterRewardIncrementData = monsterRewardIncrementDataSO.GetData();
        bossRewardIncrementData = bossRewardIncrementDataSO.GetData();

        normalMonsterGoldRewardTotal = normalMonsterRewardData.goldProbability * Consts.PERCENT_DIVIDE_VALUE;
        normalMonsterForgeTicketRewardTotal = normalMonsterRewardData.forgeTicketProbability * Consts.PERCENT_DIVIDE_VALUE;

        bossMonsterGoldRewardTotal = bossMonsterRewardData.goldProbability * Consts.PERCENT_DIVIDE_VALUE;
        bossMonsterForgeTicketRewardTotal = bossMonsterRewardData.forgeTicketProbability * Consts.PERCENT_DIVIDE_VALUE;

#if UNITY_EDITOR
        stageController.OnEditorUpgradeBoss += UpgradeBossEditor;
#endif
    }

    public MonsterData GetMonsterData(int monsterIndex)
    {
        if (!monsterCoreDataDict.ContainsKey(monsterIndex))
        {
            monsterCoreDataDict.Add(monsterIndex, monsterCoreInfoDataSO.GetCoreInfoData(monsterIndex));
        }

        return getMonsterDataDict[monsterCoreDataDict[monsterIndex].monsterType].Invoke(monsterIndex);
    }

    private MonsterData GetNormalMonsterData(int monsterIndex)
    {
        MonsterCoreInfoData monsterCoreInfoData = monsterCoreDataDict[monsterIndex];

        return new MonsterData(monsterCoreInfoData, monsterUpgradableData);

        /*// below code is extension code for future
        if (!monsterDataDict.ContainsKey(monsterIndex))
        {
            return GetMonsterBaseData(monsterIndex);
        }

        return monsterDataDict[monsterIndex];*/
    }

    private MonsterData GetBossMonsterData(int index)
    {
        MonsterCoreInfoData monsterCoreInfoData = monsterCoreDataDict[index];

        Debug.Log($"보스 몬스터 공격력 : {bossUpgradableData.damage}, 체력 : {bossUpgradableData.health}");
        return new MonsterData(monsterCoreInfoData, bossUpgradableData);

        /*// below code is extension code for future
        if (!monsterDataDict.ContainsKey(index))
        {
            MonsterCoreInfoData bossMonsterCoreInfoData = monsterCoreInfoDataSO.GetCoreInfoData(index);
            MonsterUpgradableData bossMonsterUpgradableData = bossBaseDataSO.GetData();
            MonsterData bossMonsterData = new MonsterData(bossMonsterCoreInfoData, bossMonsterUpgradableData);
            monsterDataDict.Add(index, bossMonsterData);
        }

        return monsterDataDict[index];*/
    }

    private MonsterData GetMonsterBaseData(int index)
    {
        if (!monsterBaseDataDict.ContainsKey(index))
        {
            MonsterCoreInfoData monsterCoreInfoData = monsterCoreInfoDataSO.GetCoreInfoData(index);
            MonsterUpgradableData monsterUpgradableData = monsterBaseDataSO.GetData();
            MonsterData monsterData = new MonsterData(monsterCoreInfoData, monsterUpgradableData);
            monsterBaseDataDict.Add(index, monsterData);
        }

        return monsterBaseDataDict[index];
    }

    private DungeonBossData GetDungeonBossData(DungeonType dungeonType)
    {
        if (!dungeonBossDataDict.ContainsKey(dungeonType))
        {
            return dungeonBossDataSO.GetDungeonBossData(dungeonType);
        }

        return dungeonBossDataDict[dungeonType];
    }

    private void UpgradeMonster(int difficultyNum, int mainStageNum, int subStageNum, Dictionary<string, CoreInfoData> monsterCoreInfoDict, MonsterIncrementData increment)
    {
        UpdateMonsterUpgradableDataRepeat(difficultyNum, mainStageNum, subStageNum, increment);
        preDifficultyNumForMonster = difficultyNum;
        preMainStageNumForMonster = mainStageNum;
        preSubStageNumForMonster = subStageNum;

        /*bool isUpgradePossible = preDifficultyNumForMonster != difficultyNum || preMainStageNumForMonster != mainStageNum || preSubStageNumForMonster != subStageNum;

        if (isUpgradePossible)
        {
            UpdateMonsterUpgradableDataRepeat(difficultyNum, mainStageNum, subStageNum, increment);
            preDifficultyNumForMonster = difficultyNum;
            preMainStageNumForMonster = mainStageNum;
            preSubStageNumForMonster = subStageNum;
        }*/

        /*if (monsterUpgradableData.damage == null)
        {
            UpdateMonsterUpgradableDataRepeat(difficultyNum, mainStageNum, subStageNum, increment);
            preDifficultyNumForMonster = difficultyNum;
            preMainStageNumForMonster = mainStageNum;
            preSubStageNumForMonster = subStageNum;
        }
        else
        {
            bool isUpgradePossible = preDifficultyNumForMonster != difficultyNum || preMainStageNumForMonster != mainStageNum || preSubStageNumForMonster != subStageNum;

            if (isUpgradePossible)
            {
                UpdateMonsterUpgradableDataIncreased(difficultyNum, mainStageNum, subStageNum, increment);
                preDifficultyNumForMonster = difficultyNum;
                preMainStageNumForMonster = mainStageNum;
                preSubStageNumForMonster = subStageNum;
            }
        }*/

        return;

        /*MonsterData monsterData;
        foreach (var coreInfo in monsterCoreInfoDict.Values)
        {
            if (monsterCoreInfoDataSO.GetCoreInfoData(coreInfo.index).monsterType != MonsterType.Normal)
            {
                continue;
            }


            monsterData = GetMonsterDataIncreased(difficultyNum, mainStageNum, subStageNum, coreInfo.index, increment);
            Debug.Log($"{difficultyNum},{mainStageNum},{subStageNum} / {monsterData.coreInfoData.coreInfoData.name} / " +
                $"데미지 : {monsterData.monsterUpgradableData.damage}, 체력 : {monsterData.monsterUpgradableData.health}");
            if (!monsterDataDict.ContainsKey(coreInfo.index))
            {
                monsterDataDict.Add(coreInfo.index, monsterData);
            }
            else
            {
                monsterDataDict[coreInfo.index] = monsterData;
            }
        }*/
    }

    private void UpgradeBoss(int difficultyNum, int mainStageNum, int subStageNum, BossIncrementData increment)
    {
        UpdateBossDataRepeat(difficultyNum, mainStageNum, subStageNum, increment);
        preDifficultyNumForBoss = difficultyNum;
        preMainStageNumForBoss = mainStageNum;
        preSubStageNumForBoss = subStageNum;
        /*if (bossUpgradableData.damage == null)
        {
            UpdateBossDataRepeat(difficultyNum, mainStageNum, subStageNum, increment);
            preDifficultyNumForBoss = difficultyNum;
            preMainStageNumForBoss = mainStageNum;
            preSubStageNumForBoss = subStageNum;
        }*/
        /*else
        {
            bool isUpgradePossible = preDifficultyNumForBoss != difficultyNum || preMainStageNumForBoss != mainStageNum || preSubStageNumForBoss != subStageNum;

            if (isUpgradePossible)
            {
                UpdateBossData(subStageNum, increment);
                preDifficultyNumForBoss = difficultyNum;
                preMainStageNumForBoss = mainStageNum;
                preSubStageNumForBoss = subStageNum;
            }
        }*/

        return;

        /*MonsterData bossData = GetBossDataIncreased(difficultyNum, mainStageNum, subStageNum, bossIndex, increment);

        bool isSubStageMax = subStageNum == subMaxStage;
        if (isSubStageMax)
        {
            ApplyBossDataMainStageIncreased(mainStageNum, subStageNum, ref bossData, increment);
        }

        if (!monsterDataDict.ContainsKey(bossIndex))
        {
            monsterDataDict.Add(bossIndex, bossData);
        }
        else
        {
            monsterDataDict[bossIndex] = bossData;
        }*/
    }

    private void UpgradeDungeonBoss(int level, DungeonType dungeonType, DungeonBossIncrementData increment)
    {
        DungeonBossData dungeonBossData = dungeonBossDataSO.GetDungeonBossData(dungeonType);

        MonsterData bossData = monsterDataDict[dungeonBossData.index];

        bossData.monsterUpgradableData = GetDungeonBossDataIncreased(level, dungeonBossData.monsterUpgradableData, increment);

        if (!monsterDataDict.ContainsKey(dungeonBossData.index))
        {
            monsterDataDict.Add(dungeonBossData.index, bossData);
        }
        else
        {
            monsterDataDict[dungeonBossData.index] = bossData;
        }

        UpgradeDungeonRewardData(level, ref dungeonBossData, increment);

        if (!dungeonBossDataDict.ContainsKey(dungeonType))
        {
            dungeonBossDataDict.Add(dungeonType, dungeonBossData);
        }
        else
        {
            dungeonBossDataDict[dungeonType] = dungeonBossData;
        }
    }

    private void UpdateMonsterUpgradableDataRepeat(int difficultyNum, int mainStageNum, int subStageNum, MonsterIncrementData increment)
    {
        MonsterUpgradableData monsterUpgradableData = monsterBaseDataSO.GetData();

        int totalSubStage = GetTotalSubStage(difficultyNum, mainStageNum, subStageNum);
        int totalMainStage = GetTotalMainStage(difficultyNum, mainStageNum);

        monsterUpgradableData.health += increment.hpIncrement * totalSubStage;
        monsterUpgradableData.health = monsterUpgradableData.health + monsterUpgradableData.health *
            increment.stageIncrementPercent * totalMainStage / Consts.PERCENT_DIVIDE_VALUE * increment.hpIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;
        monsterUpgradableData.damage += increment.damageIncrement * totalSubStage;
        monsterUpgradableData.damage = monsterUpgradableData.damage + monsterUpgradableData.damage *
            increment.stageIncrementPercent * totalMainStage / Consts.PERCENT_DIVIDE_VALUE * increment.damageIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;

        /*while (result > 0)
        {
            monsterUpgradableData = UpgradeMonsterBattleData(increment, monsterUpgradableData);
            result--;
        }*/

        monsterUpgradableData = UpdateMonsterSpeedData(increment, monsterUpgradableData, totalSubStage);

        Debug.Log($"monsterUpgradableData.health : {monsterUpgradableData.health}, monsterUpgradableData.damage : {monsterUpgradableData.damage}");
        this.monsterUpgradableData = monsterUpgradableData;
    }

    private MonsterUpgradableData UpgradeMonsterBattleData(MonsterIncrementData increment, MonsterUpgradableData monsterUpgradableData)
    {
        monsterUpgradableData.health += increment.hpIncrement;
        monsterUpgradableData.health = monsterUpgradableData.health +
                    monsterUpgradableData.health * increment.hpIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;
        monsterUpgradableData.damage += increment.damageIncrement;
        monsterUpgradableData.damage = monsterUpgradableData.damage +
            monsterUpgradableData.damage * increment.damageIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;
        return monsterUpgradableData;
    }

    private void UpdateMonsterUpgradableDataIncreased(int difficultyNum, int mainStageNum, int subStageNum, MonsterIncrementData increment)
    {
        MonsterUpgradableData monsterUpgradableData = this.monsterUpgradableData;

        monsterUpgradableData = UpgradeMonsterBattleData(increment, monsterUpgradableData);

        int result = GetTotalSubStage(difficultyNum, mainStageNum, subStageNum);

        monsterUpgradableData = UpdateMonsterSpeedData(increment, monsterUpgradableData, result);

        this.monsterUpgradableData = monsterUpgradableData;
    }

    private MonsterUpgradableData UpdateMonsterSpeedData(MonsterIncrementData increment, MonsterUpgradableData monsterUpgradableData, int result)
    {
#if UNITY_EDITOR
        monsterUpgradeCount = result;
#endif

        int speedMod = result / speedUpgradeUnitStage;

#if UNITY_EDITOR
        monsterSpeedMod = speedMod;
#endif

        monsterUpgradableData.attackSpeedRate += increment.speedIncrement * speedMod;
        monsterUpgradableData.attackSpeedRate = monsterUpgradableData.attackSpeedRate > MAX_SPEED ? MAX_SPEED : monsterUpgradableData.attackSpeedRate;
        monsterUpgradableData.speed += increment.speedIncrement * speedMod;
        monsterUpgradableData.speed = monsterUpgradableData.speed > MAX_SPEED ? MAX_SPEED : monsterUpgradableData.speed;
        return monsterUpgradableData;
    }

    // TODO: 오차 있음
    private MonsterData GetMonsterDataIncreased(int difficultyNum, int mainStageNum, int subStageNum, int index, MonsterIncrementData increment)
    {
        MonsterData monsterData = GetMonsterBaseData(index);

        MonsterUpgradableData monsterUpgradableData = monsterData.monsterUpgradableData;
        int result = GetTotalSubStage(difficultyNum, mainStageNum, subStageNum);

#if UNITY_EDITOR
        monsterUpgradeCount = result;
#endif

        monsterUpgradableData.health = monsterUpgradableData.health +
            monsterUpgradableData.health * result * increment.hpIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;
        monsterUpgradableData.damage = monsterUpgradableData.damage +
            monsterUpgradableData.damage * result * increment.damageIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;


        int speedMod = result / speedUpgradeUnitStage;

#if UNITY_EDITOR
        monsterSpeedMod = speedMod;
#endif

        monsterUpgradableData.attackSpeedRate += increment.speedIncrement * speedMod;
        monsterUpgradableData.attackSpeedRate = monsterUpgradableData.attackSpeedRate > MAX_SPEED ? MAX_SPEED : monsterUpgradableData.attackSpeedRate;
        monsterUpgradableData.speed += increment.speedIncrement * speedMod;
        monsterUpgradableData.speed = monsterUpgradableData.speed > MAX_SPEED ? MAX_SPEED : monsterUpgradableData.speed;

        monsterData.monsterUpgradableData = monsterUpgradableData;
        return monsterData;
    }

    private void UpdateBossDataRepeat(int difficultyNum, int mainStageNum, int subStageNum, BossIncrementData increment)
    {
        MonsterUpgradableData newBossUpgradableData = bossBaseDataSO.GetData();

        int totalSubStage = GetTotalSubStage(difficultyNum, mainStageNum, subStageNum);
        int totalMainStage = GetTotalMainStage(difficultyNum, mainStageNum);
#if UNITY_EDITOR
        bossUpgradeCount = totalSubStage;
#endif

        UpdateBossData(newBossUpgradableData, totalSubStage, totalMainStage, increment);

        /*while (result > 0)
        {
            newBossUpgradableData.health += increment.hpIncrement;
            newBossUpgradableData.health = newBossUpgradableData.health + newBossUpgradableData.health *
                increment.hpIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;
            newBossUpgradableData.damage += increment.damageIncrement;
            newBossUpgradableData.damage = newBossUpgradableData.damage + newBossUpgradableData.damage *
                increment.damageIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;

            result--;
        }*/

        Debug.Log($"monsterUpgradableData.health : {bossUpgradableData.health}, monsterUpgradableData.damage : {bossUpgradableData.damage}");
    }

    private void UpdateBossData(MonsterUpgradableData newBossUpgradableData, int subStageNum, int totalMainStage, BossIncrementData increment)
    {
        newBossUpgradableData.health += increment.hpIncrement * subStageNum;
        newBossUpgradableData.health = newBossUpgradableData.health + newBossUpgradableData.health *
            increment.stageIncrementPercent * totalMainStage / Consts.PERCENT_DIVIDE_VALUE * increment.hpIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;
        newBossUpgradableData.damage += increment.damageIncrement * subStageNum;
        newBossUpgradableData.damage = newBossUpgradableData.damage + newBossUpgradableData.damage *
            increment.stageIncrementPercent * totalMainStage / Consts.PERCENT_DIVIDE_VALUE * increment.damageIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;

        bossUpgradableData = newBossUpgradableData;
    }

    private void UpdateBossData(int difficultyNum, int mainStageNum, int subStageNum, BossIncrementData increment)
    {
        MonsterUpgradableData newBossUpgradableData = bossUpgradableData;

        newBossUpgradableData.health = newBossUpgradableData.health + newBossUpgradableData.health *
            increment.hpIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;
        newBossUpgradableData.damage = newBossUpgradableData.damage + newBossUpgradableData.damage *
            increment.damageIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;
    }

    private MonsterData GetBossDataIncreased(int difficultyNum, int mainStageNum, int subStageNum, int index, BossIncrementData increment)
    {
        MonsterCoreInfoData monsterCoreInfoData = monsterCoreInfoDataSO.GetCoreInfoData(index);
        MonsterUpgradableData monsterUpgradableData = bossBaseDataSO.GetData();

        MonsterData bossData = new MonsterData(monsterCoreInfoData, monsterUpgradableData);

        int result = GetTotalSubStage(difficultyNum, mainStageNum, subStageNum);

#if UNITY_EDITOR
        bossUpgradeCount = result;
#endif

        monsterUpgradableData.health += increment.hpIncrement * result;
        monsterUpgradableData.health = monsterUpgradableData.health + monsterUpgradableData.health * result * increment.hpIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;
        monsterUpgradableData.damage += increment.damageIncrement * result;
        monsterUpgradableData.damage = monsterUpgradableData.damage + monsterUpgradableData.damage * result * increment.damageIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;
        bossData.monsterUpgradableData = monsterUpgradableData;
        return bossData;
    }

    private void ApplyBossDataMainStageIncreased(int mainStageNum, int subStageNum, ref MonsterData bossData, BossIncrementData increment)
    {
        MonsterUpgradableData monsterUpgradableData = bossData.monsterUpgradableData;

        int result = mainStageNum - 1 + (subStageNum / subMaxStage);

#if UNITY_EDITOR
        bossAdditionalUpgradeCount = result;
#endif
        monsterUpgradableData.health = monsterUpgradableData.health + monsterUpgradableData.health * result / Consts.PERCENT_DIVIDE_VALUE;
        monsterUpgradableData.damage = monsterUpgradableData.damage + monsterUpgradableData.damage * result / Consts.PERCENT_DIVIDE_VALUE;
        bossData.monsterUpgradableData = monsterUpgradableData;
    }

    private MonsterUpgradableData ApplyBossDataMainStageIncreased(MonsterUpgradableData bossUpgradableData, BossIncrementData increment)
    {
        MonsterUpgradableData newBossUpgradableData = bossUpgradableData;

        return newBossUpgradableData;
    }

    private int GetTotalSubStage(int difficultyNum, int mainStageNum, int subStageNum)
    {
        int result = ((difficultyNum - 1) * mainMaxStage + (mainStageNum - 1)) * subMaxStage + subStageNum;
        return result;
    }

    private int GetTotalMainStage(int difficultyNum, int mainStageNum)
    {
        int result = (difficultyNum - 1) * mainMaxStage + mainStageNum;
        return result;
    }

    private MonsterData GetDungeonBossData(int index)
    {
        if (!monsterDataDict.ContainsKey(index))
        {
            MonsterCoreInfoData monsterCoreInfoData = monsterCoreInfoDataSO.GetCoreInfoData(index);
            MonsterUpgradableData monsterUpgradableData = dungeonBossDataSO.GetDungeonBossData(index).monsterUpgradableData;
            MonsterData monsterData = new MonsterData(monsterCoreInfoData, monsterUpgradableData);
            monsterDataDict.Add(index, monsterData);
        }

        return monsterDataDict[index];
    }

    private MonsterUpgradableData GetDungeonBossDataIncreased(int level, MonsterUpgradableData dungeonBossUpgradableData, DungeonBossIncrementData increment)
    {
        dungeonBossUpgradableData.health = dungeonBossUpgradableData.health + increment.health * level;
        dungeonBossUpgradableData.health += dungeonBossUpgradableData.health * increment.battlePercentMod / Consts.PERCENT_DIVIDE_VALUE;
        dungeonBossUpgradableData.damage = dungeonBossUpgradableData.damage + increment.damage * level;
        dungeonBossUpgradableData.damage += dungeonBossUpgradableData.damage * increment.battlePercentMod / Consts.PERCENT_DIVIDE_VALUE;
        return dungeonBossUpgradableData;
    }

    private void UpgradeDungeonRewardData(int level, ref DungeonBossData defaultData, DungeonBossIncrementData increment)
    {
        defaultData.dungeonReward += increment.dungeonReward * level;
        defaultData.dungeonReward += defaultData.dungeonReward * (increment.rewardPercentMod / Consts.PERCENT_DIVIDE_VALUE);
    }

#if UNITY_EDITOR
    private void UpgradeBossEditor(int difficultyNum, int mainStageNum, int subStageNum, int bossIndex, BossIncrementData increment)
    {
        MonsterData bossData = GetBossDataIncreased(difficultyNum, mainStageNum, subStageNum, bossIndex, increment);

        ApplyBossDataMainStageIncreased(mainStageNum, subStageNum, ref bossData, increment);

        if (!monsterDataDict.ContainsKey(bossIndex))
        {
            monsterDataDict.Add(bossIndex, bossData);
        }
        else
        {
            monsterDataDict[bossIndex] = bossData;
        }
    }
#endif

    private BigInteger GetMonsterGoldReward(int difficulty, int mainStageNum, int subStageNum)
    {
        int result = GetTotalSubStage(difficulty, mainStageNum, subStageNum);

        int probability = UnityEngine.Random.Range(0, Consts.PERCENT_TOTAL_VALUE);

        BigInteger tempReward = normalMonsterRewardData.baseGold + monsterRewardIncrementData.goldIncrement * result;
        tempReward += tempReward * monsterRewardIncrementData.goldIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;
        rewardValue = normalMonsterGoldRewardTotal > probability ? tempReward : 0;

        Debug.Log($"골드 획득 : {rewardValue}");

        return rewardValue;
    }

    private BigInteger GetMonsterForgeTicketReward(int difficulty, int mainStageNum, int subStageNum)
    {
        int result = GetTotalSubStage(difficulty, mainStageNum, subStageNum);

        int probability = UnityEngine.Random.Range(0, Consts.PERCENT_TOTAL_VALUE);

        BigInteger tempReward = normalMonsterRewardData.baseForgeTicket + monsterRewardIncrementData.forgeTicketIncrement * result;
        tempReward += tempReward * monsterRewardIncrementData.forgeTicketIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;

        rewardValue = normalMonsterForgeTicketRewardTotal > probability ? tempReward : 0;

        Debug.Log($"티켓 획득 : {rewardValue}");

        return rewardValue;
    }

    private BigInteger GetBossGoldReward(int difficulty, int mainStageNum, int subStageNum)
    {
        int result = GetTotalSubStage(difficulty, mainStageNum, subStageNum);

        //int probability = UnityEngine.Random.Range(0, Consts.PERCENT_TOTAL_VALUE);

        BigInteger tempReward = bossMonsterRewardData.baseGold + bossRewardIncrementData.goldIncrement * result;
        tempReward += tempReward * monsterRewardIncrementData.goldIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;

        rewardValue = tempReward;

        Debug.Log($"보스 골드 획득 : {rewardValue}");

        return rewardValue;
    }

    private BigInteger GetBossForgeTicketReward(int difficulty, int mainStageNum, int subStageNum)
    {
        int result = GetTotalSubStage(difficulty, mainStageNum, subStageNum);

        //int probability = UnityEngine.Random.Range(0, Consts.PERCENT_TOTAL_VALUE);

        BigInteger tempReward = bossMonsterRewardData.baseForgeTicket + bossRewardIncrementData.forgeTicketIncrement * result;
        tempReward += tempReward * monsterRewardIncrementData.forgeTicketIncrementPercent / Consts.PERCENT_DIVIDE_VALUE;

        rewardValue = tempReward;

        Debug.Log($"보스 티켓 획득 : {rewardValue}");

        return rewardValue;
    }
}
