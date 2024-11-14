using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviorSingleton<ResourceManager>
{
    public UI_QuestBar questBar;
    public GameObject questGuide;
    public ForgeUIButton forgeUIButton;
    public Image expPanel;
    public Image castleLevelButton;
    public GameObject castleLevelGuide;

    [SerializeField] private Sprite[] colorSprites;
    [SerializeField] private Sprite[] colorSpritesForText;
    [SerializeField] private MonsterCoreInfoDataSO monsterCoreInfoDataSO;
    [SerializeField] private MonsterCountDataSO monsterCountDataSO;
    [SerializeField] private DungeonBossDataSO dungeonBossDataSO;
    [SerializeField] private ColleagueDataSO slotDataSO;
    [SerializeField] private EffectDataSO effectDataSO;
    [SerializeField] private SkillDataSO skillDataSO;
    [SerializeField] private ColleagueProjectileDataSO colleagueProjectileDataSO;

    public DailyQuestSO dailyQuestSO { get; private set; }
    public DialogueDataSO dialogueDataSO { get; private set; }
    public UnlockDataSO unlockDataSO { get; private set; }
    public ColleagueIconDataSO colleagueIconDataSO { get; private set; }

    public CastleProgressionDataSO castleProgressionDataSO { get; private set; }

    public MonsterResourceHandler monster { get; private set; }
    public HeroProjectileHandler heroProjectile { get; private set; }
    private SlotHeroDataHandler slotHeroDataHandlerInstance;
    public SlotHeroDataHandler slotHeroData
    {
        get
        {
            if (slotHeroDataHandlerInstance == null)
            {
                slotHeroData = new SlotHeroDataHandler();
            }
            return slotHeroDataHandlerInstance;
        }
        private set
        {
            slotHeroDataHandlerInstance = value;
        }
    }
    public EffectDataHandler effect { get; private set; }
    public SkillDataHandler skill { get; private set; }
    public RankDataHandler rank;
    public ForgeEquipmentResourceDataHandler forgeEquipmentResourceDataHandler;

    public GrowthDataSO growth { get; private set; }

    public EnumToKRSO enumToKRSO { get; private set; }

    private const string DEFAULT_SO_PATH = "ScriptableObjects";

    public void Init()
    {
        monster = new MonsterResourceHandler(monsterCountDataSO.UpdateMonstersCoreData, dungeonBossDataSO.GetDungeonBossData,
            monsterCoreInfoDataSO.GetCoreInfoData);
        heroProjectile = new HeroProjectileHandler(colleagueProjectileDataSO.GetColleagueProjectileData);
        effect = new EffectDataHandler(effectDataSO.GetAllEffectData);
        skill = new SkillDataHandler(skillDataSO.GetSkillDatas, skillDataSO);

        castleProgressionDataSO = Resources.Load<CastleProgressionDataSO>("ScriptableObjects/CastleProgressionDataSO/CastleProgressionData");

        unlockDataSO = Resources.Load<UnlockDataSO>("ScriptableObjects/UnlockDataSO/UnlockData");

        dailyQuestSO = Resources.Load<DailyQuestSO>("ScriptableObjects/DailyQuestSO/DailyQuest");

        dialogueDataSO = Resources.Load<DialogueDataSO>("ScriptableObjects/DialogueDataSO/DialogueData");

        colleagueIconDataSO = Resources.Load<ColleagueIconDataSO>("ScriptableObjects/Colleague/ColleagueIconData");
        colleagueIconDataSO.InitDict();
        colleagueIconDataSO.Init();

        rank = new RankDataHandler(colorSprites, colorSpritesForText, UIManager.instance.GetUIElement<NewSlotUIPanel>());

        forgeEquipmentResourceDataHandler = new ForgeEquipmentResourceDataHandler();

        growth = Resources.Load<GrowthDataSO>($"{DEFAULT_SO_PATH}/Growth/GrowthDataSO");
        growth.InitDict();

        enumToKRSO = Resources.Load<EnumToKRSO>($"{DEFAULT_SO_PATH}/ToKR/EnumToKR");
        enumToKRSO.InitDict();
    }
}
