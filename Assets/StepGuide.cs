using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepGuide : MonoBehaviour
{
    [SerializeField] private List<GuideStep> guideSteps;
    [SerializeField] private EventQuestType questType;

    private int currentStepIndex = -1;
    private QuestManager questManager;

    private bool isStarted = false;

    void Start()
    {
        questManager = QuestManager.instance;
        questManager.OnStepQuestStarted += OnQuestStarted;
        questManager.OnStepQuestCompleted += OnQuestCompleted;
    }


    private void OnQuestStarted(EventQuestType questType)
    {
        if (this.questType == questType)
        {
            InitializeGuides();
            isStarted = true;
        }
    }

    private void InitializeGuides()
    {
        foreach (var step in guideSteps)
        {
            step.Hide();
        }

        if (guideSteps.Count > 0)
        {
            currentStepIndex = 0;
            guideSteps[currentStepIndex].Show();
        }
    }

    private void OnQuestCompleted(EventQuestType questType)
    {
        if (this.questType == questType)
        {
            NextStep(4);
        }
    }

    public void NextStep(int index)
    {
        if (!isStarted)
        {
            return;
        }
        if (index < currentStepIndex) return;

        if (currentStepIndex >= 0 && currentStepIndex < guideSteps.Count)
        {
            guideSteps[currentStepIndex].Hide();
            currentStepIndex++;
            if (currentStepIndex < guideSteps.Count)
            {
                guideSteps[currentStepIndex].Show();
            }
            else
            {
                Debug.Log("All guide steps completed.");
                isStarted = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (questManager != null)
        {
            questManager.OnStepQuestStarted -= OnQuestStarted;
            questManager.OnStepQuestCompleted -= OnQuestCompleted;
        }
    }
}

