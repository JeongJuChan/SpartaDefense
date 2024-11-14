using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EncyclopediaCategory : MonoBehaviour
{
    public event Action<EncyclopediaType, string, bool> LevelUpAbilityEvent;
    public event Action LevelUpEvent;
    [SerializeField] private List<EncyclopediaSlot> encyclopediaSlots;
    private Dictionary<ColleagueType, EncyclopediaSlot> encyclopediaSlotDict = new Dictionary<ColleagueType, EncyclopediaSlot>();
    [SerializeField] private TextMeshProUGUI categoryNameOrLevelText;
    [SerializeField] private TextMeshProUGUI levelUpAbilityText;
    [SerializeField] private Button levelUpButton;
    [SerializeField] private EncyclopediaSlot encyclopediaSlotPrefab;
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject redDot;
    [SerializeField] private TextMeshProUGUI categoryText;

    public string categoryName { get; private set; }
    public bool isLevelUpAvailable { get; private set; }
    public bool initialized { get; private set; } = false;
    public Rank rank { get; private set; }

    public Func<ColleagueType, ColleagueInfo> OnGetColleagueInfoByColleagueType;
    public Func<ColleagueEncyclopediaType, int, ColleagueEncyclopediaIncrementData> OnGetColleagueEncyclopediaIncrementData;

    private int level;
    private int goalLevel;
    private int levelUpAbility;
    private int preLevelUpAbility;
    private int slotCount;
    private StatType statType;
    private ArithmeticStatType arithmeticStatType;
    private EncyclopediaType encyclopediaType;
    private ColleagueEncyclopediaType colleagueEncyclopediaType;
    private bool isMaxLevel;

    public void Init()
    {
        //StatDataHandler.Instance.ModifyStat(arithmeticStatType, AdditionalStatType.Encyclopedia, statType, levelUpAbility, true);
        levelUpButton.onClick.AddListener(LevelUp);

        switch (encyclopediaType)
        {
            case EncyclopediaType.Equipment:
                rank = Enum.Parse<Rank>(categoryName.Split('_')[1]);

                break;
            case EncyclopediaType.Skill:
                rank = Enum.Parse<Rank>(categoryName);
                break;
        }
        initialized = true;
    }

    public void InitColleagueEncyclopediaData(ColleagueEncyclopediaData colleagueEncyclopediaData, int level, int goalLevel,
        Func<ColleagueType, ColleagueInfo> getColleagueInfoByColleagueType, Action<ColleagueType, EncyclopediaSlot> tryAddElement)
    {
        categoryName = colleagueEncyclopediaData.colleagueEncyclopediaType.ToString();
        categoryText.text = colleagueEncyclopediaData.colleagueEncyclopediaNameKR;

        colleagueEncyclopediaType = colleagueEncyclopediaData.colleagueEncyclopediaType;
        this.level = level;
        statType = colleagueEncyclopediaData.statType;
        encyclopediaType = EncyclopediaType.Colleague;

        OnGetColleagueInfoByColleagueType += getColleagueInfoByColleagueType;

        foreach (var item in encyclopediaSlots)
        {
            item.gameObject.SetActive(false);
        }

        encyclopediaSlots = new List<EncyclopediaSlot>(3);

        foreach (ColleagueType colleagueType in colleagueEncyclopediaData.colleagueTypes)
        {
            if (colleagueType != ColleagueType.None)
            {
                EncyclopediaSlot encyclopediaSlot = Instantiate(encyclopediaSlotPrefab, slotParent);
                ColleagueInfo colleagueInfo = OnGetColleagueInfoByColleagueType.Invoke(colleagueType);
                encyclopediaSlot.SetData(colleagueInfo, goalLevel, EncyclopediaType.Colleague, colleagueEncyclopediaType);
                encyclopediaSlots.Add(encyclopediaSlot);
                if (!encyclopediaSlotDict.ContainsKey(colleagueType))
                {
                    encyclopediaSlotDict.Add(colleagueType, encyclopediaSlot);
                }
                tryAddElement?.Invoke(colleagueType, encyclopediaSlot);
            }
        }

        slotCount = encyclopediaSlots.Count;
        StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Encyclopedia, statType,
            levelUpAbility - preLevelUpAbility, true);
        UpdateUI();
    }

    public void SetData(ColleagueEncyclopediaData colleagueEncyclopediaData, int level)
    {
        this.level = level;

        foreach (ColleagueType colleagueType in colleagueEncyclopediaData.colleagueTypes)
        {
            if (colleagueType != ColleagueType.None)
            {
                ColleagueInfo colleagueInfo = OnGetColleagueInfoByColleagueType.Invoke(colleagueType);
                /*encyclopediaSlot.SetData(colleagueInfo, goalLevel, EncyclopediaType.Colleague,
                    colleagueEncyclopediaData.colleagueEncyclopediaType);*/
            }
        }

        UpdateUI();
        SaveData();
    }

    public void SetData(EncyclopediaCategoryData data, EncyclopediaType type)
    {
        // Set data to UI
        categoryName = data.name;
        level = data.level;
        statType = data.statType;
        levelUpAbility = data.levelUpAbility;
        arithmeticStatType = data.arithmeticStatType;
        encyclopediaType = type;


        slotCount = data.slotDatas.Count;

        for (int i = 0; i < encyclopediaSlots.Count; i++)
        {
            encyclopediaSlots[i].Init();

            if (i < data.slotDatas.Count)
            {
                encyclopediaSlots[i].SetData(data.slotDatas[i], level, encyclopediaType);
            }
            else
            {
                encyclopediaSlots[i].gameObject.SetActive(false);
            }
        }

        SetUI();
    }

    public void UpdateLevelUpIncrementStats(ColleagueEncyclopediaIncrementData preColleagueEncyclopediaIncrementData,
        ColleagueEncyclopediaIncrementData colleagueEncyclopediaIncrementData,
        ColleagueEncyclopediaIncrementData nextColleagueEncyclopediaIncrementData)
    {
        preLevelUpAbility = colleagueEncyclopediaIncrementData.increment;
        levelUpAbility = nextColleagueEncyclopediaIncrementData.increment;
        level = nextColleagueEncyclopediaIncrementData.level;
        goalLevel = nextColleagueEncyclopediaIncrementData.goalLevelEachElement;
        UpdateUI();
    }

    public void UpdateUI()
    {
        UpdateCategoryLevelUpText();

        for (int i = 0; i < encyclopediaSlots.Count; i++)
        {
            if (encyclopediaSlots[i].isDataEmpty)
            {
                encyclopediaSlots[i].gameObject.SetActive(false);
            }
            else
            {
                encyclopediaSlots[i].gameObject.SetActive(true);
            }
        }
        CheckLevelUpAvailable();
        ActivateInteractions();
    }

    private void UpdateCategoryLevelUpText()
    {
        categoryNameOrLevelText.text = isMaxLevel ? "최대 레벨입니다." : $"LV.{level} {EnumToKRManager.instance.GetEnumToKR(statType)} {preLevelUpAbility} -> " +
                    $"LV.{level + 1} {EnumToKRManager.instance.GetEnumToKR(statType)} {levelUpAbility}";
    }

    public void SetUI()
    {
        categoryNameOrLevelText.text = $"LV.{level} {EnumToKRManager.instance.GetEnumToKR(statType)} {level * levelUpAbility} -> LV.{level + 1} {EnumToKRManager.instance.GetEnumToKR(statType)} {(level + 1) * levelUpAbility}";

        for (int i = 0; i < encyclopediaSlots.Count; i++)
        {
            if (encyclopediaSlots[i].isDataEmpty)
            {
                encyclopediaSlots[i].gameObject.SetActive(false);
            }
            else
            {
                encyclopediaSlots[i].gameObject.SetActive(true);
            }
        }
        CheckLevelUpAvailable();
        ActivateInteractions();

    }

    private void ActivateInteractions()
    {
        if (isMaxLevel)
        {
            ActivateInteractions(false);
            UpdateCategoryLevelUpText();
            foreach (EncyclopediaSlot encyclopediaSlot in encyclopediaSlotDict.Values)
            {
                encyclopediaSlot.SetMaxLevelText();
            }

            return;
        }

        if (isLevelUpAvailable)
        {
            ActivateInteractions(true);
        }
        else
        {
            ActivateInteractions(false);
        }
    }

    private void ActivateInteractions(bool isActive)
    {
        levelUpButton.interactable = isActive;
        redDot.SetActive(isActive);
        NotificationManager.instance.SetNotification(RedDotIDType.EncyclopediaButton, isActive);
    }

    public void CheckLevelUpAvailable()
    {
        if (slotCount == 0) return;
        bool currentLevelUpAvailable = true;
        for (int i = 0; i < slotCount; i++)
        {
            currentLevelUpAvailable &= encyclopediaSlots[i].GetIsLevelUpAvailable();
        }

        isLevelUpAvailable = currentLevelUpAvailable;

        if (isLevelUpAvailable)
        {
            switch (encyclopediaType)
            {
                case EncyclopediaType.Equipment:
                    LevelUpAbilityEvent?.Invoke(EncyclopediaType.Equipment, categoryName, true);
                    break;
                case EncyclopediaType.Skill:
                    LevelUpAbilityEvent?.Invoke(EncyclopediaType.Skill, categoryName, true);
                    break;
                case EncyclopediaType.Colleague:
                    LevelUpAbilityEvent?.Invoke(EncyclopediaType.Colleague, categoryName, true);
                    ActivateInteractions();
                    break;
            }
        }
        else
        {
            switch (encyclopediaType)
            {
                case EncyclopediaType.Equipment:
                    LevelUpAbilityEvent?.Invoke(EncyclopediaType.Equipment, categoryName, false);
                    break;
                case EncyclopediaType.Skill:
                    LevelUpAbilityEvent?.Invoke(EncyclopediaType.Skill, categoryName, false);
                    break;
                case EncyclopediaType.Colleague:
                    LevelUpAbilityEvent?.Invoke(EncyclopediaType.Colleague, categoryName, false);
                    ActivateInteractions();
                    break;
            }
        }
    }

    public void CheckLevelUpAvailable(EncyclopediaCategoryData data)
    {
        bool currentLevelUpAvailable = true;
        for (int i = 0; i < slotCount; i++)
        {
            encyclopediaSlots[i].UpdateLevel(data.slotDatas[i]);

            currentLevelUpAvailable &= encyclopediaSlots[i].GetIsLevelUpAvailable();
        }

        isLevelUpAvailable = currentLevelUpAvailable;

        if (isLevelUpAvailable)
        {
            LevelUpAbilityEvent?.Invoke(encyclopediaType, categoryName, true);
        }
        else
        {
            LevelUpAbilityEvent?.Invoke(encyclopediaType, categoryName, false);
        }

        NotificationManager.instance.SetNotification(RedDotIDType.EncyclopediaButton, isLevelUpAvailable);
    }

    public void LevelUp()
    {
        if (isLevelUpAvailable)
        {
            ColleagueEncyclopediaIncrementData colleagueEncyclopediaIncrementData =
                OnGetColleagueEncyclopediaIncrementData.Invoke(colleagueEncyclopediaType, level);

            ColleagueEncyclopediaIncrementData nextColleagueEncyclopediaIncrementData =
                OnGetColleagueEncyclopediaIncrementData.Invoke(colleagueEncyclopediaType, level + 1);

            levelUpAbility = colleagueEncyclopediaIncrementData.increment;

            if (preLevelUpAbility == 0 && levelUpAbility == 0)
            {
                isMaxLevel = true;
                ActivateInteractions();
                return;
            }

            level++;

            int totalLevelUpAbility = levelUpAbility == 0 ? preLevelUpAbility : levelUpAbility - preLevelUpAbility;
            //categoryNameOrLevelText.text = $"LV.{level} {EnumToKR.GetStatTypeKR(statType)} {level * levelUpAbility} -> LV.{level + 1} {EnumToKR.GetStatTypeKR(statType)} {(level + 1) * levelUpAbility}";

            //StatDataHandler.Instance.ModifyStat(arithmeticStatType, AdditionalStatType.Encyclopedia, statType, (level * levelUpAbility) - ((level - 1) * levelUpAbility), true);
            StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Encyclopedia, statType, totalLevelUpAbility, true);

            UpdateGoalLevel(nextColleagueEncyclopediaIncrementData.goalLevelEachElement);
            for (int i = 0; i < encyclopediaSlots.Count; i++)
            {
                encyclopediaSlots[i].UpdateGoalLevel(goalLevel);
                encyclopediaSlots[i].UpdateLevelText();
            }

            preLevelUpAbility = colleagueEncyclopediaIncrementData.increment;
            levelUpAbility = nextColleagueEncyclopediaIncrementData.increment;
            UpdateUI();
            LevelUpEvent?.Invoke();
            //SetUI();



            SaveData();
        }
    }

    private void UpdateGoalLevel(int goalLevelEachElement)
    {
        goalLevel = goalLevelEachElement;
    }

    private void SaveData()
    {
        ES3.Save($"EncyclopediaCategoryData_{categoryName}_Level", level, ES3.settings);
    }

    public void InitInteraction(bool isInActive)
    {
        isMaxLevel = isInActive;
        ActivateInteractions();
    }
}
