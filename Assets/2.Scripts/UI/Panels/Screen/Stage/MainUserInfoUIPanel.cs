using Keiwando.BigInteger;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUserInfoUIPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userId;
    [SerializeField] private TextMeshProUGUI userLevel;

    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI gemText;

    [SerializeField] private Image expImage;
    [SerializeField] private TextMeshProUGUI expText;

    private CurrencyManager currencyManager;

    public void Init()
    {
        currencyManager = CurrencyManager.instance;

        currencyManager.GetCurrency(CurrencyType.Gold).OnCurrencyChange += UpdateMainCurrencyGoldUI;
        currencyManager.GetCurrency(CurrencyType.Gem).OnCurrencyChange += UpdateMainCurrencyGemUI;
        UserLevelController userLevelController = FindAnyObjectByType<UserLevelController>();
        userLevelController.Init();
        userLevelController.userLevelDataHandler.OnUpdateExp += UpdateLevelUI;
        userLevelController.InitLevelUI();
    }

    private void UpdateLevelUI(int level, BigInteger exp, BigInteger maxExp)
    {
        userLevel.text = $"Lv. {level}";
        expText.text = $"{exp} / {maxExp}";
        expImage.fillAmount = exp.ToFloat() / maxExp.ToFloat();
    }

    private void UpdateMainCurrencyGoldUI(BigInteger gold)
    {
        goldText.text = gold.ToString();
    }
    private void UpdateMainCurrencyGemUI(BigInteger gem)
    {
        gemText.text = gem.ToString();
    }
}
