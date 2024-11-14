using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoSkillToggle : MonoBehaviour
{
    [SerializeField] private Button autoSkillButton;
    [SerializeField] private TextMeshProUGUI autoSkillText;
    [SerializeField] private Image autoActiveImage;
    [SerializeField] private Sprite autoActiveSprite;

    [SerializeField] private GuideController guideController;

    private Sprite offsetButtonSprite;

    private bool isAutoActive;

    public event Action<bool> OnUpdateAutoSkillActive;

    public void Init()
    {
        offsetButtonSprite = autoSkillButton.image.sprite;
        autoSkillButton.onClick.AddListener(ToggleAutoSkillButton);

        LoadAutoSkillActive();

        guideController.Initialize();

        QuestManager.instance.AddEventQuestTypeAction(EventQuestType.AutoSkillButton, () => { QuestManager.instance.UpdateCount(EventQuestType.AutoSkillButton, PlayerPrefs.HasKey("AutoSkillButton") ? 1 : 0, -1); });
    }

    private void ToggleAutoSkillButton()
    {
        isAutoActive = !isAutoActive;

        SetAutoSkillActive();

        QuestManager.instance.UpdateCount(EventQuestType.AutoSkillButton, 1, -1);

        PlayerPrefs.SetInt("AutoSkillButton", 1);

        SaveAutoSkillActive();
    }

    private void SetAutoSkillActive()
    {
        if (isAutoActive)
        {
            autoSkillButton.image.sprite = autoActiveSprite;
            autoSkillText.text = "Auto\non";
        }
        else
        {
            autoSkillButton.image.sprite = offsetButtonSprite;
            autoSkillText.text = "Auto\noff";
        }

        OnUpdateAutoSkillActive?.Invoke(isAutoActive);
    }

    private void SaveAutoSkillActive()
    {
        ES3.Save<int>(Consts.IS_AUTO_SKILL_ACITVE, isAutoActive ? 1 : 0, ES3.settings);
        ES3.StoreCachedFile();
    }

    private void LoadAutoSkillActive()
    {
        isAutoActive = ES3.Load<int>(Consts.IS_AUTO_SKILL_ACITVE, 0) == 1;

        SetAutoSkillActive();
    }
}
