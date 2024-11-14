using Keiwando.BigInteger;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewSlotUIPanel : SlotUIPanel
{
    [SerializeField] private TextMeshProUGUI expectedStatText;
    private List<KeyValuePair<StatType, BigInteger>> newStatDict = new List<KeyValuePair<StatType, BigInteger>>();
    private List<KeyValuePair<StatType, BigInteger>> currentStatDict = new List<KeyValuePair<StatType, BigInteger>>();

    public event Action<StatType, BigInteger, BigInteger> OnUpdateSummonUI;

    public event Action<int, RewardType> OnEquipmentSold;

    public event Action<EquipmentType, Rank, SlotEquipmentStatData> OnAddHero;
    public event Action<EquipmentType, SlotEquipmentStatData> OnRemoveHero;

    public event Action<ColleagueInfo> OnHeroChanged;

    public event Action<ColleagueInfo> OnInstantiateHero;

    private CastleClan castleClan;

    public event Action<EquipmentType, Sprite, Sprite, int> OnCurrentSlotChanged;

    public event Func<float> OnGetSlotElementSize;

    public event Action<Dictionary<int, float>> OnAddAttributes;
    public event Action<Dictionary<int, float>> OnRemoveAttributes;

    public event Action OnUpdateRtanAttributes;

    public event Func<Rank, Sprite> OnGetRankBackgroundSprite;
    public event Func<Rank, Color> OnGetRankColor;

    public event Action OnShowWarningPopup;

    public event Action<bool> OnActivateCurrentSlotPanel;

    readonly string increaseColorHex = "44DD4C";
    readonly string decreaseColorHex = "EC4F4F";
    BigInteger increase;

    [SerializeField] private CheckBoxButton autoDisassembleButton;

    public void Init()
    {
        /*autoDisassembleButton.Init();
        autoDisassembleButton.OnUpdateCheckBoxState += ForgeManager.instance.UpdateAutoDisassembleState;
        bool isActive = ES3.KeyExists(Consts.DIALOGUE_TYPE_SELL_ITEMS);
        if (!isActive)
        {
            DialogManager.instance.OnActivateAutoDisassembleEquipment += () => ActivateAutoDisassembleButton(true);
        }

        ActivateAutoDisassembleButton(isActive);*/
    }

    private void ActivateAutoDisassembleButton(bool isActive)
    {
        autoDisassembleButton.gameObject.SetActive(isActive);
    }

    public void UpdateSlotStatsUI(SlotUIData newSlotUIData, SlotUIData currentSlotUIData)
    {
        bool isFirstTime = currentSlotUIData.mainSprite == null;

        Sprite rankSprite = OnGetRankBackgroundSprite.Invoke(newSlotUIData.slotStatData.rank);

        mainSlotElement.UpdateSlotUIElement(newSlotUIData.equipmentType, rankSprite, newSlotUIData.mainSprite,
            newSlotUIData.slotStatData.level);

        string rankKR = EnumToKRManager.instance.GetEnumToKR(newSlotUIData.slotStatData.rank);
        titleText.text = $"[{rankKR}]{newSlotUIData.nickName}";

        Color titleColor = OnGetRankColor.Invoke(newSlotUIData.slotStatData.rank);
        titleText.color = titleColor;

        string slotTypeKR = newSlotUIData.nickName;
        typeText.text = slotTypeKR;

        hpText.text = newSlotUIData.slotStatData.equipmentStatData.health.ToString();

        damageText.text = newSlotUIData.slotStatData.equipmentStatData.mainDamage.ToString();

        defenseText.text = newSlotUIData.slotStatData.equipmentStatData.defense.ToString();

        OnUpdateSummonUI?.Invoke(StatType.HP, currentSlotUIData.slotStatData.equipmentStatData.health, newSlotUIData.slotStatData.equipmentStatData.health);
        OnUpdateSummonUI?.Invoke(StatType.Damage, currentSlotUIData.slotStatData.equipmentStatData.mainDamage, newSlotUIData.slotStatData.equipmentStatData.mainDamage);
        OnUpdateSummonUI?.Invoke(StatType.Defense, currentSlotUIData.slotStatData.equipmentStatData.defense, newSlotUIData.slotStatData.equipmentStatData.defense);

        if (currentSlotUIData.mainSprite == null)
        {

            newStatDict.Clear();
            newStatDict.Add(new KeyValuePair<StatType, BigInteger>(StatType.HP, newSlotUIData.slotStatData.equipmentStatData.health));
            newStatDict.Add(new KeyValuePair<StatType, BigInteger>(StatType.Damage, newSlotUIData.slotStatData.equipmentStatData.mainDamage));
            newStatDict.Add(new KeyValuePair<StatType, BigInteger>(StatType.Defense, newSlotUIData.slotStatData.equipmentStatData.defense));

            increase = StatDataHandler.Instance.GetTotalStat(null, newStatDict);
        }
        else
        {
            currentStatDict.Clear();
            currentStatDict.Add(new KeyValuePair<StatType, BigInteger>(StatType.HP, currentSlotUIData.slotStatData.equipmentStatData.health));
            currentStatDict.Add(new KeyValuePair<StatType, BigInteger>(StatType.Damage, currentSlotUIData.slotStatData.equipmentStatData.mainDamage));
            currentStatDict.Add(new KeyValuePair<StatType, BigInteger>(StatType.Defense, currentSlotUIData.slotStatData.equipmentStatData.defense));

            newStatDict.Clear();
            newStatDict.Add(new KeyValuePair<StatType, BigInteger>(StatType.HP, newSlotUIData.slotStatData.equipmentStatData.health));
            newStatDict.Add(new KeyValuePair<StatType, BigInteger>(StatType.Damage, newSlotUIData.slotStatData.equipmentStatData.mainDamage));
            newStatDict.Add(new KeyValuePair<StatType, BigInteger>(StatType.Defense, newSlotUIData.slotStatData.equipmentStatData.defense));

            increase = StatDataHandler.Instance.GetTotalStat(currentStatDict, newStatDict);
        }

        bool isSuperior = false;
        bool isInferior = false;

        if (currentSlotUIData.slotStatData.equipmentStatData.health != null &&
            currentSlotUIData.slotStatData.equipmentStatData.mainDamage != null &&
            currentSlotUIData.slotStatData.equipmentStatData.defense != null)
        {
            isSuperior = currentSlotUIData.slotStatData.equipmentStatData.mainDamage <= newSlotUIData.slotStatData.equipmentStatData.mainDamage &&
            currentSlotUIData.slotStatData.equipmentStatData.health <= newSlotUIData.slotStatData.equipmentStatData.health &&
            currentSlotUIData.slotStatData.equipmentStatData.defense <= newSlotUIData.slotStatData.equipmentStatData.defense;

            isInferior = currentSlotUIData.slotStatData.equipmentStatData.mainDamage >= newSlotUIData.slotStatData.equipmentStatData.mainDamage &&
            currentSlotUIData.slotStatData.equipmentStatData.health >= newSlotUIData.slotStatData.equipmentStatData.health &&
            currentSlotUIData.slotStatData.equipmentStatData.defense >= newSlotUIData.slotStatData.equipmentStatData.defense;
        }

        ForgeManager.instance.UpdateIsSuperior(isSuperior);

        if (!isInferior)
        {
            isInferior = increase > 0 ? isInferior : !isInferior;
        }

        ForgeManager.instance.UpdateIsInperior(isInferior);

        expectedStatText.text = $"전투력 <color=#{(increase < 0 ? decreaseColorHex : increaseColorHex)}>{increase.ChangeMoney()}</color>";

        //OnUpdateSlotUIAttributes?.Invoke(attributeStatDict);

        if (isFirstTime)
        {
            /*if (SlotTypeSavedDic.ContainsKey(slotType) || slotType == ColleagueType.Rtan)
            {
                SwapSlotPanels();
            }
            else*/
            //currentSlotUIData.SetSlotData(currentSlotUIData);

            OnActivateCurrentSlotPanel?.Invoke(false);
        }
        else
        {
            OnActivateCurrentSlotPanel.Invoke(true);
        }
    }
}
