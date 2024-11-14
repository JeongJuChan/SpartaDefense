using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageProgressPanel : MonoBehaviour, UIInitNeeded
{
    [SerializeField] private Button challengeBossBtn;
    [SerializeField] private Slider stageProgressBar;
    [SerializeField] private Image[] progressCircles;
    [SerializeField] private Color progressColor = Color.green;

    private StageController stageController;
    private PlaceEventSwitcher placeEventSwitcher;

    public void Init()
    {
        stageController = FindAnyObjectByType<StageController>();
        placeEventSwitcher = FindAnyObjectByType<PlaceEventSwitcher>();
        stageProgressBar.fillRect.GetComponent<Image>().color = progressColor;
        stageController.OnChallengeBoss = challengeBossBtn.onClick;
        placeEventSwitcher.OnActiveProgress += ActiveSelf;

        stageController.OnResetRoutineStage += ResetUI;
        stageController.OnUpdateRoutineStage += UpdateProgress;
        stageController.OnDefeatAfterBossEncountered += SwitchRoutineStageUI;
        stageController.OnBossChallenged += CloseBossBtn;

        ResetUI();
    }

    private void ActiveSelf(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    private void ResetUI()
    {
        stageProgressBar.value = 0f;

        for (int i = 0; i < progressCircles.Length; i++)
        {
            if (i == progressCircles.Length - 1)
            {
                return;
            }

            progressCircles[i].gameObject.SetActive(false);
            progressCircles[i].color = Color.white;
        }
    }

    private void UpdateProgress(int routineStageNum)
    {
        for (int i = 0; i < routineStageNum; i++)
        {
            if (i == progressCircles.Length - 1)
            {
                break;
            }

            progressCircles[i].gameObject.SetActive(true);
            progressCircles[i].color = Color.white;
        }

        for (int i = 0; i < routineStageNum - 1; i++)
        {
            progressCircles[i].color = progressColor;
        }

        float ratio = (float)(routineStageNum - 1) / (progressCircles.Length - 1);
        stageProgressBar.value = ratio;
    }

    private void SwitchRoutineStageUI(bool isBossButtonActive)
    {
        challengeBossBtn.gameObject.SetActive(isBossButtonActive);
        stageProgressBar.gameObject.SetActive(!isBossButtonActive);
        /*foreach (var circle in progressCircles)
        {
            circle.gameObject.SetActive(!isBossButtonActive);
        }*/
    }

    private void CloseBossBtn()
    {
        challengeBossBtn.gameObject.SetActive(false);   
    }
}
