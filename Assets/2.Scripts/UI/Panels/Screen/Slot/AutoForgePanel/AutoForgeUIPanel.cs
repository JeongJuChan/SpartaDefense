using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoForgeUIPanel : UI_Popup, UIInitNeeded
{
    [SerializeField] private CastleDoorRankProbabilityDataSO castleDoorRankProbabilityDataSO;
    [SerializeField] private GuideController guideController;

    [SerializeField] private Button closeButton;

    [SerializeField] private TMP_Dropdown minimumRankChoicedropDown;
    [SerializeField] private Button buttonDisablePanel;
    [SerializeField] private GameObject otherDisablePanel;


    [SerializeField] private Sprite upArrow;
    [SerializeField] private Sprite downArrow;

    [SerializeField] private Button startButton;

    [SerializeField] private Button autoForgeButton;

    [SerializeField] private GameObject activeStatePanel;

    [SerializeField] private Button lockButton;

    public event Action<Rank> OnAutoForge;
    public event Func<bool> OnGetNewSlotSold;
    public event Action<bool> OnShowUIPopup;
    public event Func<int> OnGetForgeLevel;

    public event Action<bool> OnInterruptWhileForging;

    private int currentIndex = 1;

    private bool isAutoForge;

    private Rank[] ranks;

    private bool isAutoStopped = true;
    private bool isAutoPossible = false;

    public event Action OnShowWarningPopup;

    private bool isWarningPopupShowed;

    private FeatureType featureType;
    private int unlockCount;

    private Animator animator;

    public void Init()
    {
        SlotEquipmentForger slotEquipmentForger = FindAnyObjectByType<SlotEquipmentForger>();
        slotEquipmentForger.OnGetAutoMinimumRank += GetAutoMinimumRank;
        slotEquipmentForger.OnGetAutoForgeStopped += GetIsAutoStopped;

        ranks = (Rank[])Enum.GetValues(typeof(Rank));
        List<string> rankTexts = new List<string>();
        for (int i = 1; i < ranks.Length; i++)
        {
            Sprite rankSprite = ResourceManager.instance.rank.GetRankBackgroundSprite(ranks[i]);
            Color color = ResourceManager.instance.rank.GetRankColor(ranks[i]);
            string colorValue = ColorUtility.ToHtmlStringRGB(color);
            string rankText = $"<color=#{colorValue}> {EnumToKRManager.instance.GetEnumToKR(ranks[i])} 이상</color>";
            rankTexts.Add(rankText);
        }
        minimumRankChoicedropDown.AddOptions(rankTexts);

        minimumRankChoicedropDown.captionText.text = "등급 선택";

        minimumRankChoicedropDown.onValueChanged.AddListener(ChangeIndex);

        ForgeUIButton forgeUIButton = FindAnyObjectByType<ForgeUIButton>();
        startButton.onClick.AddListener(TryActivateAutoForge);
        startButton.onClick.AddListener(StartAuto);
        startButton.onClick.AddListener(DeActivateSelf);
        startButton.onClick.AddListener(forgeUIButton.SetOffForgeButton);

        autoForgeButton.onClick.AddListener(TryActivateSelf);

        closeButton.onClick.AddListener(DeActivateSelf);

        buttonDisablePanel.onClick.AddListener(StopAuto);

        guideController.Initialize();
        animator = autoForgeButton.GetComponent<Animator>();

        DeActivateAutoForge();

        DeActivateSelf();

        DeActiveDisableButton();

    }

    public void StartInit()
    {
        UnlockData unlockData = ResourceManager.instance.unlockDataSO.GetUnlockData(FeatureID.Forge_AutoForge);

        if (ES3.KeyExists(Consts.IS_AUTO_FORGE_POSSIBLE))
        {
            isAutoPossible = ES3.Load<bool>(Consts.IS_AUTO_FORGE_POSSIBLE);
            lockButton.gameObject.SetActive(false);
        }
        else
        {
            featureType = unlockData.featureType;
            unlockCount = unlockData.count;
            lockButton.onClick.AddListener(SendLockMessage);
            UnlockManager.Instance.RegisterFeature(new UnlockableFeature(unlockData.featureType, unlockData.featureID, unlockData.count,
            () =>
            {
                isAutoPossible = true;
                lockButton.gameObject.SetActive(false);
                ES3.Save<bool>(Consts.IS_AUTO_FORGE_POSSIBLE, isAutoPossible, ES3.settings);
                ES3.StoreCachedFile();
            }));
        }
    }

    private void Update()
    {
        if (isAutoForge)
        {
            isAutoForge = false;
            OnAutoForge?.Invoke((Rank)currentIndex);
        }
    }

    private void ChangeIndex(int newIndex)
    {
        currentIndex = newIndex + 1;
    }

    public void TryActivateAutoForge()
    {
        CastleDoorRankProbabilityData data = castleDoorRankProbabilityDataSO.GetCastleDoorRankProbabilityData(OnGetForgeLevel.Invoke());

        if (data.rankProbabilities[currentIndex - 1] == 0f && !isWarningPopupShowed)
        {
            // 팝업 띄우기
            OnShowWarningPopup?.Invoke();
            return;
        }

        ActivateAutoForge();
    }

    public void ActivateAutoForge()
    {
        animator.SetBool(AnimatorParameters.IS_AUTO_ON, true);
        ActivateDisableButton();
        isWarningPopupShowed = true;
        isAutoForge = true;
    }

    public void DeActivateAutoForge()
    {
        animator.SetBool(AnimatorParameters.IS_AUTO_ON, false);
        isAutoForge = false;
    }

    public bool GetIsAutoStopped()
    {
        return isAutoStopped;
    }

    private Rank GetAutoMinimumRank()
    {
        return ranks[currentIndex];
    }

    private void TryActivateSelf()
    {
        if (!ForgeManager.instance.GetIsNewSlotSold())
        {
            OnShowUIPopup?.Invoke(isAutoStopped);
            return;
        }
        QuestManager.instance.UpdateCount(EventQuestType.AutoForgeOpen, 1, -1);

        ActivateSelf();
    }

    private void SendLockMessage()
    {
        if (!isAutoPossible)
        {
            UnlockManager.Instance.ToastLockMessage(featureType, unlockCount);
            return;
        }
    }

    private void ActivateSelf()
    {
        activeStatePanel.SetActive(true);
        otherDisablePanel.gameObject.SetActive(true);
        ActivateCloseButton(true);
    }

    public void DeActivateSelf()
    {
        activeStatePanel.SetActive(false);
        otherDisablePanel.gameObject.SetActive(false);
        ActivateCloseButton(false);
    }

    private void StartAuto()
    {
        isAutoStopped = false;
    }

    public void StopAuto()
    {
        isWarningPopupShowed = false;
        isAutoStopped = true;
        animator.SetBool(AnimatorParameters.IS_AUTO_ON, false);
    }

    private void ActivateCloseButton(bool isActive)
    {
        closeButton.gameObject.SetActive(isActive);
    }

    public void ActivateDisableButton()
    {
        buttonDisablePanel.gameObject.SetActive(true);
    }

    public void DeActiveDisableButton()
    {
        buttonDisablePanel.gameObject.SetActive(false);
        DeActivateAutoForge();
    }
}
