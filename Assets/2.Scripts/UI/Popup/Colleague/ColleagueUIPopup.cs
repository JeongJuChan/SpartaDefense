using Keiwando.BigInteger;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColleagueUIPopup : MonoBehaviour
{
    [SerializeField] private Image colleagueImage;
    [SerializeField] private Image[] colleagueStars;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject organizationPanel;

    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private Image classImage;

    [SerializeField] private TextMeshProUGUI skillTypeStatText;
    [SerializeField] private TextMeshProUGUI damageStatText;
    [SerializeField] private TextMeshProUGUI healthStatText;
    [SerializeField] private TextMeshProUGUI defenseStatText;

    [SerializeField] private Image skillImage;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;

    [SerializeField] private TextMeshProUGUI advancementText;
    [SerializeField] private TextMeshProUGUI levelUptext;

    [SerializeField] private Button advanceButton;
    [SerializeField] private Button levelUpButton;
    [SerializeField] private Image levelUpImage;
    [SerializeField] private Button closeButton;

    [SerializeField] private Button disablePanel;

    [SerializeField] private ColleagueIdlePanel colleagueIdlePanel;

    [SerializeField] private Image redDot;

    [SerializeField] private ParticleSystem levelUpParticle;
    [SerializeField] private ParticleSystem advanceParticle;

    public event Action<int> OnClickAdvanceButton;
    public event Action<int> OnClickLevelUpButton;

    public event Func<int, bool> OnGetIsAdvanceButtonInteractable;
    public event Func<int, bool> OnGetIsLevelUpButtonInteractable;

    public event Func<int, bool> OnGetColleagueData;
    public event Func<int, ColleagueUpgradableData> OnGetColleagueUpgradableData;

    public event Func<Rank, int, BigInteger> OnGetTotalLevelUpCost;

    public event Func<int, int> OnGetMaxLevel;

    public event Func<int, ColleagueUpgradableData, int> OnGetAdvanceCost;

    public event Func<int, SkillUpgradableData> OnGetSkillUpgradableData;

    private int index = -1;
    private BigInteger levelUpCost;

    private Rank currentRank = Rank.None;

    private Sprite normalStarSprite;
    private Sprite advancedStarSprite;

    private void OnEnable()
    {
        if (index != -1)
        {
            UpdateAdvanceButtonInteractable();
            UpdateLevelUpButtonInteractable();
        }
    }

    public void Init()
    {
        disablePanel.onClick.AddListener(() => ActivateSelf(false));
        advanceButton.onClick.AddListener(AdvanceColleague);
        levelUpButton.onClick.AddListener(LevelUpColleague);
        closeButton.onClick.AddListener(() => ActivateSelf(false));
        Currency currency = CurrencyManager.instance.GetCurrency(CurrencyType.ColleagueLevelUpStone);
        currency.OnCurrencyChange += UpdateLevelUpText;
        levelUpImage.sprite = currency.GetIcon();
        ResetStarUI();
        colleagueIdlePanel.Init();
        StatDataHandler.Instance.OnUpdateColleaguePower += UpdatePowerText;
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

    public void UpdateColleageUIPopup(ColleagueUIData colleagueUIData)
    {
        index = colleagueUIData.index;
        Rank rank = colleagueUIData.rank;
        currentRank = rank;
        //colleagueBackgroundImage.sprite = colleagueUIData.backgroundSprite;
        colleagueImage.sprite = colleagueUIData.mainSprite;
        rankText.text = EnumToKRManager.instance.GetEnumToKR(rank);
        rankText.color = ResourceManager.instance.rank.GetRankColor(rank);
        nameText.text = colleagueUIData.colleagueNameKR;
        Sprite colleagueIcon = ResourceManager.instance.colleagueIconDataSO.GetColleagueIcon(colleagueUIData.colleagueType);
        classImage.sprite = colleagueIcon;
        organizationPanel.SetActive(OnGetColleagueData.Invoke(colleagueUIData.index));

        ColleagueUpgradableData colleagueUpgradableData = OnGetColleagueUpgradableData.Invoke(colleagueUIData.index);
        UpdateDefaultStatTexts(colleagueUpgradableData);

        UpdateLevelText(colleagueUpgradableData);
        UpdateAdvancementPartCount(colleagueUpgradableData, OnGetAdvanceCost.Invoke(index, colleagueUpgradableData));
        UpdateLevelUpCost(colleagueUpgradableData);
        UpdateLevelUpText(CurrencyManager.instance.GetCurrencyValue(CurrencyType.ColleagueLevelUpStone));

        UpdateStarCount(colleagueUpgradableData, colleagueStars.Length);

        int skillIndex = colleagueUIData.skillIndex;
        SkillData skillData = ResourceManager.instance.skill.skillDataSO.GetSkillData(skillIndex);
        skillTypeStatText.text = $"{EnumToKRManager.instance.GetEnumToKR(skillData.skillTargetingType)}";
        skillImage.sprite = ResourceManager.instance.skill.GetSkillResourceData(skillIndex).skillIcon;

        SkillUpgradableData skillUpgradableData = OnGetSkillUpgradableData.Invoke(skillIndex);

        UpdatePowerText(colleagueUpgradableData.power);
        //StatDataHandler.Instance.GetColleaguePower(colleagueUIData.colleagueType, colleagueUpgradableData)

        string description = skillData.description.Replace("{i}", $"{skillUpgradableData.damagePerecent}%");
        if (description.Contains("{j}"))
        {
            description = description.Replace("{j}", $"{skillUpgradableData.targetCount}");
        }

        skillNameText.text = skillData.skillNameKR;
        skillDescriptionText.text = description;

        ActivateSelf(true);

        colleagueIdlePanel.UpdateIndex(colleagueUIData.colleagueType, rank);
    }

    private void UpdatePowerText(ColleagueUpgradableData colleagueUpgradableData)
    {
        powerText.text = $"{colleagueUpgradableData.power}";
    }

    public void UpdatePowerText(BigInteger power)
    {
        powerText.text = power.ToString();
    }

    public void UpdateLevelUpCost(ColleagueUpgradableData colleagueUpgradableData)
    {
        levelUpCost = OnGetTotalLevelUpCost.Invoke(currentRank, colleagueUpgradableData.level);
        UpdateLevelUpText(CurrencyManager.instance.GetCurrencyValue(CurrencyType.ColleagueLevelUpStone));
    }

    public void UpdateAdvancementPartCount(ColleagueUpgradableData colleagueUpgradableData, int cost)
    {
        advancementText.text = cost == 0 ? colleagueUpgradableData.count.ToString() : $"{colleagueUpgradableData.count} / {cost}";
        redDot.enabled = cost == 0 ? false : colleagueUpgradableData.count >= cost;
        UpdateAdvanceButtonInteractable();
    }

    public void UpdateDefaultStatTexts(ColleagueUpgradableData colleagueUpgradableData)
    {
        damageStatText.text = colleagueUpgradableData.damage.ToString();
        healthStatText.text = colleagueUpgradableData.health.ToString();
        defenseStatText.text = colleagueUpgradableData.defense.ToString();
    }

    public void ActivateSelf(bool isActive)
    {
        gameObject.SetActive(isActive);
        disablePanel.gameObject.SetActive(isActive);
    }

    public void UpdateLevelText(ColleagueUpgradableData colleagueUpgradableData)
    {
        levelText.text = $"LV.{colleagueUpgradableData.level} / {OnGetMaxLevel.Invoke(colleagueUpgradableData.starCount)}";
    }

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

        UpdateAdvanceButtonInteractable();
    }

    private void ResetStarUI()
    {
        for (int i = 0; i < colleagueStars.Length; i++)
        {
            colleagueStars[i].sprite = normalStarSprite;
            colleagueStars[i].color = Consts.DISABLE_COLOR;
        }
    }

    private void UpdateLevelUpText(BigInteger levelUpStoneCount)
    {
        levelUptext.text = $"{levelUpStoneCount} / {levelUpCost}";

        if (index == -1)
        {
            return;
        }

        UpdateLevelUpButtonInteractable();
    }

    private void AdvanceColleague()
    {
        OnClickAdvanceButton?.Invoke(index);
        advanceParticle.Play();
    }

    private void LevelUpColleague()
    {
        OnClickLevelUpButton?.Invoke(index);
        levelUpParticle.Play();
    }

    private void UpdateAdvanceButtonInteractable()
    {
        advanceButton.interactable = OnGetIsAdvanceButtonInteractable.Invoke(index);
        NotificationManager.instance.SetNotification(RedDotIDType.ShowColleagueButton, advanceButton.interactable);
        advanceButton.image.color = advanceButton.interactable ? Color.white : Color.gray;
        advancementText.color = advanceButton.interactable ? Color.white : Color.red;
    }

    private void UpdateLevelUpButtonInteractable()
    {
        levelUpButton.interactable = OnGetIsLevelUpButtonInteractable.Invoke(index);
        levelUpButton.image.color = levelUpButton.interactable ? Color.white : Color.gray;
        levelUptext.color = levelUpButton.interactable ? Color.white : Color.red;
    }
}
