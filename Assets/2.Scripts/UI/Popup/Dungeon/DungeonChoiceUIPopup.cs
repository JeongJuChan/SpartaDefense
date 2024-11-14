using Keiwando.BigInteger;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonChoiceUIPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dungeonTitleText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [SerializeField] private TextMeshProUGUI dungeonLevelText;

    [SerializeField] private Image rewardImage;
    [SerializeField] private TextMeshProUGUI rewardAmountText;

    [SerializeField] private Image costImage;
    [SerializeField] private TextMeshProUGUI costText;

    [SerializeField] private Button sweepButton;
    [SerializeField] private Button entranceButton;

    [SerializeField] private Button transparentButton;

    [SerializeField] private Image currencyImage;

    [SerializeField] private TextMeshProUGUI dungeonRewardKRText;

    [SerializeField] private GameObject activePanel;

    private DungeonType dungeonType;

    public event Action<DungeonType, int> OnEntranceDungeon;
    public event Action<DungeonType, int> OnClearDungeon;

    public event Func<DungeonType, int> OnGetDungeonLevel;
    public event Func<DungeonType, int> OnGetDungeonDailyCurrency;
    public event Func<DungeonType, int, BigInteger> OnGetDungeonReward;


    private int currentDungeonLevel;

    private int dungeonChallengableMaxLevel;

    private BigInteger currentCostCurrency;

    private int dailyReawrd;

    private UI_Alert uI_Alert;


    private PairCurrencyTypeData pairCurrencyType;

    public void Init()
    {
        transparentButton.onClick.AddListener(() => ActiveSelf(false));
        entranceButton.onClick.AddListener(OnClickEntranceDungeon);
        sweepButton.onClick.AddListener(OnClickSweepButton);
        leftButton.onClick.AddListener(OnClickLeftButton);
        rightButton.onClick.AddListener(OnClickRightButton);

        uI_Alert = UIManager.instance.GetUIElement<UI_Alert>();

        ActiveSelf(false);
    }

    private void ActiveSelf(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void OnShowChoiceUIPopup(DungeonType dungeonType, PairCurrencyTypeData pairCurrencyType, string dungeonName)
    {
        currentDungeonLevel = OnGetDungeonLevel.Invoke(dungeonType);

        dungeonChallengableMaxLevel = OnGetDungeonLevel.Invoke(dungeonType);

        this.pairCurrencyType = pairCurrencyType;
        this.dungeonType = dungeonType;
        dungeonTitleText.text = dungeonName;

        currencyImage.sprite = CurrencyManager.instance.GetCurrency(EnumUtility.GetCurrencyTypeByDungeonType(dungeonType)).GetIcon();

        UpdateLevelText();

        UpdateLevelPanel();
        UpdateCostUI();
        UpdateRewardUI();
        UpdateRewardKRUI();
        ActiveSelf(true);
        ActivePanel(true);
    }

    private void UpdateRewardKRUI()
    {
        dungeonRewardKRText.text = EnumToKRManager.instance.GetEnumToKR(pairCurrencyType.rewardCurrencyType);
    }

    public void UpdateCostUI()
    {
        currentCostCurrency = CurrencyManager.instance.GetCurrencyValue(pairCurrencyType.costCurrencyType);
        dailyReawrd = OnGetDungeonDailyCurrency.Invoke(dungeonType);
        costText.text = $"{currentCostCurrency}/{dailyReawrd}";
    }

    private void UpdateRewardUI()
    {
        rewardImage.sprite = CurrencyManager.instance.GetCurrency(pairCurrencyType.rewardCurrencyType).GetIcon();
        BigInteger currentReward = OnGetDungeonReward.Invoke(dungeonType, currentDungeonLevel);
        rewardAmountText.text = $"보상 : {currentReward}";
    }

    private void UpdateLevelPanel()
    {
        if (currentDungeonLevel == 1)
        {
            leftButton.gameObject.SetActive(false);
            if (dungeonChallengableMaxLevel == 1)
            {
                rightButton.gameObject.SetActive(false);
            }
            else
            {
                rightButton.gameObject.SetActive(true);
            }
        }
        else if (currentDungeonLevel == dungeonChallengableMaxLevel)
        {
            leftButton.gameObject.SetActive(true);
            rightButton.gameObject.SetActive(false);
        }
        else
        {
            leftButton.gameObject.SetActive(true);
            rightButton.gameObject.SetActive(true);
        }

        UpdateLevelText();
    }

    private void UpdateLevelText()
    {
        dungeonLevelText.text = currentDungeonLevel.ToString();
    }

    private void OnClickLeftButton()
    {
        currentDungeonLevel--;
        UpdateRewardUI();
        UpdateLevelPanel();
    }

    private void OnClickRightButton()
    {
        currentDungeonLevel++;
        UpdateRewardUI();
        UpdateLevelPanel();
    }

    private void OnClickEntranceDungeon()
    {
        if (currentCostCurrency <= 0)
        {

            uI_Alert.AlertMessage("입장권이 부족합니다.");
            return;
        }

        OnEntranceDungeon?.Invoke(dungeonType, currentDungeonLevel);
        ActiveSelf(false);
        ActivePanel(false);
    }

    private void OnClickSweepButton()
    {
        if (currentCostCurrency <= 0)
        {
            uI_Alert.AlertMessage("입장권이 부족합니다.");
            return;
        }
        else if (currentDungeonLevel >= dungeonChallengableMaxLevel)
        {
            uI_Alert.AlertMessage("클리어 하지 않은 레벨입니다.");
            return;
        }
            
        OnClearDungeon?.Invoke(dungeonType, currentDungeonLevel);
    }

    private void ActivePanel(bool isActive)
    {
        activePanel.SetActive(isActive);
    }
}
