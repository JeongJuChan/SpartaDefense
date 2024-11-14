using Keiwando.BigInteger;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private Button rollingBtn;
    [SerializeField] private Button exitButton;
    [SerializeField] private AbilitySlot[] abilitySlots;

    private bool isRollinged;
    private UI_Alert uI_Alert;
    private AbilityOptionGenerator abilityOptionGenerator;
    private BattleManager battleManager;

    [SerializeField] private Image abilityPointImage;
    [SerializeField] private TextMeshProUGUI abilityPointText;

    public void Init()
    {
        AddCallbacks();

        battleManager = FindAnyObjectByType<BattleManager>();

        Currency currency = CurrencyManager.instance.GetCurrency(CurrencyType.AbilityPoint);
        currency.OnCurrencyChange += UpdateAbilityPointText;
        Sprite currencyIcon = currency.GetIcon();

        UpdateAbilityPointText(currency.GetCurrencyValue());

        currency.OnCurrencyChange += UpdateAbilityPointText;
        abilityPointImage.sprite = currencyIcon;

        UpdateAbilityPointText(currency.GetCurrencyValue());

        uI_Alert = UIManager.instance.GetUIElement<UI_Alert>();
        abilityOptionGenerator = new AbilityOptionGenerator(
            new AbilityOptionGradeSelector(DataParser.ParseOptionGradePercentageData(Resources.Load<TextAsset>("CSV/AbilityOption/OptionGradePercentageData"))),
            new AbilityOptionEffectSelector(DataParser.ParseOptionEffectTypePercentageData(Resources.Load<TextAsset>("CSV/AbilityOption/OptionEffectTypePercentageData"))),
            new AbilityOptionCalculator(DataParser.ParseOptionPercentageData(Resources.Load<TextAsset>("CSV/AbilityOption/OptionPercentageData")))
        );

        foreach (var slot in abilitySlots)
        {
            slot.AddCallbacks();
        }

        InitAbilitySlotUI(battleManager);
    }

    private void UpdateAbilityPointText(BigInteger amount)
    {
        abilityPointText.text = amount.ToString();
    }

    public void StartInit()
    {
        foreach (var slot in abilitySlots)
        {
            slot.StartInitStat();
        }
    }

    public void ActivateSelf(bool isActivate)
    {
        gameObject.SetActive(isActivate);
    }

    public void InitAbilitySlotUI(BattleManager battleManager)
    {
        for (int i = 0; i < abilitySlots.Length; i++)
        {
            abilitySlots[i].InitSlot(i, battleManager);
        }
    }

    private void AddCallbacks()
    {
        exitButton.onClick.AddListener(CloseUI);
        rollingBtn.onClick.AddListener(AbilityOptionRolling);
    }

    private void AbilityOptionRolling()
    {
        if (CheckSlotLook()) return;

        if (CurrencyManager.instance.TryUpdateCurrency(CurrencyType.AbilityPoint, -1))
        {
            Rolling();
        }
        else
        {
            uI_Alert.AlertMessage("어빌리티 스톤이 부족합니다.");
        }
    }

    private void Rolling()
    {
        foreach (var slot in abilitySlots)
        {
            abilityOptionGenerator.GenerateAbilityOption(out Rank rank, out AbilityOptionEffectType effect, out float multiplier, out ArithmeticStatType arithmeticStatType);
            slot.SetSlot(rank, effect, multiplier, arithmeticStatType);
        }

        isRollinged = true;
    }

    public bool CheckSlotLook()
    {
        bool locked = true;
        foreach (var slot in abilitySlots)
        {
            locked &= slot.GetIsLocked();
        }

        return locked;
    }

    public void CloseUI()
    {
        if (isRollinged)
        {
            uI_Alert.isPowerMessageWaiting = true;

            foreach (var slot in abilitySlots)
            {
                // 능력치 진짜 상승
                slot.Save();
                slot.ApplyAbility();

            }

            uI_Alert.isPowerMessageWaiting = false;
        }


        gameObject.SetActive(false);

        isRollinged = false;
    }
}
