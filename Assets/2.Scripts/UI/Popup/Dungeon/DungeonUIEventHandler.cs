using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonUIEventHandler : CoroutinableUI
{
    [SerializeField] private DungeonBackgroundSO dungeonBackgroundSO;
    [SerializeField] private DungeonUIPanel[] dungeons;
    [SerializeField] private RemainTimeViewer remainTimeViewer;
    [SerializeField] private DungeonChoiceUIPopup dungeonChoiceUIPopup;
    [SerializeField] private DungeonQuitPanel dungeonQuitPanel;

    private DungeonController dungeonController;

    private Dictionary<DungeonType, int> dungeonTypeIndexDict = new Dictionary<DungeonType, int>();

    public void Init()
    {
        dungeonChoiceUIPopup.Init();
        dungeonController = FindAnyObjectByType<DungeonController>();
        DungeonType[] dungeonTypes = (DungeonType[])Enum.GetValues(typeof(DungeonType));

        for (int i = 1; i < dungeonTypes.Length; i++)
        {
            dungeonTypeIndexDict.Add(dungeonTypes[i], i - 1);
        }

        DailyRewardController dailyRewardController = FindAnyObjectByType<DailyRewardController>();

        dailyRewardController.Init();

        for (int i = 0; i < dungeons.Length; i++)
        {
            dungeons[i].OnGetDailyRewardCurrencyCount += dailyRewardController.GetDungeonDailyRewardCount;
            dungeons[i].Init(dungeonController.GetDungeonName, dungeonController.GetPairCurrencyType, dungeonBackgroundSO.dungeonBackgrounds[i]);
            dungeons[i].OnOpenDungeonChoicePopup += dungeonChoiceUIPopup.OnShowChoiceUIPopup;
        }

        dungeonChoiceUIPopup.OnEntranceDungeon += dungeonController.EntranceDungeon;
        dungeonChoiceUIPopup.OnClearDungeon += dungeonController.SweepDungeon;
        dungeonChoiceUIPopup.OnGetDungeonLevel += dungeonController.GetDungeonLevel;
        dungeonChoiceUIPopup.OnGetDungeonReward += dungeonController.GetBossReward;
        dungeonController.OnUpdateCostUI += dungeonChoiceUIPopup.UpdateCostUI;
        dungeonController.OnChangeDungeonActiveState += dungeonQuitPanel.ActiveSelf;
        dungeonController.OnupdateDungeonProgress += dungeonQuitPanel.UpdateRewardSetting;

        dungeonQuitPanel.Init();

        dungeonChoiceUIPopup.OnGetDungeonDailyCurrency += dailyRewardController.GetDungeonDailyRewardCount;

        DailyTimeCalculator dailyTimeCalculator = dailyRewardController.dailyTimeCalculator;

        OnUpdateCoroutineState += dailyTimeCalculator.ToggleCalculateDailyRemainTime;
        dailyTimeCalculator.OnUpdateDailyRewardTime += remainTimeViewer.UpdateRemainTime;
    }

    public void InitUnlock()
    {
        for (int i = 0; i < dungeons.Length; i++)
        {
            dungeons[i].InitUnlock();
        }
    }

    private void OnEnable()
    {
        OnUpdateCoroutineState?.Invoke(this, true, null);
    }

    private void OnDisable()
    {
        OnUpdateCoroutineState?.Invoke(this, false, null);

        foreach (DungeonUIPanel dungeon in dungeons)
        {
            if (dungeon.CheckDungeonAvailability())
            {
                NotificationManager.instance.SetNotification(RedDotIDType.ShowDungeonButton, true);
                break;
            }
        }
    }

}
