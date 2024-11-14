using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageInfoText;

    private StageController stageController;
    private DungeonController dungeonController;

    private void Awake()
    {
        stageController = FindAnyObjectByType<StageController>();
        dungeonController = FindAnyObjectByType<DungeonController>();
    }

    private void OnEnable()
    {
        stageController.OnUpdateStageText += UpdateUI;
        dungeonController.OnUpdateDungeonName += UpdateUI;
    }

    private void OnDisable()
    {
        stageController.OnUpdateStageText -= UpdateUI;
    }

    private void UpdateUI(int difficulty, int mainStageNum, int subStageNum)
    {
        string difficultyStr = Difficulty.GetDifficulty(difficulty);
        stageInfoText.text = $"{difficultyStr} Stage {mainStageNum}-{subStageNum}";
    }

    private void UpdateUI(string dungeonName)
    {
        stageInfoText.text = dungeonName;
    }
}
