using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankProbabilityUIPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI currentRankProbabilityText;

    [SerializeField] private TextMeshProUGUI nextRankProbabilityText;

    [SerializeField] private Image rankBackgroundImage;
    [SerializeField] private Image nextLevelRankBackgroundImage;

    public void InitSprites(Sprite rankSprite)
    {
        rankBackgroundImage.sprite = rankSprite;
        nextLevelRankBackgroundImage.sprite = rankSprite;
    }

    public void SetOffNextLevelRankBackgroundImage()
    {
        nextLevelRankBackgroundImage.gameObject.SetActive(false);   
    }

    public void UpdateCurrentRankProbabilityTexts(string rankKr, int currentProbability)
    {
        rankText.text = rankKr;
        currentRankProbabilityText.text = $"{currentProbability * Consts.PERCENT_MUTIPLY_VALUE}%";
    }

    public void UpdateNextRankProbabilityTexts(string rankKr, int nextProbability)
    {
        rankText.text = rankKr;
        nextRankProbabilityText.text = $"{nextProbability * Consts.PERCENT_MUTIPLY_VALUE}%";
    }
}
