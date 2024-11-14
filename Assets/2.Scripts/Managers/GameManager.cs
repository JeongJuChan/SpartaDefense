using System;
using UnityEngine;

public class GameManager : MonoBehaviorSingleton<GameManager>
{
    private PushNotificationManager pushNotificationManager;
    private StageController stageController;
    private ResourceManager resourceManager;
    private CastleClan castleClan;
    private SlotEquipmentForger slotEquipmentForger;
    private HeroProjectileSpawner heroProjectileSpawner;
    private EffectSpawner effectSpawner;
    private CurrencyManager currencyManager;
    private EquipmentManager equipmentManager;
    private UIManager uiManager;
    private SlotOpener slotOpener;
    private AdsManager adsManager;
    private NotificationManager notificationManager;
    private AutoForgeUIPanel autoForgeUIPanel;
    private SkillManager skillManager;
    private Castle castle;
    private ForgeProbabilityUIPanel forgeProbabilityUIPanel;
    private OfflineTimerController offlineTimerController;

    public bool isInitializing { get; private set; } = false;

    private void Awake()
    {
#if UNITY_EDITOR
        ES3.CacheFile();
        ES3.settings = new ES3Settings(ES3.Location.Cache);
        if (PushNotificationManager.instance.GetIsPlayerStartInMainScene())
        {
            PushNotificationManager.instance.StartInit();
        }
#endif
        currencyManager = CurrencyManager.instance;
        NotificationManager notificationManager = NotificationManager.instance;

        UnlockManager.Instance.Init();

        adsManager = AdsManager.instance;
        StatViewerHelper.instance.Init();
        resourceManager = FindAnyObjectByType<ResourceManager>();
        uiManager = FindAnyObjectByType<UIManager>();
        slotEquipmentForger = FindAnyObjectByType<SlotEquipmentForger>();
        skillManager = FindAnyObjectByType<SkillManager>();
        slotOpener = FindAnyObjectByType<SlotOpener>();
        MonsterDamage monsterDamage = FindAnyObjectByType<MonsterDamage>();
        Background background = FindAnyObjectByType<Background>();
        MonsterSpawnDataHandler monsterSpawnDataHandler = FindAnyObjectByType<MonsterSpawnDataHandler>();
        MonsterSpawner monsterSpawner = FindAnyObjectByType<MonsterSpawner>();
        stageController = FindAnyObjectByType<StageController>();
        BattleController battleController = FindAnyObjectByType<BattleController>();
        BattleManager battleManager = FindAnyObjectByType<BattleManager>();
        castleClan = FindAnyObjectByType<CastleClan>();
        castle = castleClan.GetComponentInParent<Castle>();
        equipmentManager = EquipmentManager.instance;
        heroProjectileSpawner = FindAnyObjectByType<HeroProjectileSpawner>();
        effectSpawner = FindAnyObjectByType<EffectSpawner>();
        MainUserInfoUIPanel mainUserInfoUIPanel = FindAnyObjectByType<MainUserInfoUIPanel>(FindObjectsInactive.Include);
        NewSlotUIPanel newSlotUIPanel = FindAnyObjectByType<NewSlotUIPanel>(FindObjectsInactive.Include);
        UpgradeMonsterDataHandler upgradeMonsterDataHandler = FindAnyObjectByType<UpgradeMonsterDataHandler>();
        forgeProbabilityUIPanel = FindAnyObjectByType<ForgeProbabilityUIPanel>(FindObjectsInactive.Include);
        PlaceEventSwitcher placeEventSwitcher = FindAnyObjectByType<PlaceEventSwitcher>();
        autoForgeUIPanel = FindAnyObjectByType<AutoForgeUIPanel>(FindObjectsInactive.Include);
        UnlockIconPanel unlockIconPanel = FindAnyObjectByType<UnlockIconPanel>(FindObjectsInactive.Include);
        ColleagueManager colleagueManager = FindAnyObjectByType<ColleagueManager>();
        ColleagueSpawner colleagueSpawner = FindAnyObjectByType<ColleagueSpawner>();
        EncyclopediaUIHandler encyclopediaUIHandler = FindAnyObjectByType<EncyclopediaUIHandler>();
        StatDataHandler.Instance.Init();
        QuestManager.instance.Initialze();
        currencyManager.Initialize();
        adsManager.Initialize();
        resourceManager.Init();
        EnumToKRManager.instance.Init();
        colleagueManager.Init();
        DialogManager.instance.Init();
        battleController.Init();
        battleManager.Init();
        castle.Init();
        castleClan.Init();
        slotEquipmentForger.Init();
        mainUserInfoUIPanel.Init();
        upgradeMonsterDataHandler.Init();
        monsterSpawnDataHandler.Init();
        monsterSpawner.Init();
        background.Init();
        stageController.Init();
        uiManager.Init();
        EncyclopediaDataHandler.Instance.Init();
        encyclopediaUIHandler.Init();
        UIManager.instance.GetUIElement<UI_Alert>().Initialize();
        UIManager.instance.GetUIElement<UI_Alert>().OpenUI();
        skillManager.Init();
        slotOpener.Init();
        monsterDamage.Init();
        effectSpawner.Init();
        heroProjectileSpawner.Init();
        colleagueSpawner.Init();
        equipmentManager.Initialize();
        RewardManager.instance.Initialize();
        currencyManager.UpdateCurrencyUI();
        UIAnimations.Instance.Initialize();
        placeEventSwitcher.Init();
        unlockIconPanel.Init();
        ForgeManager.instance.Init();

        QuestManager.instance.InitCurrentQuest();

        DailyQuestDataHandler.Instance.Init();

    }

    private void Start()
    {
        //FindAnyObjectByType<SkillEquipUIController>(FindObjectsInactive.Include).InitElements();
        isInitializing = true;

        uiManager.GetUIElement<UI_Alert>().PowerMessage(StatViewerHelper.instance.GetBattlePowerChange());
        uiManager.bottomBarController.InitUnlock();
        FindAnyObjectByType<UserLevelController>().StartInit();
        autoForgeUIPanel.StartInit();
        stageController.StartInit();
        offlineTimerController = FindAnyObjectByType<OfflineTimerController>();
        uiManager.GetUIElement<UI_Colleague>().LateInit();
        ForgeManager.instance.StartInit();
        offlineTimerController.StartApplication();
        forgeProbabilityUIPanel.StartInit();
        QuestManager.instance.StartInit();
        //uiManager.GetUIElement<UI_Growth>().StartInit();

        StatDataHandler.Instance.StartInit();


    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            /*stageController.EditorChallengeBossss();*/
            UIAnimations.Instance.ShakeCamera();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            stageController.EditorSkipCurrentRoutineStageeee();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.GoldDungeonTicket, 1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.GemDungeonTicket, 1);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            CurrencyManager.instance.TryUpdateCurrency(CurrencyType.ForgeDungeonTicket, 1);
        }
    }
#endif

    private void OnDestroy()
    {
        UnlockManager.DisPose();
        UIAnimations.DisPose();
        StatDataHandler.DisPose();
    }

    private void OnApplicationPause(bool pause)
    {
        if (ES3.KeyExists("PlayerNickName") && gameObject.scene.name == "GameScene")
        {
            if (pause)
            {
                if (offlineTimerController != null) offlineTimerController.TimeReset();
            }
            else
            {
                if (offlineTimerController != null) offlineTimerController.RefilKey();
            }
        }
    }
}
