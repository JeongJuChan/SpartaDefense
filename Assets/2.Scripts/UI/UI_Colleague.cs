using Keiwando.BigInteger;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Colleague : UI_BottomElement
{
    [SerializeField] private GuideController guideController;
    [SerializeField] private ColleagueDataSO colleagueDataSO;
    [SerializeField] private Image stageBackgroundSprite;
    [SerializeField] private ColleagueBottomUIElement[] colleagueBottomUIElements;
    [SerializeField] private ColleagueTopUIElement[] colleagueTopUIElements;

    [SerializeField] private Sprite addSprite;
    [SerializeField] private Sprite subtractSprite;

    [SerializeField] private ColleagueUIPopup colleagueUIPopup;

    [SerializeField] private Button autoEquipButton;

    public event Action<int, int, bool> OnEquipColleague;
    public event Action<int, int> OnColleagueAdded;
    public event Action<int> OnUnEquipColleagueSlot;
    public event Action<int> OnUnEquipColleague;
    public event Action<int, int> OnUnEquipColleagueUI;
    public event Action<ColleagueType, int> OnUpdateColleagueLevel;

    public event Action<int> OnOpenColleagueIndexSlot;

    private Func<int> OnGetMainStage;
    private Dictionary<int, ColleagueBottomUIElement> colleagueBottomUIElementDict = new Dictionary<int, ColleagueBottomUIElement>();
    private Dictionary<int, ColleagueTopUIElement> colleagueTopUIElementDict = new Dictionary<int, ColleagueTopUIElement>();

    private ColleagueManager colleagueManager;
    private CastleClan castleClan;

    private int openSlotIndex = 0;

    private bool[] colleagueEquipStates;

    [SerializeField] private int colleagueEquipStateLength;

    private List<int> equippableColleagueIndexes = new List<int>();

    private SkillManager skillManager;

    private UI_Castle uiCastle;

    public override void Initialize()
    {
        base.Initialize();

        AllocateColleagueManager();

        castleClan = FindAnyObjectByType<CastleClan>();

        colleagueManager.OnUpdateColleagueStats += castleClan.UpdateColleagueStatDict;

        colleagueManager.OnLevelUpEnded += colleagueUIPopup.UpdateDefaultStatTexts;
        colleagueManager.OnLevelUpEnded += colleagueUIPopup.UpdateLevelUpCost;
        colleagueManager.OnLevelUpEnded += colleagueUIPopup.UpdateLevelText;
        colleagueManager.OnUpdateStarUI += colleagueUIPopup.UpdateStarCount;

        colleagueManager.OnLevelUpEnded += UpdateElementsLevelUpUI;

        colleagueManager.OnUpdateColleagueAdvanceText += colleagueUIPopup.UpdateAdvancementPartCount;

        colleagueManager.OnUpdateColleagueAdvanceText += UpdateBottomElementAdvanceUI;
        colleagueManager.OnUpdateStarUIForBottomUIElement += UpdateStarCountOfBottomUIElement;

        colleagueUIPopup.OnGetAdvanceCost += colleagueManager.GetAdvanceCost;

        colleagueUIPopup.ActivateSelf(false);
        colleagueUIPopup.Init();
        colleagueUIPopup.OnGetIsAdvanceButtonInteractable += colleagueManager.GetIsColleagueAdvanceable;
        colleagueUIPopup.OnGetIsLevelUpButtonInteractable += colleagueManager.GetIsColleagueLevelUpPossibleState;

        colleagueUIPopup.OnClickAdvanceButton += colleagueManager.AdvanceColleague;
        colleagueUIPopup.OnClickLevelUpButton += colleagueManager.LevelUpColleague;

        colleagueUIPopup.OnGetTotalLevelUpCost += colleagueManager.GetTotalLevelUpCost;

        colleagueUIPopup.OnGetMaxLevel += colleagueManager.GetMaxLevel;

        skillManager = FindAnyObjectByType<SkillManager>();

        colleagueUIPopup.OnGetSkillUpgradableData += skillManager.GetSkillUpgradableData;

        autoEquipButton.onClick.AddListener(EquipColleagueAuto);

        colleagueEquipStates = new bool[colleagueEquipStateLength];

        List<UnlockData> unlockDatas = ResourceManager.instance.unlockDataSO.GetColleagueUnlockDatas();

        colleagueUIPopup.OnGetColleagueData += GetIsColleagueEquipped;
        colleagueUIPopup.OnGetColleagueUpgradableData += colleagueManager.GetColleagueUpgradableData;

        openSlotIndex = ES3.Load<int>(Consts.COLLEAGUE_OPEN_SLOT_INDEX, openSlotIndex, ES3.settings);

        UnlockColleagueSlot();

        for (int i = openSlotIndex; i < unlockDatas.Count; i++)
        {
            int currentCount = i;
            UnlockManager.Instance.RegisterFeature(new UnlockableFeature(unlockDatas[currentCount].featureType,
                unlockDatas[currentCount].featureID, unlockDatas[currentCount].count, () => UnlockColleagueSlot(currentCount + 1)));
        }

        Sprite normalStar = ResourceManager.instance.colleagueIconDataSO.GetNormalStar();
        Sprite advancedStar = ResourceManager.instance.colleagueIconDataSO.GetAdvancedStar();

        colleagueUIPopup.InitStars(normalStar, advancedStar);

        foreach (ColleagueBottomUIElement element in colleagueBottomUIElements)
        {
            element.Init();
            element.InitStars(normalStar, advancedStar);
        }

        List<ColleagueData> colleagueDatas = new List<ColleagueData>(colleagueDataSO.GetColleagueDataAll());
        for (int i = 0; i < colleagueDatas.Count; i++)
        {
            ColleagueData colleagueData = colleagueDatas[i];

            ColleagueUIData colleagueUIData = new ColleagueUIData(colleagueData.index, colleagueData.skillIndex,
                colleagueData.colleagueInfo.colleagueType, colleagueData.colleagueInfo.rank, colleagueData.nickName,
                EnumToKRManager.instance.GetEnumToKR(colleagueData.colleagueInfo.colleagueType),
                ResourceManager.instance.slotHeroData.GetResource(colleagueData.colleagueInfo).defaultSprite,
                ResourceManager.instance.rank.GetRankBackgroundSprite(colleagueData.colleagueInfo.rank), addSprite);

            UnityAction equipAction = () => EquipSlotLeft(colleagueUIData);
            UnityAction unEquipAction = () => UnEquipSlot(colleagueUIData.index);

            colleagueBottomUIElements[i].Init();
            colleagueBottomUIElements[i].OnOpenColleagueUIPopup += colleagueUIPopup.UpdateColleageUIPopup;

            colleagueBottomUIElements[i].OnEquip += equipAction;
            colleagueBottomUIElements[i].OnUnEquip += unEquipAction;

            colleagueBottomUIElements[i].UpdateColleagueUIData(colleagueUIData);



            if (!colleagueBottomUIElementDict.ContainsKey(colleagueData.index))
            {
                colleagueBottomUIElementDict.Add(colleagueData.index, colleagueBottomUIElements[i]);
            }
        }

        foreach (ColleagueTopUIElement element in colleagueTopUIElements)
        {
            element.Init();
            element.SetSubtractSprite(subtractSprite);
            element.OnOpenColleagueUIPopup += colleagueUIPopup.UpdateColleageUIPopup;
            element.UpdateUnEquipState(() => UnEquipSlot(element));
        }

        colleagueManager.OnUpdateColleagueUI += UpdateColleaguesSummoned;
        OnGetMainStage = FindAnyObjectByType<StageController>().GetCurrentMainStage;
        guideController.Initialize();
        
        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.EquipColleagueAuto, 
            () => { 
                QuestManager.instance.UpdateCount(EventQuestType.EquipColleagueAuto, PlayerPrefs.HasKey("EquipColleagueAuto") ? 1 : 0, -1); 
            });


        if (GetIsColleagueAutoEquippable())
        {
            autoEquipButton.interactable = false;
            NotificationManager.instance.SetNotification(RedDotIDType.AutoEquipColleagueButton, true);
        }

    }

    private void OnEnable()
    {
        colleagueUIPopup.ActivateSelf(false);
    }

    private void LoadDatas()
    {
        UnlockManager.Instance.CheckUnlocks(FeatureType.Quest);

        List<ColleagueData> colleagueDatas = new List<ColleagueData>(colleagueDataSO.GetColleagueDataAll());

        for (int i = 0; i < colleagueDatas.Count; i++)
        {
            int index = colleagueDatas[i].index;

            if (!ES3.KeyExists($"{index}_{Consts.COLLEAGUE_UPGRADABLE_DATA}", ES3.settings))
            {
                continue;
            }

            ColleagueUpgradableData tempColleagueUpgradableData = LoadColleagueUpgradableData(index);

            if (tempColleagueUpgradableData.count != 0)
            {
                colleagueManager.OnColleagueSummoned(tempColleagueUpgradableData);
            }
        }

        for (int j = 0; j <= openSlotIndex; j++)
        {
            if (!ES3.KeyExists($"{j}_{Consts.COLLEAGUE_EQUIP_INDEX}", ES3.settings))
            {
                continue;
            }

            int tempIndex = ES3.Load<int>($"{j}_{Consts.COLLEAGUE_EQUIP_INDEX}", ES3.settings);

            if (tempIndex == -1)
            {
                continue;
            }

            EquipColleague(colleagueBottomUIElementDict[tempIndex].GetColleagueUIData(), j, j == openSlotIndex);
        }

        ActivateAutoEquipButton();

        foreach (ColleagueBottomUIElement colleagueBottomUIElement in colleagueBottomUIElementDict.Values)
        {
            if (colleagueBottomUIElement.GetReddotEnabled())
            {
                NotificationManager.instance.SetNotification(RedDotIDType.ShowColleagueButton, true);
                break;
            }
        }

        StatDataHandler.Instance.SetColleagueLoaded();
    }

    public void UpdatePowerText(BigInteger power)
    {
        colleagueUIPopup.UpdatePowerText(power);
    }

    public void SummonColleague(int index, int count, bool isLastColleague)
    {
        colleagueManager.OnColleagueSummoned(index, count, isLastColleague);
    }

    private ColleagueUpgradableData LoadColleagueUpgradableData(int index)
    {
        ColleagueUpgradableData tempColleagueUpgradableData =
            ES3.Load<ColleagueUpgradableData>($"{index}_{Consts.COLLEAGUE_UPGRADABLE_DATA}", ES3.settings);

        BigInteger count = new BigInteger(ES3.Load<string>($"{index}_{Consts.COLLEAGUE_UPGRADABLE_COUNT}", ES3.settings));
        BigInteger damage = new BigInteger(ES3.Load<string>($"{index}_{Consts.COLLEAGUE_UPGRADABLE_DAMAGE}", ES3.settings));
        BigInteger health = new BigInteger(ES3.Load<string>($"{index}_{Consts.COLLEAGUE_UPGRADABLE_HEALTH}", ES3.settings));
        BigInteger defense = new BigInteger(ES3.Load<string>($"{index}_{Consts.COLLEAGUE_UPGRADABLE_DEFENSE}", ES3.settings));
        BigInteger power = new BigInteger(ES3.Load<string>($"{index}_{Consts.COLLEAGUE_UPGRADABLE_POWER}", ES3.settings));

        tempColleagueUpgradableData.count = count;
        tempColleagueUpgradableData.damage = damage;
        tempColleagueUpgradableData.health = health;
        tempColleagueUpgradableData.defense = defense;
        tempColleagueUpgradableData.power = power;
        return tempColleagueUpgradableData;
    }

    public BigInteger GetPower(ColleagueType colleagueType)
    {
        int index = colleagueDataSO.GetIndexByColleagueType(colleagueType);
        return colleagueManager.GetColleaguePower(index);
    }

    public BigInteger GetTotalPower()
    {
        BigInteger totalPower = 0;
        AllocateColleagueManager();

        foreach (ColleagueUpgradableData colleagueUpgradableData in colleagueManager.GetColleagueUpgradableDataDict().Values)
        {
            if (!colleagueTopUIElementDict.ContainsKey(colleagueUpgradableData.index))
            {
                continue;
            }

            if (colleagueUpgradableData.power != null)
            {
                totalPower += colleagueUpgradableData.power;
            }
        }
        return totalPower;

    }

    private void UpdateStarCountOfBottomUIElement(int index, ColleagueUpgradableData colleagueUpgradableData, int starUnitCount)
    {
        if (!colleagueBottomUIElementDict.ContainsKey(index))
        {
            return;
        }

        colleagueBottomUIElementDict[index].UpdateStarCount(colleagueUpgradableData, starUnitCount);
    }

    private void AllocateColleagueManager()
    {
        if (colleagueManager == null)
        {
            colleagueManager = FindAnyObjectByType<ColleagueManager>();
        }
    }

    public void LateInit()
    {
        if (!ES3.KeyExists($"0_{Consts.IS_COLLEAGUE_HAVE_BEEN_INSTANTIATED}", ES3.settings))
        {
            colleagueManager.OnColleagueSummoned(0, 1, false);
            EquipColleague(colleagueBottomUIElementDict[0].GetColleagueUIData(), 0, false);
            ES3.Save<bool>($"0_{Consts.IS_COLLEAGUE_HAVE_BEEN_INSTANTIATED}", true, ES3.settings);
            ES3.StoreCachedFile();
            StatDataHandler.Instance.SetColleagueLoaded();
        }
        else
        {
            LoadDatas();
        }
    }

    public void UpdateColleaguePopup(int index)
    {
        colleagueUIPopup.UpdateColleageUIPopup(colleagueBottomUIElementDict[index].GetColleagueUIData());
    }

    private void UpdateColleaguesSummoned(int index, ColleagueUpgradableData colleagueUpgradableData, bool isLastColleague)
    {
        if (colleagueBottomUIElementDict.ContainsKey(index))
        {
            //colleagueBottomUIElementDict[index].OnClickEquipButton += EquipSlotLeft;
            colleagueBottomUIElementDict[index].SetSubtractSprite(subtractSprite);
            colleagueBottomUIElementDict[index].SetAddSprite(addSprite);
            int advanceCost = colleagueManager.GetAdvanceCost(index, colleagueUpgradableData);
            colleagueBottomUIElementDict[index].UpdateSummonAmount(colleagueUpgradableData, advanceCost);
            if (!equippableColleagueIndexes.Contains(index) && index != 0)
            {
                equippableColleagueIndexes.Add(index);
            }
        }

        if (isLastColleague)
        {
            ActivateAutoEquipButton();
        }
    }

    private void EquipSlotLeft(ColleagueUIData colleagueUIData)
    {
        if (colleagueTopUIElementDict.ContainsKey(colleagueUIData.index))
        {
            return;
        }

        for (int i = 1; i <= openSlotIndex; i++)
        {
            if (colleagueEquipStates[i])
            {
                EquipColleague(colleagueUIData, i, true);
                ActivateAutoEquipButton();
                return;
            }
        }
    }

    private void UpdateBottomElementAdvanceUI(ColleagueUpgradableData colleagueUpgradableData, int cost)
    {
        int index = colleagueUpgradableData.index;

        if (!colleagueBottomUIElementDict.ContainsKey(index))
        {
            return;
        }

        colleagueBottomUIElementDict[index].UpdateAdvanceUI(colleagueUpgradableData.count, cost);
        NotificationManager.instance.SetNotification(RedDotIDType.ShowColleagueButton, colleagueBottomUIElementDict[index].GetReddotEnabled());
    }

    private void UpdateElementsLevelUpUI(ColleagueUpgradableData colleagueUpgradableData)
    {
        int index = colleagueUpgradableData.index;
        int level = colleagueUpgradableData.level;
        if (colleagueTopUIElementDict.ContainsKey(index))
        {
            colleagueTopUIElementDict[index].UpdateLevelText(level);
        }

        colleagueBottomUIElementDict[index].UpdateLevelText(level);
    }

    private void ActivateAutoEquipButton()
    {
        bool isColleagueAutoEquippable = GetIsColleagueAutoEquippable();
        autoEquipButton.interactable = isColleagueAutoEquippable;
        NotificationManager.instance.SetNotification(RedDotIDType.AutoEquipColleagueButton, isColleagueAutoEquippable);

    }

    private void EquipColleague(ColleagueUIData colleagueUIData, int slotIndex, bool isLastColleague)
    {
        int index = colleagueUIData.index;
        colleagueEquipStates[slotIndex] = false;
        colleagueBottomUIElementDict[index].Equip();
        ColleagueUpgradableData colleagueUpgradableData = colleagueManager.GetColleagueUpgradableData(index);
        colleagueTopUIElements[slotIndex].UpdateColleagueUIData(colleagueUIData);
        colleagueTopUIElements[slotIndex].UpdateLevelText(colleagueUpgradableData.level);
        if (!colleagueTopUIElementDict.ContainsKey(index))
        {
            colleagueTopUIElementDict.Add(index, colleagueTopUIElements[slotIndex]);
        }

        ES3.Save<int>($"{slotIndex}_{Consts.COLLEAGUE_EQUIP_INDEX}", index, ES3.settings);
        ES3.StoreCachedFile();

        OnEquipColleague?.Invoke(slotIndex, index, isLastColleague);
        OnColleagueAdded?.Invoke(slotIndex, index);
    }

    private void UnEquipSlot(ColleagueTopUIElement element)
    {
        UnEquipSlot(element.GetIndex());
    }

    private void UnEquipSlot(int index)
    {
        colleagueTopUIElementDict[index].ResetColleagueUI();
        colleagueBottomUIElementDict[index].UnEquip();
        for (int i = 1; i < colleagueTopUIElements.Length; i++)
        {
            if (colleagueTopUIElements[i] == colleagueTopUIElementDict[index])
            {
                colleagueEquipStates[i] = true;
                colleagueTopUIElementDict.Remove(index);
                OnUnEquipColleagueSlot?.Invoke(i);
                OnUnEquipColleague?.Invoke(index);
                ActivateAutoEquipButton();
                ES3.Save<int>($"{i}_{Consts.COLLEAGUE_EQUIP_INDEX}", -1, ES3.settings);
                ES3.StoreCachedFile();
                return;
            }
        }
    }

    private void UnlockColleagueSlot(int slotNum)
    {
        if (openSlotIndex < slotNum)
        {
            for (int i = openSlotIndex + 1; i <= slotNum; i++)
            {
                colleagueTopUIElements[i].ResetColleagueUI();
                colleagueEquipStates[i] = true;
                OnOpenColleagueIndexSlot?.Invoke(i);
            }

            openSlotIndex = slotNum;
            if (uiCastle == null)
            {
                uiCastle = UIManager.instance.GetUIElement<UI_Castle>();
            }

            ES3.Save<int>(Consts.COLLEAGUE_OPEN_SLOT_INDEX, openSlotIndex, ES3.settings);
            ES3.StoreCachedFile();
            uiCastle.UpdateCastleQuestUI();
        }

        ActivateAutoEquipButton();
    }

    private void UnlockColleagueSlot()
    {
        for (int i = 0; i <= openSlotIndex; i++)
        {
            colleagueTopUIElements[i].ResetColleagueUI();
            colleagueEquipStates[i] = true;
            OnOpenColleagueIndexSlot?.Invoke(i);
        }

        ActivateAutoEquipButton();
    }

    private bool GetIsColleagueAutoEquippable()
    {
        SortColleagueIndexesDescended();

        for (int i = 1; i <= equippableColleagueIndexes.Count && i <= openSlotIndex; i++)
        {
            if (!colleagueTopUIElementDict.ContainsKey(equippableColleagueIndexes[i - 1]))
            {
                NotificationManager.instance.SetNotification(RedDotIDType.ShowColleagueButton, true);
                return true;
            }
        }

        NotificationManager.instance.SetNotification(RedDotIDType.ShowColleagueButton, false);
        return false;
    }

    private void EquipColleagueAuto()
    {
        StatDataHandler.Instance.UpdateIsColleagueAutoEquipState(true);
        QuestManager.instance.UpdateCount(EventQuestType.EquipColleagueAuto, 1, -1);

        for (int i = 1; i <= openSlotIndex; i++)
        {
            if (!colleagueEquipStates[i])
            {
                int index = colleagueTopUIElements[i].GetIndex();
                UnEquipSlot(index);
            }
        }

        for (int i = 1; i <= equippableColleagueIndexes.Count && i <= openSlotIndex; i++)
        {
            int index = i - 1;
            bool isLastColleague = index == equippableColleagueIndexes.Count || i == openSlotIndex;
            if (isLastColleague)
            {
                StatDataHandler.Instance.UpdateIsColleagueAutoEquipState(false);
            }

            EquipColleague(colleagueBottomUIElementDict[equippableColleagueIndexes[index]].GetColleagueUIData(), i, isLastColleague);
        }

        ActivateAutoEquipButton();
    }

    private void SortColleagueIndexesDescended()
    {
        equippableColleagueIndexes.Sort((x, y) =>
        {
            BigInteger first = colleagueManager.GetColleaguePower(x);
            BigInteger second = colleagueManager.GetColleaguePower(y);
            return -first.CompareTo(second);
        });
    }

    /*private bool GetIsAutoAdvanceableColleague()
    {
        advanceableColleagueIndexes.Clear();

        foreach (int ownColleagueIndex in ownColleagueIndexes)
        {
            if (colleagueManager.GetIsColleagueEnhanceable(ownColleagueIndex))
            {
                advanceableColleagueIndexes.Add(ownColleagueIndex);
            }
        }

        return advanceableColleagueIndexes.Count > 0;
    }*/

    private bool GetIsColleagueEquipped(int index)
    {
        return colleagueTopUIElementDict.ContainsKey(index);
    }

    public void UpdateEncyclopediaSlots(ColleagueType colleagueType, int level)
    {
        OnUpdateColleagueLevel?.Invoke(colleagueType, level);
    }

    /*private void SaveDatas()
    {
        ES3.Save(Consts.COLLEAGUESLOTINDEX, openSlotIndex, ES3.settings);

        ES3.StoreCachedFile();
    }

    private void LoadDatas()
    {
        if (ES3.KeyExists(Consts.COLLEAGUESLOTINDEX))
        {
            openSlotIndex = ES3.Load<int>(Consts.COLLEAGUESLOTINDEX);
        }
    }*/
}
