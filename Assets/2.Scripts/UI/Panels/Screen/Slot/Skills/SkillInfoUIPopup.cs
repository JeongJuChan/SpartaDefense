using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoUIPopup : MonoBehaviour
{
    [SerializeField] private Button skillInfoBackgroundButton;

    [Header("TopPanel")]
    [SerializeField] private Image rankImage;
    [SerializeField] private Image skillImage;
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI skillLevelText;
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI skillExp;

    [Header("BottomPanel")]
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI coolTimeText;
    [SerializeField] private TextMeshProUGUI additionalStatText;

    [Header("ButtonPanel")]
    [SerializeField] private Button equipSkillButton;
    [SerializeField] private TextMeshProUGUI equipSkillButtonText;
    [SerializeField] private Button enhanceSkillButton;

    private bool isEquipState = true;

    private SkillUIData skillUIData;

    public event Action<SkillUIData> OnUpdateLevelUI;

    public event Action<int, bool> OnUpdateEquipState;

    public event Func<bool> OnGetIsEquippable;

    public event Action<int> OnEquipSkill;
    public event Action<int> OnUnEquipSkill;
    public event Action<int, int> OnEquipSkillBySlotIndex;

    public event Action<float, float> OnClickEnhanceButtuon;

    public event Func<int, bool> OnGetSkillEquipState;

    public event Action<int> OnSetCacheIndex;

    public event Func<int, int> OnGetEquippedSkillSlotIndex;

    public event Func<int, SkillUpgradableData, SkillUpgradableData> OnGetSkillUpgraded;
    public event Func<int, SkillUpgradableData, SkillUpgradableData> OnGetSkillUpgradedOnce;

    public event Action<bool> OnEquipChoiceState;

    public void Init()
    {
        skillInfoBackgroundButton.onClick.AddListener(CloseUI);
        enhanceSkillButton.onClick.AddListener(OnClickEnhanceButton);

        // QuestManager.instance.GetEventQuestTypeAction.Add(EventQuestType.SkillUpgradeCount, () => { QuestManager.instance.UpdateCount(EventQuestType.SkillUpgradeCount, PlayerPrefs.HasKey("SkillUpgradeCount") ? 1 : 0, -1); });
        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.SkillUpgradeCount, () => { QuestManager.instance.UpdateCount(EventQuestType.SkillUpgradeCount, PlayerPrefs.HasKey("SkillUpgradeCount") ? 1 : 0, -1); });
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }

    public void CloseBackgroundButton()
    {
        OnEquipChoiceState?.Invoke(false);
        skillInfoBackgroundButton.gameObject.SetActive(false);
    }

    public void UpdateSKillInfo(SkillUIData skillUIData)
    {
        this.skillUIData = skillUIData;

        isEquipState = skillUIData.skillUpgradableData.isEquipped;

        //rankImage.sprite = skillUIData.backgroundSprite;
        skillImage.sprite = skillUIData.mainSprite;
        skillText.text = skillUIData.skillNameKR;
        //rankText.text = skillUIData.rankKR;
        //rankText.color = skillUIData.rankColor;
        UpdateSkillLevel();

        string description = skillUIData.description.Replace("{i}", $"{skillUIData.skillUpgradableData.damagePerecent}%");
        if (description.Contains("{j}"))
        {
            description = description.Replace("{j}", $"{skillUIData.skillUpgradableData.targetCount}");
        }
        descriptionText.text = description;

        coolTimeText.text = skillUIData.skillUpgradableData.coolTime.ToString();

        //additionalStatText.text = $"보유효과 : <color=#00FF00>기본HP&공격력&방어력 : +{skillUIData.skillUpgradableData.additionalStatPercent}%</color>";
    }

    public void UpdateEquipButtonText(int index)
    {
        isEquipState = OnGetSkillEquipState.Invoke(index);
        equipSkillButtonText.text = !skillUIData.skillUpgradableData.isEquipped ? "장착" : "해제";
    }

    public void UpdateEquipButtonEvent()
    {
        equipSkillButton.onClick.RemoveAllListeners();

        equipSkillButton.onClick.AddListener(() =>
        {
            if (!skillUIData.skillUpgradableData.isEquipped)
            {
                if (!OnGetIsEquippable.Invoke())
                {
                    OnSetCacheIndex?.Invoke(skillUIData.index);
                    OnEquipChoiceState?.Invoke(true);
                    CloseUI();
                    return;
                }

                OnEquipSkill?.Invoke(skillUIData.index);
                OnEquipChoiceState?.Invoke(false);
                skillUIData.skillUpgradableData.isEquipped = true;
            }
            else
            {
                OnUnEquipSkill?.Invoke(skillUIData.index);
            }

            skillUIData.skillUpgradableData.isEquipped = false;
            OnUpdateEquipState?.Invoke(skillUIData.index, false);
            CloseUI();
        });
    }

    public void UpdateEquipStateForSwitchedSkills(int equipIndex, int unEquipIndex)
    {
        isEquipState = !isEquipState;
        OnUpdateEquipState?.Invoke(equipIndex, isEquipState);
        OnUpdateEquipState?.Invoke(unEquipIndex, !isEquipState);
    }

    private void OnClickEnhanceButton()
    {
        /*if (skillUIData.skillUpgradableData.currentExp < skillUIData.skillUpgradableData.maxExp)
        {
            return;
        }*/

        int slotIndex = OnGetEquippedSkillSlotIndex.Invoke(skillUIData.index);
        if (slotIndex != -1)
        {
            OnUnEquipSkill?.Invoke(skillUIData.index);
            UpdateSkillUI();
            OnEquipSkillBySlotIndex?.Invoke(skillUIData.index, slotIndex);
        }
        else
        {
            UpdateSkillUI();
        }


        QuestManager.instance.UpdateCount(EventQuestType.SkillUpgradeCount, 1, -1);

        PlayerPrefs.SetInt("SkillUpgradeCount", 1);
    }

    private void UpdateSkillUI()
    {
        //float previousStat = skillUIData.skillUpgradableData.additionalStatPercent;
        skillUIData.skillUpgradableData = OnGetSkillUpgradedOnce.Invoke(skillUIData.index, skillUIData.skillUpgradableData);
        //OnClickEnhanceButtuon?.Invoke(previousStat, skillUIData.skillUpgradableData.additionalStatPercent);

        UpdateSkillLevel();
        OnUpdateLevelUI?.Invoke(skillUIData);
        UpdateSKillInfo(skillUIData);
    }

    public void EnhanceSkillAuto(SkillUIData skillUIData)
    {
        /*if (skillUIData.skillUpgradableData.currentExp < skillUIData.skillUpgradableData.maxExp)
        {
            return;
        }*/

        QuestManager.instance.UpdateCount(EventQuestType.SkillUpgradeCount, 1, -1);

        PlayerPrefs.SetInt("SkillUpgradeCount", 1);

        if (skillUIData.skillUpgradableData.isEquipped)
        {
            int slotIndex = OnGetEquippedSkillSlotIndex.Invoke(skillUIData.index);
            OnUnEquipSkill?.Invoke(skillUIData.index);
            skillUIData.skillUpgradableData = OnGetSkillUpgraded.Invoke(skillUIData.index, skillUIData.skillUpgradableData);
            OnUpdateLevelUI?.Invoke(skillUIData);
            OnEquipSkillBySlotIndex?.Invoke(skillUIData.index, slotIndex);
        }
        else
        {
            skillUIData.skillUpgradableData = OnGetSkillUpgraded.Invoke(skillUIData.index, skillUIData.skillUpgradableData);
            OnUpdateLevelUI?.Invoke(skillUIData);
        }

        UpdateSKillInfo(skillUIData);
    }

    private void UpdateSkillLevel()
    {
        /*expSlider.value = (float)skillUIData.skillUpgradableData.currentExp / skillUIData.skillUpgradableData.maxExp;
        skillLevelText.text = $"LV. {skillUIData.skillUpgradableData.level}";
        skillExp.text = $"{skillUIData.skillUpgradableData.currentExp} / {skillUIData.skillUpgradableData.maxExp}";*/
    }
}
