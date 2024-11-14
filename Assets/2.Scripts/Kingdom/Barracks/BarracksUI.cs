using Keiwando.BigInteger;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarracksUI : MonoBehaviour
{
    [SerializeField] private BarracksLevelDataSO barracksLevelDataSO;
    [SerializeField] private TrainingSoldierDataSO trainingSoldierDataSO;
    [SerializeField] private TrainingElement[] trainingElements;
    [SerializeField] private AccelerationUIPopup accelerationUIPopup;

    [SerializeField] private Slider barracksExpSlider;
    [SerializeField] private TextMeshProUGUI barracksExpText;

    [SerializeField] private TextMeshProUGUI barracksLevelText;

    [SerializeField] private Button exitButton;

    [SerializeField] private GameObject kingdomObject;

    private TrainingElement currentTrainingElement;

    private Action<int> OnUpdateSoldierLevel;

    [SerializeField] private TextMeshProUGUI abilityPointText;

    [SerializeField] private Image abilityPointImage;

    [SerializeField] private LockElement[] barracksLockElements;

    private BarracksLevelData barracksLevelData;
    private TrainingSoldierData trainingSoldierData;

    private const string BARRACKS_LEVEL = "barracksLevel";
    private const string BARRACKS_EXP = "barracksExp";

    private int rewardStorageAmount = 5;

    private int currentMaxExp;
    private Action<FeatureType> OnUpdateFeature;

    public void Init()
    {
        UnlockManager.Instance.SetUnlockCondition(FeatureType.BarracksLevel, CheckUnlockBarracks);
        foreach (LockElement lockElement in barracksLockElements)
        {
            lockElement.InitUnlock();
        }

        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.Training, () => { QuestManager.instance.UpdateCount(EventQuestType.Training, PlayerPrefs.HasKey("Training") ? 1 : 0, -1); });
        OnUpdateFeature += UnlockManager.Instance.CheckUnlocks;

        exitButton.onClick.AddListener(CloseUI);
        barracksLevelData = barracksLevelDataSO.GetData();
        trainingSoldierData = trainingSoldierDataSO.GetData();

        barracksLevelData.level = ES3.Load<int>(BARRACKS_LEVEL, 1, ES3.settings);
        barracksLevelData.currentExp = ES3.Load<int>(BARRACKS_EXP, barracksLevelData.currentExp, ES3.settings);

        UpdateBarracksLevelText();
        UpdateCurrentMaxExp();
        UpdateBarracksExp();

        Currency currency = CurrencyManager.instance.GetCurrency(CurrencyType.AbilityPoint);
        currency.OnCurrencyChange += AbilityPointText;
        Sprite currencyIcon = currency.GetIcon();

        AbilityPointText(currency.GetCurrencyValue());

        currency.OnCurrencyChange += AbilityPointText;
        abilityPointImage.sprite = currencyIcon;

        AbilityPointText(currency.GetCurrencyValue());

        int count = 1;

        foreach (TrainingElement trainingElement in trainingElements)
        {
            accelerationUIPopup.OnGetUsableAccelerationTicketCount += trainingElement.GetUserableTicketCount;
            accelerationUIPopup.OnUseTicket += trainingElement.AccelerateWaitDuration;
            accelerationUIPopup.OnGetAccelTimeText += trainingElement.ConvertTime;
            trainingElement.OnAccelerationButtonClicked += UpdateCurrentTrainingElement;
            trainingElement.OnUpdateAccelerationTicketCount += accelerationUIPopup.SetTicketCount;
            trainingElement.SetCurrencyIcon(currencyIcon);
            OnUpdateSoldierLevel += trainingElement.UpdateSoldierLevel;
            trainingElement.SetMaxRewardAmount(rewardStorageAmount * count);
            trainingElement.OnRewardReceived += GetReward;
            count++;
            trainingElement.Init();
        }

        OnUpdateSoldierLevel?.Invoke(barracksLevelData.level);
        accelerationUIPopup.Init();
        OnUpdateFeature?.Invoke(FeatureType.BarracksLevel);

        ActivateSelf(false);
    }
    
    public void ActivateSelf(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    private void CloseUI()
    {
        kingdomObject.SetActive(true);
        ActivateSelf(false);
    }

    private bool CheckUnlockBarracks(int index)
    {
        return barracksLevelData.level >= index;
    }

    private void UpdateCurrentTrainingElement(TrainingElement trainingElement)
    {
        if (currentTrainingElement != null)
        {
            accelerationUIPopup.OnGetUsableAccelerationTicketCount -= currentTrainingElement.GetUserableTicketCount;
            accelerationUIPopup.OnUseTicket -= currentTrainingElement.AccelerateWaitDuration;
            accelerationUIPopup.OnGetAccelTimeText -= currentTrainingElement.ConvertTime;
        }

        currentTrainingElement = trainingElement;
        accelerationUIPopup.OnGetUsableAccelerationTicketCount += currentTrainingElement.GetUserableTicketCount;
        accelerationUIPopup.OnUseTicket += currentTrainingElement.AccelerateWaitDuration;
        accelerationUIPopup.OnGetAccelTimeText += currentTrainingElement.ConvertTime;

        accelerationUIPopup.ActivateSelf(true);
    }

    private void AbilityPointText(BigInteger amount)
    {
        abilityPointText.text = amount.ToString();
    }

    private void UpdateBarracksLevelText()
    {
        barracksLevelText.text = $"병영 Lv.{barracksLevelData.level}";
    }

    private void UpdateBarracksExp()
    {
        barracksExpText.text = $"{barracksLevelData.currentExp} / {currentMaxExp}";
        barracksExpSlider.value = (float)barracksLevelData.currentExp / currentMaxExp;
    }

    private void UpdateCurrentMaxExp()
    {
        currentMaxExp = barracksLevelData.baseMaxExp + barracksLevelData.level * barracksLevelData.increment;
    }

    private void GetReward(int amount)
    {
        int exp = trainingSoldierData.baseExp + barracksLevelData.level * trainingSoldierData.expIncrement;
        int reward = (trainingSoldierData.baseItemAmount + barracksLevelData.level * trainingSoldierData.itemIncrement) * amount;

        UpdateLevel(exp);
        CurrencyManager.instance.TryUpdateCurrency(CurrencyType.AbilityPoint, reward);
    }

    private void UpdateLevel(int exp)
    {
        barracksLevelData.currentExp += exp;
        if (barracksLevelData.currentExp >= currentMaxExp)
        {
            barracksLevelData.currentExp -= currentMaxExp;
            barracksLevelData.level++;
            UpdateBarracksLevelText();
            UpdateCurrentMaxExp();
            OnUpdateSoldierLevel?.Invoke(barracksLevelData.level);
            OnUpdateFeature?.Invoke(FeatureType.BarracksLevel);
            ES3.Save<int>(BARRACKS_LEVEL, barracksLevelData.level, ES3.settings);
        }

        UpdateBarracksExp();
        ES3.Save<int>(BARRACKS_EXP, barracksLevelData.currentExp, ES3.settings);
        ES3.StoreCachedFile();
    }
}
