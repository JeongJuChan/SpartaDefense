using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailyQuestUIHandler : MonoBehaviour
{
    [SerializeField] Button closeButton;
    [SerializeField] GameObject dailyQuestPanel;
    [SerializeField] DailyQuestSlot dailyQuestSlotPrefab;
    [SerializeField] Transform dailyQuestSlotParent;
    [SerializeField] TextMeshProUGUI questPointText;
    [SerializeField] Slider questPointSlider;
    [SerializeField] List<RewardButton> questPointRewardSlots;
    private int currentQuestPoint;
    private List<DailyQuestData> dailyQuestDatas;
    private List<DailyQuestSlotData> dailyQuestSlotDatas;
    private List<DailyQuestSlot> dailyQuestSlots = new List<DailyQuestSlot>();
    private DailyQuestDataHandler dailyQuestDataHandler;


    private void Awake()
    {
        dailyQuestDataHandler = DailyQuestDataHandler.Instance;
        dailyQuestDatas = dailyQuestDataHandler.dailyQuestDatas;
        dailyQuestSlotDatas = dailyQuestDataHandler.dailyQuestSlotDatas;

        InitDailyQuestUI();
        closeButton.onClick.AddListener(() => dailyQuestPanel.SetActive(false));

        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.TouchMission,
            () => QuestManager.instance.UpdateCount(EventQuestType.TouchMission, PlayerPrefs.HasKey(Consts.TOUCH_MISSION) ? 1 : 0, -1));
    }

    private void Start()
    {
        dailyQuestDataHandler.resetDailyQuestsAtMidnight += ResetDailyQuestsAtMidnight;

        DailyQuestDataHandler.Instance.InitStart();
    }

    private void ResetDailyQuestsAtMidnight()
    {
        Debug.Log("어디보자 : " + questPointRewardSlots[0]);
        currentQuestPoint = 0;

        foreach (var questPointRewardSlot in questPointRewardSlots)
        {
            questPointRewardSlot.Reset();
        }

        SetQuestPoint();

        foreach (DailyQuestSlotData dailyQuestSlotData in dailyQuestSlotDatas)
        {
            dailyQuestSlotData.isCompleted = false;
            dailyQuestSlotData.isRewarded = false;
            dailyQuestSlotData.currentCount = 0;
        }

        for (int i = 0; i < dailyQuestSlots.Count; i++)
        {
            dailyQuestSlots[i].SetDailyQuestSlot(dailyQuestSlotDatas[i]);
        }

        SortDailyQuestSlots();
    }

    public void InitDailyQuestUI()
    {
        CreateDailyQuest();

        for (int i = 0; i < questPointRewardSlots.Count; i++)
        {
            int index = i;

            questPointRewardSlots[index].SetValue(dailyQuestDatas[0].QuestPointPerRewards[index], dailyQuestDatas[0].QuestPointPerRewardAmount[index], RewardManager.instance.GetRewardInfo(dailyQuestDatas[0].QuestPointPerRewards[index]));
            questPointRewardSlots[index].SetUI(RewardManager.instance.GetRewardInfo(dailyQuestDatas[0].QuestPointPerRewards[index]), dailyQuestDatas[0].QuestPointPerRewardAmount[index]);

            questPointRewardSlots[index].Init();

            questPointRewardSlots[index].rewardButton.onClick.AddListener(() =>
            {
                if (questPointRewardSlots[index].isRewarded)
                    return;
                OnClick_RewardButton(index);
            });
        }


        SetQuestPoint();
    }

    void OnClick_RewardButton(int index)
    {
        if (currentQuestPoint >= dailyQuestDatas[0].QuestPointPerRewardAmount[index])
        {
            RewardManager.instance.GiveReward(dailyQuestDatas[0].QuestPointPerRewards[index], dailyQuestDatas[0].QuestPointPerRewardAmount[index]);
            RewardManager.instance.ShowRewardPanel();


            questPointRewardSlots[index].GetReward();

            SetQuestPoint();
        }
        else
        {
            Debug.Log("퀘스트 포인트가 부족합니다.");
        }
    }


    public void OnClick_DailyQuestButton()
    {
        PlayerPrefs.SetInt(Consts.TOUCH_MISSION, 1);
        QuestManager.instance.UpdateCount(EventQuestType.TouchMission, PlayerPrefs.HasKey(Consts.TOUCH_MISSION) ? 1 : 0, -1);

        for (int i = 0; i < dailyQuestSlots.Count; i++)
        {
            dailyQuestSlots[i].SetDailyQuestSlot(dailyQuestSlotDatas[i]);
        }
        SortDailyQuestSlots();  // 슬롯 정렬 메서드 호출

        SetQuestPoint();


        // y값을 0으로 초기화하여 패널이 위로 올라오도록 함
        dailyQuestSlotParent.localPosition = new Vector3(dailyQuestSlotParent.localPosition.x, 0, dailyQuestSlotParent.localPosition.z);


        dailyQuestPanel.SetActive(true);
    }

    private void CreateDailyQuest()
    {
        foreach (DailyQuestSlotData dailyQuestSlotData in dailyQuestSlotDatas)
        {
            DailyQuestSlot dailyQuestSlot = Instantiate(dailyQuestSlotPrefab, dailyQuestSlotParent);
            dailyQuestSlot.Init();
            dailyQuestSlot.OnCompleteQuest += GiveRewardQuest;
            dailyQuestSlot.OnMove += MoveToQuestPlace;
            dailyQuestSlot.SetDailyQuestSlot(dailyQuestSlotData);
            dailyQuestSlots.Add(dailyQuestSlot);

            if (dailyQuestSlotData.isRewarded)
            {
                currentQuestPoint += dailyQuestSlotData.QuestPoint;
            }
        }
    }

    private void UpdateQuestPoint(int point)
    {
        currentQuestPoint += point;

        SetQuestPoint();
    }

    private void SetQuestPoint()
    {
        questPointText.text = $"{currentQuestPoint}";
        questPointSlider.value = currentQuestPoint;

        int rewardIndex = currentQuestPoint / 20;

        // 반복문을 사용하여 조건을 체크하고 적절한 보상 슬롯을 완료 상태로 설정
        for (int i = 0; i < questPointRewardSlots.Count; i++)
        {
            if (rewardIndex > i)  // rewardIndex가 1부터 시작하므로, i+1을 하지 않고 i와 비교
            {
                questPointRewardSlots[i].SetComplete();
            }
        }
    }

    private void GiveRewardQuest(DailyQuestType questType)
    {
        dailyQuestDataHandler.GiveRewardQuest(questType);

        UpdateQuestPoint(dailyQuestSlotDatas.Find(q => q.type == questType).QuestPoint);

        SortDailyQuestSlots();
    }

    private void MoveToQuestPlace(DailyQuestType questType)
    {
        dailyQuestDataHandler.MoveToQuestPlace(questType);
        dailyQuestPanel.SetActive(false);
    }

    public void SortDailyQuestSlots()
    {
        // 먼저 isCompleted가 true인 슬롯을 정렬하되, 그 중에서 isRewarded가 true인 슬롯을 맨 아래로 내림
        dailyQuestSlots.Sort((slot1, slot2) =>
        {
            bool completed1 = slot1.dailyQuestSlotData.isCompleted;
            bool rewarded1 = slot1.dailyQuestSlotData.isRewarded;
            bool completed2 = slot2.dailyQuestSlotData.isCompleted;
            bool rewarded2 = slot2.dailyQuestSlotData.isRewarded;

            // 둘 다 완료 상태이고 보상도 받았다면 맨 아래로
            if (completed1 && rewarded1 && completed2 && rewarded2) return 0;
            if (completed1 && rewarded1) return 1;  // 첫 번째 slot이 완료되었고 보상 받았으면 아래로
            if (completed2 && rewarded2) return -1; // 두 번째 slot이 완료되었고 보상 받았으면 위로

            // 완료 상태가 같지 않다면, 완료되지 않은 슬롯을 아래로
            if (completed1 && !completed2) return -1;
            if (!completed1 && completed2) return 1;

            return 0;  // 나머지 경우 순서를 유지
        });

        // Transform의 순서를 업데이트하여 UI에 반영
        for (int i = 0; i < dailyQuestSlots.Count; i++)
        {
            dailyQuestSlots[i].transform.SetSiblingIndex(i);
        }
    }
}
