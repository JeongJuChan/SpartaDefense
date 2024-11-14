using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Dungeon : UI_BottomElement
{
    [SerializeField] private DungeonUIEventHandler dungeonUIEventHandler;
    [SerializeField] private GameObject activePanel;
    [SerializeField] private GuideController guide;

    private void Awake()
    {
        dungeonUIEventHandler.Init();
        guide.Initialize();
    }

    public override void StartInit()
    {
        dungeonUIEventHandler.InitUnlock();
    }

    public override void OpenUI()
    {
        base.OpenUI();
        activePanel.SetActive(true);
    }
}
