using Keiwando.BigInteger;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColleagueBottomUIElement : ColleagueBaseUIElement
{
    [SerializeField] private TextMeshProUGUI equipmentText;
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private Image[] colleagueStars;
    [SerializeField] private Image redDot;

    public event Action<ColleagueUIData> OnClickEquipButton;

    protected Sprite addSprite;

    public event UnityAction OnEquip;
    public event UnityAction OnUnEquip;

    private Sprite normalStarSprite;
    private Sprite advancedStarSprite;

    public override void Init()
    {
        base.Init();
        ResetStarUI();
        equipStateButton.interactable = false;
    }

    public void InitStars(Sprite normalStar, Sprite advancedStar)
    {
        normalStarSprite = normalStar;
        advancedStarSprite = advancedStar;

        foreach (Image star in colleagueStars)
        {
            star.sprite = normalStarSprite;
        }
    }

    public void UpdateSummonAmount(ColleagueUpgradableData colleagueUpgradableData, int advanceCost)
    {
        if (!disablePanel.gameObject.activeInHierarchy && equipStateButton.image.sprite != subtractSprite)
        {
            equipStateButton.interactable = true;
            disablePanel.gameObject.SetActive(false);
            equipStateButton.image.sprite = addSprite;
            equipStateButton.onClick.AddListener(OnEquip);
            lockIcon.SetActive(false);
            if (colleagueUIData.colleagueType != ColleagueType.Rtan_Rare)
            {
                equipStateButton.gameObject.SetActive(true);
            }
        }

        UpdateLevelText(colleagueUpgradableData.level);
        UpdateAdvanceUI(colleagueUpgradableData.count, advanceCost);
    }

    public void UpdateAdvanceUI(BigInteger count, int advanceCost)
    {
        expText.text = advanceCost == 0 ? count.ToString() : $"{count} / {advanceCost}";
        expSlider.value = advanceCost == 0 ? 1 : count.ToFloat() / advanceCost;

        redDot.enabled = advanceCost == 0 ? false : count.ToInt() >= advanceCost;
    }

    public bool GetReddotEnabled()
    {
        return redDot.enabled;
    }

    /*public void UpdateStarCount(ColleagueUpgradableData colleagueUpgradableData, int starUnitCount)
    {
        ResetStarUI();

        for (int i = 0; i < colleagueUpgradableData.starCount; i++)
        {
            colleagueStars[i].color = Color.white;
        }
    }*/

    public void UpdateStarCount(ColleagueUpgradableData colleagueUpgradableData, int starUnitCount)
    {
        ResetStarUI();

        int enableRepeatCount = colleagueUpgradableData.starCount >= starUnitCount ?
            starUnitCount : colleagueUpgradableData.starCount;

        for (int i = 0; i < enableRepeatCount; i++)
        {
            colleagueStars[i].color = Color.white;
        }

        if (colleagueUpgradableData.starCount > starUnitCount)
        {
            int rest = colleagueUpgradableData.starCount % starUnitCount;
            int advancedCount = rest == 0 ? starUnitCount : rest;

            for (int i = 0; i < advancedCount; i++)
            {
                colleagueStars[i].sprite = advancedStarSprite;
            }
        }
    }

    public void SetAddSprite(Sprite addSprite)
    {
        this.addSprite = addSprite;
    }

    // Update Data for this class
    public override void UpdateColleagueUIData(ColleagueUIData colleagueUIData)
    {
        this.colleagueUIData = colleagueUIData;
        mainImage.sprite = colleagueUIData.mainSprite;
        colleaguePopupButton.image.sprite = colleagueUIData.backgroundSprite;
        equipStateButton.gameObject.SetActive(false);
        //equipStateButton.image.sprite = subtractSprite;
        //UpdateSummonAmount();
    }

    public void Equip()
    {
        equipStateButton.image.sprite = subtractSprite;
        equipStateButton.onClick.RemoveAllListeners();
        equipStateButton.onClick.AddListener(OnUnEquip);
    }

    public void UnEquip()
    {
        equipStateButton.image.sprite = addSprite;
        equipStateButton.onClick.RemoveAllListeners();
        equipStateButton.onClick.AddListener(OnEquip);
    }

    private void ResetStarUI()
    {
        for (int i = 0; i < colleagueStars.Length; i++)
        {
            colleagueStars[i].color = Consts.DISABLE_COLOR;
        }
    }
}
