using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Growth : UI_BottomElement
{
    [SerializeField] private GrowthUIPanel growthUIPanel;

    public override void Initialize()
    {
        base.Initialize();
        growthUIPanel.Init();
    }

    public override void StartInit()
    {
        base.StartInit();
        growthUIPanel.StartInit();
    }

    public void UpdateForgeLevel(int forgeLevel)
    {
        growthUIPanel.UpdateForgeLevel(forgeLevel);
    }
}
