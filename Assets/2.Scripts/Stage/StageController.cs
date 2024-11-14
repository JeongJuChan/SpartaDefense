using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Button;

public class StageController : MonoBehaviour
{
    [SerializeField] private MonsterCountDataSO monsterCountDataSO;
    [SerializeField] private MonsterIncrementDataSO monsterIncrementDataSO;
    [SerializeField] private BossIncrementDataSO bossIncrementDataSO;

    private int subMaxStage;
    private int bossRoutineStage;

    private int currentDifficulty = 1;
    private int currentMainStageNum = 1;
    private int currentSubStageNum = 1;
    private int currentRoutineStageNum = 1;

    public int currentIndex { get; set; } = 1101;

    private int mainStageRepeatDivideNum;
    // private int currentRepeatedMainStageNum;

    public event Action<int> OnSpawnAction;

    private int currentRoutineMonsterCount;

    [field: Header("StageUIEvent")]
    public event Action<int, int, int> OnUpdateStageText;
    public event Action OnResetRoutineStage;
    public event Action<int> OnUpdateRoutineStage;
    public event Action<int> OnStartTimer;
    public event Action OnStopTimer;

    public event Action<int> OnMainStageUpdated;

    public event Action<int> OnUpdateStageIndex;

    public event Action OnUpdateCastleQuest;

    [HideInInspector] public ButtonClickedEvent OnChallengeBoss;

    public event Action<int, int, int, Dictionary<string, CoreInfoData>, MonsterIncrementData> OnUpgradeMonster;
    public event Action<int, int, int, BossIncrementData> OnUpgradeBoss;

    private RewardMovingController rewardMovingController;

#if UNITY_EDITOR
    public event Action<int, int, int, int, BossIncrementData> OnEditorUpgradeBoss;
#endif

    private Castle castle;

    private bool isBossEncountered = false;

    public event Action<bool> OnDefeatAfterBossEncountered;

    [SerializeField] private float waitTimeForNextStage = 0.1f;

    private WaitForSeconds nextStageWaitForSeconds;

    private bool isWaitEventProgressing = false;

    public event Action OnClearRoutineStage;

    public event Action OnBossChallenged;

    [SerializeField] private float bossWaitDuration = 3f;
    private WaitForSeconds bossWaitForSeconds;
    public event Action<float> OnBossShowUI;

    private bool isChallengeBossByButton;

    private Action<int> UpdateMonsterResource;

    private BattlePlaceType battlePlaceType = BattlePlaceType.Stage;

    [SerializeField] private int bossClearLimit = 40;

    /*private Coroutine spawnBossCoroutine;
    private Coroutine spawnMonsterCoroutine;*/

    private Queue<MonsterCountData> monsterQueue = new Queue<MonsterCountData>();

    [SerializeField] private float monsterSpawnInterval = 0.5f;
    private WaitForSeconds monsterSpawnIntervalWaitForSeconds;
    private Action<FeatureType> OnUpdateFeature;

    public event Func<int, int, int, BigInteger> OnGetBossGoldReward;
    public event Func<int, int, int, BigInteger> OnGetBossForgeTicketReward;
    public event Func<int, int, int, BigInteger> OnGetNormalMonsterGoldReward;
    public event Func<int, int, int, BigInteger> OnGetNormalMonsterForgeTicketReward;

    public event Action<bool> OnChangeCastleDamageableState;
    public event Action<bool> OnChangeMonsterDamageableState;

    private Queue<Coroutine> spawnQueue = new Queue<Coroutine>();

    public void Init()
    {
        LoadDatas();

        ResourceManager.instance.monster.LoadStageMonsters(currentMainStageNum);
        UpdateMonsterResource += ResourceManager.instance.monster.LoadStageMonsters;

        bossRoutineStage = monsterCountDataSO.bossRoutineStage;
        bossWaitForSeconds = CoroutineUtility.GetWaitForSeconds(bossWaitDuration);
        nextStageWaitForSeconds = CoroutineUtility.GetWaitForSeconds(waitTimeForNextStage);
        castle = FindAnyObjectByType<Castle>();
        castle.OnDead += ResetRoutineStage;
        subMaxStage = monsterCountDataSO.subMaxStage;

        rewardMovingController = FindObjectOfType<RewardMovingController>();


        monsterSpawnIntervalWaitForSeconds = CoroutineUtility.GetWaitForSeconds(monsterSpawnInterval);

        mainStageRepeatDivideNum = monsterCountDataSO.GetMainMaxStageNum() + 1;

        UpdateReapeatedMainStage();

        // QuestManager.instance.GetQuestTypeAction.Add(QuestType.StageClear, () => QuestManager.instance.UpdateCount(QuestType.StageClear, GetCurrentIndex()));
        QuestManager.instance.AddQuestTypeAction(QuestType.StageClear, () => QuestManager.instance.UpdateCount(QuestType.StageClear, GetCurrentIndex()));
        QuestManager.instance.AddQuestTypeAction(QuestType.MonsterKillCount, () => QuestManager.instance.UpdateCount(QuestType.MonsterKillCount, 1));


        UnlockManager.Instance.SetUnlockCondition(FeatureType.Stage, CheckCurrentStage);
        OnUpdateFeature += UnlockManager.Instance.CheckUnlocks;

        OnUpgradeMonster?.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum,
            monsterCountDataSO.GetStageMonsterCoreDataSet(currentMainStageNum), monsterIncrementDataSO.GetData());
        OnUpgradeBoss?.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum, bossIncrementDataSO.GetData());
        OnMainStageUpdated?.Invoke(currentMainStageNum);
    }

    private bool CheckCurrentStage(int stage)
    {
        if (stage <= GetCurrentIndex())
        {
            return true;
        }
        return false;
    }

    private void InitAfterMonsterPoolReady()
    {
        bossRoutineStage = monsterCountDataSO.bossRoutineStage;
        OnChallengeBoss.AddListener(ChallengeBoss);
        OnResetRoutineStage?.Invoke();
        StartCurrentRoutineStage();
    }

    public void StartInit()
    {
        if (ES3.KeyExists(Consts.FirstOpen, ES3.settings))
        {
            StartStage();
        }
        else
        {
            DialogManager.instance.OnStartStage += StartStage;
        }
    }

    private void StartStage()
    {
        InitAfterMonsterPoolReady();
        OnDefeatAfterBossEncountered(isBossEncountered);
        OnUpdateStageText?.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum);
        OnUpdateFeature?.Invoke(FeatureType.Stage);
        OnUpdateStageIndex?.Invoke(GetCurrentIndex());
    }

    private void OnDestroy()
    {
        OnChallengeBoss.RemoveListener(ChallengeBoss);
    }

    private void BossClearReward(Vector2 lastPos)
    {
        // TODO: 정우님 오시면 BigInteger로 바꿔야 한다고 말 해야함
        BigInteger gold = OnGetBossGoldReward.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum);
        RewardManager.instance.GiveReward(RewardType.Gold, gold);
        BigInteger forgeTicket = OnGetBossForgeTicketReward.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum);
        RewardManager.instance.GiveReward(RewardType.ForgeTicket, forgeTicket);

        rewardMovingController.RequestMovingCurrency(3, RewardType.Gold, lastPos);
        rewardMovingController.RequestMovingCurrency(3, RewardType.ForgeTicket, lastPos);

        RewardManager.instance.ShowStageClearRewardPanel();
    }

    private void StartCurrentRoutineStage()
    {
        OnChangeCastleDamageableState?.Invoke(true);
        OnChangeMonsterDamageableState?.Invoke(true);

        List<MonsterCountData> monsterCountDatas = monsterCountDataSO.GetMonsterCountDatas(currentMainStageNum, currentRoutineStageNum);

        if (monsterCountDatas == null)
        {
            StartCoroutine(CoGoNextSubStage());
            return;
        }
        else
        {
            if (currentRoutineStageNum == bossRoutineStage)
            {
                if (isBossEncountered)
                {
                    if (!isChallengeBossByButton)
                    {
                        isChallengeBossByButton = false;
                        currentRoutineStageNum = 1;
                        StartCurrentRoutineStage();
                        return;
                    }
                    else
                    {
                        OnBossChallenged?.Invoke();
                    }
                }

                rewardMovingController.ClearActiveCurrency();
                castle.Die(false);

                isBossEncountered = true;
                SaveDatas();
                ES3.Save<bool>(Consts.IS_BOSS_ENCOUNTERED, isBossEncountered, ES3.settings);
                ES3.StoreCachedFile();
                spawnQueue.Enqueue(StartCoroutine(CoSpawnBossDelay(monsterCountDatas)));
                //spawnBossCoroutine = StartCoroutine(CoSpawnBossDelay(monsterCountDatas));
                return;
            }
        }

        MonsterIncrementData increment = monsterIncrementDataSO.GetData();
        OnUpgradeMonster?.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum,
            monsterCountDataSO.GetStageMonsterCoreDataSet(currentMainStageNum), increment);

        SaveDatas();
        spawnQueue.Enqueue(StartCoroutine(CoSpawnMonstersDelay(monsterCountDatas)));
        //spawnMonsterCoroutine = StartCoroutine(CoSpawnMonstersDelay(monsterCountDatas));
    }

    private IEnumerator CoSpawnBossDelay(List<MonsterCountData> monsterCountDatas)
    {
        isWaitEventProgressing = true;
        OnUpgradeBoss?.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum, bossIncrementDataSO.GetData());
        OnBossShowUI?.Invoke(bossWaitDuration);
        yield return bossWaitForSeconds;
        OnStartTimer?.Invoke(bossClearLimit);
        isWaitEventProgressing = false;
        spawnQueue.Enqueue(StartCoroutine(CoSpawnMonstersDelay(monsterCountDatas)));
        //spawnBossCoroutine = StartCoroutine(CoSpawnMonstersDelay(monsterCountDatas));
    }

    private IEnumerator CoSpawnMonstersDelay(List<MonsterCountData> monsterCountDatas)
    {
        if (monsterCountDatas.Count > 1)
        {
            RollMonsterOrder(monsterCountDatas);
            yield return null;
        }

        OnUpdateRoutineStage?.Invoke(currentRoutineStageNum);

        monsterQueue = new Queue<MonsterCountData>(monsterCountDatas);

        while (monsterQueue.Count > 0)
        {
            MonsterCountData monsterCountData = monsterQueue.Dequeue();

            currentRoutineMonsterCount += monsterCountData.count;
            for (int i = 0; i < monsterCountData.count; i++)
            {
                yield return monsterSpawnIntervalWaitForSeconds;
                OnSpawnAction?.Invoke(monsterCountData.coreInfoData.index);
            }
        }
    }

    private void RollMonsterOrder(List<MonsterCountData> monsterCountDatas)
    {
        if (monsterCountDatas.Count > 1)
        {
            for (int i = 0; i < monsterCountDatas.Count; i++)
            {
                MonsterCountData temp = monsterCountDatas[i];
                int tempIndex = UnityEngine.Random.Range(0, currentRoutineMonsterCount);
                monsterCountDatas[i] = monsterCountDatas[tempIndex];
                monsterCountDatas[tempIndex] = temp;
            }
        }
    }

    public void TryGoNextRoutineStage(Vector2 lastPos)
    {
        if (isWaitEventProgressing)
        {
            return;
        }

        currentRoutineMonsterCount = currentRoutineMonsterCount - 1 >= 0 ? currentRoutineMonsterCount - 1 : 0;

        Debug.Log($"currentRoutineMonsterCount : {currentRoutineMonsterCount}");

        TryGetRewardForHuntingMonster(lastPos);

        if (currentRoutineMonsterCount <= 0 && battlePlaceType == BattlePlaceType.Stage)
        {
            OnChangeCastleDamageableState?.Invoke(false);
            OnChangeMonsterDamageableState?.Invoke(false);
            StartCoroutine(CoGoNextRoutineStage(lastPos));
        }
    }

    public int GetCurrentMainStage()
    {
        return currentMainStageNum;
    }

    private void GoNextMainStage()
    {
        currentSubStageNum = 1;
        currentMainStageNum++;

        castle.Die(false);
        QuestManager.instance.UpdateCount(QuestType.StageClear, GetCurrentIndex());
        OnUpdateStageIndex?.Invoke(currentIndex);
        UpdateReapeatedMainStage();
        UpdateMonsterResource?.Invoke(currentMainStageNum);
        OnMainStageUpdated?.Invoke(currentMainStageNum);
        OnUpdateStageText?.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum);
        ResetRoutineStage();

        OnUpdateFeature?.Invoke(FeatureType.Stage);

        SaveDatas();

        isWaitEventProgressing = false;
    }

    private IEnumerator CoGoNextSubStage()
    {
        if (isWaitEventProgressing)
        {
            yield break;
        }

        isWaitEventProgressing = true;
        castle.Die(false);

        yield return nextStageWaitForSeconds;

        currentRoutineStageNum = 1;
        currentSubStageNum++;

        isBossEncountered = false;

        isWaitEventProgressing = false;

        if (subMaxStage < currentSubStageNum)
        {
            GoNextMainStage();
            isWaitEventProgressing = false;
            yield break;
        }

        OnUpdateFeature?.Invoke(FeatureType.Stage);

        OnUpdateStageText?.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum);
        ResetRoutineStage();

        Firebase.Analytics.FirebaseAnalytics.LogEvent($"current_stage_{GetCurrentIndex()}");
    }

    private int GetCurrentIndex()
    {
        int stageWeight = 100;
        int sectionWeight = 10;

        currentIndex = currentDifficulty * stageWeight * sectionWeight + currentMainStageNum * stageWeight + currentSubStageNum;
        return currentIndex;
    }

    public void ResetRoutineStage()
    {
        OnStopTimer?.Invoke();
        OnUpdateStageText?.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum);
        StartCoroutine(CoResetRoutineStage());
    }

    public void ReturnPreStage()
    {
        OnStopTimer?.Invoke();
        OnUpdateStageText?.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum);

        StartCoroutine(CoReturnRoutineStage());
    }

    private IEnumerator CoReturnRoutineStage()
    {
        if (isWaitEventProgressing)
        {
            yield break;
        }

        isWaitEventProgressing = true;
        yield return nextStageWaitForSeconds;

        OnDefeatAfterBossEncountered?.Invoke(isBossEncountered);

        currentRoutineMonsterCount = 0;
        OnResetRoutineStage?.Invoke();
        isWaitEventProgressing = false;
        QuestManager.instance.UpdateCount(QuestType.StageClear, GetCurrentIndex());
        OnUpdateStageIndex?.Invoke(currentIndex);

        SaveDatas();
        StartCurrentRoutineStage();
    }

    private IEnumerator CoResetRoutineStage()
    {
        if (isWaitEventProgressing)
        {
            yield break;
        }

        isWaitEventProgressing = true;
        yield return nextStageWaitForSeconds;

        OnDefeatAfterBossEncountered?.Invoke(isBossEncountered);

        currentRoutineMonsterCount = 0;
        currentRoutineStageNum = 1;
        OnResetRoutineStage?.Invoke();
        isWaitEventProgressing = false;
        QuestManager.instance.UpdateCount(QuestType.StageClear, GetCurrentIndex());
        OnUpdateStageIndex?.Invoke(currentIndex);

        SaveDatas();
        StartCurrentRoutineStage();
    }

    private IEnumerator CoGoNextRoutineStage(Vector2 lastPos)
    {
        isWaitEventProgressing = true;
        yield return nextStageWaitForSeconds;
        currentRoutineMonsterCount = 0;
        currentRoutineStageNum++;
        OnClearRoutineStage?.Invoke();

        if (currentRoutineStageNum > bossRoutineStage)
        {
            BossClearReward(lastPos);
        }
        isWaitEventProgressing = false;
        StartCurrentRoutineStage();
    }

    private void ChallengeBoss()
    {
        StopSpawnMonster();
        currentRoutineStageNum = bossRoutineStage;
        isChallengeBossByButton = true;
        OnClearRoutineStage?.Invoke();
        currentRoutineMonsterCount = 0;
        StartCurrentRoutineStage();
    }

    public int GetSubMaxStage()
    {
        return subMaxStage;
    }

    public void UpdateBattlePlace(BattlePlaceType battlePlaceType)
    {
        this.battlePlaceType = battlePlaceType;
    }

    public void StopSpawnMonster()
    {
        while (spawnQueue.Count > 0)
        {
            StopCoroutine(spawnQueue.Dequeue());
        }

        isWaitEventProgressing = false;
        /*if (currentRoutineStageNum < bossRoutineStage)
        {
            if (spawnMonsterCoroutine != null)
            {
                StopCoroutine(spawnMonsterCoroutine);
            }
        }
        else
        {
            if (spawnBossCoroutine != null)
            {
                StopCoroutine(spawnBossCoroutine);
            }
        }*/
    }

    private void TryGetRewardForHuntingMonster(Vector2 lastPos)
    {
        if (isBossEncountered)
        {
            return;
        }

        BigInteger gold = OnGetNormalMonsterGoldReward.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum);
        BigInteger forgeTicket = OnGetNormalMonsterForgeTicketReward.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum);
        CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gold, gold);
        CurrencyManager.instance.TryUpdateCurrency(CurrencyType.ForgeTicket, forgeTicket);

        if (gold > 0)
        {
            rewardMovingController.RequestMovingCurrency(3, RewardType.Gold, lastPos);
        }
        if (forgeTicket > 0)
        {
            rewardMovingController.RequestMovingCurrency(1, RewardType.ForgeTicket, lastPos);
        }
    }

    private void SaveDatas()
    {
        ES3.Save<int>(Consts.CURRENT_MAIN_STAGE_NUM, currentMainStageNum, ES3.settings);
        ES3.Save<int>(Consts.CURRENT_SUB_STAGE_NUM, currentSubStageNum, ES3.settings);
        ES3.Save<int>(Consts.CURRENT_ROUTINE_STAGE_NUM, currentRoutineStageNum, ES3.settings);
        ES3.Save<int>(Consts.CURRENT_DIFFICULTY, currentDifficulty, ES3.settings);
        ES3.Save<int>(Consts.CURRENT_STAGE_INDEX, currentIndex, ES3.settings);
        ES3.Save<bool>(Consts.IS_BOSS_ENCOUNTERED, isBossEncountered, ES3.settings);

        ES3.StoreCachedFile();

        OnUpdateCastleQuest?.Invoke();
    }

    private void LoadDatas()
    {
        currentMainStageNum = ES3.Load<int>(Consts.CURRENT_MAIN_STAGE_NUM, 1, ES3.settings);
        currentSubStageNum = ES3.Load<int>(Consts.CURRENT_SUB_STAGE_NUM, 1, ES3.settings);
        currentRoutineStageNum = ES3.Load<int>(Consts.CURRENT_ROUTINE_STAGE_NUM, 1, ES3.settings);
        currentDifficulty = ES3.Load<int>(Consts.CURRENT_DIFFICULTY, 1, ES3.settings);
        currentIndex = ES3.Load<int>(Consts.CURRENT_STAGE_INDEX, 1101, ES3.settings);
        isBossEncountered = ES3.Load<bool>(Consts.IS_BOSS_ENCOUNTERED, false, ES3.settings);
    }

    private void UpdateReapeatedMainStage()
    {
        if (currentMainStageNum >= mainStageRepeatDivideNum)
        {
            currentDifficulty++;
            currentMainStageNum = 1;
            currentSubStageNum = 1;
        }
    }



#if UNITY_EDITOR
    public void EditorChallengeBossss()
    {
        isChallengeBossByButton = true;
        StopSpawnMonster();
        currentRoutineStageNum = bossRoutineStage;
        OnClearRoutineStage?.Invoke();
        currentRoutineMonsterCount = 0;
        StartCurrentRoutineStage();
        isChallengeBossByButton = false;
    }
    public void EditorSkipCurrentRoutineStageeee()
    {
        StopSpawnMonster();
        currentRoutineStageNum++;
        OnClearRoutineStage?.Invoke();
        currentRoutineMonsterCount = 0;
        StartCurrentRoutineStage();
    }


    public void EditorSkipCurrentRoutineStage()
    {
        StopSpawnMonster();
        currentRoutineStageNum++;
        OnClearRoutineStage?.Invoke();
        currentRoutineMonsterCount = 0;
        StartCurrentRoutineStage();
    }

    public void EditorChallengeBoss()
    {
        StopSpawnMonster();
        isChallengeBossByButton = true;
        currentRoutineStageNum = bossRoutineStage;
        OnClearRoutineStage?.Invoke();
        currentRoutineMonsterCount = 0;
        StartCurrentRoutineStage();
        isChallengeBossByButton = false;
    }

    public void EditorSkipCurrentSubStage()
    {
        if (isWaitEventProgressing)
        {
            return;
        }

        StopSpawnMonster();

        OnClearRoutineStage?.Invoke();
        currentRoutineMonsterCount = 0;
        StartCoroutine(CoGoNextSubStage());
    }

    public void EditorSkipCurrentMainStage()
    {
        StopSpawnMonster();
        currentSubStageNum = 1;
        currentRoutineMonsterCount = 0;
        currentRoutineStageNum = 1;


        castle.Die(false);

        currentMainStageNum++;
        UpdateReapeatedMainStage();

        OnClearRoutineStage?.Invoke();
        OnUpdateStageText?.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum);
        UpdateMonsterResource?.Invoke(currentMainStageNum);
        OnMainStageUpdated?.Invoke(currentMainStageNum);

        MonsterIncrementData increment = monsterIncrementDataSO.GetData();
        OnUpgradeMonster?.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum,
            monsterCountDataSO.GetStageMonsterCoreDataSet(currentMainStageNum), increment);

        int bossIndex = monsterCountDataSO.GetMonsterCountDatas(currentMainStageNum, bossRoutineStage)[0].coreInfoData.index;
        BossIncrementData bossIncrementData = bossIncrementDataSO.GetData();

        OnEditorUpgradeBoss?.Invoke(currentDifficulty, currentMainStageNum, currentSubStageNum, bossIndex, bossIncrementData);

        OnUpdateStageIndex?.Invoke(GetCurrentIndex());

        SaveDatas();

        StartCurrentRoutineStage();
    }
#endif
}
