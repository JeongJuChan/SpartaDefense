using Keiwando.BigInteger;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrowthUIPanel : MonoBehaviour
{
    private GrowthDataHandler growthDataHandler;

    [SerializeField] private CurrencyTextPanel goldPanel;
    [SerializeField] private TextMeshProUGUI powerText;

    [SerializeField] private GrowthLevelPanel growthLevelPanel;
    [SerializeField] private Button[] levelUpCountButtons;
    [SerializeField] private LevelUpButtonPanel[] levelUpButtonPanels;

    [SerializeField] private GameObject announceGoalPanel;
    [SerializeField] private TextMeshProUGUI announceText;
    [SerializeField] private TextMeshProUGUI forgeLevelText;
    [SerializeField] private Button goToForgeButton;

    [SerializeField] private GameObject breakDownPanel;
    [SerializeField] private TextMeshProUGUI possibleText;
    [SerializeField] private Button breakDownButton;

    [SerializeField] private GameObject limitPanel;

    private StatType[] statTypes;

    private int forgeLevel;

    public void Init()
    {
        goldPanel.Init();
        growthDataHandler = new GrowthDataHandler();
        statTypes = (StatType[])Enum.GetValues(typeof(StatType));
        GrowthButtonType[] growthButtonTypes = (GrowthButtonType[])Enum.GetValues(typeof(GrowthButtonType));

        growthLevelPanel.OnChangeLevelInfo += growthDataHandler.UpdateGrowthUIByLevelChanging;
        growthLevelPanel.Init();

        growthDataHandler.OnUpdateStatLevelUpButtonInteractable += UpdateGrowthPartButtonInteractable;
        growthDataHandler.OnUpdateGrowthLevelUI += ResetUIPanels;
        growthDataHandler.OnUpdateGrowthLevel += growthLevelPanel.UpdateGrowthLevelUI;
        growthDataHandler.OnActivateAnnounceGoalPanel += UpdateAnnounceGoalPanelActive;
        growthDataHandler.OnActivateBreakDownPanel += UpdateBreakDownPanelActive;
        growthDataHandler.OnPartLevelUpdated += growthLevelPanel.UpdateProgressUI;
        growthDataHandler.OnUpdateLevelUpButtonCurrency += UpdateLevelUpButtonCurrency;
        growthDataHandler.OnUpdateGrowthPartUI += UpdateGrowthPartUI;
        growthDataHandler.OnUpdateGrowthLevelArrowButtons += growthLevelPanel.UpdateButtonsActive;
        growthDataHandler.Init(statTypes.Length - 1, goldPanel.GetCurrencyType());

        for (int i = 0; i < levelUpCountButtons.Length; i++)
        {
            int colorIndex = i;
            int buttonTypeIndex = i + 1;
            levelUpCountButtons[i].onClick.AddListener(() => UpdateCurrentCount((int)growthButtonTypes[buttonTypeIndex]));
            levelUpCountButtons[i].onClick.AddListener(() => UpdateLevelUpCountButtonColor(colorIndex));
        }

        UpdateLevelUpCountButtonColor(0);
        UpdateCurrentCount(1);

        for (int i = 0; i < levelUpButtonPanels.Length; i++)
        {
            StatType statType = statTypes[i + 1];
            levelUpButtonPanels[i].OnLevelUp += () => GrowthPartLevelUp(statType);
            levelUpButtonPanels[i].Init();
            UpdateGrowthPartUI(statType);
        }

        StatDataHandler.Instance.OnUpdateTotalPower += UpdatePower;
        breakDownButton.onClick.AddListener(growthDataHandler.GrowthLevelUp);

        UpdateAnnounceGoalPanelActive(false);
        UpdateBreakDownPanelActive(false);
        UpdateLimitPanelActive(false);

        goToForgeButton.onClick.AddListener(GotoForgeSlot);
    }

    public void StartInit()
    {
        growthDataHandler.StartInit();
        TryUpdatePanelActiveState();
    }

    public void UpdateForgeLevel(int level)
    {
        forgeLevel = level;

        if (announceGoalPanel.activeInHierarchy)
        {
            growthDataHandler.UpdatePanelActiveStateByAcommpliment(forgeLevel);
        }
    }

    private void UpdatePower(BigInteger power)
    {
        powerText.text = $"전투력 : {power}";
    }

    private void GrowthPartLevelUp(StatType statType)
    {
        growthDataHandler.GrowthPartLevelUp(statType);
        UpdateGrowthPartUI(statType);
    }

    private void GotoForgeSlot()
    {
        UIManager.instance.GetUIElement<UI_Growth>().cloaseBtn.onClick.Invoke();
    }

    private void UpdateGrowthPartUI(StatType statType)
    {
        int statTypeIndex = (int)statType - 1;

        BigInteger level = growthDataHandler.GetPartLevel(statTypeIndex);
        levelUpButtonPanels[statTypeIndex].UpdateLevelText(level);

        BigInteger stat = growthDataHandler.GetPartStat(statTypeIndex);
        levelUpButtonPanels[statTypeIndex].UpdateStatText(statType, stat);

        BigInteger currencyAmount = growthDataHandler.GetCurrencyAmount(statTypeIndex);
        levelUpButtonPanels[statTypeIndex].UpdatePriceText(currencyAmount);
    }

    private void UpdateGrowthPartUI(StatType statType, int level, BigInteger stat)
    {
        int statTypeIndex = (int)statType - 1;

        levelUpButtonPanels[statTypeIndex].UpdateLevelText(level);

        levelUpButtonPanels[statTypeIndex].UpdateStatText(statType, stat);

        levelUpButtonPanels[statTypeIndex].UpdateButtonInteractable(false, true);
    }

    private void UpdateLevelUpCountButtonColor(int index)
    {
        for (int i = 0; i < levelUpCountButtons.Length; i++)
        {
            levelUpCountButtons[i].image.color = i == index ? Color.white : Consts.DISABLE_COLOR;
        }
    }

    private void UpdateCurrentCount(int count)
    {
        growthDataHandler.UpdateCurrentLevelUpCount(count);

        for (int i = 1; i < statTypes.Length; i++)
        {
            growthDataHandler.TryUpdateLevelUpUI();
        }
    }

    private void UpdateGrowthPartButtonInteractable(int statTypeIndex, bool isActive, bool isLevelMax)
    {
        levelUpButtonPanels[statTypeIndex].UpdateButtonInteractable(isActive, isLevelMax);
        TryUpdatePanelActiveState();
    }

    private void TryUpdatePanelActiveState()
    {
        for (int i = 0; i < levelUpButtonPanels.Length; i++)
        {
            bool isProgressing = growthDataHandler.GetIsProgressingGrowthPartLevelUp(i);
            if (isProgressing)
            {
                return;
            }
        }

        growthDataHandler.UpdatePanelActiveStateByAcommpliment(forgeLevel);
    }

    private void UpdateAnnounceGoalPanelActive(bool isActive)
    {
        if (isActive)
        {
            UpdateLimitPanelActive(true);
        }
        announceGoalPanel.SetActive(isActive);
    }

    private void UpdateLimitPanelActive(bool isActive)
    {
        limitPanel.SetActive(isActive);
    }

    private void UpdateBreakDownPanelActive(bool isActive)
    {
        int nextGrowthLevel = growthDataHandler.GetCurrentGrowthLevel() + 1;
        int desiredLevel = growthDataHandler.GetDesiredForgeLevel();
        string forgeLevelStr = $"<color=orange>성문 레벨 {desiredLevel}</color>";
        announceText.text = $"훈련 {nextGrowthLevel}단계 돌파를 위해 {forgeLevelStr}이 필요합니다.";
        forgeLevelText.text = forgeLevelStr;
        breakDownPanel.SetActive(isActive);
        if (isActive)
        {
            UpdateLimitPanelActive(true);
            UpdateAnnounceGoalPanelActive(false);
            possibleText.text = $"<color=yellow>훈련 {nextGrowthLevel}단계</color> 진입 가능!";
        }
        else
        {
            UpdateLimitPanelActive(false);
        }
    }

    private void ResetUIPanels(bool isMaxGrowthLevel)
    {
        UpdateGrowthUI(isMaxGrowthLevel);
    }

    private void UpdateGrowthUI(bool isMaxGrowthLevel)
    {
        if (isMaxGrowthLevel)
        {
            growthLevelPanel.SetLevelMaxProgressUI();
        }
        else
        {
            for (int i = 0; i < levelUpButtonPanels.Length; i++)
            {
                levelUpButtonPanels[i].UpdateButtonInteractable(true, false);
                UpdateGrowthPartUI(statTypes[i + 1]);
            }
        }
    }

    private void UpdateLevelUpButtonCurrency(int index, BigInteger currency)
    {
        levelUpButtonPanels[index].UpdatePriceText(currency);
    }
}