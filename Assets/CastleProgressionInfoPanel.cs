using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Keiwando.BigInteger;

public class CastleProgressionInfoPanel : MonoBehaviour
{
    [Serializable]
    private struct CastleQuestUI
    {
        public RedDotIDType redDotIDType;
        public Button questButton;
        public CastleQuestType castleQuestType;
        public TextMeshProUGUI description;
        public TextMeshProUGUI progressText;
        public Slider progressSlider;
        public RewardSlot rewardSlot;
        public GameObject ClearImage;

        public void SetActiveClearImage(bool active)
        {
            questButton.interactable = !active;
            ClearImage.SetActive(active);
            NotificationManager.instance.SetNotification(redDotIDType.ToString(), !active);
        }
    }

    [Header("Before")]
    [SerializeField] private Image beforeCastleImage;
    [SerializeField] private TextMeshProUGUI beforeCastleNameText;
    [SerializeField] private TextMeshProUGUI beforeCastleHPText;
    [SerializeField] private TextMeshProUGUI beforeCastleAttackText;
    [SerializeField] private TextMeshProUGUI beforeCastleDefenseText;

    [Header("\nAfter")]
    [SerializeField] private Image afterCastleImage;
    [SerializeField] private TextMeshProUGUI afterCastleNameText;
    [SerializeField] private TextMeshProUGUI afterCastleHPText;
    [SerializeField] private TextMeshProUGUI afterCastleAttackText;
    [SerializeField] private TextMeshProUGUI afterCastleDefenseText;

    [Header("\n")]
    [SerializeField] private CastleQuestUI[] castleQuestUIs;

    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI upgradeButtonText;

    [SerializeField] private int castleElementReward = 300;

    private CastleProgressionDataHandler castleProgressionDataHandler;

    private NotificationManager notificationManager;

    public void Init()
    {
        castleProgressionDataHandler = new CastleProgressionDataHandler();

        castleProgressionDataHandler.Init();
        upgradeButton.onClick.AddListener(OnClickUpgradeButton);

        UIManager.instance.GetUIElement<UI_Castle>().OnOpenUI += OnnnnnEnable;

    }

    public void StartInit()
    {
        if (notificationManager == null) notificationManager = NotificationManager.instance;

        castleProgressionDataHandler.SetCastleQuestDatas();
        notificationManager.SetNotification(RedDotIDType.ShowCastleButton,
                castleProgressionDataHandler.CheckCastleRedDot());

        if (notificationManager == null) notificationManager = NotificationManager.instance;

        notificationManager.SetNotification(RedDotIDType.ShowCastleButton,
        notificationManager.GetNotificationState(RedDotIDType.CastleMission_1) ||
        notificationManager.GetNotificationState(RedDotIDType.CastleMission_2) ||
        notificationManager.GetNotificationState(RedDotIDType.CastleMission_3) ||
        notificationManager.GetNotificationState(RedDotIDType.CastleUpgradeButton));
    }
    private void OnnnnnEnable(UI_Base uI_Base = null)
    {
        if (castleProgressionDataHandler.SetCastleQuestDatas())
        {
            SetCastleMaxLevelInfoPanel();
            SetCastleMaxLevelQuestUIs();
            return;
        }
        SetCastleProgressionInfoPanel();
    }

    private void SetCastleProgressionInfoPanel()
    {
        CastleProgressionData beforeCastleProgressionData = castleProgressionDataHandler.beforeCastleProgressionData;
        CastleProgressionData afterCastleProgressionData = castleProgressionDataHandler.afterCastleProgressionData;

        beforeCastleImage.sprite = beforeCastleProgressionData.CastleSprite;
        beforeCastleNameText.text = beforeCastleProgressionData.Name;
        beforeCastleHPText.text = $"기본 HP    {beforeCastleProgressionData.BaseHP}";
        beforeCastleAttackText.text = $"기본 공격력    {beforeCastleProgressionData.BaseAttack}";
        beforeCastleDefenseText.text = $"기본 방어력    {beforeCastleProgressionData.BaseDefense}";

        afterCastleImage.sprite = afterCastleProgressionData.CastleSprite;
        afterCastleNameText.text = afterCastleProgressionData.Name;
        afterCastleHPText.text = afterCastleProgressionData.BaseHP.ToString();
        afterCastleAttackText.text = afterCastleProgressionData.BaseAttack.ToString();
        afterCastleDefenseText.text = afterCastleProgressionData.BaseDefense.ToString();

        upgradeButton.interactable = castleProgressionDataHandler.CanUpgradeCastle();
    }

    private void SetCastleMaxLevelInfoPanel()
    {
        CastleProgressionData beforeCastleProgressionData = castleProgressionDataHandler.beforeCastleProgressionData;

        beforeCastleImage.sprite = beforeCastleProgressionData.CastleSprite;
        beforeCastleNameText.text = beforeCastleProgressionData.Name;
        beforeCastleHPText.text = $"기본 HP    {beforeCastleProgressionData.BaseHP}";
        beforeCastleAttackText.text = $"기본 공격력    {beforeCastleProgressionData.BaseAttack}";
        beforeCastleDefenseText.text = $"기본 방어력    {beforeCastleProgressionData.BaseDefense}";

        afterCastleImage.gameObject.SetActive(false);
        afterCastleNameText.text = "최대 레벨";
        afterCastleHPText.text = "-";
        afterCastleAttackText.text = "-";
        afterCastleDefenseText.text = "-";

        upgradeButton.interactable = false;
        upgradeButtonText.text = "최대 레벨";
    }

    public void SetCastleQuestUIs()
    {
        foreach (CastleQuestUI castleQuestUI in castleQuestUIs)
        {
            CastleQuestData castleQuestData = castleProgressionDataHandler.GetCastleQuestData(castleQuestUI.castleQuestType);

            castleQuestData.CheckCompleted();

            if (castleQuestData.castleQuestType == CastleQuestType.CastleStageClear)
            {
                castleQuestUI.description.text = castleQuestData.description;
                castleQuestUI.progressText.text = $"{castleQuestData.progress} / {1}";
                castleQuestUI.progressSlider.value = castleQuestData.progress;
            }
            else
            {
                castleQuestUI.description.text = castleQuestData.description;
                castleQuestUI.progressText.text = $"{castleQuestData.progress} / {castleQuestData.maxProgress}";
                castleQuestUI.progressSlider.value = (float)castleQuestData.progress / castleQuestData.maxProgress;
            }

            castleQuestUI.rewardSlot.SetUI(new GemRewardInfo(), castleElementReward);

            if (castleQuestUI.progressSlider.value == 1)
            {
                if (castleQuestData.LoadCompleted())
                {
                    castleQuestUI.SetActiveClearImage(true);
                    // NotificationManager.instance.SetNotification(castleQuestUI.redDotIDType.ToString(), false);
                }
                else
                {
                    castleQuestUI.SetActiveClearImage(false);
                    // NotificationManager.instance.SetNotification(castleQuestUI.redDotIDType.ToString(), true);
                }
            }
            else
            {
                castleQuestUI.SetActiveClearImage(false);
                NotificationManager.instance.SetNotification(castleQuestUI.redDotIDType.ToString(), false);
                castleQuestUI.questButton.interactable = false;
            }
            castleQuestUI.questButton.onClick.RemoveAllListeners();
            castleQuestUI.questButton.onClick.AddListener(() =>
            {
                RewardManager.instance.GiveReward(RewardType.Gem, castleElementReward);
                RewardManager.instance.ShowRewardPanel();
                castleQuestUI.SetActiveClearImage(true);
                castleQuestData.SaveCompleted();
                upgradeButton.interactable = castleProgressionDataHandler.CanUpgradeCastle();

                notificationManager.SetNotification(RedDotIDType.ShowCastleButton,
                notificationManager.GetNotificationState(RedDotIDType.CastleMission_1) ||
                notificationManager.GetNotificationState(RedDotIDType.CastleMission_2) ||
                notificationManager.GetNotificationState(RedDotIDType.CastleMission_3) ||
                notificationManager.GetNotificationState(RedDotIDType.CastleUpgradeButton));
            });
        }
    }

    private void SetCastleMaxLevelQuestUIs()
    {
        foreach (CastleQuestUI castleQuestUI in castleQuestUIs)
        {
            castleQuestUI.description.text = "만렙";
            castleQuestUI.progressText.text = "";
            castleQuestUI.progressSlider.value = 1;
            castleQuestUI.rewardSlot.SetUI(new GemRewardInfo(), castleElementReward);
            castleQuestUI.SetActiveClearImage(true);
            castleQuestUI.questButton.interactable = false;
        }
    }

    private void TryUpgradeCastle()
    {
        if (castleProgressionDataHandler.GetIsUpgradePossible())
        {
            castleProgressionDataHandler.UpgradeCastle();
            if (castleProgressionDataHandler.GetIsUpgradePossible())
            {
                UpdateSettings();
            }
            else
            {
                SetCastleMaxLevelInfoPanel();
                SetCastleMaxLevelQuestUIs();
            }
        }
    }

    public void OnClickUpgradeButton()
    {
        TryUpgradeCastle();
    }

    private void UpdateSettings()
    {
        SetCastleProgressionInfoPanel();
        SetCastleQuestUIs();
    }

#if UNITY_EDITOR
    public void EditorUpgradeCastle()
    {
        upgradeButton.onClick.Invoke();
    }
#endif
}
