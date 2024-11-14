using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceEventSwitcher : MonoBehaviour
{
    public event Action OnClearMonsters;
    public event Action<int> OnBackToStage;

    public event Action<bool> OnActiveProgress;

    private StageController stageController;
    private DungeonController dungeonController;

    private Castle castle;
    private Action<BattlePlaceType> OnUpdateBattlePlace;

    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
    [SerializeField] private DungeonBackgroundSO dungeonBackgroundSO;

    [SerializeField] private StageProgressPanel stageProgressPanel;
    [SerializeField] private StageTimePanel stageTimePanel;

    [SerializeField] private BossBattlePanel bossBattlePanel;

    private Func<DungeonType> OnGetDungeonType;

    private Dictionary<BattlePlaceType, Coroutine> placeCoroutineDict = new Dictionary<BattlePlaceType, Coroutine>();

    private BattlePlaceType currentPlaceType;

    private DungeonQuitPanel dungeonQuitPanel;

    private RewardMovingController rewardMovingController;

    public event Action OnStopSpawnMonster;

    public void Init()
    {
        stageController = FindAnyObjectByType<StageController>();
        OnUpdateBattlePlace += stageController.UpdateBattlePlace;

        dungeonController = FindAnyObjectByType<DungeonController>();
        OnGetDungeonType += dungeonController.GetDungeonType;

        rewardMovingController = FindAnyObjectByType<RewardMovingController>();

        dungeonController.OnSwitchDungeonEvent += SwitchDungeonEvent;
        dungeonController.OnSwitchStageEvent += SwitchStageEvent;
        dungeonController.OnStartTimer += stageTimePanel.StartTimer;

        stageController.OnStartTimer += stageTimePanel.StartTimer;
        stageTimePanel.OnTimeOver += () => SwitchStageEvent(true);

        stageController.OnStopTimer += stageTimePanel.StopTimer;

        dungeonController.OnStopTimer += stageTimePanel.StopTimer;

        OnStopSpawnMonster += stageController.StopSpawnMonster;

        dungeonQuitPanel = UIManager.instance.GetUIElement<DungeonQuitPanel>();
        dungeonQuitPanel.OnDungeonQuit += SwitchStageEvent;
        dungeonQuitPanel.OnStopShowingLoadingPanel += bossBattlePanel.StopShowingLoadingPanelCoroutine;


        BattlePlaceType[] battlePlaceTypes = (BattlePlaceType[])Enum.GetValues(typeof(BattlePlaceType));

        for (int i = 1; i < battlePlaceTypes.Length; i++)
        {
            placeCoroutineDict.Add(battlePlaceTypes[i], null);
        }

        castle = FindAnyObjectByType<Castle>();
    }

    private void SwitchDungeonEvent(DungeonType dungeonType, Func<DungeonType, IEnumerator> OnGetDungeonCoroutine)
    {
        BattlePlaceType battlePlaceType = BattlePlaceType.Dungeon;
        currentPlaceType = battlePlaceType;
        StopPreCoroutine(battlePlaceType);

        OnActiveProgress?.Invoke(false);

        castle.OnDead -= stageController.ResetRoutineStage;
        backgroundSpriteRenderer.sprite = dungeonBackgroundSO.dungeonBackgrounds[(int)OnGetDungeonType.Invoke() - 1];

        OnStopSpawnMonster?.Invoke();
        OnClearMonsters?.Invoke();
        castle.OnDead += dungeonController.FailedDungeon;

        placeCoroutineDict[battlePlaceType] = StartCoroutine(OnGetDungeonCoroutine(dungeonType));
    }

    private void StopPreCoroutine(BattlePlaceType battlePlaceType)
    {
        if (currentPlaceType != battlePlaceType && placeCoroutineDict.ContainsKey(currentPlaceType))
        {
            StopCoroutine(placeCoroutineDict[currentPlaceType]);
        }
    }

    private void SwitchStageEvent(bool isFailed)
    {
        BattlePlaceType battlePlaceType = BattlePlaceType.Stage;
        StopPreCoroutine(battlePlaceType);

        OnActiveProgress?.Invoke(true);

        castle.OnDead -= dungeonController.FailedDungeon;
        OnStopSpawnMonster?.Invoke();
        OnClearMonsters?.Invoke();
        OnUpdateBattlePlace?.Invoke(battlePlaceType);
        OnBackToStage?.Invoke(stageController.GetCurrentMainStage());
        castle.OnDead += stageController.ResetRoutineStage;
        placeCoroutineDict[battlePlaceType] = StartCoroutine(CoWaitForNextStep(isFailed));

        rewardMovingController.ClearActiveCurrency();
    }

    private IEnumerator CoWaitForNextStep(bool isFailed)
    {
        yield return null;
        castle.Die(isFailed);
        stageController.ReturnPreStage();
    }
}
