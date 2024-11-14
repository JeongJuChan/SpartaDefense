using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillEquipUIController : UI_Base
{
    [SerializeField] private SkillDataSO skillDataSO;

    [SerializeField] private GameObject skillCanvas;

    [SerializeField] private SkillUIElement skillIconPrefab;
    [SerializeField] private Transform parent;

    [SerializeField] private SkillTopUIElement[] topEquipSkillUIElements;

    [SerializeField] private Sprite equipSprite;
    [SerializeField] private Sprite unEquipSprite;
    [SerializeField] private Sprite lockSprite;

    [SerializeField] private SkillInfoUIPopup skillInfoUIPopup;

    [SerializeField] private Button topBarBackgroundButton;

    [SerializeField] private SkillAutoButtonsUIPanel skillAutoButtonsUIPanel;

    [SerializeField] private TextMeshProUGUI additionalStatText;
    [SerializeField] private GuideController guide;

    [SerializeField] private Sprite openSprite;

    private Dictionary<int, SkillUIElement> totalSkillUIElementDict;

    private Dictionary<int, SkillUIData> skillUIDataDict = new Dictionary<int, SkillUIData>();

    private event Func<int, SkillData> OnGetBaseSkillData;

    private SkillManager skillManager;

    public event Func<Rank, Sprite> OnGetRankBackgroundSprite;
    public event Func<Rank, Color> OnGetRankColor;

    private int openSlotIndex = 0;

    private bool[] equipProbabilityArr;

    public event Action<int, Sprite> OnUpdateSkillImage;
    public event Action<int, int> OnUpdateSkillIndex;
    public event Action<int, float, bool> OnUpdateSkillCoolTime;

    private Dictionary<int, bool> skillEquipStateDict = new Dictionary<int, bool>();

    private int skillInfoCacheIndex;

    private event Action<int, int> OnUpdateEquipStateForSkillsSwitched;

    private List<SkillUIElement> skillUIElements;

    private Action<SkillUIData> OnEnhanceSkillAuto;

    public event Action<int, SkillUpgradableData> OnUpdateSkillUpgradableData;

    private SlotOpener slotOpener;

    private List<int> ownSkillsIndex = new List<int>();

    private List<int> enhanceableSkills = new List<int>();

    private Action<bool> OnUpdateAutoEquipRedot;
    private Action<bool> OnUpdateAutoEnhanceRedot;

    private float preOwnSkillStat;

    private List<int> topSkillIndexes = new List<int>();

    private List<int> skillIndexesRightBeforeAdded = new List<int>();

    private void OnSummonSkill(int index, int count, bool isLastSkill)
    {
        if (totalSkillUIElementDict.ContainsKey(index))
        {
            if (skillUIDataDict.ContainsKey(index))
            {
                SkillUIData skillUIData = skillUIDataDict[index];
                SkillUpgradableData skillUpgradableData = skillUIDataDict[index].skillUpgradableData;

                /*skillUpgradableData.currentExp += count;
                if (skillUpgradableData.level == 0)
                {
                    skillUpgradableData.level = 1;
                    skillUpgradableData.currentExp--;
                    if (!ownSkillsIndex.Contains(skillUIData.index))
                    {
                        ownSkillsIndex.Add(skillUIData.index);
                    }

                    UpdateOwnSKillEffect(0, skillUIData.skillUpgradableData.additionalStatPercent);
                }*/

                skillUIData.skillUpgradableData = skillUpgradableData;

                for (int i = 0; i < openSlotIndex; i++)
                {
                    if (topEquipSkillUIElements[i].GetIndex() == index)
                    {
                        skillUIDataDict[index] = skillUIData;
                        topEquipSkillUIElements[i].UpdateSkill(skillUIDataDict[index], UnEquipSlot, true);
                        totalSkillUIElementDict[index].UpdateSkill(skillUIDataDict[index], UnEquipSlot, false);
                        return;
                    }
                }

                skillUIData.equipStateSprite = equipSprite;
                skillUIDataDict[index] = skillUIData;
                //skillUIData.skillUpgradableData.SaveSkillData();

                //EncyclopediaDataHandler.Instance.SlotLevelChangeEvent(EncyclopediaType.Skill, $"{skillUIData.rank}");

                totalSkillUIElementDict[index].UpdateSkill(skillUIData, EquipSlotLeft, false);

            }
        }

        if (isLastSkill)
        {
            bool isAutoEquippable = GetIsSkillUIElementAutoEquippable();
            OnUpdateAutoEquipRedot?.Invoke(isAutoEquippable);
            bool isAutoEnhanceable = GetIsAutoEnhanceableSkill();
            OnUpdateAutoEnhanceRedot?.Invoke(isAutoEnhanceable);
            UpdateAddtionalStatText();
        }
    }

    private void OpenSlot()
    {
        topEquipSkillUIElements[openSlotIndex].OnSlotOpened();
        OnUpdateSkillImage?.Invoke(openSlotIndex, null);
        OnUpdateSkillCoolTime?.Invoke(openSlotIndex, 0f, false);
        equipProbabilityArr[openSlotIndex] = true;
        openSlotIndex++;
        openSlotIndex = openSlotIndex > topEquipSkillUIElements.Length ? topEquipSkillUIElements.Length : openSlotIndex;

        ES3.Save<int>(Consts.COLLEAGUE_OPEN_SLOT_INDEX, openSlotIndex, ES3.settings);
        ES3.StoreCachedFile();
    }

    private void OnDisable()
    {
        NotificationManager.instance.SetNotification(RedDotIDType.ShowSkillButton, NotificationManager.instance.GetNotificationState(RedDotIDType.AutoSkillEquipButton) || NotificationManager.instance.GetNotificationState(RedDotIDType.AutoSkillEnforeceButton));
    }

    public void Init()
    {
        skillManager = FindAnyObjectByType<SkillManager>();
        SkillManager.OnUpdateSkillEquipUI += OnSummonSkill;

        slotOpener = FindAnyObjectByType<SlotOpener>();

        slotOpener.OnOpenSkillSlot += OpenSlot;
        slotOpener.OnGetOpenSkillSlotIndex += GetOpenSlotIndex;
        OnGetBaseSkillData += skillDataSO.GetSkillData;
        equipProbabilityArr = new bool[topEquipSkillUIElements.Length];

        InitAdditionalStatText();

        skillCanvas.SetActive(false);

        int tempSlotCount = ES3.Load<int>(Consts.COLLEAGUE_OPEN_SLOT_INDEX, 0);

        for (int i = 0; i < tempSlotCount; i++)
        {
            OpenSlot();
        }

        skillInfoUIPopup.Init();

        skillInfoUIPopup.CloseUI();

        guide.Initialize();

        // QuestManager.instance.GetEventQuestTypeAction.Add(EventQuestType.SkillEquip, () => { QuestManager.instance.UpdateCount(EventQuestType.SkillEquip, PlayerPrefs.HasKey("SkillEquip") ? 1 : 0, -1); });
        //QuestManager.instance.AddEventQuestTypeAction(EventQuestType.ColleagueEquip, () => { QuestManager.instance.UpdateCount(EventQuestType.ColleagueEquip, PlayerPrefs.HasKey("SkillEquip") ? 1 : 0, -1); });
    }

    public void StartInit()
    {
        bool isAutoEquippable = GetIsSkillUIElementAutoEquippable();
        bool isAutoEnhanceable = GetIsAutoEnhanceableSkill();
        Debug.Log(isAutoEquippable);
        Debug.Log(isAutoEnhanceable);
        OnUpdateAutoEquipRedot?.Invoke(isAutoEquippable);
        OnUpdateAutoEnhanceRedot?.Invoke(isAutoEnhanceable);

    }

    private void EquipSlotLeft(int index)
    {
        for (int i = 0; i < openSlotIndex; i++)
        {
            if (equipProbabilityArr[i])
            {
                EquipSkill(index, i);
                return;
            }
        }
    }

    private void CacheSlotIndex(int index)
    {
        skillInfoCacheIndex = index;
        ActiveTopBarBackground(true);
        for (int i = 0; i < openSlotIndex; i++)
        {
            topEquipSkillUIElements[i].EnableEquipButton();
        }
    }

    private void EquipNewSkill(int unEquipIndex, SkillTopUIElement skillTopUIElement)
    {
        int topSkillIndex = Array.FindIndex(topEquipSkillUIElements, x => x == skillTopUIElement);

        int unEquipSkillIndex = topEquipSkillUIElements[topSkillIndex].GetIndex();
        EquipSkill(skillInfoCacheIndex, topSkillIndex);

        ActiveTopBarBackground(false);
        OnUpdateEquipStateForSkillsSwitched?.Invoke(skillInfoCacheIndex, unEquipIndex);
    }

    private void EquipSkill(int index, int i) //! 여기서 i가 -1인 경우가 있음.
    {
        SkillUIData skillUIData = skillUIDataDict[index];
        skillUIData.equipStateSprite = unEquipSprite;
        skillUIData.skillUpgradableData.equipIndex = i;
        skillUIData.skillUpgradableData.isEquipped = true;
        //skillUIData.skillUpgradableData.SaveSkillData();

        skillUIDataDict[index] = skillUIData;
        totalSkillUIElementDict[index].UpdateSkill(skillUIData, UnEquipSlot, false);
        totalSkillUIElementDict[index].ChangeActiveStateEquipText(true);
        totalSkillUIElementDict[index].ChangeActiveStateSkillEquipmentSlot(true);
        topEquipSkillUIElements[i].ChangeActiveStateSkillEquipmentSlot(true);
        topEquipSkillUIElements[i].UpdateSkill(skillUIData, UnEquipSlot, true);
        topEquipSkillUIElements[i].ChangeActiveStateEquipIcon(false);
        topEquipSkillUIElements[i].ActiveLevelPanel(true);
        topEquipSkillUIElements[i].EnableEquipButton();

        equipProbabilityArr[i] = false;
        OnUpdateSkillIndex?.Invoke(i, index);
        OnUpdateSkillImage?.Invoke(i, skillUIData.mainSprite);
        OnUpdateSkillCoolTime?.Invoke(i, skillUIData.skillUpgradableData.coolTime, true);
        OnUpdateSkillUpgradableData?.Invoke(skillUIData.index, skillUIData.skillUpgradableData);

        bool isAutoEquippable = GetIsSkillUIElementAutoEquippable();
        OnUpdateAutoEquipRedot?.Invoke(isAutoEquippable);

        QuestManager.instance.UpdateCount(EventQuestType.ColleagueEquip, 1, -1);

        PlayerPrefs.SetInt("SkillEquip", 1);
    }

    private void UnEquipSlot(int index)
    {
        for (int i = 0; i < openSlotIndex; i++)
        {
            if (topEquipSkillUIElements[i].GetIndex() == index)
            {
                topEquipSkillUIElements[i].OnSlotOpened();
                SkillUIData skillUIData = skillUIDataDict[index];
                skillUIData.equipStateSprite = equipSprite;
                skillUIData.skillUpgradableData.equipIndex = -1;
                skillUIData.skillUpgradableData.isEquipped = false;
                //skillUIData.skillUpgradableData.SaveSkillData();
                skillUIDataDict[index] = skillUIData;

                totalSkillUIElementDict[index].ChangeActiveStateEquipText(false);
                totalSkillUIElementDict[index].UpdateSkill(skillUIData, EquipSlotLeft, false);
                equipProbabilityArr[i] = true;
                OnUpdateSkillIndex?.Invoke(i, -1);
                OnUpdateSkillImage?.Invoke(i, null);
                OnUpdateSkillCoolTime?.Invoke(i, 0f, false);
                OnUpdateSkillUpgradableData?.Invoke(skillUIData.index, skillUIData.skillUpgradableData);

                bool isAutoEquippable = GetIsSkillUIElementAutoEquippable();
                OnUpdateAutoEquipRedot?.Invoke(isAutoEquippable);
                return;
            }
        }
    }

    

    private void ActiveTopBarBackground(bool isActive)
    {
        topBarBackgroundButton.gameObject.SetActive(isActive);
        if (!isActive)
        {
            for (int i = 0; i < openSlotIndex; i++)
            {
                topEquipSkillUIElements[i].SwapEvent(false);
            }
        }
    }

    public void InitElements()
    {
        if (totalSkillUIElementDict != null)
        {
            return;
        }

        Dictionary<int, SkillResourceData> skillResourceDict = ResourceManager.instance.skill.GetSkillResourceDict();

        totalSkillUIElementDict = new Dictionary<int, SkillUIElement>(skillResourceDict.Count);

        skillInfoUIPopup.OnUpdateLevelUI += UpdateSkillUIData;
        skillInfoUIPopup.OnUpdateEquipState += UpdateIsEquipState;
        skillInfoUIPopup.OnGetIsEquippable += GetIsSkillEquippable;
        skillInfoUIPopup.OnEquipSkill += EquipSlotLeft;
        skillInfoUIPopup.OnUnEquipSkill += UnEquipSlot;
        skillInfoUIPopup.OnGetSkillEquipState += GetSkillEquipState;
        skillInfoUIPopup.OnSetCacheIndex += CacheSlotIndex;
        skillInfoUIPopup.OnGetEquippedSkillSlotIndex += GetEquippedSkillSlotIndex;
        skillInfoUIPopup.OnEquipSkillBySlotIndex += EquipSkill;
        skillInfoUIPopup.OnClickEnhanceButtuon += UpdateOwnSKillEffect;

        OnUpdateEquipStateForSkillsSwitched += skillInfoUIPopup.UpdateEquipStateForSwitchedSkills;
        OnEnhanceSkillAuto += skillInfoUIPopup.EnhanceSkillAuto;

        topBarBackgroundButton.onClick.AddListener(() => ActiveTopBarBackground(false));
        ActiveTopBarBackground(false);

        skillUIElements = new List<SkillUIElement>(skillResourceDict.Count);

        skillAutoButtonsUIPanel.OnEquipSkillsAuto += EquipSkillsAuto;
        skillAutoButtonsUIPanel.OnEnhanceSkillsAuto += EnhanceSkillsAuto;

        skillAutoButtonsUIPanel.OnGetAutoEquipState += GetIsSkillUIElementAutoEquippable;
        skillAutoButtonsUIPanel.OnGetAutoEnhanceState += GetIsAutoEnhanceableSkill;

        OnUpdateAutoEquipRedot += (x) =>
        {
            NotificationManager.instance.SetNotification(RedDotIDType.AutoSkillEquipButton, x);
            NotificationManager.instance.SetNotification(RedDotIDType.ShowSkillButton, x && !skillCanvas.gameObject.activeSelf || NotificationManager.instance.GetNotificationState(RedDotIDType.ShowSkillButton));
        };
        OnUpdateAutoEquipRedot += skillAutoButtonsUIPanel.UpdateActiveEquipState;
        OnUpdateAutoEnhanceRedot += (x) =>
        {
            NotificationManager.instance.SetNotification(RedDotIDType.AutoSkillEnforeceButton, x);
            NotificationManager.instance.SetNotification(RedDotIDType.ShowSkillButton, (x && !skillCanvas.gameObject.activeSelf) || NotificationManager.instance.GetNotificationState(RedDotIDType.ShowSkillButton));
        };

        OnUpdateAutoEnhanceRedot += skillAutoButtonsUIPanel.UpdateActiveEnhanceState;


        for (int i = openSlotIndex; i < topEquipSkillUIElements.Length; i++)
        {
            topEquipSkillUIElements[i].ChangeActiveStateEquipText(false);
            topEquipSkillUIElements[i].ChangeActiveStateMainSprite(false);
            topEquipSkillUIElements[i].ChangeActiveStateSkillEquipmentSlot(false);
            topEquipSkillUIElements[i].ChangeActiveStateEquipIcon(false);
            topEquipSkillUIElements[i].ChangeActiveStateLockIcon(true);
            topEquipSkillUIElements[i].DisableEquipButton();
            topEquipSkillUIElements[i].OnShowUIPopup += skillInfoUIPopup.ShowUI;
            topEquipSkillUIElements[i].OnUpdateEquipButtonText += skillInfoUIPopup.UpdateEquipButtonText;
            topEquipSkillUIElements[i].OnClickEquipEvent += skillInfoUIPopup.UpdateEquipButtonEvent;
            topEquipSkillUIElements[i].OnUpdateSkillInfo += skillInfoUIPopup.UpdateSKillInfo;
            topEquipSkillUIElements[i].OnEquipNewSkill += EquipNewSkill;
            skillInfoUIPopup.OnUpdateLevelUI += topEquipSkillUIElements[i].UpdateSkillStatUI;
            skillInfoUIPopup.OnEquipChoiceState += topEquipSkillUIElements[i].SwapEvent;
        }

        for (int i = 0; i < openSlotIndex; i++)
        {
            topEquipSkillUIElements[i].OnEquipNewSkill += EquipNewSkill;
            topEquipSkillUIElements[i].OnShowUIPopup += skillInfoUIPopup.ShowUI;
            topEquipSkillUIElements[i].OnUpdateEquipButtonText += skillInfoUIPopup.UpdateEquipButtonText;
            topEquipSkillUIElements[i].OnClickEquipEvent += skillInfoUIPopup.UpdateEquipButtonEvent;
            topEquipSkillUIElements[i].OnUpdateSkillInfo += skillInfoUIPopup.UpdateSKillInfo;
            skillInfoUIPopup.OnUpdateLevelUI += topEquipSkillUIElements[i].UpdateSkillStatUI;
            skillInfoUIPopup.OnEquipChoiceState += topEquipSkillUIElements[i].SwapEvent;
        }

        List<int> skillResourceIndexes = new List<int>();
        skillResourceIndexes.AddRange(skillResourceDict.Keys);

        skillResourceIndexes.Sort((x, y) =>
        {
            return OnGetBaseSkillData.Invoke(x).rank.CompareTo(OnGetBaseSkillData.Invoke(y).rank);
        });

        foreach (var key in skillResourceIndexes)
        {
            if (!skillUIDataDict.ContainsKey(key))
            {
                SkillData skillData = OnGetBaseSkillData.Invoke(key);

                string rankKR = EnumToKRManager.instance.GetEnumToKR(skillData.rank);

                Sprite rankBackgroundSprite = OnGetRankBackgroundSprite.Invoke(skillData.rank);

                Color rankColor = OnGetRankColor.Invoke(skillData.rank);

                SkillUIData skillUIData = new SkillUIData(key, skillData.skillName, skillData.skillNameKR, skillData.description, 
                    skillResourceDict[key].skillIcon, lockSprite, skillData.skillUpgradableData);

                //skillUIData.skillUpgradableData.LoadSkillData();

                skillUIDataDict.Add(key, skillUIData);

                skillEquipStateDict.Add(key, true);

                totalSkillUIElementDict.Add(key, Instantiate(skillIconPrefab, parent));
                totalSkillUIElementDict[key].ChangeActiveStateEquipText(false);
                totalSkillUIElementDict[key].ChangeActiveStateLockIcon(false);
                totalSkillUIElementDict[key].ChangeActiveStateEquipIcon(false);
                totalSkillUIElementDict[key].OnShowUIPopup += skillInfoUIPopup.ShowUI;
                totalSkillUIElementDict[key].OnUpdateEquipButtonText += skillInfoUIPopup.UpdateEquipButtonText;
                totalSkillUIElementDict[key].OnClickEquipEvent += skillInfoUIPopup.UpdateEquipButtonEvent;
                totalSkillUIElementDict[key].OnUpdateSkillInfo += skillInfoUIPopup.UpdateSKillInfo;
                totalSkillUIElementDict[key].UpdateSkill(skillUIData, null, false);

                /*if (skillUIData.skillUpgradableData.level > 0)
                {
                    if (!ownSkillsIndex.Contains(skillUIData.index))
                    {
                        ownSkillsIndex.Add(skillUIData.index);
                        UpdateOwnSKillEffect(0, skillUIData.skillUpgradableData.additionalStatPercent);
                    }

                    if (skillUIData.skillUpgradableData.isEquipped)
                    {
                        EquipSkill(skillUIData.index, skillUIData.skillUpgradableData.equipIndex);
                    }
                }*/
            }
        }

        foreach (SkillUIData skillUIData in skillUIDataDict.Values)
        {
            /*if (skillUIData.skillUpgradableData.level != 0 && !skillUIData.skillUpgradableData.isEquipped)
            {
                SkillUIData newSkillUIData = skillUIData;
                newSkillUIData.equipStateSprite = equipSprite;
                totalSkillUIElementDict[skillUIData.index].UpdateSkill(newSkillUIData, EquipSlotLeft, false);
            }*/
        }

        UpdateAddtionalStatText();
    }

    private void UpdateSkillUIData(SkillUIData skillUIData)
    {
        int index = skillUIData.index;
        if (skillUIDataDict.ContainsKey(index))
        {
            skillUIDataDict[index] = skillUIData;
            totalSkillUIElementDict[index].UpdateSkillStatUI(skillUIData);
        }

        OnUpdateAutoEquipRedot?.Invoke(GetIsSkillUIElementAutoEquippable());
        OnUpdateAutoEnhanceRedot?.Invoke(GetIsAutoEnhanceableSkill());
        UpdateAddtionalStatText();
    }

    private bool GetSkillEquipState(int index)
    {
        return skillEquipStateDict[index];
    }

    private void UpdateIsEquipState(int index, bool isEquipState)
    {
        if (skillEquipStateDict.ContainsKey(index))
        {
            skillEquipStateDict[index] = isEquipState;
        }
    }

    private bool GetIsSkillEquippable()
    {
        for (int i = 0; i < equipProbabilityArr.Length; i++)
        {
            if (equipProbabilityArr[i])
            {
                return true;
            }
        }

        return false;
    }

    private void EquipSkillsAuto()
    {
        if (!GetIsSkillUIElementAutoEquippable())
        {
            return;
        }

        skillIndexesRightBeforeAdded.Clear();
        UpdateTopSkillIndexes();

        int index = openSlotIndex - 1 >= ownSkillsIndex.Count ? ownSkillsIndex.Count - 1 : openSlotIndex - 1;

        for (int i = index; i >= 0 && i < ownSkillsIndex.Count; i--)
        {
            int topSkillIndex = topSkillIndexes.IndexOf(ownSkillsIndex[i]);
            if (topSkillIndex != -1)
            {
                skillIndexesRightBeforeAdded.Add(topSkillIndex);
                continue;
            }

            for (int j = 0; j < openSlotIndex; j++)
            {
                if (topEquipSkillUIElements[j].GetIndex() == -1)
                {
                    skillIndexesRightBeforeAdded.Add(j);
                    break;
                }

                if (skillIndexesRightBeforeAdded.Contains(j))
                {
                    continue;
                }

                SkillUIData skillUIData = skillUIDataDict[ownSkillsIndex[i]];

                //Rank rank = topEquipSkillUIElements[j].GetRank();
                /*if (skillUIData.rank > rank ||
                    skillUIData.rank == rank && skillUIData.skillUpgradableData.level > topEquipSkillUIElements[j].GetLevel())
                {
                    UnEquipSlot(topEquipSkillUIElements[j].GetIndex());
                    skillIndexesRightBeforeAdded.Add(j);
                }*/
            }

            EquipSlotLeft(ownSkillsIndex[i]);
        }

        QuestManager.instance.UpdateCount(EventQuestType.EquipColleagueAuto, 1, -1);
        bool isAutoEquippable = GetIsSkillUIElementAutoEquippable();
        OnUpdateAutoEquipRedot?.Invoke(isAutoEquippable);
    }

    private bool GetIsSkillUIElementAutoEquippable()
    {
        SortSkillIndexDescended();
        UpdateTopSkillIndexes();

        int index = openSlotIndex - 1 >= ownSkillsIndex.Count ? ownSkillsIndex.Count - 1 : openSlotIndex - 1;

        for (int i = index; i < openSlotIndex && i < ownSkillsIndex.Count; i++)
        {
            for (int j = 0; j < topSkillIndexes.Count; j++)
            {
                if (ownSkillsIndex.Count <= j)
                {
                    continue;
                }

                if (topSkillIndexes[j] == -1)
                {
                    return true;
                }

                if (topSkillIndexes[j] == ownSkillsIndex[i])
                {
                    continue;
                }

                /*if (skillUIDataDict[topSkillIndexes[j]].rank < skillUIDataDict[ownSkillsIndex[i]].rank && !topSkillIndexes.Contains(ownSkillsIndex[i]))
                {
                    return true;
                }
                else if (skillUIDataDict[topSkillIndexes[j]].rank == skillUIDataDict[ownSkillsIndex[i]].rank)
                {
                    if (skillUIDataDict[topSkillIndexes[j]].skillUpgradableData.level < skillUIDataDict[ownSkillsIndex[i]].skillUpgradableData.level)
                    {
                        return true;
                    }
                }*/
            }
        }

        return false;
    }

    private void UpdateTopSkillIndexes()
    {
        topSkillIndexes.Clear();

        for (int i = 0; i < openSlotIndex; i++)
        {
            topSkillIndexes.Add(topEquipSkillUIElements[i].GetIndex());
        }
    }

    private void SortSkillIndexDescended()
    {
        /*ownSkillsIndex.Sort((x, y) =>
        {
            SkillUIData first = skillUIDataDict[x];
            SkillUIData second = skillUIDataDict[y];
            int ret = -first.rank.CompareTo(second.rank);
            return ret != 0 ? ret : -first.skillUpgradableData.level.CompareTo(second.skillUpgradableData.level);
        });*/
    }

    private bool GetIsAutoEnhanceableSkill()
    {
        foreach (int ownSkillIndex in ownSkillsIndex)
        {
            //SkillUpgradableData skillUpgradableData = skillUIDataDict[ownSkillIndex].skillUpgradableData;
            //if (skillUpgradableData.maxExp <= skillUpgradableData.currentExp)
            //{
            //    if (!enhanceableSkills.Contains(ownSkillIndex))
            //    {
            //        enhanceableSkills.Add(ownSkillIndex);
            //    }
            //}
        }

        bool isEnhanceableSkills = enhanceableSkills.Count > 0;

        if (isEnhanceableSkills)
        {
            // NotificationManager.instance.SetNotification(RedDotIDType.ShowSkillButton, true);
        }

        return isEnhanceableSkills;
    }

    private void EnhanceSkillsAuto()
    {
        enhanceableSkills.RemoveAll(x =>
        {
            //float previousStat = skillUIDataDict[x].skillUpgradableData.additionalStatPercent;
            //OnEnhanceSkillAuto?.Invoke(skillUIDataDict[x]);
            //UpdateOwnSKillEffect(previousStat, skillUIDataDict[x].skillUpgradableData.additionalStatPercent);
            return true;
        });

        OnUpdateAutoEnhanceRedot?.Invoke(false);
        OnUpdateAutoEquipRedot?.Invoke(GetIsSkillUIElementAutoEquippable());

        UpdateAddtionalStatText();
    }

    private void UpdateAddtionalStatText()
    {
        AdditionalStatData skillAdditionalStatData = StatDataHandler.Instance.GetAdditionalStatData(AdditionalStatType.Skill)[(int)ArithmeticStatType.Rate];
        additionalStatText.text = $"보유효과 : <color=#00FF00>기본HP&공격력&방어력 : +{skillAdditionalStatData.Stats[StatType.Damage]}%</color>";
    }

    private void InitAdditionalStatText()
    {
        additionalStatText.text = $"보유효과 : <color=#00FF00>기본HP&공격력&방어력 : +0%</color>";

    }

    private int GetOpenSlotIndex()
    {
        return openSlotIndex;
    }

    private int GetEquippedSkillSlotIndex(int skillIndex)
    {
        for (int i = 0; i < topEquipSkillUIElements.Length; i++)
        {
            if (topEquipSkillUIElements[i].GetIndex() == skillIndex)
            {
                return i;
            }
        }

        return -1;
    }

    private void UpdateOwnSKillEffect(float previousStat, float afterStat)
    {
        if (preOwnSkillStat != 0)
        {
            StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Rate, AdditionalStatType.Skill, StatType.Damage, previousStat, true);
        }
        else
        {
            float diff = afterStat - previousStat;
            StatDataHandler.Instance.ModifyStat(ArithmeticStatType.Rate, AdditionalStatType.Skill, StatType.Damage, diff, true);
        }
    }
}
