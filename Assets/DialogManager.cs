using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using System;

public class DialogManager : MonoBehaviorSingleton<DialogManager>
{
    private struct DialogState
    {
        public Transform originalParent;
        public Vector3 originalPosition;
        public Vector3 originalScale;
        public int siblingIndex;
        public Dictionary<SortingGroup, string> originalSortingLayers;
    }

    [SerializeField] private InitMaskPanel initMaskPanel;
    [SerializeField] private GameObject dialogBackground; // 다이얼로그의 배경
    [SerializeField] private TextMeshProUGUI dialogueDescription;
    [SerializeField] private RectTransform dialogContainer; // 다이얼로그와 배경을 포함하는 컨테이너

    [SerializeField] private GameObject guide;
    // private Vector2 originalAnchoredPosition; // UI 요소의 원래 anchoredPosition 저장
    // private RectTransform currentActiveRectTransform; // 현재 활성화된 UI 요소의 RectTransform
    private Transform currentActiveDialog; // 현재 활성화된 다이얼로그의 RectTransform
    private DialogueType currentDialogueType; // 현재 활성화된 다이얼로그의 타입
    private Dictionary<DialogueType, DialogueData> dialogueDict;
    // private Dictionary<RectTransform, DialogState> dialogStates = new Dictionary<RectTransform, DialogState>();
    private Dictionary<Transform, DialogState> dialogStates = new Dictionary<Transform, DialogState>();
    public Dictionary<SortingGroup, string> originalSortingLayers = new Dictionary<SortingGroup, string>();
    public Dictionary<DialogueType, bool> isDialogue = new Dictionary<DialogueType, bool>();

    private Action<FeatureType> OnUpdateFeature;

    public Action OnSummonForgeDialogueFinished;

    public event Action OnUnlockQuestUI;
    
    public event Action OnStartStage;

    public event Action OnActivateAutoDisassembleEquipment;

    private Dictionary<int, Action> dialogueDictByQuest = new Dictionary<int, Action>();

    public void Init()
    {
        dialogueDict = ResourceManager.instance.dialogueDataSO.GetDialogueDict();

        dialogContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);

        dialogBackground.transform.SetParent(dialogContainer, false);

        UnlockManager.Instance.SetUnlockCondition(FeatureType.Dialogue, CheckCurrentDialogue);

        OnUpdateFeature += UnlockManager.Instance.CheckUnlocks;

        foreach (DialogueData data in dialogueDict.Values)
        {
            if (!dialogueDictByQuest.ContainsKey(data.questArriveNum))
            {
                dialogueDictByQuest.Add(data.questArriveNum, () => ShowDialog(data.type));
            }
        }

        QuestManager.instance.OnTryShowDialogue += TryShowDialogue;

        /*// FirstOpen이 저장되어있지 않으면 FirstOpen부터 Open
        if (!ES3.KeyExists(Consts.DIALOGUE_TYPE_FIRST_OPEN, ES3.settings))
        {
            ShowDialog(DialogueType.FirstOpen);
            return;
        }*/
    }

    private void TryShowDialogue(int questNum)
    {
        if (dialogueDictByQuest.ContainsKey(questNum))
        {
            dialogueDictByQuest[questNum]?.Invoke();
        }
    }

    private bool CheckCurrentDialogue(int index)
    {
        return dialogueDict[currentDialogueType].index == index;
    }

    public void OnClick_DialogueBackground()
    {
        HideDialog();
        /*if (*//*!(currentDialogueType == DialogueType.CompleteMission) &&*//* !(currentDialogueType == DialogueType.LevelUpForge))
        {
            return;
        }*/
    }

    public bool IsBackgroundActivate()
    {
        return dialogBackground.activeInHierarchy;
    }

    public bool ShowDialog(DialogueType type)
    {
        if (ES3.Load(ChangeDialogueType(type), false))
        {
            return false;
        }

        if (type == DialogueType.FirstOpen)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent($"first_startpoint_1");
        }

        currentDialogueType = type;
        dialogBackground.SetActive(true);
        SetDialogueDescription(type);

        OnUpdateFeature?.Invoke(FeatureType.Dialogue);

        isDialogue[type] = true;

        ES3.Save<bool>(Consts.IS_DIALOGUE_OPENED, true, ES3.settings);
        ES3.StoreCachedFile();

        return true;
    }

    public bool ShowDialog(Transform dialog, DialogueType type)
    {
        Debug.Assert(dialog != null, "Dialog is null");
        Debug.Assert(dialog.parent != null, "Dialog parent is null");

        if (dialogStates.ContainsKey(dialog) || ES3.Load(ChangeDialogueType(type), false))
        {
            return false;
        }

        currentDialogueType = type;
        OnUpdateFeature?.Invoke(FeatureType.Dialogue);

        var sortingGroups = dialog.GetComponentsInChildren<SortingGroup>();
        var originalSortingLayers = new Dictionary<SortingGroup, string>();

        foreach (var sortingGroup in sortingGroups)
        {
            originalSortingLayers[sortingGroup] = sortingGroup.sortingLayerName;
            sortingGroup.sortingLayerName = "Dialogue";
        }

        dialogBackground.SetActive(true);
        dialogStates[dialog] = new DialogState
        {
            originalParent = dialog.parent,
            originalPosition = dialog.position,
            originalScale = dialog.localScale,
            siblingIndex = dialog.GetSiblingIndex(),
            originalSortingLayers = originalSortingLayers

        };

        currentDialogueType = type;

        dialog.SetParent(dialogContainer, true);
        dialog.SetAsLastSibling();
        dialog.position = dialogStates[dialog].originalPosition; // 월드 좌표 설정
        currentActiveDialog = dialog;

        SetDialogueDescription(type);

        isDialogue[type] = true;

        ES3.Save<bool>(Consts.IS_DIALOGUE_OPENED, true, ES3.settings);
        ES3.StoreCachedFile();

        return true;
    }

    private void SetDialogueDescription(DialogueType type)
    {
        if (dialogueDict.TryGetValue(type, out DialogueData data))
        {
            dialogueDescription.text = $"{data.dialogueDescription}";
            dialogueDescription.text = data.dialogueDescription.Replace("/n", "\n");
        }

        if (/*!(currentDialogueType == DialogueType.CompleteMission) && */!(currentDialogueType == DialogueType.LevelUpForge))
        {
            guide.SetActive(true);
        }
    }

    public void HideDialog()
    {
        if (currentDialogueType == DialogueType.None) return;

        if (currentActiveDialog != null)
        {
            var state = dialogStates[currentActiveDialog];
            currentActiveDialog.SetParent(state.originalParent, true);
            currentActiveDialog.transform.position = state.originalPosition; // 원래의 월드 좌표로 복귀
            currentActiveDialog.localScale = state.originalScale;
            currentActiveDialog.SetSiblingIndex(state.siblingIndex);

            foreach (var pair in state.originalSortingLayers)
            {
                pair.Key.sortingLayerName = pair.Value;
            }

            // if (currentDialogueType == DialogueType.SellItems)
            // {
            //     currentActiveDialog.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
            // }

            currentActiveDialog = null;

            isDialogue[currentDialogueType] = false;
        }
        ES3.Save(ChangeDialogueType(), true, ES3.settings);

        guide.SetActive(false);
        dialogBackground.SetActive(false);

        if (currentDialogueType == DialogueType.FirstOpen && 
            !ES3.Load<bool>(Consts.FirstOpen, false, ES3.settings))
        {
            currentDialogueType = DialogueType.None;
            Firebase.Analytics.FirebaseAnalytics.LogEvent($"first_startpoint_2");
            ES3.Save<bool>(Consts.FirstOpen, true, ES3.settings);
            ES3.StoreCachedFile();
            OnStartStage?.Invoke();
            OnUnlockQuestUI?.Invoke();
            //ShowDialog(DialogueType.KillEnemies);
            //initMaskPanel.CheckInit();
        }
        /*if (currentDialogueType == DialogueType.AcquireEquipment)
        {
            currentDialogueType = DialogueType.None;
            ES3.Save<bool>(Consts.DIALOGUE_TYPE_ACQUIRE_EQUIPMENT, true, ES3.settings);
            ES3.StoreCachedFile();
            ShowDialog(ResourceManager.instance.questBar.transform, DialogueType.CompleteMission);
            ResourceManager.instance.questGuide.SetActive(true);
        }*/
        /*else if (currentDialogueType == DialogueType.CompleteMission)
        {
            currentDialogueType = DialogueType.None;
            ResourceManager.instance.questGuide.SetActive(false);
            ShowDialog(DialogueType.KillEnemies);
        }*/
        else if (currentDialogueType == DialogueType.KillEnemies)
        {
            currentDialogueType = DialogueType.None;
            ES3.Save<bool>(Consts.DIALOGUE_TYPE_KILL_ENEMIES, true, ES3.settings);
            ES3.StoreCachedFile();
            // ShowDialog(ResourceManager.instance.forgeUIButton.transform, DialogueType.SummonForge);
            //ShowDialog(DialogueType.SummonForge);
        }
        else if (currentDialogueType == DialogueType.SummonForge)
        {
            OnSummonForgeDialogueFinished?.Invoke();
        }
        else if (currentDialogueType == DialogueType.LevelUpForge)
        {
            OnActivateAutoDisassembleEquipment?.Invoke();
            ES3.Save<bool>(Consts.DIALOGUE_TYPE_SELL_ITEMS, true, ES3.settings);
            ES3.StoreCachedFile();
        }
        /*else if (currentDialogueType == DialogueType.LevelUpHero)
        {
            currentDialogueType = DialogueType.None;
            ShowDialog(ResourceManager.instance.castleLevelButton.transform, DialogueType.UpgradeForge);
            ResourceManager.instance.castleLevelGuide.SetActive(true);
        }
        else if (currentDialogueType == DialogueType.UpgradeForge)
        {
            currentDialogueType = DialogueType.None;
            ResourceManager.instance.castleLevelGuide.SetActive(false);
        }*/
        else
        {
            currentDialogueType = DialogueType.None;
        }

        ES3.Save<bool>(Consts.IS_DIALOGUE_OPENED, false, ES3.settings);
        ES3.StoreCachedFile();
    }

    public bool GetIsSellEventTime()
    {
        return false;
    }

    private string ChangeDialogueType()
    {
        switch (currentDialogueType)
        {
            case DialogueType.FirstOpen:
                return Consts.DIALOGUE_TYPE_FIRST_OPEN;
            case DialogueType.KillEnemies:
                return Consts.DIALOGUE_TYPE_KILL_ENEMIES;
            case DialogueType.LevelUpColleague:
                return Consts.DIALOGUE_TYPE_LEVEL_UP_COLLEAGUE;
            case DialogueType.SummonForge:
                return Consts.DIALOGUE_TYPE_SUMMON_FORGE;
            case DialogueType.LevelUpForge:
                return Consts.DIALOGUE_TYPE_LEVEL_UP_FORGE;
            case DialogueType.ReSummonForge:
                return Consts.DIALOGUE_TYPE_RE_SUMMON_FORGE;
            case DialogueType.SummonColleague:
                return Consts.DIALOGUE_TYPE_SUMMON_COLLEAGUE;
            case DialogueType.EquipColleague:
                return Consts.DIALOGUE_TYPE_EQUIP_COLLEAGUE;
            case DialogueType.LastOverview:
                return Consts.DIALOGUE_TYPE_LAST_OVERVIEW;
            case DialogueType.AdvanceUpColleague:
                return Consts.DIALOGUE_TYPE_ADVANCE_UP_COLLEAGUE;
            case DialogueType.ColleagueBook:
                return Consts.DIALOGUE_TYPE_COLLEAGUE_BOOK;
            /*case DialogueType.AcquireEquipment:
                return Consts.DIALOGUE_TYPE_ACQUIRE_EQUIPMENT;
            case DialogueType.CompleteMission:
                return Consts.DIALOGUE_TYPE_COMPLETE_MISSION;
            case DialogueType.SellItems:
                return Consts.DIALOGUE_TYPE_SELL_ITEMS;*/
        }

        return "";
    }
    private string ChangeDialogueType(DialogueType type)
    {
        switch (type)
        {
            case DialogueType.FirstOpen:
                return Consts.DIALOGUE_TYPE_FIRST_OPEN;
            case DialogueType.KillEnemies:
                return Consts.DIALOGUE_TYPE_KILL_ENEMIES;
            case DialogueType.LevelUpColleague:
                return Consts.DIALOGUE_TYPE_LEVEL_UP_COLLEAGUE;
            case DialogueType.SummonForge:
                return Consts.DIALOGUE_TYPE_SUMMON_FORGE;
            case DialogueType.LevelUpForge:
                return Consts.DIALOGUE_TYPE_LEVEL_UP_FORGE;
            case DialogueType.ReSummonForge:
                return Consts.DIALOGUE_TYPE_RE_SUMMON_FORGE;
            case DialogueType.SummonColleague:
                return Consts.DIALOGUE_TYPE_SUMMON_COLLEAGUE;
            case DialogueType.EquipColleague:
                return Consts.DIALOGUE_TYPE_EQUIP_COLLEAGUE;
            case DialogueType.LastOverview:
                return Consts.DIALOGUE_TYPE_LAST_OVERVIEW;
            case DialogueType.AdvanceUpColleague:
                return Consts.DIALOGUE_TYPE_ADVANCE_UP_COLLEAGUE;
            case DialogueType.ColleagueBook:
                return Consts.DIALOGUE_TYPE_COLLEAGUE_BOOK;
            /*case DialogueType.AcquireEquipment:
                return Consts.DIALOGUE_TYPE_ACQUIRE_EQUIPMENT;
            case DialogueType.CompleteMission:
                return Consts.DIALOGUE_TYPE_COMPLETE_MISSION;
            case DialogueType.SellItems:
                return Consts.DIALOGUE_TYPE_SELL_ITEMS;*/
        }

        return "";
    }
}
