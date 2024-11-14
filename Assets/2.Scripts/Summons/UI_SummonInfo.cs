using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SummonInfo : UI_Base
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text levelText;
    [SerializeField] private Button prevBtn;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Text[] proportions;
    [SerializeField] private Text[] proportionTitles;
    [SerializeField] private Button closeBtn;

    private Dictionary<SummonType, SummonProbabilityDataSO> dataDic;

    private SummonType currentType;

    SummonProbabilityDataSO currentData;
    int currentLevel;
    int maxLevel;

    public void Initialize()
    {
        SetCollections();
        AddCallbacks();
        SetTextColors();
    }

    private void SetTextColors()
    {
        Rank[] ranks = (Rank[])Enum.GetValues(typeof(Rank));

        for (int i = 1; i < ranks.Length; i++)
        {
            Color color = ResourceManager.instance.rank.GetRankColor(ranks[i]);
            proportionTitles[i - 1].color = color;
            proportions[i - 1].color = color;
        }
    }

    private void SetCollections()
    {
        dataDic = new Dictionary<SummonType, SummonProbabilityDataSO>();
    }

    private void AddCallbacks()
    {
        closeBtn.onClick.AddListener(CloseUI);
        prevBtn.onClick.AddListener(DecreaseLevel);
        nextBtn.onClick.AddListener(IncreaseLevel);
    }

    public void ShowUI(SummonType type, int level = 1)
    {
        titleText.text = $"{GetSummonTypeKR(type)} 소환 확률";

        GetProportionData(type);
        currentType = type;

        currentLevel = level;

        prevBtn.enabled = true;
        nextBtn.enabled = (currentLevel < maxLevel);

        ChangeContents();
        OpenUI();
    }

    private string GetSummonTypeKR(SummonType type)
    {
        switch (type)
        {
            case SummonType.Equipment:
                return "장비";
            case SummonType.Skill:
                return "스킬";
            default:
                return "";
        }
    }


    private void GetProportionData(SummonType type)
    {
        dataDic.TryGetValue(type, out SummonProbabilityDataSO data);

        if (!data)
        {
            data = Resources.Load<SummonProbabilityDataSO>($"ScriptableObjects/SummonProbabilityDataSO/{type}SummonProbabilityDataSO");
            dataDic[type] = data;
        }

        currentData = data;
        currentLevel = 1;
        maxLevel = currentData.GetMaxLevel();
    }

    private void IncreaseLevel()
    {
        currentLevel++;

        currentLevel = Mathf.Min(currentLevel, maxLevel);

        prevBtn.enabled = true;
        nextBtn.enabled = (currentLevel < maxLevel);

        ChangeContents();
    }

    private void DecreaseLevel()
    {
        currentLevel--;

        currentLevel = Mathf.Max(currentLevel, 1);

        prevBtn.enabled = (currentLevel > 1);
        nextBtn.enabled = true;

        ChangeContents();
    }

    private void ChangeContents()
    {
        levelText.text = $"소환 레벨 {currentLevel}";

        int[] currentProportions = currentData.GetProbabillitiesOfLevel(currentLevel);

        for (int i = 0; i < proportions.Length; i++)
        {
            float currentProportion;
            if (currentType != SummonType.Equipment)
            {
                currentProportion = (float)currentProportions[i] / 1000;
            }
            else
            {
                currentProportion = (float)currentProportions[i] / 1000;
            }
            proportions[i].text = (currentProportion != 0) ? $"{currentProportion}%" : "-";
        }
    }
}