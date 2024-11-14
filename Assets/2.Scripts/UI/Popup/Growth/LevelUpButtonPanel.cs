using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpButtonPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI statText;

    [SerializeField] private LongTouchButton levelUpButton;
    [SerializeField] private Image currencyImage;
    [SerializeField] private TextMeshProUGUI priceText;

    [SerializeField] private TextMeshProUGUI levelMaxText;

    [SerializeField] private float touchDuration = 0.15f;
    [SerializeField] private float touchInterval = 0.1f;
    [SerializeField] private float touchMinimumInterval = 0.025f;

    public event Action OnLevelUp;

    public void Init()
    {
        currencyImage.sprite = CurrencyManager.instance.GetCurrency(CurrencyType.Gold).GetIcon();
        levelUpButton.onClick.AddListener(UpdateLevelUpUI);
        levelUpButton.SetTouchDuration(touchDuration);
        levelUpButton.SetClickInterval(touchInterval, touchMinimumInterval);
    }

    public void UpdateLevelText(BigInteger level)
    {
        levelText.text = $"LV.{level}";
    }

    public void UpdatePriceText(BigInteger price)
    {
        priceText.text = price.ToString();
    }

    public void UpdateStatText(StatType statType, BigInteger stat)
    {
        statText.text = $"{EnumToKRManager.instance.GetEnumToKR(statType)}\n<color=green>+{stat}</color>";
    }

    public void UpdateButtonInteractable(bool isActive, bool isLevelMax)
    {
        levelUpButton.interactable = isActive;
        levelUpButton.image.color = isActive ? Color.white : Consts.DISABLE_COLOR;
        levelMaxText.gameObject.SetActive(isLevelMax);
        levelUpButton.gameObject.SetActive(!isLevelMax);
    }

    public bool GetIsLevelUpButtonInteractable()
    {
        return levelUpButton.interactable;
    }

    private void UpdateLevelUpUI()
    {
        OnLevelUp?.Invoke();
    }
}
