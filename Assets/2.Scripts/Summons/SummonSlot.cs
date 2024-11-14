using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.UI;

public class SummonSlot : MonoBehaviour
{
    SummonController controller;

    [SerializeField] SummonType type;
    [SerializeField] private FeatureID featureID;
    [SerializeField] private CurrencyType currencyType;

    [SerializeField] GameObject lockPanel;

    [Header("소환 레벨")]
    [SerializeField] Text summonLevel;
    [SerializeField] Text summonExp;
    [SerializeField] Slider summonExpBar;
    [SerializeField] Button infoBtn;

    [Header("소환 버튼")]
    [SerializeField] Button adsSummon;
    [SerializeField] Button smallSummon;
    [SerializeField] Button largeSummon;
    [SerializeField] Text adsTitle;
    [SerializeField] Text smallTitle;
    [SerializeField] Text largeTitle;
    [SerializeField] Text adsQuantityTitleText;
    [SerializeField] Text adsPriceText;
    [SerializeField] Text smallQuantityTitleText;
    [SerializeField] Text smallPriceText;
    [SerializeField] Text largeQuantityTitleText;
    [SerializeField] Text largePriceText;
    [SerializeField] private Image smallPriceImage;
    [SerializeField] private Image largePriceImage;
    

    [Header("색상")]
    [SerializeField] Color btnNormalBgColor;
    [SerializeField] Color btnDisabledBgColor;
    [SerializeField] Color textNormalColor;
    [SerializeField] Color textDisabledColor;
    [SerializeField] Color priceNormalColor;
    [SerializeField] Color priceDisabledColor;

    private UI_SummonInfo ui_summonInfo;

    private int currentExp;
    private int currentLevel;
    private int maxExp;

    private bool isUnlocked;

    [SerializeField] private bool isLevelless;
    [SerializeField] private bool isAtMaxLevel;

    private Sprite currencySprite;
    private Sprite gemSprite;

    private CurrencyType currentSmallCurrencyType;
    private CurrencyType currentLargeCurrencyType;

    public void Initialize(SummonController controller)
    {
        this.controller = controller;

        // ui_SummonInfo = // 가져오는 로직 구현

        ui_summonInfo = UIManager.instance.GetUIElement<UI_SummonInfo>();

        AddEventListeners();
        LoadSavedDatas();
        InitCurrencyInfo();

        ActivateSummonButtons(CurrencyManager.instance.GetCurrencyValue(currencyType));

        InitUnlock();
    }

    private void InitCurrencyInfo()
    {
        currencySprite = CurrencyManager.instance.GetCurrency(currencyType).GetIcon();
        gemSprite = CurrencyManager.instance.GetCurrency(CurrencyType.Gem).GetIcon();

        smallPriceImage.sprite = currencySprite;
        largePriceImage.sprite = currencySprite;

        currentSmallCurrencyType = currencyType;
        currentLargeCurrencyType = currencyType;

        CurrencyManager.instance.GetCurrency(currencyType).OnCurrencyChange += ActivateSummonButtons;
        if (currencyType != CurrencyType.Gem)
        {
            CurrencyManager.instance.GetCurrency(CurrencyType.Gem).OnCurrencyChange += ActivateSummonButtonsByGemUpdated;
        }

        int smallPrice = controller.GetSmallSummonInfo(type).quantity;
        int largePrice = controller.GetLargeSummonInfo(type).quantity;
        int adsPrice = controller.GetAdsSummonInfo(type).quantity;
        smallPriceText.text = smallPrice.ToString();
        smallQuantityTitleText.text = $"{smallPrice}회";
        largePriceText.text = largePrice.ToString();
        largeQuantityTitleText.text = $"{largePrice}회";
        adsQuantityTitleText.text = $"{adsPrice}회";
    }

    public void InitUnlock()
    {
        isUnlocked = ES3.Load($"{type}{Consts.SUMMON_IS_LOCKED}", true);

        if (isUnlocked)
        {
            UnlockData unlockData = ResourceManager.instance.unlockDataSO.GetUnlockData(featureID);

            Debug.Log(unlockData);

            UnlockManager.Instance.RegisterFeature(new UnlockableFeature(unlockData.featureType, unlockData.featureID, unlockData.count, () =>
            {
                isUnlocked = false;

                ES3.Save($"{type}{Consts.SUMMON_IS_LOCKED}", isUnlocked, ES3.settings);

                ES3.StoreCachedFile();
                lockPanel.SetActive(isUnlocked);
            }));
        }


        lockPanel.SetActive(isUnlocked);
    }

    private void AddEventListeners()
    {
        adsSummon.onClick.AddListener(AdsSummon);
        smallSummon.onClick.AddListener(SmallSummon);
        largeSummon.onClick.AddListener(LargeSummon);

        if (!isLevelless)
        {
            infoBtn.onClick.AddListener(() => ui_summonInfo.ShowUI(type, currentLevel));
            controller.AddSummonCallbacks(type, exp: UpdateCurrentExp, level: UpdateLevel, maxExp: UpdateMaxExp);
        }
        else
        {
            summonExpBar.value = 1;
        }
    }

    private void AdsSummon()
    {
        AdsManager.instance.ShowRewardedAd($"{type}Summon", (reward, adInfo) =>
        {
            controller.AdsSummon(type);
            Firebase.Analytics.FirebaseAnalytics.LogEvent($"ad_summon_{type}");
            UpdateAdsUI();
        });
    }

    private void SmallSummon()
    {
        int price = currentSmallCurrencyType == CurrencyType.Gem ?
            controller.GetSmallSummonPriceReplacedByGem(type) : controller.GetSmallSummonInfo(type).price;
        controller.SmallSummon(type, currentSmallCurrencyType, price);
    }

    private void LargeSummon()
    {
        int price = currentLargeCurrencyType == CurrencyType.Gem ?
            controller.GetLargeSummonPriceReplacedByGem(type) : controller.GetLargeSummonInfo(type).price;
        controller.LargeSummon(type, currentLargeCurrencyType, price);
    }

    private void UpdateCurrentExp(int exp)
    {
        currentExp = exp;
        UpdateUI();
    }

    private void UpdateLevel(int level)
    {
        currentLevel = level;
        UpdateUI();
    }

    private void UpdateMaxExp(int maxExp)
    {
        this.maxExp = maxExp;
        UpdateUI();
    }

    private void UpdateUI()
    {
        summonExp.text = $"{currentExp} / {maxExp}";
        summonExpBar.value = (maxExp == 0) ? 0 : (float)currentExp / maxExp;
        summonLevel.text = $"{currentLevel}";
    }

    private void ActivateSummonButtons(BigInteger value)
    {
        bool isSmallAvailable = value >= controller.GetSmallSummonInfo(type).price;
        bool isLargeAvailable = value >= controller.GetLargeSummonInfo(type).price;

        NotificationManager.instance.SetNotification(RedDotIDType.ColleagueSummonButton, isSmallAvailable);
        NotificationManager.instance.SetNotification(RedDotIDType.ColleagueSummonButton_12, isLargeAvailable);

        BigInteger smallPrice = controller.GetSmallSummonInfo(type).price;
        BigInteger largePrice = controller.GetLargeSummonInfo(type).price;

        if (currencyType != CurrencyType.Gem)
        {
            if (isLargeAvailable)
            {
                currentSmallCurrencyType = currencyType;
                currentLargeCurrencyType = currencyType;
                largePriceImage.sprite = currencySprite;
                smallPriceImage.sprite = currencySprite;
            }
            else
            {
                currentLargeCurrencyType = CurrencyType.Gem;
                BigInteger gem = CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gem);

                largePrice = controller.GetLargeSummonPriceReplacedByGem(type);
                largePriceImage.sprite = gemSprite;

                if (gem >= largePrice)
                {
                    isLargeAvailable = true;
                }

                if (isSmallAvailable)
                {
                    currentSmallCurrencyType = currencyType;
                    smallPriceImage.sprite = currencySprite;
                }
                else
                {
                    currentSmallCurrencyType = CurrencyType.Gem;
                    smallPrice = controller.GetSmallSummonPriceReplacedByGem(type);
                    smallPriceImage.sprite = gemSprite;

                    if (gem >= smallPrice)
                    {
                        isSmallAvailable = true;
                    }
                }
            }
        }

        //TODO: 여기에서 버튼 레드닷 표시 - 고민: 이미 버튼은 색과 버튼의 활성화 여부로 표시되고 있음, 그런데 여기서 굳이 레드닷으로 또 표시해야할까?

        UpdateSummonSlotUI(largePrice, smallPrice, isSmallAvailable, isLargeAvailable);
    }

    private void ActivateSummonButtonsByGemUpdated(BigInteger value)
    {
        BigInteger currency = CurrencyManager.instance.GetCurrencyValue(currencyType);
        bool isSmallAvailable = currency >= controller.GetSmallSummonInfo(type).price;
        bool isLargeAvailable = currency >= controller.GetLargeSummonInfo(type).price;

        NotificationManager.instance.SetNotification(RedDotIDType.ColleagueSummonButton, isSmallAvailable);
        NotificationManager.instance.SetNotification(RedDotIDType.ColleagueSummonButton_12, isLargeAvailable);

        BigInteger smallGemPrice = isSmallAvailable ? controller.GetSmallSummonInfo(type).price : controller.GetSmallSummonPriceReplacedByGem(type);
        BigInteger largeGemPrice = isLargeAvailable ? controller.GetLargeSummonInfo(type).price : controller.GetLargeSummonPriceReplacedByGem(type);

        bool isSmallAvailableByGem = value >= controller.GetSmallSummonPriceReplacedByGem(type) || isSmallAvailable;
        bool isLargeAvailableByGem = value >= controller.GetLargeSummonPriceReplacedByGem(type) || isLargeAvailable;

        if (!isLargeAvailable && isLargeAvailableByGem)
        {
            isLargeAvailableByGem = true;
        }

        if (!isSmallAvailable && isSmallAvailableByGem)
        {
            isSmallAvailableByGem = true;
        }
        //TODO: 여기에서 버튼 레드닷 표시 - 고민: 이미 버튼은 색과 버튼의 활성화 여부로 표시되고 있음, 그런데 여기서 굳이 레드닷으로 또 표시해야할까?

        UpdateSummonSlotUI(largeGemPrice, smallGemPrice, isSmallAvailableByGem, isLargeAvailableByGem);
    }

    private void UpdateSummonSlotUI(BigInteger largePrice, BigInteger smallPrice, bool isSmallAvailable, bool isLargeAvailable)
    {
        UpdateAdsUI();

        smallSummon.enabled = isSmallAvailable;
        smallSummon.image.color = (isSmallAvailable) ? btnNormalBgColor : btnDisabledBgColor;
        smallTitle.color = (isSmallAvailable) ? textNormalColor : textDisabledColor;
        smallPriceText.color = (isSmallAvailable) ? priceNormalColor : priceDisabledColor;
        smallPriceText.text = smallPrice.ToString();

        largeSummon.enabled = isLargeAvailable;
        largeSummon.image.color = (isLargeAvailable) ? btnNormalBgColor : btnDisabledBgColor;
        largeTitle.color = (isLargeAvailable) ? textNormalColor : textDisabledColor;
        largePriceText.color = (isLargeAvailable) ? priceNormalColor : priceDisabledColor;
        largePriceText.text = largePrice.ToString();

        controller.UpdateResultUI(type, currentSmallCurrencyType, currentLargeCurrencyType, isSmallAvailable, isLargeAvailable, 
            smallPrice, largePrice);
    }

    private void UpdateAdsUI()
    {
        bool isAdsAvailable = AdsManager.instance.GetAdCount($"{type}Summon") < 3;

        adsSummon.enabled = isAdsAvailable;
        adsSummon.image.color = (isAdsAvailable) ? btnNormalBgColor : btnDisabledBgColor;
        adsTitle.color = (isAdsAvailable) ? textNormalColor : textDisabledColor;
        adsPriceText.color = (isAdsAvailable) ? priceNormalColor : priceDisabledColor;
        adsPriceText.text = $"{3 - AdsManager.instance.GetAdCount($"{type}Summon")} / 3";
    }

    private void UpdateQuestInfo(int questNum)
    {
        UpdateLockState();
        SaveUnlockedDatas();
    }


    private void LoadSavedDatas()
    {
        UpdateLockState();
    }

    private void UpdateLockState()
    {
        // if (!isUnlocked)
        // {
        //     // bool isUnlocking = !questManager.CheckQuestOnNumber(unlockQuestNum);
        //     // Debug.Log($"Unlocking {type} : {isUnlocking}");
        //     // if (isUnlocking)
        //     // {
        //     //     isUnlocked = isUnlocking;
        //     //     SaveUnlockedDatas();
        //     // }
        // }

        // lockPanel.SetActive(!isUnlocked);
    }

    private void SaveUnlockedDatas()
    {
        // ES3.Save($"SummonSlot_{type}_Unlocked", isUnlocked);
    }

    private void RemoveEventListeners()
    {
        adsSummon.onClick.RemoveAllListeners();
        smallSummon.onClick.RemoveAllListeners();
        largeSummon.onClick.RemoveAllListeners();
        controller.RemoveCallbacks();

        // questManager.OnQuestNumberChange -= UpdateQuestInfo;
        // currencyManager.GetCurrency(currencyType).OnCurrencyChange -= ActivateSummonButtons;
    }

    private void OnDestroy()
    {
        RemoveEventListeners();
    }
}