using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrowthLevelPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI growthLevelText;
    [SerializeField] private Image trainingFillImage;
    [SerializeField] private TextMeshProUGUI progressText;

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    public event Action<int> OnChangeLevelInfo;

    public void Init()
    {
        leftButton.onClick.AddListener(() => OnChangeLevelInfo?.Invoke(-1));
        rightButton.onClick.AddListener(() => OnChangeLevelInfo?.Invoke(1));
    }

    public void UpdateButtonsActive(bool isLeftActive, bool isRightActive)
    {
        leftButton.gameObject.SetActive(isLeftActive);
        rightButton.gameObject.SetActive(isRightActive);
    }

    public void UpdateGrowthLevelUI(int growthLevel, bool isCurrentGrowthData)
    {
        trainingFillImage.fillAmount = isCurrentGrowthData ? 1f : 0f;
        growthLevelText.text = $"훈련 {growthLevel}단계";
    }

    public void UpdateProgressUI(int totalPartLevel, int goalLevel)
    {
        trainingFillImage.fillAmount = (float)totalPartLevel / goalLevel;
        progressText.text = $"{totalPartLevel}/{goalLevel}";
    }

    public void SetLevelMaxProgressUI()
    {
        progressText.text = $"<color=orange>최대 레벨입니다!</color>";
        trainingFillImage.fillAmount = 1f;
    }
}