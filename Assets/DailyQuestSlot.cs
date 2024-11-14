using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DailyQuestSlot : MonoBehaviour
{
    public event Action<DailyQuestType> OnCompleteQuest;
    public event Action<bool> OnGetReward;
    public event Action<DailyQuestType> OnMove;

    [SerializeField] RewardSlot rewardSlot;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI progressCount;
    [SerializeField] TextMeshProUGUI questPoint;
    [SerializeField] Button CompleteButton;
    [SerializeField] Button MoveButton;
    [SerializeField] Slider progressSlider;

    public DailyQuestSlotData dailyQuestSlotData { get; private set; }

    public void Init()
    {
        CompleteButton.onClick.AddListener(() =>
        {
            OnCompleteQuest?.Invoke(dailyQuestSlotData.type);
            CheckButton();
        });

        MoveButton.onClick.AddListener(() =>
        {
            OnMove?.Invoke(dailyQuestSlotData.type);
        });
    }

    public void SetDailyQuestSlot(DailyQuestSlotData dailyQuestSlotData)
    {
        this.dailyQuestSlotData = dailyQuestSlotData;

        rewardSlot.SetUI(RewardManager.instance.GetRewardInfo(dailyQuestSlotData.reward), dailyQuestSlotData.amount);

        description.text = dailyQuestSlotData.Description.Replace("n", dailyQuestSlotData.GoalCount.ToString());

        questPoint.text = $"달성 P: {dailyQuestSlotData.QuestPoint}";

        SetProgressCount(dailyQuestSlotData.currentCount, dailyQuestSlotData.GoalCount);

        CheckButton();
    }

    // void UpdateCurrentCount(int count)
    // {
    //     currentCount += count;
    //     SetProgressCount(currentCount, dailyQuestSlotData.GoalCount);

    //     if (currentCount >= dailyQuestSlotData.GoalCount)
    //     {
    //         isCompleted = true;
    //         CheckButton();
    //     }
    // }

    private void SetProgressCount(int currentCount, int goalCount)
    {
        progressCount.text = $"{currentCount} / {goalCount}";
        progressSlider.value = (float)currentCount / goalCount;
    }

    private void CheckButton()
    {
        if (dailyQuestSlotData.isRewarded)
        {
            CompleteButton.gameObject.SetActive(false);
            MoveButton.gameObject.SetActive(false);
        }
        else if (dailyQuestSlotData.isCompleted)
        {
            CompleteButton.gameObject.SetActive(true);
            MoveButton.gameObject.SetActive(false);
        }
        else
        {
            CompleteButton.gameObject.SetActive(false);
            MoveButton.gameObject.SetActive(true);
        }
    }
}
