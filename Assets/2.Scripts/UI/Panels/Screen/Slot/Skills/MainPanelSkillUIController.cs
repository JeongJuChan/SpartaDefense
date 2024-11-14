using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MainPanelSkillUIController : MonoBehaviour
{
    [SerializeField] private AutoSkillToggle autoSkillToggle;
    [SerializeField] private ColleagueDataSO colleagueDataSO;
    [SerializeField] private MainPanelSkillUIElement[] mainPanelSkillUIElements;
    [SerializeField] private GuideController guideController;
    [SerializeField] private float skillDelayDuration = 0.5f;

    private SkillEquipUIController skillEquipUIController;

    private SkillManager skillManager;

    private Queue<MainPanelSkillUIElement> skillElements = new Queue<MainPanelSkillUIElement>();

    private bool isAuto = false;

    private event Action OnActiveAutoSkill;

    private int preSkillIndex = -1;

    private WaitForSeconds skillDelayWaitForSeconds;

    public event Func<bool> OnGetActiveTargetsExist;

    private const string DEFAULT_COLLEAGUE_SLOT_FEATURE_ID = "ColleagueSlot_";

    private void Update()
    {
        TryUsingSkillAuto();
    }

    private void TryUsingSkillAuto()
    {
        if (isAuto)
        {
            if (skillElements.Count > 0)
            {
                if (OnGetActiveTargetsExist.Invoke())
                {
                    MainPanelSkillUIElement mainPanelSkillUIElement = skillElements.Peek();
                    int skillIndex = mainPanelSkillUIElement.GetSkillIndex();
                    if (skillIndex == -1)
                    {
                        skillElements.Dequeue();
                        return;
                    }

                    if (preSkillIndex != skillIndex)
                    {
                        preSkillIndex = skillIndex;
                        StartCoroutine(CoUseSkillDelay(mainPanelSkillUIElement));
                    }
                }
            }
        }
    }

    private IEnumerator CoUseSkillDelay(MainPanelSkillUIElement mainPanelSkillUIElement)
    {
        yield return skillDelayWaitForSeconds;
        TryToUseSkill(mainPanelSkillUIElement);
        preSkillIndex = -1;
    }

    private void TryToUseSkill(MainPanelSkillUIElement mainPanelSkillUIElement)
    {
        bool isPossible = mainPanelSkillUIElement.TryUsingSkill();
        if (isPossible)
        {
            if (skillElements.Count > 0)
            {
                skillElements.Dequeue();
            }
        }
    }

    public void Init()
    {
        skillDelayWaitForSeconds = CoroutineUtility.GetWaitForSeconds(skillDelayDuration);

        skillManager = FindAnyObjectByType<SkillManager>();

        UI_Colleague ui_Colleague = UIManager.instance.GetUIElement<UI_Colleague>();
        ui_Colleague.OnOpenColleagueIndexSlot += OpenColleagueSlot;
        ui_Colleague.OnColleagueAdded += OnColleagueEquipped;
        ui_Colleague.OnUnEquipColleagueSlot += OpenColleagueSlot;

        for (int i = 0; i < mainPanelSkillUIElements.Length; i++)
        {
            mainPanelSkillUIElements[i].OnUseSkill += skillManager.UseSkill;
            mainPanelSkillUIElements[i].OnEnqueueUsingSkillElement += EnqueueUsingSkill;
            OnActiveAutoSkill += mainPanelSkillUIElements[i].OnChangeAutoSkillState;
            mainPanelSkillUIElements[i].Init();
            if (i > 0)
            {
                InitElementUnlockData(i);
            }
        }

        autoSkillToggle.OnUpdateAutoSkillActive += UpdateAutoSkill;

        autoSkillToggle.Init();
        guideController.Initialize();
    }

    private void InitElementUnlockData(int i)
    {
        FeatureID featureID = EnumUtility.GetEqualValue<FeatureID>($"{DEFAULT_COLLEAGUE_SLOT_FEATURE_ID}{i + 1}");
        UnlockData unlockData = ResourceManager.instance.unlockDataSO.GetUnlockData(featureID);
        mainPanelSkillUIElements[i].SetUnlockDataValues(unlockData.featureType, unlockData.count);
    }

    private void OpenColleagueSlot(int slotIndex)
    {
        UpdateSkillUIImage(slotIndex, null);
        UpdateColleagueData(slotIndex, new ColleagueData(-1, -1, null, null, new ColleagueInfo(Rank.None, ColleagueType.None), ShootingType.Parabola));
    }

    private void OnColleagueEquipped(int slotIndex, int index)
    {
        ColleagueData colleagueData = colleagueDataSO.GetColleagueData(index);
        Sprite mainSprite = ResourceManager.instance.slotHeroData.GetResource(colleagueData.colleagueInfo).defaultSprite;
        Sprite backgroundSprite = ResourceManager.instance.rank.GetRankBackgroundSprite(colleagueData.colleagueInfo.rank);
        float skillCoolTime = skillManager.GetSkillCoolTime(colleagueData.skillIndex);
        UpdateColleagueData(slotIndex, colleagueData);
        UpdateSkillCoolTime(slotIndex, skillCoolTime);
        UpdateSkillUIImage(slotIndex, mainSprite, backgroundSprite);
    }

    private void UpdateSkillUIImage(int index, Sprite mainSprite)
    {
        mainPanelSkillUIElements[index].UpdateMainSprite(mainSprite);
        mainPanelSkillUIElements[index].ResetBackgroundSprite();
    }

    private void UpdateSkillUIImage(int index, Sprite mainSprite, Sprite backgroundSprite)
    {
        mainPanelSkillUIElements[index].UpdateMainSprite(mainSprite);
        mainPanelSkillUIElements[index].UpdateBackgroundSprite(backgroundSprite);
    }

    private void UpdateColleagueData(int index, ColleagueData colleagueData)
    {
        mainPanelSkillUIElements[index].UpdateColleagueData(colleagueData);
    }

    private void UpdateSkillCoolTime(int index, float coolTime)
    {
        mainPanelSkillUIElements[index].UpdateCoolTimeState(coolTime);
    }

    private void UpdateAutoSkill(bool isActive)
    {
        isAuto = isActive;
        if (isAuto)
        {
            OnActiveAutoSkill?.Invoke();
        }
    }

    private void EnqueueUsingSkill(MainPanelSkillUIElement mainPanelSkillUIElement)
    {
        if (!skillElements.Contains(mainPanelSkillUIElement) && mainPanelSkillUIElement.GetSkillIndex() != -1)
        {
            skillElements.Enqueue(mainPanelSkillUIElement);
        }
     }
}
