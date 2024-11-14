using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthDataHandler
{
    private GrowthDataSO growthDataSO;

    public event Action<int, bool, bool> OnUpdateStatLevelUpButtonInteractable;
    public event Action<int, int> OnPartLevelUpdated;
    public event Action<int, bool> OnUpdateGrowthLevel;
    public event Action<int, BigInteger> OnUpdateLevelUpButtonCurrency;

    public event Action<bool> OnActivateAnnounceGoalPanel;
    public event Action<bool> OnActivateBreakDownPanel;

    public event Action<bool> OnUpdateGrowthLevelUI;

    public event Action<bool, bool> OnUpdateGrowthLevelArrowButtons;
    public event Action<StatType, int, BigInteger> OnUpdateGrowthPartUI;

    private int currentGrowthLevel;

    private int[] currentGrowthLevels;

    private CurrencyType currencyType;

    private CurrencyManager currencyManager;

    private int currentLevelUpCount;

    private GrowthData growthData;

    private int enableGrowthLevel;

    private int userLevel;

    public GrowthDataHandler()
    {
        growthDataSO = ResourceManager.instance.growth;
        currencyManager = CurrencyManager.instance;
    }

    public void Init(int levelsLength, CurrencyType currencyType)
    {
        currentGrowthLevels = new int[levelsLength];
        Load();


        StatType[] statTypes = (StatType[])Enum.GetValues(typeof(StatType));

        for (int i = 1; i < currentGrowthLevel; i++)
        {
            GrowthData beforeGrowthData = growthDataSO.GetGrowthData(i);
            for (int j = 0; j < statTypes.Length - 1; j++)
            {
                BigInteger totalStat = beforeGrowthData.incrementsPerGrowthLevel[j] * beforeGrowthData.levelUpMax;
                StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Growth, statTypes[j + 1],
                    totalStat, true);
            }
        }

        growthData = growthDataSO.GetGrowthData(currentGrowthLevel);
        OnUpdateGrowthLevel?.Invoke(currentGrowthLevel, true);

        if (growthData.levelUpMax == 0)
        {
            GrowthData beforeGrowthData = growthDataSO.GetGrowthData(currentGrowthLevel - 1);
            UpdateCurrentGrowthStat(statTypes, beforeGrowthData);
            OnUpdateGrowthLevelUI?.Invoke(true);
        }
        else
        {
            UpdateCurrentGrowthStat(statTypes, growthData);
            UpdateTotalLevel();
        }

        this.currencyType = currencyType;
        currencyManager.GetCurrency(currencyType).OnCurrencyChange += TryUpdateLevelUpUIByCurrency;
        ResetEnableGrowthLevel();
        UpdateGrowthArrowButtonActive();

        UIManager.instance.GetUIElement<UI_Growth>().cloaseBtn.onClick.AddListener(ResetEnableGrowthLevel);
    }

    private void ResetEnableGrowthLevel()
    {
        enableGrowthLevel = currentGrowthLevel;
    }

    public void UpdateGrowthUIByLevelChanging(int changingValue)
    {
        enableGrowthLevel += changingValue;

        bool isGrowthLevelDiff = enableGrowthLevel != currentGrowthLevel;

        if (isGrowthLevelDiff)
        {
            GrowthData tempGrowthData = growthDataSO.GetGrowthData(enableGrowthLevel);
            OnUpdateGrowthLevel?.Invoke(enableGrowthLevel, isGrowthLevelDiff);
            int maxLevel = tempGrowthData.levelUpMax;
            for (int i = 0; i < currentGrowthLevels.Length; i++)
            {
                BigInteger stat = tempGrowthData.incrementsPerGrowthLevel[i] * maxLevel;
                OnUpdateGrowthPartUI?.Invoke((StatType)(i + 1), maxLevel, stat);
            }
        }
        else
        {
            TryUpdateLevelUpUI();
            bool isGrowthLevelMax = currentGrowthLevel == growthDataSO.GetGrowthDatasLength();
            OnUpdateGrowthLevelUI?.Invoke(isGrowthLevelMax);
            OnUpdateGrowthLevel?.Invoke(enableGrowthLevel, !isGrowthLevelDiff);
            UpdateTotalLevel();
        }

        UpdateGrowthArrowButtonActive();
    }

    private void UpdateGrowthArrowButtonActive()
    {
        if (enableGrowthLevel == 1)
        {
            if (enableGrowthLevel == currentGrowthLevel)
            {
                OnUpdateGrowthLevelArrowButtons?.Invoke(false, false);
            }
            else
            {
                OnUpdateGrowthLevelArrowButtons?.Invoke(false, true);
            }
        }
        else if (enableGrowthLevel <= currentGrowthLevel || enableGrowthLevel == growthDataSO.GetGrowthDatasLength())
        {
            OnUpdateGrowthLevelArrowButtons?.Invoke(true, false);
        }
        else
        {
            OnUpdateGrowthLevelArrowButtons?.Invoke(true, true);
        }
    }

    private void UpdateLimitPanelAcitve()
    {
        bool isAccomplished = userLevel >= growthData.desiredForgeLevel;
        if (isAccomplished)
        {
            OnActivateBreakDownPanel?.Invoke(true);
        }
        else
        {
            OnActivateBreakDownPanel?.Invoke(false);
            OnActivateAnnounceGoalPanel?.Invoke(true);
        }
    }

    public void StartInit()
    {
        TryUpdateLevelUpUIByCurrency(currencyManager.GetCurrencyValue(currencyType));
    }

    public void TryUpdateLevelUpUI()
    {
        for (int i = 0; i < currentGrowthLevels.Length; i++)
        {
            BigInteger price = GetCurrencyAmount(i);
            bool isLevelUpPossible = currentGrowthLevels[i] < growthData.levelUpMax;
            bool isInteractable = isLevelUpPossible && currencyManager.GetCurrencyValue(currencyType) >= price;
            OnUpdateStatLevelUpButtonInteractable?.Invoke(i, isInteractable, !isLevelUpPossible);
            OnUpdateLevelUpButtonCurrency?.Invoke(i, price);
        }
    }

    public int GetCurrentGrowthLevel()
    {
        return currentGrowthLevel;
    }

    public void GrowthPartLevelUp(StatType statType)
    {
        int statTypeIndex = (int)statType - 1;
        int beforeLevel = currentGrowthLevels[statTypeIndex];
        int afterLevel = beforeLevel + currentLevelUpCount;
        int levelUpMax = growthData.levelUpMax;

        if (afterLevel > levelUpMax)
        {
            afterLevel = growthData.levelUpMax;
        }

        BigInteger currencyAmount = GetCurrencyAmount(statTypeIndex);

        currentGrowthLevels[statTypeIndex] = afterLevel;

        CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gold, -currencyAmount);

        BigInteger totalStat = growthData.incrementsPerGrowthLevel[statTypeIndex] * afterLevel
            - growthData.incrementsPerGrowthLevel[statTypeIndex] * beforeLevel;

        StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Growth, statType, totalStat, true);

        if (afterLevel >= growthData.levelUpMax)
        {
            OnUpdateStatLevelUpButtonInteractable?.Invoke(statTypeIndex, false, true);
        }

        UpdateTotalLevel();

        ES3.Save<int[]>(Consts.GROWTH_LEVELS, currentGrowthLevels, ES3.settings);
        ES3.StoreCachedFile();
    }

    public BigInteger GetPartLevel(int statTypeIndex)
    {
        return currentGrowthLevels[statTypeIndex];
    }

    public bool GetIsProgressingGrowthPartLevelUp(int index)
    {
        return currentGrowthLevels[index] < growthData.levelUpMax;
    }

    public BigInteger GetPartStat(int statTypeIndex)
    {
        BigInteger stat = 0;

        if (growthData.levelUpMax == 0)
        {
            GrowthData beforeGrowthData = growthDataSO.GetGrowthData(currentGrowthLevel - 1);
            stat = beforeGrowthData.incrementsPerGrowthLevel[statTypeIndex] * currentGrowthLevels[statTypeIndex];
        }
        else
        {
            stat = growthData.incrementsPerGrowthLevel[statTypeIndex] * currentGrowthLevels[statTypeIndex];
        }

        return stat;
    }

    public BigInteger GetCurrencyAmount(int statTypeIndex)
    {
        if (currentGrowthLevels[statTypeIndex] >= growthData.levelUpMax)
        {
            return 0;
        }

        int subtract = growthData.levelUpMax - (currentGrowthLevels[statTypeIndex] + currentLevelUpCount);
        int actualLevelUpCount = subtract >= 0 ? currentLevelUpCount : growthData.levelUpMax - currentGrowthLevels[statTypeIndex];

        if (actualLevelUpCount == 0)
        {
            return growthData.currencyBasePerGrowthLevel;
        }

        BigInteger currentAmount = growthData.currencyBasePerGrowthLevel +
            currentGrowthLevels[statTypeIndex] * growthData.currencyIncrementPerGrowthLevel;

        return GetCurrencyAmountRecursive(statTypeIndex, currentGrowthLevels[statTypeIndex], actualLevelUpCount,
            currentAmount);
    }

    public void GrowthLevelUp()
    {
        currentGrowthLevel++;
        enableGrowthLevel = currentGrowthLevel;
        growthData = growthDataSO.GetGrowthData(currentGrowthLevel);
        OnUpdateGrowthLevel?.Invoke(currentGrowthLevel, true);
        UpdateGrowthArrowButtonActive();

        if (growthData.levelUpMax == 0)
        {
            OnUpdateGrowthLevelUI?.Invoke(true);
            ES3.Save<int>(Consts.GROWTH_LEVEL, currentGrowthLevel, ES3.settings);
            ES3.Save<int[]>(Consts.GROWTH_LEVELS, currentGrowthLevels, ES3.settings);
            ES3.StoreCachedFile();
            return;
        }

        for (int i = 0; i < currentGrowthLevels.Length; i++)
        {
            currentGrowthLevels[i] = 0;
        }

        OnUpdateGrowthLevelUI?.Invoke(false);
        OnActivateAnnounceGoalPanel?.Invoke(false);
        OnActivateBreakDownPanel?.Invoke(false);

        ES3.Save<int>(Consts.GROWTH_LEVEL, currentGrowthLevel, ES3.settings);
        ES3.Save<int[]>(Consts.GROWTH_LEVELS, currentGrowthLevels, ES3.settings);
        ES3.StoreCachedFile();
    }

    public void UpdatePanelActiveStateByAcommpliment(int userLevel)
    {
        this.userLevel = userLevel;

        if (growthData.levelUpMax == 0)
        {
            return;
        }

        UpdateLimitPanelAcitve();
    }

    public int GetDesiredForgeLevel()
    {
        return growthData.desiredForgeLevel;
    }

    public void UpdateCurrentLevelUpCount(int currentLevelUpCount)
    {
        this.currentLevelUpCount = currentLevelUpCount;
    }

    private void UpdateCurrentGrowthStat(StatType[] statTypes, GrowthData growthData)
    {
        for (int i = 0; i < currentGrowthLevels.Length; i++)
        {
            BigInteger totalStat = growthData.incrementsPerGrowthLevel[i] * growthData.levelUpMax;
            StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Base, AdditionalStatType.Growth, statTypes[i + 1],
                    totalStat, true);
        }
    }

    private void TryUpdateLevelUpUIByCurrency(BigInteger currency)
    {
        if (growthData.levelUpMax == 0 || enableGrowthLevel != currentGrowthLevel)
        {
            return;
        }

        for (int i = 0; i < currentGrowthLevels.Length; i++)
        {
            BigInteger price = GetCurrencyAmount(i);
            bool isLevelUpPossible = currentGrowthLevels[i] < growthData.levelUpMax;
            bool isInteractable = isLevelUpPossible && currencyManager.GetCurrencyValue(currencyType) >= price;
            OnUpdateStatLevelUpButtonInteractable?.Invoke(i, isInteractable, !isLevelUpPossible);
        }
    }

    private void UpdateTotalLevel()
    {
        int totalLevel = 0;
        foreach (int level in currentGrowthLevels)
        {
            totalLevel += level;
        }

        int goalLevel = currentGrowthLevels.Length * growthData.levelUpMax;

        OnPartLevelUpdated?.Invoke(totalLevel, goalLevel);
    }

    private BigInteger GetCurrencyAmountRecursive(int statTypeIndex, int currentStatLevel, int count, 
        BigInteger currentAmount)
    {
        if (count <= 0)
        {
            return currentAmount;
        }

        currentAmount += growthData.currencyBasePerGrowthLevel + currentStatLevel * growthData.currencyIncrementPerGrowthLevel;
        currentStatLevel++;
        count--;

        return GetCurrencyAmountRecursive(statTypeIndex, currentStatLevel, count, currentAmount);
    }

    private void Load()
    {
        currentGrowthLevel = ES3.Load<int>(Consts.GROWTH_LEVEL, 1, ES3.settings);
        currentGrowthLevels = ES3.Load<int[]>(Consts.GROWTH_LEVELS, currentGrowthLevels, ES3.settings);
    }
}
