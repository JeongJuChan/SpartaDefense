using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    [SerializeField] private DungeonBossDataSO dungeonBossDataSO;
    [SerializeField] private DungeonBossIncrementDataSO dungeonBossIncrementDataSO;

    [SerializeField] private float bossWaitDuration = 3f;
    private WaitForSeconds bossWaitForSeconds;
    public event Action<int, float> OnBossShowUI;

    private Dictionary<DungeonType, int> dungeonBossIndexDict = new Dictionary<DungeonType, int>();

    public event Action<int, DungeonType> OnSpawnDungeonBoss;
    public event Action<DungeonType> OnUpdateDungeonPool;
    public event Action<int, DungeonType, DungeonBossIncrementData> OnUpgradeDungeonBoss;
    public event Func<DungeonType, DungeonBossData> OnGetDungeonBossData;

    public event Action<bool> OnSwitchStageEvent;
    public event Action<DungeonType, Func<DungeonType, IEnumerator>> OnSwitchDungeonEvent;
    public event Action OnUpdateCostUI;
    public event Action<bool> OnChangeDungeonActiveState;
    public event Action<bool> OnupdateDungeonProgress;
    public event Action<string> OnUpdateDungeonName;

    public event Action<int> OnStartTimer;
    public event Action OnStopTimer;

    private Dictionary<DungeonType, PairCurrencyTypeData> dungeonPairCurrencyTypeDict;
    private Dictionary<DungeonType, QuestType> dungeonQuestTypeDict;

    private Castle castle;

    private Dictionary<DungeonType, int> dungeonChallengableLevelDict = new Dictionary<DungeonType, int>();

    private DungeonType currentDungeonType;

    private int currentLevel;

    private BigInteger currentRewardAmount = new BigInteger();

    private Dictionary<CurrencyType, RewardType> rewardToCurrencyDict;

    private int currentSpawnAmount;

    [SerializeField] private float spawnInterval = 0.3f;
    [SerializeField] private float waveInterval = 1f;
    [SerializeField] private float deadDuration = 1f;
    [SerializeField] private float SwitchStageDelay = 2f;

    private WaitForSeconds spawnIntervalWaitForSeconds;
    private WaitForSeconds waveIntervalWaitForSeconds;

    private WaitForSeconds waitForDeadDuration;
    private WaitForSeconds waitForSwitchStageDelay;

    public event Action<bool> OnChangeCastleDamageableState;

    public void Init()
    {
        castle = FindAnyObjectByType<Castle>();

        bossWaitForSeconds = CoroutineUtility.GetWaitForSeconds(bossWaitDuration);

        spawnIntervalWaitForSeconds = CoroutineUtility.GetWaitForSeconds(spawnInterval);
        waveIntervalWaitForSeconds = CoroutineUtility.GetWaitForSeconds(waveInterval);

        waitForDeadDuration = CoroutineUtility.GetWaitForSeconds(deadDuration);

        waitForSwitchStageDelay = CoroutineUtility.GetWaitForSeconds(SwitchStageDelay);

        dungeonPairCurrencyTypeDict = new Dictionary<DungeonType, PairCurrencyTypeData>()
        {
            { DungeonType.GoldDungeon, new PairCurrencyTypeData(CurrencyType.GoldDungeonTicket, CurrencyType.Gold) },
            { DungeonType.GemDungeon, new PairCurrencyTypeData(CurrencyType.GemDungeonTicket, CurrencyType.Gem) },
            { DungeonType.ForgeTicketDungeon, new PairCurrencyTypeData(CurrencyType.ForgeDungeonTicket, CurrencyType.ForgeTicket) },
            { DungeonType.ColleagueLevelUpStoneDungeon, new PairCurrencyTypeData(CurrencyType.ColleagueLevelUpDungeonTicket, CurrencyType.ColleagueLevelUpStone) },
        };

        rewardToCurrencyDict = new Dictionary<CurrencyType, RewardType>()
        {   { CurrencyType.Gold, RewardType.Gold },
            { CurrencyType.Gem, RewardType.Gem },
            { CurrencyType.ForgeTicket, RewardType.ForgeTicket },
            { CurrencyType.ColleagueLevelUpStone, RewardType.ColleagueLevelUpStone },
            { CurrencyType.AbilityPoint, RewardType.AbilityPoint },
            { CurrencyType.ColleagueSummonTicket, RewardType.ColleagueSummonTicket },
        };

        dungeonQuestTypeDict = new Dictionary<DungeonType, QuestType>()
        {
            { DungeonType.GoldDungeon, QuestType.GoldDungeonLevelClear },
            { DungeonType.GemDungeon, QuestType.GemDungeonLevelClear },
            { DungeonType.ForgeTicketDungeon, QuestType.ForgeDungeonLevelClear },
            { DungeonType.ColleagueLevelUpStoneDungeon, QuestType.ColleagueLevelUpStoneDungeonLevelClear },
        };

        foreach (DungeonType dungeonType in dungeonPairCurrencyTypeDict.Keys)
        {
            dungeonBossIndexDict.Add(dungeonType, dungeonBossDataSO.GetDungeonBossData(dungeonType).index);
        }

        QuestManager.instance.AddQuestTypeAction(QuestType.ForgeDungeonLevelClear, () => QuestManager.instance.UpdateCount(QuestType.ForgeDungeonLevelClear, (GetDungeonLevel(DungeonType.ForgeTicketDungeon) - 1) < 0 ? 0 : GetDungeonLevel(DungeonType.ForgeTicketDungeon) - 1));
        QuestManager.instance.AddQuestTypeAction(QuestType.GemDungeonLevelClear, () => QuestManager.instance.UpdateCount(QuestType.GemDungeonLevelClear, (GetDungeonLevel(DungeonType.GemDungeon) - 1) < 0 ? 0 : GetDungeonLevel(DungeonType.GemDungeon) - 1));
        QuestManager.instance.AddQuestTypeAction(QuestType.GoldDungeonLevelClear, () => QuestManager.instance.UpdateCount(QuestType.GoldDungeonLevelClear, (GetDungeonLevel(DungeonType.GoldDungeon) - 1) < 0 ? 0 : GetDungeonLevel(DungeonType.GoldDungeon) - 1));
        QuestManager.instance.AddQuestTypeAction(QuestType.ColleagueLevelUpStoneDungeonLevelClear, () => QuestManager.instance.UpdateCount(QuestType.GoldDungeonLevelClear, (GetDungeonLevel(DungeonType.ColleagueLevelUpStoneDungeon) - 1) < 0 ? 0 : GetDungeonLevel(DungeonType.ColleagueLevelUpStoneDungeon) - 1));
    }

    public string GetDungeonName(DungeonType dungeonType)
    {
        return dungeonBossDataSO.GetDungeonBossData(dungeonType).dungeonName;
    }

    public void EntranceDungeon(DungeonType dungeonType, int level)
    {
        if (!dungeonBossIndexDict.ContainsKey(dungeonType))
        {
            return;
        }

        if (!dungeonChallengableLevelDict.ContainsKey(dungeonType))
        {
            return;
        }

        OnupdateDungeonProgress?.Invoke(false);
        OnChangeDungeonActiveState?.Invoke(true);
        castle.Die(false);

        currentLevel = level;
        currentDungeonType = dungeonType;

        string dungeonName = OnGetDungeonBossData.Invoke(dungeonType).dungeonName;
        OnUpdateDungeonName.Invoke($"{dungeonName} {currentLevel}");

        OnSwitchDungeonEvent.Invoke(dungeonType, CoSpawnDungeonBossDelay);
    }

    private IEnumerator CoSpawnDungeonBossDelay(DungeonType dungeonType)
    {
        OnUpgradeDungeonBoss?.Invoke(currentLevel, currentDungeonType, dungeonBossIncrementDataSO.GetDungeonBossIncrementData(dungeonType));
        DungeonBossData dungeonBossData = OnGetDungeonBossData.Invoke(dungeonType);
        int dungeonBossIndex = dungeonBossData.index;
        int totalCount = dungeonBossData.count;

        OnBossShowUI?.Invoke(dungeonBossIndex, bossWaitDuration);
        yield return bossWaitForSeconds;
        OnStartTimer?.Invoke(dungeonBossData.duration);

        currentSpawnAmount = totalCount;

        int waveCount = dungeonBossData.waveCount;
        if (waveCount > 1)
        {
            int spawnAmountPerWave = totalCount / waveCount;

            for (int i = 0; i < waveCount; i++)
            {
                for (int j = 0; j < spawnAmountPerWave; j++)
                {
                    OnSpawnDungeonBoss?.Invoke(dungeonBossIndex, dungeonType);
                    yield return spawnIntervalWaitForSeconds;
                }

                yield return waveIntervalWaitForSeconds;
            }
        }
        else
        {
            OnSpawnDungeonBoss?.Invoke(dungeonBossIndex, dungeonType);
        }
    }

    public void TryClearDungeon(Vector2 lastPos)
    {
        if (castle.isDead)
        {
            return;
        }

        currentSpawnAmount = currentSpawnAmount - 1 >= 0 ? currentSpawnAmount - 1 : 0;

        if (currentSpawnAmount != 0)
        {
            return;
        }

        OnChangeCastleDamageableState?.Invoke(false);

        if (dungeonChallengableLevelDict[currentDungeonType] <= currentLevel)
        {
            dungeonChallengableLevelDict[currentDungeonType]++;

            ES3.Save<int>($"{currentDungeonType}{Consts.DUNGEON_CHALLENGEABLE_LEVEL}", dungeonChallengableLevelDict[currentDungeonType], ES3.settings);

            ES3.StoreCachedFile();
        }

        StartCoroutine(CoWaitForDungeonClear(lastPos));
    }

    private IEnumerator CoWaitForDungeonClear(Vector2 lastPos)
    {
        OnStopTimer?.Invoke();
        OnupdateDungeonProgress?.Invoke(true);

        yield return waitForDeadDuration;
        OnChangeCastleDamageableState?.Invoke(true);
        CurrencyType rewardCurrencyType = dungeonPairCurrencyTypeDict[currentDungeonType].rewardCurrencyType;
        currentRewardAmount = OnGetDungeonBossData.Invoke(currentDungeonType).dungeonReward;
        CurrencyManager.instance.TryUpdateCurrency(dungeonPairCurrencyTypeDict[currentDungeonType].costCurrencyType, -1);
        RewardManager.instance.GiveReward(rewardToCurrencyDict[rewardCurrencyType], int.Parse(currentRewardAmount.ToString()));
        RewardManager.instance.ShowRewardPanel();

        yield return waitForSwitchStageDelay;

        OnChangeDungeonActiveState?.Invoke(false);
        OnSwitchStageEvent?.Invoke(false);

        UIManager.instance.bottomBarController.OpenDungeonCanvas();

        QuestManager.instance.UpdateCount(dungeonQuestTypeDict[currentDungeonType], (GetDungeonLevel(currentDungeonType) - 1) < 0 ? 0 : GetDungeonLevel(currentDungeonType) - 1);

        switch (currentDungeonType)
        {
            case DungeonType.GoldDungeon:
                DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.Dungeon_GoldDungeon, 1);
                break;
            case DungeonType.GemDungeon:
                DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.Dungeon_GemDungeon, 1);
                break;
            case DungeonType.ForgeTicketDungeon:
                DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.Dungeon_ForgeDungeon, 1);
                break;
            case DungeonType.ColleagueLevelUpStoneDungeon:
                DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.Dungeon_ColleagueLevelUpStoneDungeon, 1);
                break;
        }
    }

    public void SweepDungeon(DungeonType dungeonType, int level)
    {
        if (!dungeonBossIndexDict.ContainsKey(dungeonType))
        {
            return;
        }

        if (!dungeonChallengableLevelDict.ContainsKey(dungeonType))
        {
            return;
        }

        currentLevel = level;
        currentDungeonType = dungeonType;

        PairCurrencyTypeData pairCurrencyTypeData = dungeonPairCurrencyTypeDict[currentDungeonType];

        CurrencyManager.instance.TryUpdateCurrency(pairCurrencyTypeData.costCurrencyType, -1);
        OnUpdateCostUI?.Invoke();

        CurrencyType rewardCurrencyType = pairCurrencyTypeData.rewardCurrencyType;
        OnUpgradeDungeonBoss?.Invoke(currentLevel, dungeonType, dungeonBossIncrementDataSO.GetDungeonBossIncrementData(dungeonType));
        currentRewardAmount = OnGetDungeonBossData.Invoke(currentDungeonType).dungeonReward;
        RewardManager.instance.GiveReward(rewardToCurrencyDict[rewardCurrencyType], int.Parse(currentRewardAmount.ToString()));
        RewardManager.instance.ShowRewardPanel();

        switch (currentDungeonType)
        {
            case DungeonType.GoldDungeon:
                DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.Dungeon_GoldDungeon, 1);
                break;
            case DungeonType.GemDungeon:
                DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.Dungeon_GemDungeon, 1);
                break;
            case DungeonType.ForgeTicketDungeon:
                DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.Dungeon_ForgeDungeon, 1);
                break;
        }
    }

    public void FailedDungeon()
    {
        OnChangeDungeonActiveState?.Invoke(false);
        OnSwitchStageEvent?.Invoke(true);
    }

    public int GetDungeonLevel(DungeonType dungeonType)
    {
        if (!dungeonChallengableLevelDict.ContainsKey(dungeonType))
        {
            dungeonChallengableLevelDict.Add(dungeonType, ES3.Load($"{dungeonType}{Consts.DUNGEON_CHALLENGEABLE_LEVEL}", 1));
            ResourceManager.instance.monster.LoadDungeonMonsters(dungeonType);
            OnUpdateDungeonPool?.Invoke(dungeonType);
        }

        return dungeonChallengableLevelDict[dungeonType];
    }

    public DungeonType GetDungeonType()
    {
        return currentDungeonType;
    }

    public PairCurrencyTypeData GetPairCurrencyType(DungeonType dungeonType)
    {
        if (dungeonPairCurrencyTypeDict == null)
        {
            Init();
        }

        return dungeonPairCurrencyTypeDict[dungeonType];
    }

    public BigInteger GetBossReward(DungeonType dungeonType, int level)
    {
        OnUpgradeDungeonBoss?.Invoke(level, dungeonType, dungeonBossIncrementDataSO.GetDungeonBossIncrementData(dungeonType));
        return OnGetDungeonBossData.Invoke(dungeonType).dungeonReward;
    }
}
