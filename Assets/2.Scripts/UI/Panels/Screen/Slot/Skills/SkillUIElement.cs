using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIElement : MonoBehaviour
{
    [SerializeField] protected SkillUIData skillUIData;
    [SerializeField] protected Image mainImage;
    [SerializeField] protected Button backgroundButton;
    [SerializeField] protected Image equipIcon;
    [SerializeField] protected Image lockIcon;

    [SerializeField] protected TextMeshProUGUI levelText;
    [SerializeField] protected TextMeshProUGUI expText;
    [SerializeField] protected Slider expSlider;

    [SerializeField] protected GameObject equipmentText;
    [SerializeField] protected Button equipStateButton;

    [SerializeField] private GameObject disablePanel;

    protected bool isFirstActive = true;

    public event Action OnShowUIPopup;

    public event Action<SkillUIData> OnUpdateSkillInfo;

    public event Action<int> OnUpdateEquipButtonText;

    private bool isSkillSummoned;

    public event Action OnClickEquipEvent;

    protected virtual void Awake()
    {
        backgroundButton.onClick.AddListener(ShowSkillInfoPopup);
    }

    public int GetIndex()
    {
        return skillUIData.index;
    }

    public virtual void OnSlotOpened()
    {
        skillUIData.index = -1;
        ChangeActiveStateSkillEquipmentSlot(false);
        ChangeActiveStateMainSprite(false);
        ChangeActiveStateLockIcon(false);
        ChangeActiveStateEquipIcon(true);
    }

    public void ChangeActiveStateLockIcon(bool isActive)
    {
        lockIcon.gameObject.SetActive(isActive);
    }

    public void ChangeActiveStateEquipIcon(bool isActive)
    {
        equipIcon.gameObject.SetActive(isActive);
    }

    public void ChangeActiveStateEquipText(bool isActive)
    {
        equipmentText.gameObject.SetActive(isActive);
    }

    public void ChangeActiveStateMainSprite(bool isActive)
    {
        mainImage.gameObject.SetActive(isActive);
    }

    public void ChangeActiveStateSkillEquipmentSlot(bool isActive)
    {
        expSlider.gameObject.SetActive(isActive);
        equipStateButton.gameObject.SetActive(isActive);
        levelText.gameObject.SetActive(isActive);
    }

    public void UpdateSkill(SkillUIData skillUIData, Action<int> onClickEvent, bool isEquipPanel)
    {
        this.skillUIData = skillUIData;

        isSkillSummoned = onClickEvent != null;

        if (isSkillSummoned)
        {
            equipStateButton.onClick.RemoveAllListeners();

            equipStateButton.onClick.AddListener(() =>
            {
                Debug.Log(name);
                onClickEvent?.Invoke(skillUIData.index);
            });

            OnClickEquipEvent?.Invoke();
        }

        UpdateUI(isEquipPanel);

    }

    /*public Rank GetRank()
    {
        return skillUIData.rank;
    }*/

    /*public int GetLevel()
    {
        return skillUIData.skillUpgradableData.level;
    }*/

    public bool GetIsEquiped()
    {
        return skillUIData.skillUpgradableData.isEquipped;
    }

    protected virtual void UpdateUI(bool isEquipPanel)
    {
        mainImage.sprite = skillUIData.mainSprite;
        /*backgroundButton.image.sprite = skillUIData.backgroundSprite;
        levelText.text = $"Lv. {skillUIData.skillUpgradableData.level}";

        disablePanel.SetActive(skillUIData.skillUpgradableData.level == 0);

        int maxExp = skillUIData.skillUpgradableData.maxExp;*/

        if (isEquipPanel)
        {
            mainImage.gameObject.SetActive(true);
            expSlider.gameObject.SetActive(false);
            ChangeActiveStateEquipText(false);
            expText.gameObject.SetActive(false);
        }

        /*expText.text = $"{skillUIData.skillUpgradableData.currentExp} / {maxExp}";
        expSlider.value = (float)skillUIData.skillUpgradableData.currentExp / maxExp;*/
        equipStateButton.image.sprite = skillUIData.equipStateSprite;
    }

    public void UpdateSkillStatUI(SkillUIData skillUIData)
    {
        if (this.skillUIData.index != skillUIData.index)
        {
            return;
        }

        this.skillUIData = skillUIData;

        /*levelText.text = $"Lv. {skillUIData.skillUpgradableData.level}";

        int maxExp = skillUIData.skillUpgradableData.level + 1;

        expText.text = $"{skillUIData.skillUpgradableData.currentExp} / {maxExp}";
        expSlider.value = (float)skillUIData.skillUpgradableData.currentExp / maxExp;*/
    }

    protected void ShowSkillInfoPopup()
    {
        if (isSkillSummoned)
        {
            if (GetIndex() == -1)
            {
                return;
            }

            OnShowUIPopup?.Invoke();
            OnUpdateSkillInfo?.Invoke(skillUIData);
            OnUpdateEquipButtonText?.Invoke(skillUIData.index);
        }
    }
}
