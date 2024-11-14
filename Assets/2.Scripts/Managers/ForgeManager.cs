using Keiwando.BigInteger;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ForgeManager : MonoBehaviorSingleton<ForgeManager>
{
    [SerializeField] private SlotEquipmentForger slotEquipmentForger;
    [SerializeField] private AttributeDataSO attributeDataSO;
    [SerializeField] private CastleDoorRankProbabilityDataSO castleDoorRankProbabilityDataSO;
    [SerializeField] private SlotStatDataSO slotStatDataSO;
    [SerializeField] private ForgeEquipmentDataSO forgeEquipmentDataSO;

    [SerializeField] private int levelVariation = 2;

    private UIManager uiManager;

    private SlotUIDataHandler slotUIDataHandler;
    private NewSlotUIPanel newSlotUIPanel;
    private CurrentSlotUIPanel currentSlotUIPanel;
    private SlotDetailPanel slotDetailPanel;
    private ForgeUIButton forgeUIButton;
    private MainSlotUIPanel mainSlotUIPanel;
    private ForgePreImagePanel forgePreImagePanel;
    private AutoForgeUIPanel autoForgeUIPanel;
    private ForgeProbabilityUIPanel forgeProbabilityUIPanel;
    private SlotEquipmentForgeUIPopup slotEquipmentForgeUIPopup;
    private ForgeWarningPanel forgeWarningPanel;
    private SlotDecisionUIPanel slotDecisionUIPanel;

    private SlotStatRoller slotStatRoller;
    private SlotStatCalculator slotStatCalculator;
    private RewardMovingController rewardMovingController;

    private bool isNewSlotSold = true;

    private const int SELLING_MOD_VALUE = 10;

    private EquipmentType[] equipmentTypes = (EquipmentType[])Enum.GetValues(typeof(EquipmentType));

    private SlotUIData newSlotUIData;

    private List<KeyValuePair<StatType, BigInteger>> currentStats = new List<KeyValuePair<StatType, BigInteger>>()
    {
        new KeyValuePair<StatType, BigInteger>(StatType.Damage, 0),
        new KeyValuePair<StatType, BigInteger>(StatType.HP, 0),
        new KeyValuePair<StatType, BigInteger>(StatType.Defense, 0),
    };

    public event Action<Dictionary<int, float>> OnAddAttributes;
    public event Action<Dictionary<int, float>> OnRemoveAttributes;
    public event Action<BigInteger> OnIncreaseExp;
    public event Action<Rank> OnUpdateRtanArrow;
    public event Func<int> OnGetUserLevel;

    private bool isSuperior;
    private bool isInferior;
    private bool isAutoAssemble;

    private Dictionary<EquipmentType, SlotEquipmentStatData> slotEquipmentStatDataDict = new Dictionary<EquipmentType, SlotEquipmentStatData>();

    private bool isDataLoaded;

    private ForgeLevelDataHandler castleLevelDataHandler;

    public void Init()
    {
        uiManager = UIManager.instance;
        slotEquipmentForgeUIPopup = uiManager.GetUIElement<SlotEquipmentForgeUIPopup>();
        newSlotUIPanel = uiManager.GetUIElement<NewSlotUIPanel>();
        currentSlotUIPanel = uiManager.GetUIElement<CurrentSlotUIPanel>();
        slotDetailPanel = uiManager.GetUIElement<SlotDetailPanel>();
        forgeUIButton = uiManager.GetUIElement<ForgeUIButton>();
        mainSlotUIPanel = uiManager.GetUIElement<MainSlotUIPanel>();
        autoForgeUIPanel = uiManager.GetUIElement<AutoForgeUIPanel>();
        forgePreImagePanel = uiManager.GetUIElement<ForgePreImagePanel>();
        forgeProbabilityUIPanel = uiManager.GetUIElement<ForgeProbabilityUIPanel>();
        forgeWarningPanel = uiManager.GetUIElement<ForgeWarningPanel>();
        slotDecisionUIPanel = uiManager.GetUIElement<SlotDecisionUIPanel>();
        slotUIDataHandler = new SlotUIDataHandler(newSlotUIPanel, currentSlotUIPanel, slotDetailPanel);
        slotStatRoller = new SlotStatRoller(slotEquipmentForger, attributeDataSO.GetAttributeData);
        rewardMovingController = FindAnyObjectByType<RewardMovingController>();
        forgePreImagePanel.OnClickDisableButton += autoForgeUIPanel.StopAuto;
        forgeUIButton.OnForgeSlot += ForgeSlotEquipment;
        slotEquipmentForger.OnGetRandomRank += castleDoorRankProbabilityDataSO.GetRandomRank;
        forgeUIButton.OnUpdateTriggerDuration += slotEquipmentForger.UpdateForgeAnimationDuration;
        newSlotUIPanel.OnActivateCurrentSlotPanel += currentSlotUIPanel.SetActive;
        currentSlotUIPanel.OnGetSlotDefaultRankSprite += mainSlotUIPanel.GetSlotDefaultRankSprite;
        slotDecisionUIPanel.OnClickSellingButton += forgePreImagePanel.OnSlotSold;
        slotDecisionUIPanel.OnClickSellingButton += () => SellNewSlot(false);
        slotDecisionUIPanel.OnClickSellingButton += () => forgeUIButton.EndForge(true);
        slotEquipmentForgeUIPopup.OnSellNewSlotDelay += SellNewSlot;

        newSlotUIPanel.Init();

        slotStatCalculator = new SlotStatCalculator(slotStatDataSO.GetEquipmentData, levelVariation);

        castleLevelDataHandler = new ForgeLevelDataHandler(slotEquipmentForger, forgeProbabilityUIPanel,
            castleDoorRankProbabilityDataSO.GetCastleDoorRankProbabilityData, castleDoorRankProbabilityDataSO.GetCastleDataCount,
            autoForgeUIPanel);

        QuestManager questManager = QuestManager.instance;
        questManager.AddEventQuestTypeAction(EventQuestType.CastleDoorUpgradeButtonTouch,
            () => questManager.UpdateCount(EventQuestType.CastleDoorUpgradeButtonTouch, 0, -1));
    }

    public void StartInit()
    {
        forgeProbabilityUIPanel.Init();
        slotEquipmentForgeUIPopup.Init();
        LoadIsNewSlotSold();

        SlotUIData preNewSlotUIData = default;
        bool preIsNewSlotSold = isNewSlotSold;

        for (int i = 1; i < equipmentTypes.Length; i++)
        {
            if (!preIsNewSlotSold)
            {
                if (ES3.KeyExists($"{equipmentTypes[i]}{Consts.SLOT_NICKNAME}{false}", ES3.settings))
                {
                    preNewSlotUIData.equipmentType = equipmentTypes[i];
                    preNewSlotUIData.LoadDatas(false);
                    preNewSlotUIData.SetSlotData(preNewSlotUIData);
                    //SetCurrentSlotUIData(slotUIData);
                }
            }

            if (ES3.KeyExists($"{equipmentTypes[i]}{Consts.SLOT_NICKNAME}{true}", ES3.settings))
            {
                SlotUIData slotUIData = default;
                slotUIData.equipmentType = equipmentTypes[i];
                slotUIData.LoadDatas(true);
                slotUIData.SetSlotData(slotUIData);
                SetNewSlotUIData(slotUIData);
                newSlotUIData = slotUIData;
                SwapSlotPanels();
            }
        }


        if (preNewSlotUIData.equipmentType != EquipmentType.None)
        {
            isNewSlotSold = preIsNewSlotSold;
            SaveIsNewSlotSold(isNewSlotSold);
            newSlotUIData = preNewSlotUIData;
            slotUIDataHandler.SetNewSlotUIData(newSlotUIData);
            newSlotUIData.SaveDatas(false);
            SlotUIData currentSlotUIData = slotUIDataHandler.GetCurrentSlotUIData(newSlotUIData.equipmentType);
            newSlotUIPanel.UpdateSlotStatsUI(newSlotUIData, currentSlotUIData);
            if (currentSlotUIData.equipmentType != EquipmentType.None)
            {
                currentSlotUIPanel.UpdateCurrentStatsText(currentSlotUIData);
            }
            ShowPreImageRightAway();
        }

        EquipmentType arrowType = EquipmentType.Arrow;
        if (!ES3.KeyExists($"{arrowType}{Consts.SLOT_NICKNAME}{false}", ES3.settings) &&
            !ES3.KeyExists($"{arrowType}{Consts.SLOT_NICKNAME}{true}", ES3.settings))
        {
            ForgeEquipmentInfo forgeEquipmentInfo = new ForgeEquipmentInfo(arrowType, Rank.Common);

            SlotStatData slotStatData = slotStatCalculator.GetSlotStatDataSolidUpdated(Rank.Common, 1);
            Sprite mainSprite = ResourceManager.instance.forgeEquipmentResourceDataHandler.GetForgeEquipmentSprite(forgeEquipmentInfo);

            ForgeEquipmentData forgeEquipmentData = forgeEquipmentDataSO.GetForgeEquipmentData(forgeEquipmentInfo);

            UpdateSlotUIPanel(forgeEquipmentData.equipmentNameKR, arrowType, slotStatData);
            SwapSlotPanels();
        }

        isDataLoaded = true;
    }


    public void ForgeSlotEquipment()
    {
        if (!GetIsNewSlotSold())
        {
            slotEquipmentForgeUIPopup.ActivateSelf(!autoForgeUIPanel.GetIsAutoStopped());
            return;
        }

        ForgeEquipment();
    }

    public void SetCurrentSlotUIData(SlotUIData slotUIData)
    {
        slotUIDataHandler.SetCurrentSlotUIData(slotUIData);
    }

    public void SetNewSlotUIData(SlotUIData slotUIData)
    {
        slotUIDataHandler.SetNewSlotUIData(slotUIData);
    }

    private void ForgeEquipment()
    {
        int level = OnGetUserLevel.Invoke();

        /*Rank rank = Rank.Magic;
        EquipmentType equipmentType = EquipmentType.Arrow;*/

        int minLevel = level - levelVariation < 1 ? 1 : level - levelVariation;
        int maxlevel = level + levelVariation + 1;
        int randomLevel = UnityEngine.Random.Range(minLevel, maxlevel);

        EquipmentType equipmentType = slotStatRoller.GetRandomEquipmentType();
        bool isEquipmentTypeSummoned = slotStatRoller.GetIsEquipmentTypeSummoned(equipmentType);

        int forgeLevel = forgeProbabilityUIPanel.GetCastleDoorProbailityLevel();

        Rank rank = castleDoorRankProbabilityDataSO.GetRandomRank(forgeLevel, isEquipmentTypeSummoned);

        ForgeEquipmentInfo forgeEquipmentInfo = new ForgeEquipmentInfo(equipmentType, rank);

        SlotStatData slotStatData = slotStatCalculator.GetSlotStatDataUpdated(rank, randomLevel);
        Sprite mainSprite = ResourceManager.instance.forgeEquipmentResourceDataHandler.GetForgeEquipmentSprite(forgeEquipmentInfo);

        CurrencyManager.instance.TryUpdateCurrency(CurrencyType.ForgeTicket, -1);

        ForgeEquipmentData forgeEquipmentData = forgeEquipmentDataSO.GetForgeEquipmentData(forgeEquipmentInfo);

        UpdateSlotUIPanel(forgeEquipmentData.equipmentNameKR, equipmentType, slotStatData);
        SetIsNewSlotSold(false, equipmentType);

        DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.Forge_Equipment, 1);

        slotEquipmentForger.StartForgeEffect(forgeEquipmentData, slotStatData, mainSprite);
    }

    public void ShowPreImageRightAway()
    {
        ForgeEquipmentData forgeEquipmentData = forgeEquipmentDataSO.GetForgeEquipmentData(new ForgeEquipmentInfo(
            newSlotUIData.equipmentType, newSlotUIData.slotStatData.rank));
        slotEquipmentForger.ShowForgePreImageRightAway(forgeEquipmentData, newSlotUIData.mainSprite);
    }

    public bool GetIsNewSlotSold()
    {
        return isNewSlotSold;
    }

    private void SetIsNewSlotSold(bool isNewSlotSold, EquipmentType equipmentType)
    {
        this.isNewSlotSold = isNewSlotSold;
        SaveIsNewSlotSold(isNewSlotSold);

        slotUIDataHandler.GetCurrentSlotUIData(equipmentType).SaveDatas(true);
        if (!isNewSlotSold)
        {
            slotUIDataHandler.GetNewSlotUIData(equipmentType).SaveDatas(false);
        }
        else
        {
            SlotUIData slotUIData = slotUIDataHandler.GetCurrentSlotUIData(equipmentType);
            slotUIData.equipmentType = EquipmentType.None;
            slotUIData.SaveDatas(false);
            //slotUIData.DeleteNewSlotData();
        }
    }

    private void SaveIsNewSlotSold(bool isNewSlotSold)
    {
        ES3.Save(Consts.IS_NEW_EQUIPMENT_SLOT_SOLD, isNewSlotSold, ES3.settings);
        ES3.StoreCachedFile();
    }

    private void LoadIsNewSlotSold()
    {
        isNewSlotSold = ES3.Load(Consts.IS_NEW_EQUIPMENT_SLOT_SOLD, true, ES3.settings);
    }

    public void UpdateSlotUIPanel(string nickName, EquipmentType equipmentType, SlotStatData slotStatData)
    {
        SlotUIData currentSlotUIData = slotUIDataHandler.GetCurrentSlotUIData(equipmentType);

        SlotUIData tempSlotUIData = default;
        tempSlotUIData.nickName = nickName;
        tempSlotUIData.slotStatData = slotStatData;
        tempSlotUIData.equipmentType = equipmentType;
        tempSlotUIData.slotStatData.rank = slotStatData.rank;
        newSlotUIData = tempSlotUIData;
        newSlotUIData.SetSlotData(tempSlotUIData);

        slotUIDataHandler.SetNewSlotUIData(newSlotUIData);

        newSlotUIPanel.UpdateSlotStatsUI(newSlotUIData, currentSlotUIData);

        newSlotUIData.SaveDatas(false);

        SetIsNewSlotSold(false, equipmentType);

        if (currentSlotUIData.equipmentType != EquipmentType.None)
        {
            currentSlotUIPanel.UpdateCurrentStatsText(currentSlotUIData);
        }
    }

    public void SwapSlotPanels()
    {
        EquipmentType equipmentType = this.newSlotUIData.equipmentType;

        SlotUIData newSlotUIData = slotUIDataHandler.GetNewSlotUIData(equipmentType);
        SlotUIData currentSlotUIData = slotUIDataHandler.GetCurrentSlotUIData(equipmentType);

        slotUIDataHandler.SwapSlotPanels(newSlotUIData);

        currentSlotUIData = slotUIDataHandler.GetCurrentSlotUIData(equipmentType);
        newSlotUIData = slotUIDataHandler.GetNewSlotUIData(equipmentType);

        ForgeEquipmentInfo slotInfo = new ForgeEquipmentInfo(currentSlotUIData.equipmentType, currentSlotUIData.slotStatData.rank);

        if (!slotEquipmentStatDataDict.ContainsKey(equipmentType))
        {
            slotEquipmentStatDataDict.Add(equipmentType, new SlotEquipmentStatData(0, 0, 0, default));
        }

        SlotEquipmentStatData currentSlotEquipmentStatData = new SlotEquipmentStatData(
            currentSlotUIData.slotStatData.equipmentStatData.mainDamage, currentSlotUIData.slotStatData.equipmentStatData.health,
            currentSlotUIData.slotStatData.equipmentStatData.defense, new SlotEquipmentStatDataSave());

        currentStats[(int)StatType.Damage - 1] = new KeyValuePair<StatType, BigInteger>(StatType.Damage, currentSlotEquipmentStatData.mainDamage - slotEquipmentStatDataDict[equipmentType].mainDamage);
        currentStats[(int)StatType.HP - 1] = new KeyValuePair<StatType, BigInteger>(StatType.HP, currentSlotEquipmentStatData.health - slotEquipmentStatDataDict[equipmentType].health);
        currentStats[(int)StatType.Defense - 1] = new KeyValuePair<StatType, BigInteger>(StatType.Defense, currentSlotEquipmentStatData.defense - slotEquipmentStatDataDict[equipmentType].defense);

        StatDataHandler.Instance.ModifyStats(ArithmeticStatType.Base, AdditionalStatType.Equipment, currentStats, true,
            true & isDataLoaded);

        slotEquipmentStatDataDict[equipmentType] = currentSlotEquipmentStatData;

        mainSlotUIPanel.UpdateMainSlotUI(currentSlotUIData.equipmentType, currentSlotUIData.rankSprite, currentSlotUIData.mainSprite,
            currentSlotUIData.slotStatData.level);

        OnRemoveAttributes?.Invoke(newSlotUIData.attributeStatDict);
        OnAddAttributes?.Invoke(currentSlotUIData.attributeStatDict);

        if (currentSlotUIData.equipmentType == EquipmentType.Arrow)
        {
            OnUpdateRtanArrow?.Invoke(currentSlotUIData.slotStatData.rank);
        }

        if (newSlotUIData.slotStatData.level != 0)
        {
            if (isAutoAssemble)
            {
                TrySellNewSlot();
                newSlotUIPanel.UpdateSlotStatsUI(newSlotUIData, currentSlotUIData);
                currentSlotUIPanel.UpdateCurrentStatsText(slotUIDataHandler.GetCurrentSlotUIData(newSlotUIData.equipmentType));
                return;
            }

            newSlotUIPanel.UpdateSlotStatsUI(newSlotUIData, currentSlotUIData);
            currentSlotUIPanel.UpdateCurrentStatsText(slotUIDataHandler.GetCurrentSlotUIData(newSlotUIData.equipmentType));
            SetIsNewSlotSold(false, equipmentType);
            return;
        }
        else
        {
            SetIsNewSlotSold(true, equipmentType);
            forgePreImagePanel.OnSlotSold();
            slotEquipmentForgeUIPopup.DeActivateSelf(true);
            slotStatRoller.AddSlotType(newSlotUIData.equipmentType);
            newSlotUIData.DeleteNewSlotData();
            if (!autoForgeUIPanel.GetIsAutoStopped())
            {
                autoForgeUIPanel.TryActivateAutoForge();
            }
            return;
        }

        /*if (SlotTypeSavedDic.ContainsKey(equipmentType) && newSlotUIData.mainSprite == null)
        {
            SetIsNewSlotSold(true, equipmentType);
            forgePreImagePanel.OnSlotSold();
        }*/
    }

    public void SellNewSlot(bool isFromWarningPopup)
    {
        if (isSuperior && !isFromWarningPopup)
        {
            forgeWarningPanel.ActivateParentPopup(true);
            return;
        }

        OnIncreaseExp?.Invoke(newSlotUIData.slotStatData.exp);

        CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gold, newSlotUIData.slotStatData.gold);

        SetIsNewSlotSold(true, newSlotUIData.equipmentType);
        newSlotUIData.DeleteNewSlotData();
        SellingReward();
        Selling();
    }

    private void TrySellNewSlot()
    {
        if (isInferior)
        {
            forgeWarningPanel.ActivateParentPopup(true);
            return;
        }

        OnIncreaseExp?.Invoke(newSlotUIData.slotStatData.exp);

        CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gold, newSlotUIData.slotStatData.gold);

        SetIsNewSlotSold(true, newSlotUIData.equipmentType);
        newSlotUIData.DeleteNewSlotData();
        SellingReward();
        Selling();
    }

    private void SellNewSlot(EquipmentType equipmentType, int level, BigInteger originExp, BigInteger originGold)
    {
        OnIncreaseExp?.Invoke(originExp);

        CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gold, originGold);

        SetIsNewSlotSold(true, equipmentType);
        SellingReward();
        Selling();
    }

    private void SellingReward()
    {
        rewardMovingController.RequestForgeRewardMovingCurrency(3, RewardType.Gold);
        rewardMovingController.RequestForgeRewardMovingCurrency(3, RewardType.Exp);

        QuestManager.instance.UpdateCount(EventQuestType.ForgeSummonSaleCount, 1, -1);
    }

    public void UpdateIsInperior(bool isInferior)
    {
        this.isInferior = isInferior;
    }

    public void UpdateIsSuperior(bool isSuperior)
    {
        this.isSuperior = isSuperior;
    }

    public void UpdateAutoDisassembleState(bool isActive)
    {
        isAutoAssemble = isActive;
    }

    public void Selling()
    {
        //DialogManager.instance.ShowDialog(DialogueType.LevelUpColleague);
        forgePreImagePanel.OnSlotSold();
        slotEquipmentForgeUIPopup.DeActivateSelf(true);

        // DialogManager.instance.HideDialog();

        // DialogManager.instance.ShowDialog(ResourceManager.instance.expPanel.transform, DialogueType.LevelUpHero);
    }

    private BigInteger GetSellingValue(BigInteger origin, int level)
    {
        return origin + origin * level * SELLING_MOD_VALUE / Consts.PERCENT_DIVIDE_VALUE; // 0.1
    }


#if UNITY_EDITOR
    public void EditorForgeSlotEquipment(Rank rank, ColleagueType slotType)
    {
        if (!GetIsNewSlotSold())
        {
            return;
        }

        ForgeEquipment();
    }

#endif
}
