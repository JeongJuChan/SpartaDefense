using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UI_Summon : UI_BottomElement
{
    [SerializeField] private SummonController controller;
    [SerializeField] private UI_SummonResult resultUI;
    [SerializeField] private UI_SummonInfo infoUI;
    [SerializeField] private GuideController guide;

    [SerializeField] private CurrencyTextPanel[] currencyTextPanels;

    private bool initialized = false;


    public override void OpenUI()
    {
        base.OpenUI();

    }

    public override void Initialize()
    {
        base.Initialize();
        //NotificationManager.instance.SetNotification(RedDotIDType.ShowSummonButton, CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gem) >= 300);
        foreach (CurrencyTextPanel panel in currencyTextPanels)
        {
            panel.Init();
        }

        Hes_Init();
    }

    public override void StartInit()
    {
        controller.InitUnlock();
    }

    private void OnDisable()
    {
        NotificationManager.instance.SetNotification(RedDotIDType.ShowSummonButton, CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gem) >= 300);
    }

    public void Hes_Init()
    {
        if (initialized) return;
        GetElements();
        InitializeFunctions();

        guide.Initialize();

        initialized = true;
    }

    private void GetElements()
    {
        resultUI = UIManager.instance.GetUIElement<UI_SummonResult>();
        infoUI = UIManager.instance.GetUIElement<UI_SummonInfo>();
    }

    private void InitializeFunctions()
    {
        resultUI.Initialize();
        infoUI.Initialize();

        controller.Initalize();
    }
}
