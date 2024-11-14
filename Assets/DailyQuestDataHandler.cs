using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyQuestDataHandler : Singleton<DailyQuestDataHandler>
{
    public event Action resetDailyQuestsAtMidnight;
    private DailyQuestSO dailyQuestSO;
    private RewardManager rewardManager;
    public List<DailyQuestData> dailyQuestDatas { get; private set; }
    public List<DailyQuestSlotData> dailyQuestSlotDatas { get; private set; }
    private DateTime lastResetDate;

    private Dictionary<DailyQuestType, Action> dailyMoveQuestPlaceDict;
    private Dictionary<FeatureID, int> bottomBarIndexDict;

    public void Init()
    {
        resetDailyQuestsAtMidnight = null;

        bottomBarIndexDict = new Dictionary<FeatureID, int>()
        {
            { FeatureID.BottomBar_Castle, 0 },
            { FeatureID.BottomBar_Summon, 1 },
            { FeatureID.BottomBar_Dungeon, 3 },
            { FeatureID.BottomBar_Equipment, 2 },
            { FeatureID.BottomBar_Colleague, 4 },
            { FeatureID.BottomBar_Kingdom, 5 },
        };

        rewardManager = RewardManager.instance;
        dailyQuestSO = ResourceManager.instance.dailyQuestSO;
        dailyQuestDatas = dailyQuestSO.GetDailyQuestDatas();
        dailyQuestSlotDatas = dailyQuestSO.GetDailyQuestSlotDatas();

        dailyMoveQuestPlaceDict = new Dictionary<DailyQuestType, Action>()
        {
            { DailyQuestType.Dungeon_ForgeDungeon, MoveToDungeon },
            { DailyQuestType.Dungeon_GemDungeon, MoveToDungeon },
            { DailyQuestType.Dungeon_GoldDungeon, MoveToDungeon },
            { DailyQuestType.Kill_Monster, null },
            { DailyQuestType.LevelUP_Colleague, MoveToEquipmentPanel },
            { DailyQuestType.LevelUP_Skill, MoveToSkillPanel },
            { DailyQuestType.Forge_Equipment, MoveToSummon },
            { DailyQuestType.Summon_Colleague, MoveToSummon },
        };

        if (!ES3.KeyExists(Consts.LastCheckTimeKey))
        {
            ES3.Save(Consts.LastCheckTimeKey, DateTime.Now, ES3.settings);
        }
    }

    public void InitStart()
    {
        CheckAndResetQuestsIfNeeded();
        ResourceManager.instance.StartCoroutine(WaitUntilMidnightToResetQuests());
    }

    public void UpdateQuestProgress(DailyQuestType questType, int progressAmount)
    {
        var quest = dailyQuestSlotDatas.Find(q => q.type == questType);

        if (quest != null)
        {
            if (quest.isRewarded) return;

            quest.currentCount += progressAmount;

            ES3.Save<int>($"{questType}_CurrentCount", quest.currentCount, ES3.settings);

            CheckQuestCompletion(quest);
        }
    }

    private void MoveToDungeon()
    {
        bool[] isLocked = ES3.Load<bool[]>(Consts.IS_LOCKED, ES3.settings);
        if (isLocked != null)
        {
            if (!isLocked[bottomBarIndexDict[FeatureID.BottomBar_Colleague]])
            {
                UIManager.instance.GetUIElement<UI_Dungeon>().openUI.Invoke();
            }
        }
    }

    private void MoveToSummon()
    {
        bool[] isLocked = ES3.Load<bool[]>(Consts.IS_LOCKED, ES3.settings);
        if (isLocked != null)
        {
            if (!isLocked[bottomBarIndexDict[FeatureID.BottomBar_Summon]])
            {
                UIManager.instance.GetUIElement<UI_Summon>().openUI.Invoke();
            }
        }
    }

    private void MoveToSkillPanel()
    {
        bool[] isLocked = ES3.Load<bool[]>(Consts.IS_LOCKED, ES3.settings);
        if (isLocked != null)
        {
            if (!isLocked[bottomBarIndexDict[FeatureID.BottomBar_Dungeon]])
            {
                UIManager.instance.GetUIElement<UI_Skills>().openUI.Invoke();
            }
        }
    }

    private void MoveToEquipmentPanel()
    {
        bool[] isLocked = ES3.Load<bool[]>(Consts.IS_LOCKED, ES3.settings);
        if (isLocked != null)
        {
            if (isLocked[bottomBarIndexDict[FeatureID.BottomBar_Equipment]])
            {
                UIManager.instance.GetUIElement<UI_Equipment>().openUI.Invoke();
            }
        }
    }

    private void CheckQuestCompletion(DailyQuestSlotData quest)
    {
        if (quest.currentCount >= quest.GoalCount)
        {
            quest.isCompleted = true;
            NotificationManager.instance.SetNotification(RedDotIDType.DailyQuestButton, true);
        }
    }

    public void MoveToQuestPlace(DailyQuestType dailyQuestType)
    {
        dailyMoveQuestPlaceDict[dailyQuestType].Invoke();
    }

    public void GiveRewardQuest(DailyQuestType questType)
    {
        var quest = dailyQuestSlotDatas.Find(q => q.type == questType);

        if (quest != null)
        {
            rewardManager.GiveReward(quest.reward, quest.amount);
            rewardManager.ShowRewardPanel();

            quest.isRewarded = true;
            ES3.Save<bool>($"{questType}_IsRewarded", true, ES3.settings);
            NotificationManager.instance.SetNotification(RedDotIDType.DailyQuestButton, false);
        }
    }

    private void CheckAndResetQuestsIfNeeded()
    {
        lastResetDate = ES3.Load(Consts.LastCheckTimeKey, DateTime.Now);

        if (DateTime.Now.Date > lastResetDate.Date)
        {
            ResetAllQuests();
        }
    }

    IEnumerator WaitUntilMidnightToResetQuests()
    {
        while (true)
        {
            DateTime now = DateTime.Now;
            DateTime nextMidnight = now.AddDays(1).Date;
            double secondsUntilMidnight = (nextMidnight - now).TotalSeconds;

            yield return new WaitForSeconds((float)secondsUntilMidnight);

            ResetAllQuests();
        }
    }

    private void ResetAllQuests()
    {
        foreach (var quest in dailyQuestSlotDatas)
        {
            quest.currentCount = 0;
            quest.isCompleted = false;
            quest.isRewarded = false;

            ES3.Save<int>($"{quest.type}_CurrentCount", quest.currentCount, ES3.settings);
            ES3.Save<bool>($"{quest.type}_IsRewarded", quest.isRewarded, ES3.settings);
        }

        lastResetDate = DateTime.Now;
        ES3.Save(Consts.LastCheckTimeKey, lastResetDate, ES3.settings);

        resetDailyQuestsAtMidnight?.Invoke();
    }
}