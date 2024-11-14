using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Keiwando.BigInteger;

public struct CurrentEquipmentLevelData
{
    public BigInteger statIncrease; // 스텟 증가량
    public BigInteger currencyIncrease; // 필요한 화폐량
    public BigInteger startStat; // 시작 스텟
    public BigInteger startCurrency; // 시작 화폐량

    public CurrentEquipmentLevelData(BigInteger statIncrease, BigInteger currencyIncrease, BigInteger startStat, BigInteger startCurrency)
    {
        this.statIncrease = statIncrease;
        this.currencyIncrease = currencyIncrease;
        this.startStat = startStat;
        this.startCurrency = startCurrency;
    }
}

public class UI_EquipmentLevelUp : UI_Base
{
    private EquipmentManager equipmentManager;

    [SerializeField] private Button backPanel;
    [SerializeField] private Text title;
    [SerializeField] private EquipmentSlot equipmentView;
    [SerializeField] private Image[] currencyIcons;
    [SerializeField] private Text currentCurrency;
    [SerializeField] private Text price;

    [SerializeField] private Button levelUpBtn;
    // [SerializeField] private HoldButton holdButton;
    [SerializeField] private Text levelUpText;

    [Header("LevelUpInfo")]
    [SerializeField] private GameObject levelUpInfo;
    [SerializeField] private Text[] effectType;
    [SerializeField] private Text effectValue_before;
    [SerializeField] private Text effectValue_after;

    [Header("Sources")]
    [SerializeField] private Sprite enhanceStone;
    private UI_Alert alert;

    private EquipmentData equipment;
    private bool isLevelUpAvailable = true;
    private EquipmentLevelData equipmentLevelDatas;
    private CurrentEquipmentLevelData currentEquipmentLevelData;
    private BigInteger currentPrice = 0;
    private BigInteger changedValue = 0;
    public void Initialize()
    {
        equipmentManager = EquipmentManager.instance;
        AddCallbacks();

        alert = UIManager.instance.GetUIElement<UI_Alert>();

        //equipmentLevelDatas = DataParser.ParseLevelData(Resources.Load<TextAsset>("CSV/Equipment/EquipmentLevelData"));
    }

    private void AddCallbacks()
    {
        backPanel.onClick.AddListener(CloseUI);
        levelUpBtn.onClick.AddListener(OnLevelUpBtnClicked);
        // holdButton.onHold.AddListener(OnLevelUpBtnClicked);
        // holdButton.SetActions(PointerDownAction, PointerUpAction);
    }

    public void SetEquipment(EquipmentData equipment)
    {
        if (this.equipment != null) equipmentView.RemoveCallbacks();

        this.equipment = equipment;

        equipmentView.SetEquipmentInfo(equipment);

        CurrentEquipmentLevelData(equipment.effectType);

        SetUpUI();
    }

    public void CurrentEquipmentLevelData(StatType statType)
    {
        currentEquipmentLevelData = equipmentLevelDatas.GetStatIncrease(statType, $"{equipment.rank}_{equipment.Index + 1}");
    }

    private void SetUpUI()
    {
        levelUpInfo.SetActive(isLevelUpAvailable);

        currentCurrency.text = CurrencyManager.instance.GetCurrencyValue(CurrencyType.ColleagueLevelUpStone).ToString();

        if (isLevelUpAvailable) SetLevelUpUI();
        else if (CheckCurrency()) SetLevelUpUI();
    }

    private void SetLevelUpUI()
    {
        title.text = Consts.TITLE_LEVELUP;

        foreach (Image icon in currencyIcons)
        {
            icon.sprite = enhanceStone;
        }

        //TODO: Set Currency and Price Info

        currentCurrency.text = CurrencyManager.instance.GetCurrencyValue(CurrencyType.ColleagueLevelUpStone).ToString();
        price.text = $"{currentEquipmentLevelData.currencyIncrease * equipment.GetLevel()}";


        foreach (Text type in effectType)
        {
            // type.text = $"{Strings.DataType.GetName(equipment.EEType)}";
        }

        effectValue_before.text = $"{equipmentManager.GetEquipEffectValue(equipment)}";
        effectValue_after.text = $"{equipmentManager.GetNextEquipEffectValue(equipment)}";

        levelUpText.text = Consts.BUTTON_LEVELUP;
    }


    private void OnLevelUpBtnClicked()
    {
        if (isLevelUpAvailable)
        {
            if (CheckCurrency())
            {
                DeductCurrency();

                equipmentManager.UpdateEquipmentLevel(equipment);
                EncyclopediaDataHandler.Instance.SlotLevelChangeEvent(EncyclopediaType.Equipment, $"{equipment.EquipmentType}_{equipment.rank}");
                DailyQuestDataHandler.Instance.UpdateQuestProgress(DailyQuestType.LevelUP_Colleague, 1);

            }
            else
            {
                alert.AlertMessage(Consts.CURRENCY_LACK_EQUIPMENTENFORCESTONE);
            }
        }
        else
        {
            if (CheckCurrency())
            {
                DeductCurrency();
            }
            else
            {
            }
        }

        if (equipment.IsAtMaxLevel())
        {
            alert.AlertMessage(Consts.EQUIP_MAX_LEVEL);
            CloseUI();
        }
        else SetUpUI();
    }

    private void PointerDownAction()
    {
        if (isLevelUpAvailable)
        {
            OnLevelUpBtnClicked();
        }
        else alert.AlertMessage(Consts.CURRENCY_LACK_EQUIPMENTENFORCESTONE);
    }

    private void PointerUpAction()
    {
        if (changedValue == 0) return;
        // PlayerDataManager.Instance.CalculateTotalCombatPower();
        changedValue = 0;
    }

    bool CheckCurrency()
    {
        return CurrencyManager.instance.GetCurrencyValue(CurrencyType.ColleagueLevelUpStone) >= currentEquipmentLevelData.currencyIncrease * equipment.GetLevel();
    }

    void DeductCurrency()
    {
        CurrencyManager.instance.TryUpdateCurrency(CurrencyType.ColleagueLevelUpStone, -currentEquipmentLevelData.currencyIncrease * equipment.GetLevel());
    }


}
